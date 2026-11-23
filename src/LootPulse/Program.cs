using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace LootPulse;

public static class Program
{
    private static Mutex? _instanceMutex;
    private static EventWaitHandle? _showEvent;

    [STAThread]
    public static void Main(string[] args)
    {
        var dataDir = AppInfo.DefaultDataDirectory;

        // Clean up previous .old executable if present from a recent self-update
        try
        {
            var currentExe = Environment.ProcessPath;
            if (!string.IsNullOrEmpty(currentExe))
            {
                var oldExe = currentExe + ".old";
                if (File.Exists(oldExe)) File.Delete(oldExe);
            }
        }
        catch { /* ignore */ }

        // Single instance check
        var mutexName = AppInfo.InstanceMutexName(dataDir);
        var showEventName = AppInfo.ShowEventName(dataDir);

        _instanceMutex = new Mutex(true, mutexName, out var createdNew);
        if (!createdNew)
        {
            // Another instance is already running -> bring it to front and exit
            try
            {
                using var existingEvent = EventWaitHandle.OpenExisting(showEventName);
                existingEvent.Set();
            }
            catch { /* ignore */ }
            return;
        }

        _showEvent = new EventWaitHandle(false, EventResetMode.AutoReset, showEventName);
        var storage = new Storage(dataDir);
        var config = storage.LoadConfig();
        var data = storage.LoadData();

        var partyHub = new PartyHub(config);
        var engine = new MatchEngine(storage, config, data, partyHub);

        // Listen for second launch events to bring window to front
        var showCts = new CancellationTokenSource();
        ThreadPool.RegisterWaitForSingleObject(_showEvent, (_, _) => GuiWindow.RestoreWindow(), null, -1, false);

        // Start overlay and match engine
        NativeOverlay.Start(config, engine, partyHub);
        engine.OnStateChanged += () => NativeOverlay.NotifyUpdate();
        partyHub.OnRosterChanged += () => NativeOverlay.NotifyUpdate();

        engine.Start();

        GuiServer? server = null;
        server = new GuiServer(engine, partyHub, storage, config, act => GuiWindow.HandleWindowControl(act));
        server.Start();

        // Check for updates on startup
        if (config.AutoCheckUpdates && !string.IsNullOrWhiteSpace(config.UpdateRepo))
        {
            _ = System.Threading.Tasks.Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(2500);
                var res = await Updater.CheckAsync(config.UpdateRepo, AppInfo.Version);
                if (res.HasUpdate)
                {
                    server?.SetAvailableUpdate(res);
                }
            });
        }

        // Launch UI Window
        GuiWindow.Launch(server.Url, dataDir, () =>
        {
            showCts.Cancel();
            engine.Stop();
            server.Stop();
            storage.SaveConfig(config);
            storage.SaveData(data);
        });
    }
}
