using System;

namespace LootPulse;

public static class GuiPage
{
    public static string Render(string token)
    {
        return HtmlTemplate.Replace("{{TOKEN}}", token);
    }

    private const string HtmlTemplate = """
<!DOCTYPE html>
<html lang="ru" data-theme="dark">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>LootPulse</title>
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Geist+Mono:wght@400..700&family=Geist:wght@400..700&display=swap" rel="stylesheet">
<style>
:root, [data-theme="dark"] {
  --bg: #0B0D12;
  --surface-1: #12151D;
  --surface: #171B26;
  --surface-hover: #1F2433;
  --surface-well: #0E1117;
  --border: rgba(255, 255, 255, 0.08);
  --border-hover: rgba(255, 255, 255, 0.14);
  --border-active: rgba(217, 119, 6, 0.45);
  --text-main: #F1F5F9;
  --text-muted: #94A3B8;
  --text-ghost: #64748B;
  --gold: #D97706;
  --gold-bright: #E5A93C;
  --gold-soft: rgba(217, 119, 6, 0.12);
  --green: #16A34A;
  --green-bright: #22C55E;
  --green-soft: rgba(34, 197, 94, 0.12);
  --red: #DC2626;
  --shadow-card: 0 4px 20px rgba(0, 0, 0, 0.28);
  --dot-grid: rgba(255, 255, 255, 0.05);
  --font: "Geist", "Segoe UI Variable Text", "Segoe UI", system-ui, -apple-system, sans-serif;
  --mono: "Geist Mono", "JetBrains Mono", "Bahnschrift", "Cascadia Mono", monospace;
}

[data-theme="light"] {
  --bg: #F8FAFC;
  --surface-1: #FFFFFF;
  --surface: #FFFFFF;
  --surface-hover: #F1F5F9;
  --surface-well: #F1F5F9;
  --border: rgba(15, 23, 42, 0.09);
  --border-hover: rgba(15, 23, 42, 0.16);
  --border-active: rgba(217, 119, 6, 0.50);
  --text-main: #0F172A;
  --text-muted: #475569;
  --text-ghost: #94A3B8;
  --gold: #D97706;
  --gold-bright: #B45309;
  --gold-soft: rgba(217, 119, 6, 0.10);
  --green: #16A34A;
  --green-bright: #15803D;
  --green-soft: rgba(22, 163, 74, 0.10);
  --red: #DC2626;
  --shadow-card: 0 2px 10px rgba(15, 23, 42, 0.05);
  --dot-grid: rgba(15, 23, 42, 0.06);
}

* { box-sizing: border-box; margin: 0; padding: 0; user-select: none; }
body {
  background: var(--bg);
  color: var(--text-main);
  font-family: var(--font);
  font-size: 14px;
  line-height: 1.5;
  height: 100vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  transition: background 180ms ease, color 180ms ease;
}

::-webkit-scrollbar { width: 6px; height: 6px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: var(--border-hover); border-radius: 3px; }
::-webkit-scrollbar-thumb:hover { background: var(--text-ghost); }

/* Titlebar */
#titlebar {
  height: 44px;
  background: var(--surface-1);
  border-bottom: 1px solid var(--border);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 16px;
  flex-shrink: 0;
}

.brand {
  display: flex;
  align-items: center;
  gap: 10px;
  font-weight: 700;
  font-size: 14px;
  letter-spacing: 0.75px;
  text-transform: uppercase;
}
.brand svg { width: 20px; height: 20px; fill: var(--gold); }

.mono-pill {
  font-family: var(--mono);
  font-size: 11px;
  font-weight: 600;
  background: var(--surface-well);
  border: 1px solid var(--border);
  padding: 2px 8px;
  border-radius: 4px;
  color: var(--text-muted);
  text-transform: none;
  letter-spacing: 0;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.mono-pill.amber {
  background: var(--gold-soft);
  border-color: var(--border-active);
  color: var(--gold-bright);
}

.mono-pill.green {
  background: var(--green-soft);
  border-color: rgba(34, 197, 94, 0.3);
  color: var(--green-bright);
}

.titlebar-right {
  display: flex;
  align-items: center;
  gap: 10px;
}

.live-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: var(--green-bright);
  display: inline-block;
  animation: pulseDot 2s ease-in-out infinite;
}
@keyframes pulseDot {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.35; }
}

.room-badge {
  font-family: var(--mono);
  font-size: 11px;
  font-weight: 600;
  color: var(--text-muted);
  background: var(--surface-well);
  border: 1px solid var(--border);
  padding: 4px 10px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  gap: 6px;
}
.room-badge svg { width: 13px; height: 13px; fill: var(--gold); }

.win-controls { display: flex; align-items: center; gap: 4px; }
.win-btn {
  width: 30px;
  height: 30px;
  background: transparent;
  border: 1px solid transparent;
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
  transition: background 120ms ease, color 120ms ease;
}
.win-btn:hover { background: var(--surface-hover); color: var(--text-main); }
.win-btn.close:hover { background: var(--red); color: #fff; }
.win-btn svg { width: 14px; height: 14px; fill: currentColor; }

/* Navigation Bar */
#nav {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 24px;
  height: 42px;
  background: var(--surface-1);
  border-bottom: 1px solid var(--border);
  flex-shrink: 0;
}

.nav-tabs { display: flex; gap: 2px; height: 100%; }

.tab-btn {
  display: flex;
  align-items: center;
  gap: 7px;
  background: transparent;
  border: none;
  border-bottom: 2px solid transparent;
  color: var(--text-muted);
  font-family: var(--font);
  font-size: 12px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.6px;
  height: 100%;
  padding: 0 14px;
  cursor: pointer;
  transition: color 140ms ease, border-color 140ms ease, background 140ms ease;
}
.tab-btn:hover { color: var(--text-main); background: var(--surface-hover); }
.tab-btn.active {
  color: var(--gold-bright);
  border-bottom-color: var(--gold);
}
.tab-btn svg { width: 15px; height: 15px; fill: currentColor; }

.nav-meta {
  font-family: var(--mono);
  font-size: 11px;
  color: var(--text-ghost);
  letter-spacing: 0.5px;
}

/* Main Content */
#main {
  flex: 1;
  overflow-y: auto;
  padding: 20px 24px;
}

#main-inner {
  max-width: 1240px;
  margin: 0 auto;
  width: 100%;
}

.tab-view {
  display: none;
  flex-direction: column;
  gap: 16px;
  animation: fadeIn 160ms cubic-bezier(0.16, 1, 0.3, 1);
}
.tab-view.active { display: flex; }

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(4px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Grids */
.grid-2 { display: grid; grid-template-columns: 1.05fr 1fr; gap: 16px; }
.grid-split { display: grid; grid-template-columns: 1fr 1.25fr; gap: 16px; align-items: start; }
.grid-4 { display: grid; grid-template-columns: repeat(4, 1fr); gap: 12px; }

@media (max-width: 820px) {
  .grid-2, .grid-split { grid-template-columns: 1fr; }
  .grid-4 { grid-template-columns: repeat(2, 1fr); }
  .nav-meta { display: none; }
}

/* Cards */
.card {
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 14px;
  box-shadow: var(--shadow-card);
  transition: border-color 150ms ease;
}
.card:hover { border-color: var(--border-hover); }

.card-title {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.75px;
  color: var(--text-muted);
  font-weight: 700;
}
.card-title-left {
  display: flex;
  align-items: center;
  gap: 8px;
}
.card-title svg { width: 15px; height: 15px; fill: var(--gold); }

/* Dashboard Hero Telemetry */
.hero-split {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  padding: 4px 0;
}

.hero-number {
  font-family: var(--mono);
  font-size: 44px;
  font-weight: 700;
  line-height: 1.0;
  color: var(--text-main);
  font-variant-numeric: tabular-nums;
  display: flex;
  align-items: baseline;
  gap: 10px;
}
.hero-number .unit {
  font-size: 13px;
  color: var(--text-muted);
  font-family: var(--font);
  font-weight: 500;
}

.well-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
  padding-top: 4px;
}

.well-box {
  background: var(--surface-well);
  border: 1px solid var(--border);
  border-radius: 7px;
  padding: 10px 14px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 13px;
  color: var(--text-muted);
}
.well-box b {
  font-family: var(--mono);
  font-variant-numeric: tabular-nums;
  font-size: 14px;
  color: var(--text-main);
}

/* Squad Roster Cards */
.squad-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 14px;
}

.teammate-card {
  background: var(--surface-well);
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  transition: border-color 140ms ease;
}
.teammate-card.me { border-color: var(--border-active); }

.teammate-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.teammate-name {
  font-weight: 700;
  font-size: 15px;
  display: flex;
  align-items: center;
  gap: 8px;
  color: var(--text-main);
}
.teammate-rows {
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.teammate-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 12px;
  color: var(--text-muted);
}
.teammate-row b {
  font-family: var(--mono);
  font-variant-numeric: tabular-nums;
  font-size: 13px;
  color: var(--text-main);
}

/* Party Peer Cards */
.party-peer-card {
  background: var(--surface-well);
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 14px 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.party-peer-card.me { border-color: var(--border-active); }

.monogram {
  width: 34px;
  height: 34px;
  border-radius: 6px;
  background: var(--surface);
  border: 1px solid var(--border);
  display: flex;
  align-items: center;
  justify-content: center;
  font-family: var(--mono);
  font-weight: 700;
  font-size: 14px;
  color: var(--text-main);
}
.party-peer-card.me .monogram {
  border-color: var(--border-active);
  color: var(--gold-bright);
  background: var(--gold-soft);
}

.peer-metrics-3 {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 10px;
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: 6px;
  padding: 10px 12px;
}
.peer-metric-label {
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: var(--text-ghost);
}
.peer-metric-val {
  font-family: var(--mono);
  font-variant-numeric: tabular-nums;
  font-size: 15px;
  font-weight: 700;
  color: var(--text-main);
  margin-top: 2px;
}

.share-track {
  height: 4px;
  background: rgba(128, 128, 128, 0.14);
  border-radius: 3px;
  overflow: hidden;
  margin-top: 4px;
}
.share-fill {
  height: 100%;
  background: var(--gold);
  border-radius: 3px;
  transition: width 240ms cubic-bezier(0.16, 1, 0.3, 1);
}

/* Tables */
.table { width: 100%; border-collapse: collapse; }
.table th {
  text-align: left;
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.6px;
  color: var(--text-ghost);
  padding: 10px 14px;
  border-bottom: 1px solid var(--border);
  text-transform: uppercase;
}
.table td {
  padding: 11px 14px;
  border-bottom: 1px solid var(--border);
  font-size: 13px;
  font-variant-numeric: tabular-nums;
}
.table tbody tr:last-child td { border-bottom: none; }
.table tr:hover td { background: var(--surface-hover); }

.poi-pill {
  background: var(--gold-soft);
  color: var(--gold-bright);
  border: 1px solid var(--border-active);
  border-radius: 4px;
  padding: 2px 8px;
  font-family: var(--mono);
  font-size: 11px;
  font-weight: 600;
  display: inline-block;
}

.filter-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}
.chip-btn {
  background: var(--surface-well);
  border: 1px solid var(--border);
  color: var(--text-muted);
  padding: 5px 10px;
  border-radius: 5px;
  font-family: var(--font);
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 120ms ease;
}
.chip-btn:hover { border-color: var(--border-hover); color: var(--text-main); }
.chip-btn.active {
  background: var(--gold);
  border-color: var(--gold);
  color: #0B0D12;
}

/* Forms & Buttons */
.form-group { display: flex; flex-direction: column; gap: 6px; }
.form-label { font-size: 12px; font-weight: 600; color: var(--text-muted); }
.input-row { display: flex; gap: 8px; }
.input {
  flex: 1;
  background: var(--surface-well);
  border: 1px solid var(--border);
  padding: 9px 12px;
  border-radius: 6px;
  color: var(--text-main);
  font-size: 13px;
  font-family: var(--font);
  outline: none;
  transition: border-color 150ms ease;
}
.input:focus { border-color: var(--gold); }

.btn {
  background: var(--surface-well);
  border: 1px solid var(--border);
  color: var(--text-main);
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-family: var(--font);
  font-weight: 600;
  font-size: 13px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  transition: background 120ms ease, border-color 120ms ease, transform 80ms ease;
}
.btn:hover { background: var(--surface-hover); border-color: var(--border-hover); }
.btn:active { transform: translateY(-1px) scale(0.98); }
.btn.primary { background: var(--gold); color: #0B0D12; border-color: var(--gold); }
.btn.primary:hover { filter: brightness(1.08); }
.btn svg { width: 14px; height: 14px; fill: currentColor; }

/* Charts & Stat Boxes */
.chart-container {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 12px;
  height: 220px;
  padding: 18px 14px 10px 14px;
  background: var(--surface-well);
  border: 1px solid var(--border);
  border-radius: 8px;
}

.chart-col {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-end;
  height: 100%;
  cursor: pointer;
  padding: 6px 4px;
  border: 1px solid transparent;
  border-radius: 6px;
  transition: background 150ms ease, border-color 150ms ease;
}
.chart-col:hover { background: var(--surface-hover); }
.chart-col.selected {
  background: var(--gold-soft);
  border-color: var(--border-active);
}

.chart-val {
  font-size: 13px;
  font-family: var(--mono);
  font-variant-numeric: tabular-nums;
  font-weight: 700;
  color: var(--text-muted);
  margin-bottom: 6px;
}
.chart-col.selected .chart-val,
.chart-col:hover .chart-val { color: var(--gold-bright); }

.chart-bar-wrap {
  width: 100%;
  max-width: 44px;
  height: 128px;
  display: flex;
  align-items: flex-end;
  background: rgba(128, 128, 128, 0.08);
  border-radius: 4px;
  overflow: hidden;
}

.chart-bar {
  width: 100%;
  background: rgba(217, 119, 6, 0.35);
  border-radius: 4px 4px 0 0;
  transition: height 240ms cubic-bezier(0.16, 1, 0.3, 1), background 150ms ease;
  min-height: 3px;
}
.chart-col:hover .chart-bar { background: rgba(217, 119, 6, 0.65); }
.chart-col.selected .chart-bar { background: var(--gold); }

.chart-label {
  margin-top: 8px;
  font-size: 11px;
  font-weight: 600;
  color: var(--text-muted);
  text-align: center;
  line-height: 1.2;
}
.chart-col.selected .chart-label { color: var(--text-main); font-weight: 700; }

.stat-box {
  background: var(--surface-well);
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 14px 16px;
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.stat-box .stat-label {
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
  color: var(--text-ghost);
  letter-spacing: 0.6px;
}
.stat-box .stat-val {
  font-family: var(--mono);
  font-variant-numeric: tabular-nums;
  font-size: 24px;
  font-weight: 700;
  color: var(--text-main);
}
.stat-box .stat-val.highlight { color: var(--gold-bright); }
.stat-box .stat-sub { font-size: 12px; color: var(--text-muted); }

/* Overlay Preview Stage */
.overlay-stage {
  background-color: var(--surface-well);
  background-image: radial-gradient(var(--dot-grid) 1px, transparent 1px);
  background-size: 16px 16px;
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 14px;
  align-items: center;
}

/* Status Footer */
#statusbar {
  height: 28px;
  background: var(--surface-1);
  border-top: 1px solid var(--border);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 18px;
  font-family: var(--mono);
  font-size: 11px;
  color: var(--text-ghost);
  flex-shrink: 0;
}

.toast {
  position: fixed;
  bottom: 40px;
  right: 24px;
  background: var(--surface);
  color: var(--text-main);
  border: 1px solid var(--border-active);
  padding: 10px 16px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 600;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.35);
  display: none;
  z-index: 999;
  animation: toastIn 180ms cubic-bezier(0.16, 1, 0.3, 1);
}
@keyframes toastIn { from { transform: translateY(10px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }
</style>
</head>
<body>

<!-- Frameless Titlebar -->
<div id="titlebar">
  <div class="brand">
    <svg viewBox="0 0 24 24"><path d="M4 6h16a2 2 0 0 1 2 2v3H2V8a2 2 0 0 1 2-2zm-2 7h20v5a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2v-5zm9-1v3h2v-3h-2z"/></svg>
    <span>LootPulse</span>
    <span class="mono-pill" id="titlebarVersion">v2.1.1</span>
    <div class="room-badge" id="roomBadge">
      <svg viewBox="0 0 24 24"><path d="M16 11c1.66 0 3-1.34 3-3s-1.34-3-3-3-3 1.34-3 3 1.34 3 3 3zm-8 0c1.66 0 3-1.34 3-3S9.66 5 8 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5z"/></svg>
      <span id="roomCodeText">Комната не задана</span>
    </div>
  </div>

  <div class="titlebar-right">
    <div class="mono-pill">
      <span class="live-dot"></span>
      <span>TELEMETRY LIVE</span>
    </div>

    <button class="win-btn" id="themeToggleBtn" onclick="toggleTheme()" title="Сменить тему">
      <svg id="themeIconSun" viewBox="0 0 24 24" style="display:none;"><path d="M12 7c-2.76 0-5 2.24-5 5s2.24 5 5 5 5-2.24 5-5-2.24-5-5-5zM2 13h2c.55 0 1-.45 1-1s-.45-1-1-1H2c-.55 0-1 .45-1 1s.45 1 1 1zm18 0h2c.55 0 1-.45 1-1s-.45-1-1-1h-2c-.55 0-1 .45-1 1s.45 1 1 1zM11 2v2c0 .55.45 1 1 1s1-.45 1-1V2c0-.55-.45-1-1-1s-1 .45-1 1zm0 18v2c0 .55.45 1 1 1s1-.45 1-1v-2c0-.55-.45-1-1-1s-1 .45-1 1zM5.99 4.58a.996.996 0 0 0-1.41 0 .996.996 0 0 0 0 1.41l1.06 1.06c.39.39 1.03.39 1.41 0s.39-1.03 0-1.41L5.99 4.58zm12.37 12.37a.996.996 0 0 0-1.41 0 .996.996 0 0 0 0 1.41l1.06 1.06c.39.39 1.03.39 1.41 0a.996.996 0 0 0 0-1.41l-1.06-1.06zm1.06-10.96a.996.996 0 0 0 0-1.41.996.996 0 0 0-1.41 0l-1.06 1.06c-.39.39-.39 1.03 0 1.41s1.03.39 1.41 0l1.06-1.06zM7.05 18.36a.996.996 0 0 0 0-1.41.996.996 0 0 0-1.41 0l-1.06 1.06c-.39.39-.39 1.03 0 1.41s1.03.39 1.41 0l1.06-1.06z"/></svg>
      <svg id="themeIconMoon" viewBox="0 0 24 24"><path d="M12 3a9 9 0 1 0 9 9c0-.46-.04-.92-.1-1.36a5.389 5.389 0 0 1-4.4 2.26 5.403 5.403 0 0 1-3.14-9.8c-.44-.06-.9-.1-1.36-.1z"/></svg>
    </button>

    <div class="win-controls">
      <button class="win-btn" onclick="api('wmin')"><svg viewBox="0 0 24 24"><path d="M5 11h14v2H5z"/></svg></button>
      <button class="win-btn" onclick="api('wmax')"><svg viewBox="0 0 24 24"><path d="M4 4h16v16H4V4zm2 2v12h12V6H6z"/></svg></button>
      <button class="win-btn close" onclick="api('wclose')"><svg viewBox="0 0 24 24"><path d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/></svg></button>
    </div>
  </div>
</div>

<!-- Tabs Navigation -->
<div id="nav">
  <div class="nav-tabs">
    <button class="tab-btn active" data-tab="tab-dashboard">
      <svg viewBox="0 0 24 24"><path d="M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z"/></svg>
      Главная
    </button>
    <button class="tab-btn" data-tab="tab-party">
      <svg viewBox="0 0 24 24"><path d="M16 11c1.66 0 3-1.34 3-3s-1.34-3-3-3-3 1.34-3 3 1.34 3 3 3zm-8 0c1.66 0 3-1.34 3-3S9.66 5 8 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5z"/></svg>
      Пати
    </button>
    <button class="tab-btn" data-tab="tab-history">
      <svg viewBox="0 0 24 24"><path d="M13 3a9 9 0 0 0-9 9H1l3.89 3.89.07.14L9 12H6a7 7 0 1 1 2.05 4.95l-1.42 1.42A9 9 0 1 0 13 3zm-1 5v5l4.25 2.52.77-1.28-3.52-2.09V8z"/></svg>
      История
    </button>
    <button class="tab-btn" data-tab="tab-stats">
      <svg viewBox="0 0 24 24"><path d="M5 9.2h3V19H5zM10.6 5h2.8v14h-2.8zm5.6 8H19v6h-2.8z"/></svg>
      Статистика
    </button>
    <button class="tab-btn" data-tab="tab-overlay">
      <svg viewBox="0 0 24 24"><path d="M21 3H3c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h18c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H3V5h18v14zM5 15h14v3H5z"/></svg>
      Оверлей
    </button>
    <button class="tab-btn" data-tab="tab-settings">
      <svg viewBox="0 0 24 24"><path d="M19.14 12.94c.04-.3.06-.61.06-.94 0-.32-.02-.64-.07-.94l2.03-1.58a.49.49 0 0 0 .12-.61l-1.92-3.32a.49.49 0 0 0-.59-.22l-2.39.96c-.5-.38-1.03-.7-1.62-.94l-.36-2.54a.48.48 0 0 0-.48-.41h-3.84c-.24 0-.43.17-.47.41l-.36 2.54c-.59.24-1.13.57-1.62.94l-2.39-.96a.49.49 0 0 0-.59.22L2.74 8.87c-.12.21-.08.47.12.61l2.03 1.58c-.05.3-.09.63-.09.94s.02.64.07.94l-2.03 1.58a.49.49 0 0 0-.12.61l1.92 3.32c.12.22.37.29.59.22l2.39-.96c.5.38 1.03.7 1.62.94l.36 2.54c.05.24.24.41.48.41h3.84c.24 0 .44-.17.47-.41l.36-2.54c.59-.24 1.13-.56 1.62-.94l2.39.96c.22.08.47 0 .59-.22l1.92-3.32c.12-.22.07-.47-.12-.61l-2.01-1.58zM12 15.6c-1.98 0-3.6-1.62-3.6-3.6s1.62-3.6 3.6-3.6 3.6 1.62 3.6 3.6-1.62 3.6-3.6 3.6z"/></svg>
      Настройки
    </button>
  </div>
  <div class="nav-meta">WIN32 CLIENT GDI+</div>
</div>

<!-- Main Area -->
<div id="main">
  <div id="main-inner">

  <!-- TAB: ГЛАВНАЯ -->
  <div id="tab-dashboard" class="tab-view active">
    <div class="grid-2">
      <div class="card">
        <div class="card-title">
          <div class="card-title-left">
            <svg viewBox="0 0 24 24"><path d="M4 6h16a2 2 0 0 1 2 2v3H2V8a2 2 0 0 1 2-2zm-2 7h20v5a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2v-5zm9-1v3h2v-3h-2z"/></svg>
            <span>Сундуков всего</span>
          </div>
          <span class="mono-pill">LIFETIME TELEMETRY</span>
        </div>
        <div class="hero-split">
          <div class="hero-number">
            <span id="totalChests">0</span>
            <span class="unit">добыто тобой</span>
          </div>
          <div style="text-align:right;">
            <div style="font-family:var(--mono); font-size:10px; color:var(--text-ghost); margin-bottom:4px; letter-spacing:0.5px;">8-MATCH TREND</div>
            <svg id="lifetimeSparkline" width="128" height="32" viewBox="0 0 128 32" style="overflow:visible;">
              <polyline fill="none" stroke="var(--gold)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" points="4,24 24,20 44,22 64,14 84,18 104,10 124,8" />
            </svg>
          </div>
        </div>
        <div class="well-row">
          <div class="well-box">
            <span>За сессию</span>
            <b id="sessionChests" style="color:var(--green-bright);">+0</b>
          </div>
          <div class="well-box">
            <span>Последний матч</span>
            <b id="lastMatchChestsVal">0</b>
          </div>
        </div>
      </div>

      <div class="card">
        <div class="card-title">
          <div class="card-title-left">
            <svg viewBox="0 0 24 24"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 14H9V8h2v8zm4 0h-2V8h2v8z"/></svg>
            <span>Текущий матч</span>
          </div>
          <span class="mono-pill green"><span class="live-dot"></span> LIVE</span>
        </div>
        <div class="hero-split">
          <div class="hero-number">
            <span id="currentMatchChests">0</span>
            <span class="unit">сундуков</span>
          </div>
          <svg width="42" height="42" viewBox="0 0 42 42">
            <circle cx="21" cy="21" r="17" fill="none" stroke="var(--border)" stroke-width="3" />
            <circle id="matchProgressRing" cx="21" cy="21" r="17" fill="none" stroke="var(--green-bright)" stroke-width="3" stroke-linecap="round" stroke-dasharray="106.8" stroke-dashoffset="72" transform="rotate(-90 21 21)" />
          </svg>
        </div>
        <div class="well-row">
          <div class="well-box">
            <span>Время в матче</span>
            <b id="matchTimer">00:00</b>
          </div>
          <div class="well-box">
            <span>Темп:</span>
            <b id="currentMatchCpm">0.0/мин</b>
          </div>
        </div>
      </div>
    </div>

    <!-- Squad Members in App -->
    <div class="card">
      <div class="card-title">
        <div class="card-title-left">
          <svg viewBox="0 0 24 24"><path d="M16 11c1.66 0 3-1.34 3-3s-1.34-3-3-3-3 1.34-3 3 1.34 3 3 3zm-8 0c1.66 0 3-1.34 3-3S9.66 5 8 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5z"/></svg>
          <span>Игроки в комнате</span>
        </div>
        <span class="mono-pill green" id="squadCount">В сети: 1</span>
      </div>

      <div id="squadGrid" class="squad-grid">
        <!-- Rendered dynamically -->
      </div>
    </div>
  </div>

  <!-- TAB: ПАТИ -->
  <div id="tab-party" class="tab-view">
    <div class="grid-split">
      <div class="card">
        <div class="card-title">
          <div class="card-title-left">
            <svg viewBox="0 0 24 24"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 17.93c-3.95-.49-7-3.85-7-7.93 0-.62.08-1.21.21-1.79L9 15v1c0 1.1.9 2 2 2v1.93zm6.9-2.54c-.26-.81-1-1.39-1.9-1.39h-1v-3c0-.55-.45-1-1-1H8v-2h2c.55 0 1-.45 1-1V7h2c1.1 0 2-.9 2-2v-.41c2.93 1.19 5 4.06 5 7.41 0 2.08-.8 3.97-2.1 5.39z"/></svg>
            <span>Комната и подключение</span>
          </div>
          <span class="mono-pill green">SYNC_OK</span>
        </div>
        <p style="color:var(--text-muted); font-size:13px;">
          Введите одинаковое кодовое слово с друзьями, чтобы видеть сундуки и темп друг друга в реальном времени.
        </p>

        <div class="form-group">
          <label class="form-label">Код комнаты</label>
          <div class="input-row">
            <input type="text" id="roomInput" class="input" style="font-family:var(--mono);" placeholder="Введите код комнаты...">
            <button class="btn primary" onclick="saveRoom()">Подключиться</button>
          </div>
          <div class="input-row" style="margin-top:6px;">
            <button class="btn" style="flex:1;" onclick="genRoom()">Случайный</button>
            <button class="btn" style="flex:1;" onclick="copyRoom()">Скопировать</button>
          </div>
        </div>

        <div style="background:var(--surface-well); border:1px solid var(--border); border-radius:6px; padding:10px 14px; font-family:var(--mono); font-size:11px; color:var(--text-muted); display:flex; flex-direction:column; gap:4px;">
          <div style="display:flex; justify-content:space-between;"><span>STORAGE:</span><span style="color:var(--green-bright);">RAM ONLY (NO DISK SAVE)</span></div>
          <div style="display:flex; justify-content:space-between;"><span>ISOLATION:</span><span style="color:var(--gold-bright);">STRICT ROOM FILTER</span></div>
        </div>
      </div>

      <div style="display:flex; flex-direction:column; gap:16px;">
        <div class="card">
          <div class="card-title">
            <span>Суммарная телеметрия сквада</span>
            <span class="mono-pill">MATCH LIVE</span>
          </div>
          <div style="display:grid; grid-template-columns: repeat(3, 1fr); gap:10px;">
            <div class="stat-box">
              <div class="stat-label">Всего в матче</div>
              <div class="stat-val" id="partyTotalMatch">0</div>
            </div>
            <div class="stat-box">
              <div class="stat-label">За сессию</div>
              <div class="stat-val highlight" id="partyTotalSession">0</div>
            </div>
            <div class="stat-box">
              <div class="stat-label">Участников</div>
              <div class="stat-val" id="partyPeerCount">1</div>
            </div>
          </div>
        </div>

        <div class="card">
          <div class="card-title"><span>Подключенные игроки</span></div>
          <div id="partyList" style="display:flex; flex-direction:column; gap:10px;">
            <span style="color:var(--text-muted); font-size:13px;">Никого нет. Введите код комнаты.</span>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- TAB: ИСТОРИЯ -->
  <div id="tab-history" class="tab-view">
    <div class="grid-4">
      <div class="stat-box">
        <div class="stat-label">Всего матчей</div>
        <div class="stat-val" id="histTotalMatches">0</div>
      </div>
      <div class="stat-box">
        <div class="stat-label">Средний темп</div>
        <div class="stat-val highlight" id="histAvgCpm">0.0</div>
      </div>
      <div class="stat-box">
        <div class="stat-label">Топ локация</div>
        <div class="stat-val" style="font-size:18px; padding-top:4px;" id="histTopPoi">—</div>
      </div>
      <div class="stat-box">
        <div class="stat-label">Parser Sync</div>
        <div class="stat-val" style="color:var(--green-bright);">OK</div>
      </div>
    </div>

    <div class="card">
      <div class="card-title">
        <div class="card-title-left">
          <svg viewBox="0 0 24 24"><path d="M13 3a9 9 0 0 0-9 9H1l3.89 3.89.07.14L9 12H6a7 7 0 1 1 2.05 4.95l-1.42 1.42A9 9 0 1 0 13 3zm-1 5v5l4.25 2.52.77-1.28-3.52-2.09V8z"/></svg>
          <span>История матчей</span>
        </div>
        <span class="mono-pill" id="histShownBadge">Показано: 0</span>
      </div>
      
      <div style="display:flex; flex-direction:column; gap:10px;">
        <input type="text" id="historySearch" class="input" style="width:100%;" placeholder="Поиск по дате, локации или ID..." oninput="filterHistory()">
        <div id="historyFilterChips" class="filter-chips"></div>
      </div>

      <table class="table">
        <thead>
          <tr>
            <th>Дата</th>
            <th>Локация</th>
            <th>Время</th>
            <th>Сундуков добыто</th>
            <th>Счёт Пати</th>
          </tr>
        </thead>
        <tbody id="historyTableBody">
          <tr><td colspan="5" style="text-align:center; color:var(--text-muted);">Матчей пока нет</td></tr>
        </tbody>
      </table>
    </div>
  </div>

  <!-- TAB: СТАТИСТИКА -->
  <div id="tab-stats" class="tab-view">
    <div class="card">
      <div class="card-title">
        <div class="card-title-left">
          <svg viewBox="0 0 24 24"><path d="M5 9.2h3V19H5zM10.6 5h2.8v14h-2.8zm5.6 8H19v6h-2.8z"/></svg>
          <span>Сундуки по дням за неделю</span>
        </div>
        <span id="statsWeekTotal" style="color:var(--gold-bright); font-family:var(--mono); font-size:15px; font-weight:700;">0 сундуков</span>
      </div>
      <p style="color:var(--text-muted); font-size:13px;">
        Нажмите на любой день недели на графике, чтобы посмотреть подробную статистику за этот день.
      </p>

      <div class="chart-container" id="weeklyChartContainer">
        <!-- Rendered dynamically -->
      </div>
    </div>

    <div class="grid-split">
      <!-- Selected Day Detailed Stats Card -->
      <div class="card" id="dayDetailCard">
        <div class="card-title">
          <span id="dayDetailTitle">Показатели за день</span>
          <span id="dayDetailDate" class="mono-pill amber">Сегодня</span>
        </div>

        <div style="display:grid; grid-template-columns: repeat(2, 1fr); gap:12px;">
          <div class="stat-box">
            <div class="stat-label">Сундуков в час</div>
            <div class="stat-val" id="dayChestsPerHour">0</div>
            <div class="stat-sub">интенсивность сбора</div>
          </div>

          <div class="stat-box">
            <div class="stat-label">Время в игре</div>
            <div class="stat-val" id="dayTimeInGame">0 мин</div>
            <div class="stat-sub">чистое время в катках</div>
          </div>

          <div class="stat-box">
            <div class="stat-label">В среднем за катку</div>
            <div class="stat-val" id="dayAvgChests">0</div>
            <div class="stat-sub">средняя добыча</div>
          </div>

          <div class="stat-box">
            <div class="stat-label">Всего сундуков</div>
            <div class="stat-val highlight" id="dayTotalChests">0</div>
            <div class="stat-sub" id="dayMatchCount">Сыграно каток: 0</div>
          </div>
        </div>

        <div style="margin-top:6px;">
          <div style="font-size:11px; font-weight:700; color:var(--text-ghost); text-transform:uppercase; letter-spacing:0.6px; margin-bottom:8px;">Катки за этот день</div>
          <table class="table">
            <thead>
              <tr>
                <th>Время</th>
                <th>Локация</th>
                <th>Длительность</th>
                <th>Сундуков добыто</th>
                <th>В минуту (CPM)</th>
              </tr>
            </thead>
            <tbody id="dayMatchesTableBody"></tbody>
          </table>
        </div>
      </div>

      <!-- Overall Top Locations Card -->
      <div class="card" id="topLocationsCard">
        <div class="card-title">
          <span>Популярные локации (Drop Spots)</span>
          <span class="mono-pill">по всем матчам</span>
        </div>
        <p style="color:var(--text-muted); font-size:13px;">
          Локации, где вы приземляетесь и открываете больше всего сундуков.
        </p>
        <div id="topLocationsList" style="display:flex; flex-direction:column; gap:8px;"></div>
      </div>
    </div>
  </div>

  <!-- TAB: ОВЕРЛЕЙ -->
  <div id="tab-overlay" class="tab-view">
    <div class="grid-split">
      <div class="card">
        <div class="card-title">
          <div class="card-title-left">
            <svg viewBox="0 0 24 24"><path d="M21 3H3c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h18c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H3V5h18v14zM5 15h14v3H5z"/></svg>
            <span>Игровой оверлей</span>
          </div>
          <span class="mono-pill green">GDI+ ACTIVE</span>
        </div>

        <div style="display:flex; flex-wrap:wrap; gap:8px;">
          <span class="mono-pill amber">Ctrl + Shift + O — скрыть / показать</span>
          <span class="mono-pill">Ctrl + Shift + L — закрепить</span>
        </div>

        <div class="overlay-stage">
          <div style="width:100%; display:flex; justify-content:space-between; font-family:var(--mono); font-size:10px; color:var(--text-ghost);">
            <span>1920x1080 // SCALED VIEWPORT</span>
            <span>GDI+ TRANSLUCENT</span>
          </div>
          <div id="hudPreviewBox" style="background:rgba(14,17,23,0.7); border:1px solid rgba(217,119,6,0.5); border-radius:8px; padding:14px 16px; width:100%; max-width:430px; transition:background 150ms ease, border-color 150ms ease;">
            <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:8px; font-family:var(--mono); font-size:10px; color:var(--text-ghost);">
              <span>GDI+ OVERLAY PREVIEW // 430PX</span>
              <span style="color:var(--green-bright);">LIVE</span>
            </div>
            <table id="hudPreviewTable" style="width:100%; border-collapse:collapse; text-align:left; font-size:13px; font-family:var(--mono); opacity:1.0; transition:opacity 150ms ease;">
              <thead>
                <tr style="color:#94A3B8; font-size:11px; font-weight:700; border-bottom:1px solid rgba(255,255,255,0.12);">
                  <th style="padding-bottom:6px;">Player</th>
                  <th style="padding-bottom:6px;">Last</th>
                  <th style="padding-bottom:6px;">All</th>
                  <th style="padding-bottom:6px;">Session</th>
                </tr>
              </thead>
              <tbody id="overlayPrevTableBody"></tbody>
            </table>
          </div>
        </div>
      </div>

      <div class="card">
        <div class="card-title"><span>Параметры отображения</span></div>

        <div class="form-group">
          <label class="form-label" style="display:flex; justify-content:space-between;">
            <span>Прозрачность фона (GUI)</span>
            <span id="opacityVal" style="color:var(--gold-bright); font-family:var(--mono);">70%</span>
          </label>
          <input type="range" id="opacitySlider" min="0" max="100" value="70" style="width:100%; accent-color:var(--gold); cursor:pointer;" oninput="onOpacityChange(this.value)">
          <div style="display:flex; justify-content:space-between; font-size:10px; color:var(--text-ghost); font-family:var(--mono);">
            <span>0% (Прозрачный)</span><span>100% (Плотный)</span>
          </div>
        </div>

        <div class="form-group" style="margin-top:6px;">
          <label class="form-label" style="display:flex; justify-content:space-between;">
            <span>Прозрачность текста</span>
            <span id="textOpacityVal" style="color:var(--gold-bright); font-family:var(--mono);">100%</span>
          </label>
          <input type="range" id="textOpacitySlider" min="10" max="100" value="100" style="width:100%; accent-color:var(--gold); cursor:pointer;" oninput="onTextOpacityChange(this.value)">
          <div style="display:flex; justify-content:space-between; font-size:10px; color:var(--text-ghost); font-family:var(--mono);">
            <span>10% (Контур)</span><span>100% (Четкий)</span>
          </div>
        </div>

        <div style="margin-top:10px; background:var(--surface-well); border:1px solid var(--border); border-radius:6px; padding:10px 12px; display:flex; align-items:center; gap:10px;">
          <input type="checkbox" id="onlyInGameCheckbox" style="accent-color:var(--gold); width:16px; height:16px; cursor:pointer;" onchange="toggleOnlyInGame(this.checked)">
          <label for="onlyInGameCheckbox" style="color:var(--text-main); font-size:13px; cursor:pointer;">
            Показывать только в игре
          </label>
        </div>

        <div style="display:flex; gap:10px; margin-top:10px;">
          <button class="btn primary" style="flex:1;" onclick="api('toggleOverlay')">Скрыть / Показать</button>
          <button class="btn" style="flex:1;" onclick="api('toggleLock')">Закрепить / Разблокировать</button>
        </div>
      </div>
    </div>
  </div>

  <!-- TAB: НАСТРОЙКИ -->
  <div id="tab-settings" class="tab-view">
    <div class="grid-split">
      <div class="card">
        <div class="card-title">
          <span>Настройки профиля</span>
          <span class="mono-pill">CFG // USER</span>
        </div>

        <div class="form-group">
          <label class="form-label">Никнейм в игре (Epic Games)</label>
          <div class="input-row">
            <input type="text" id="profileInput" class="input" placeholder="Ваш ник в игре...">
            <button class="btn primary" onclick="saveProfile()">Сохранить</button>
          </div>
        </div>

        <div class="form-group" style="margin-top:6px;">
          <label class="form-label">Папка реплеев Fortnite</label>
          <div class="input-row">
            <input type="text" id="demosInput" class="input" style="font-family:var(--mono); font-size:12px;" readonly>
          </div>
        </div>

        <div class="form-group" style="margin-top:6px;">
          <label class="form-label">Добавить сундуки вручную</label>
          <div class="input-row">
            <input type="number" id="manualInput" class="input" style="font-family:var(--mono);" placeholder="+100">
            <button class="btn" onclick="saveManual()">Добавить</button>
          </div>
        </div>
      </div>

      <div class="card">
        <div class="card-title">
          <span>Обновление программы</span>
          <span class="mono-pill" id="currentVersionBadge">v2.1.1</span>
        </div>
        <p style="color:var(--text-muted); font-size:13px;">
          Проверка и автоматическая установка обновлений программы напрямую из релизов GitHub.
        </p>

        <div class="form-group">
          <label class="form-label">Репозиторий GitHub (owner/repo)</label>
          <div class="input-row">
            <input type="text" id="updateRepoInput" class="input" style="font-family:var(--mono);" placeholder="например, fjdjdhidjfj/LootPulse">
            <button class="btn" onclick="saveUpdateRepo()">Сохранить</button>
          </div>
        </div>

        <div style="display:flex; align-items:center; gap:8px; margin-top:4px;">
          <input type="checkbox" id="autoUpdateCheckbox" style="accent-color:var(--gold); width:16px; height:16px; cursor:pointer;" onchange="toggleAutoUpdate(this.checked)">
          <label for="autoUpdateCheckbox" style="color:var(--text-main); font-size:13px; cursor:pointer;">
            Проверять обновления при запуске
          </label>
        </div>

        <div style="display:flex; gap:12px; align-items:center; margin-top:6px;">
          <button class="btn primary" id="checkUpdateBtn" onclick="checkUpdates(true)">Проверить обновление</button>
          <span id="updateCheckStatus" style="font-size:13px; color:var(--text-muted);"></span>
        </div>

        <div id="updateNoticeBox" style="display:none; margin-top:8px; background:var(--surface-well); border:1px solid var(--border-active); border-radius:8px; padding:14px 16px;">
          <div style="display:flex; justify-content:space-between; align-items:center;">
            <div>
              <div style="font-size:14px; font-weight:700; color:var(--gold-bright);" id="updateNoticeTitle">Доступна новая версия</div>
              <div style="font-size:12px; color:var(--text-muted); margin-top:2px;" id="updateNoticeMeta"></div>
            </div>
            <button class="btn primary" id="applyUpdateBtn" onclick="applyUpdate()">Обновить сейчас</button>
          </div>
          <div id="updateChangelog" style="font-size:12px; color:var(--text-main); margin-top:10px; border-top:1px solid var(--border); padding-top:10px; line-height:1.5;"></div>
        </div>
      </div>
    </div>
  </div>

  </div>
</div>

<!-- Telemetry Status Bar -->
<div id="statusbar">
  <span>LOOTPULSE COMPANION // TELEMETRY ACTIVE</span>
  <span>REPLAY PARSER: OK</span>
</div>

<div id="toast" class="toast"></div>

<script>
const TOKEN = "{{TOKEN}}";
let state = null;
let selectedDayIndex = 6;
let weeklyDaysData = [];
let activePoiFilter = '';

function applyTheme(theme) {
  document.documentElement.setAttribute('data-theme', theme);
  const sun = document.getElementById('themeIconSun');
  const moon = document.getElementById('themeIconMoon');
  if (sun && moon) {
    sun.style.display = theme === 'light' ? 'block' : 'none';
    moon.style.display = theme === 'dark' ? 'block' : 'none';
  }
}
function toggleTheme() {
  const cur = document.documentElement.getAttribute('data-theme') || 'dark';
  const next = cur === 'dark' ? 'light' : 'dark';
  localStorage.setItem('lootpulse_theme', next);
  applyTheme(next);
}
applyTheme(localStorage.getItem('lootpulse_theme') || 'dark');

// Tab switcher
document.querySelectorAll('.tab-btn').forEach(btn => {
  btn.addEventListener('click', () => {
    document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
    document.querySelectorAll('.tab-view').forEach(v => v.classList.remove('active'));
    btn.classList.add('active');
    const target = btn.getAttribute('data-tab');
    document.getElementById(target).classList.add('active');
  });
});

// Titlebar dragging & maximizing
const titlebar = document.getElementById('titlebar');
if (titlebar) {
  titlebar.addEventListener('mousedown', (e) => {
    if (e.target.closest('.win-btn') || e.target.closest('button') || e.target.closest('input')) return;
    if (e.button === 0) {
      if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage('wdrag');
      } else {
        api('wdrag');
      }
    }
  });

  titlebar.addEventListener('dblclick', (e) => {
    if (e.target.closest('.win-btn') || e.target.closest('button') || e.target.closest('input')) return;
    if (window.chrome && window.chrome.webview) {
      window.chrome.webview.postMessage('wmax');
    } else {
      api('wmax');
    }
  });
}

function showToast(msg) {
  const t = document.getElementById('toast');
  t.innerText = msg;
  t.style.display = 'block';
  setTimeout(() => { t.style.display = 'none'; }, 2400);
}

async function api(act, param = "") {
  try {
    const res = await fetch(`/action?act=${encodeURIComponent(act)}&val=${encodeURIComponent(param)}&token=${TOKEN}`);
    return await res.json();
  } catch(e) { console.error(e); }
}

async function pollState() {
  try {
    const res = await fetch(`/state?token=${TOKEN}`);
    if (res.ok) {
      state = await res.json();
      if (state.notification) showToast(state.notification);
      render();
    }
  } catch(e) {}
  setTimeout(pollState, 1500);
}

function render() {
  if (!state) return;

  const roomText = state.roomCode ? `SYNC: ${state.roomCode}` : 'Комната не задана';
  document.getElementById('roomCodeText').innerText = roomText;

  // Главная
  document.getElementById('totalChests').innerText = (state.totalChests || 0).toLocaleString();
  document.getElementById('sessionChests').innerText = `+${state.sessionChests || 0}`;
  document.getElementById('lastMatchChestsVal').innerText = state.lastMatchChests || 0;

  document.getElementById('currentMatchChests').innerText = state.currentMatchChests || 0;
  document.getElementById('matchTimer').innerText = fmtTime(state.matchSeconds);

  const cpmElem = document.getElementById('currentMatchCpm');
  if (cpmElem) {
    const cpm = state.currentMatchCpm || 0;
    cpmElem.innerText = `${cpm.toFixed(1)}/мин`;
    cpmElem.style.color = cpm >= 1.5 ? 'var(--green-bright)' : (cpm >= 0.8 ? 'var(--gold-bright)' : 'var(--text-muted)');
  }

  // Update 8-Match Sparkline & Live Ring
  const spark = document.getElementById('lifetimeSparkline');
  if (spark && state.matches && state.matches.length > 1) {
    const recent = state.matches.slice(-8).map(m => m.chests || 0);
    const maxC = Math.max(1, ...recent);
    const pts = recent.map((c, i) => {
      const x = 4 + (i * (120 / Math.max(1, recent.length - 1)));
      const y = 28 - Math.round((c / maxC) * 22);
      return `${x.toFixed(1)},${y}`;
    }).join(' ');
    spark.innerHTML = `<polyline fill="none" stroke="var(--gold)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" points="${pts}" />`;
  }

  const ring = document.getElementById('matchProgressRing');
  if (ring) {
    const ratio = Math.min(1, (state.currentMatchChests || 0) / 20);
    ring.setAttribute('stroke-dashoffset', (106.8 * (1 - ratio)).toFixed(1));
  }

  // Squad Grid (Dashboard)
  const squadGrid = document.getElementById('squadGrid');
  squadGrid.innerHTML = '';

  const myCard = document.createElement('div');
  myCard.className = 'teammate-card me';
  const myCpm = (state.currentMatchCpm || 0).toFixed(1);
  myCard.innerHTML = `
    <div class="teammate-header">
      <span class="teammate-name"><span class="live-dot" style="background:var(--gold);"></span> ${state.profile || 'Вы'}</span>
      <span class="mono-pill amber">LOCAL</span>
    </div>
    <div class="teammate-rows">
      <div class="teammate-row"><span>Матч</span><b style="color:var(--green-bright);">${state.currentMatchChests || 0} (${myCpm}/м)</b></div>
      <div class="teammate-row"><span>Сессия</span><b>${state.sessionChests || 0}</b></div>
      <div class="teammate-row"><span>Всего</span><b>${(state.totalChests || 0).toLocaleString()}</b></div>
    </div>
  `;
  squadGrid.appendChild(myCard);

  const peers = state.peers || [];
  document.getElementById('squadCount').innerText = peers.length > 0 ? `В сети: ${peers.length + 1}` : 'В сети: 1 (Соло)';

  peers.forEach(p => {
    const card = document.createElement('div');
    card.className = 'teammate-card';
    card.innerHTML = `
      <div class="teammate-header">
        <span class="teammate-name"><span class="live-dot"></span> ${p.name}</span>
        <span class="mono-pill">SQUAD</span>
      </div>
      <div class="teammate-rows">
        <div class="teammate-row"><span>Матч</span><b>${p.matchChests}</b></div>
        <div class="teammate-row"><span>Сессия</span><b>${p.sessionChests}</b></div>
        <div class="teammate-row"><span>Всего</span><b>${p.allChests > 0 ? p.allChests.toLocaleString() : '-'}</b></div>
      </div>
    `;
    squadGrid.appendChild(card);
  });

  // Party Tab Telemetry & Roster
  const totalMatchSquad = (state.currentMatchChests || 0) + peers.reduce((a, p) => a + (p.matchChests || 0), 0);
  const totalSessionSquad = (state.sessionChests || 0) + peers.reduce((a, p) => a + (p.sessionChests || 0), 0);
  const pm = document.getElementById('partyTotalMatch'); if (pm) pm.innerText = totalMatchSquad;
  const ps = document.getElementById('partyTotalSession'); if (ps) ps.innerText = totalSessionSquad;
  const pc = document.getElementById('partyPeerCount'); if (pc) pc.innerText = peers.length + 1;

  const partyList = document.getElementById('partyList');
  if (partyList) {
    const allMembers = [
      { name: state.profile || 'Вы', isMe: true, match: state.currentMatchChests || 0, session: state.sessionChests || 0, all: state.totalChests || 0 }
    ].concat(peers.map(p => ({ name: p.name, isMe: false, match: p.matchChests || 0, session: p.sessionChests || 0, all: p.allChests || 0 })));

    const denom = Math.max(1, totalSessionSquad);
    partyList.innerHTML = allMembers.map(m => {
      const sharePct = Math.round((m.session / denom) * 100);
      const initial = (m.name || 'P').charAt(0).toUpperCase();
      return `
        <div class="party-peer-card ${m.isMe ? 'me' : ''}">
          <div style="display:flex; justify-content:space-between; align-items:center;">
            <div style="display:flex; align-items:center; gap:10px;">
              <div class="monogram">${initial}</div>
              <div>
                <div style="display:flex; align-items:center; gap:8px;">
                  <span style="font-weight:700; color:var(--text-main);">${m.name}</span>
                  <span class="mono-pill ${m.isMe ? 'amber' : ''}">${m.isMe ? 'ХОСТ // YOU' : 'SQUAD'}</span>
                </div>
              </div>
            </div>
            <span class="mono-pill green"><span class="live-dot"></span> ONLINE</span>
          </div>
          <div class="peer-metrics-3">
            <div><div class="peer-metric-label">В матче</div><div class="peer-metric-val">${m.match}</div></div>
            <div><div class="peer-metric-label">Сессия</div><div class="peer-metric-val" style="color:var(--gold-bright);">${m.session}</div></div>
            <div><div class="peer-metric-label">Всего</div><div class="peer-metric-val">${m.all > 0 ? m.all.toLocaleString() : '-'}</div></div>
          </div>
          <div>
            <div style="display:flex; justify-content:space-between; font-size:11px; color:var(--text-ghost); font-family:var(--mono);">
              <span>ВКЛАД В СЕССИЮ</span><span>${sharePct}%</span>
            </div>
            <div class="share-track"><div class="share-fill" style="width:${sharePct}%;"></div></div>
          </div>
        </div>
      `;
    }).join('');
  }

  // History Summary & Table
  renderHistorySummary();
  filterHistory();

  // Weekly Stats
  renderWeeklyStats();

  // HUD Table Preview
  const prevTbody = document.getElementById('overlayPrevTableBody');
  if (prevTbody) {
    const myName = (state.profile || 'Вы') + ' (Вы)';
    let rowsHtml = `
      <tr style="color:var(--gold-bright); font-weight:700;">
        <td style="padding:6px 0;">${myName}</td>
        <td>${state.lastMatchChests || 0}</td>
        <td style="color:#F1F5F9;">${(state.totalChests || 0).toLocaleString()}</td>
        <td>${state.sessionChests || 0}</td>
      </tr>
    `;
    peers.forEach(p => {
      rowsHtml += `
        <tr style="color:#F1F5F9;">
          <td style="padding:4px 0;">${p.name}</td>
          <td style="color:var(--gold-bright);">${p.lastChests !== undefined ? p.lastChests : (p.matchChests || 0)}</td>
          <td>${p.allChests > 0 ? p.allChests.toLocaleString() : '-'}</td>
          <td style="color:var(--gold-bright);">${p.sessionChests}</td>
        </tr>
      `;
    });
    prevTbody.innerHTML = rowsHtml;
  }

  // Inputs
  if (document.activeElement.id !== 'roomInput') document.getElementById('roomInput').value = state.roomCode || '';
  if (document.activeElement.id !== 'profileInput') document.getElementById('profileInput').value = state.profile || '';
  document.getElementById('demosInput').value = state.demosFolder || 'Не найдена';

  const slider = document.getElementById('opacitySlider');
  if (slider && document.activeElement !== slider && state.overlayOpacity !== undefined) {
    slider.value = state.overlayOpacity;
    document.getElementById('opacityVal').innerText = state.overlayOpacity + '%';
    const prevBox = document.getElementById('hudPreviewBox');
    if (prevBox) {
      const a = state.overlayOpacity / 100;
      prevBox.style.background = `rgba(14, 17, 23, ${a})`;
      prevBox.style.borderColor = `rgba(217, 119, 6, ${a * 0.7})`;
    }
  }

  const textSlider = document.getElementById('textOpacitySlider');
  if (textSlider && document.activeElement !== textSlider && state.overlayTextOpacity !== undefined) {
    textSlider.value = state.overlayTextOpacity;
    document.getElementById('textOpacityVal').innerText = state.overlayTextOpacity + '%';
    const prevTable = document.getElementById('hudPreviewTable');
    if (prevTable) prevTable.style.opacity = (state.overlayTextOpacity / 100);
  }

  const onlyGameBox = document.getElementById('onlyInGameCheckbox');
  if (onlyGameBox && document.activeElement !== onlyGameBox && state.overlayOnlyInGame !== undefined) {
    onlyGameBox.checked = !!state.overlayOnlyInGame;
  }

  if (state.currentVersion) {
    const vText = 'v' + state.currentVersion;
    const tbBadge = document.getElementById('titlebarVersion');
    if (tbBadge) tbBadge.innerText = vText;
    const curBadge = document.getElementById('currentVersionBadge');
    if (curBadge) curBadge.innerText = vText;
  }

  if (document.activeElement.id !== 'updateRepoInput') {
    const repoIn = document.getElementById('updateRepoInput');
    if (repoIn) repoIn.value = state.updateRepo || '';
  }

  const autoBox = document.getElementById('autoUpdateCheckbox');
  if (autoBox && document.activeElement !== autoBox) autoBox.checked = !!state.autoCheckUpdates;

  if (state.availableUpdate) renderUpdateNotice(state.availableUpdate);
}

function renderHistorySummary() {
  if (!state || !state.matches) return;
  const ms = state.matches;
  const totalEl = document.getElementById('histTotalMatches');
  if (totalEl) totalEl.innerText = ms.length;

  const avgCpmEl = document.getElementById('histAvgCpm');
  if (avgCpmEl) {
    const totalC = ms.reduce((a, m) => a + (m.chests || 0), 0);
    const totalMin = ms.reduce((a, m) => a + (m.duration || 0), 0) / 60;
    avgCpmEl.innerText = totalMin > 0 ? (totalC / totalMin).toFixed(1) : '0.0';
  }

  const topPoiEl = document.getElementById('histTopPoi');
  if (topPoiEl && state.topLocations && state.topLocations.length > 0) {
    topPoiEl.innerText = state.topLocations[0].name;
  }

  const chipsEl = document.getElementById('historyFilterChips');
  if (chipsEl && state.topLocations) {
    const pois = state.topLocations.slice(0, 4).map(l => l.name);
    let html = `<button class="chip-btn ${activePoiFilter === '' ? 'active' : ''}" onclick="setPoiFilter('')">Все матчи</button>`;
    pois.forEach(p => {
      html += `<button class="chip-btn ${activePoiFilter === p ? 'active' : ''}" onclick="setPoiFilter('${p.replace(/'/g, "\\'")}')">${p}</button>`;
    });
    chipsEl.innerHTML = html;
  }
}

function setPoiFilter(poi) {
  activePoiFilter = poi;
  renderHistorySummary();
  filterHistory();
}

function filterHistory() {
  const tbody = document.getElementById('historyTableBody');
  if (!tbody || !state || !state.matches) return;

  const q = (document.getElementById('historySearch')?.value || '').trim().toLowerCase();
  let list = state.matches.slice().reverse();

  if (activePoiFilter) {
    list = list.filter(m => (m.mainLocation || 'Окрестности') === activePoiFilter);
  }
  if (q) {
    list = list.filter(m =>
      (m.date && m.date.toLowerCase().includes(q)) ||
      (m.mainLocation && m.mainLocation.toLowerCase().includes(q)) ||
      (m.sessionId && m.sessionId.toLowerCase().includes(q))
    );
  }

  const shownEl = document.getElementById('histShownBadge');
  if (shownEl) shownEl.innerText = `Показано: ${Math.min(50, list.length)}`;

  if (list.length === 0) {
    tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; color:var(--text-muted); padding:16px;">Матчи не найдены</td></tr>';
    return;
  }

  tbody.innerHTML = list.slice(0, 50).map(m => {
    const cpm = m.duration > 0 ? (m.chests / (m.duration / 60)).toFixed(1) : '0.0';
    return `
      <tr>
        <td style="font-family:var(--mono);">${m.date}</td>
        <td><span class="poi-pill">${m.mainLocation || 'Окрестности'}</span></td>
        <td style="font-family:var(--mono);">${fmtTime(m.duration)}</td>
        <td><b style="color:var(--gold-bright); font-family:var(--mono);">${m.chests}</b> <span class="mono-pill" style="margin-left:6px;">${cpm} CPM</span></td>
        <td style="font-family:var(--mono);"><b>${m.squadChests}</b></td>
      </tr>
    `;
  }).join('');
}

function renderWeeklyStats() {
  if (!state) return;
  const matches = state.matches || [];
  const now = new Date();
  const dayNames = ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'];
  const fullDayNames = ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'];

  weeklyDaysData = [];
  let weekTotalChests = 0;

  for (let i = 6; i >= 0; i--) {
    const d = new Date(now.getFullYear(), now.getMonth(), now.getDate() - i);
    const yyyy = d.getFullYear();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    const dateKey = `${yyyy}-${mm}-${dd}`;
    const dayOfWeek = d.getDay();

    let label = `${dayNames[dayOfWeek]} ${dd}.${mm}`;
    let fullTitle = `${fullDayNames[dayOfWeek]}, ${dd}.${mm}`;
    if (i === 0) { label = `Сегодня (${dd}.${mm})`; fullTitle = `Сегодня (${fullDayNames[dayOfWeek]}, ${dd}.${mm})`; }
    else if (i === 1) { label = `Вчера (${dd}.${mm})`; fullTitle = `Вчера (${fullDayNames[dayOfWeek]}, ${dd}.${mm})`; }

    const dayMatches = matches.filter(m => m.dateIso === dateKey);
    const dayChests = dayMatches.reduce((acc, m) => acc + (m.chests || 0), 0);
    const dayDuration = dayMatches.reduce((acc, m) => acc + (m.duration || 0), 0);

    weekTotalChests += dayChests;
    weeklyDaysData.push({ index: 6 - i, dateKey, label, fullTitle, matches: dayMatches, chests: dayChests, duration: dayDuration, matchCount: dayMatches.length });
  }

  const weekTotalElem = document.getElementById('statsWeekTotal');
  if (weekTotalElem) weekTotalElem.innerText = `${weekTotalChests.toLocaleString()} сундуков`;

  const container = document.getElementById('weeklyChartContainer');
  if (container) {
    const maxChests = Math.max(1, ...weeklyDaysData.map(d => d.chests));
    container.innerHTML = weeklyDaysData.map((d, idx) => {
      const pct = Math.max(3, Math.round((d.chests / maxChests) * 100));
      const isSelected = (idx === selectedDayIndex);
      const isMax = d.chests > 0 && d.chests === maxChests;
      return `
        <div class="chart-col ${isSelected ? 'selected' : ''}" onclick="selectDay(${idx})">
          ${isMax ? '<span class="mono-pill amber" style="font-size:9px; padding:1px 5px; margin-bottom:2px;">MAX</span>' : ''}
          <div class="chart-val">${d.chests}</div>
          <div class="chart-bar-wrap"><div class="chart-bar" style="height:${pct}%;"></div></div>
          <div class="chart-label">${d.label}</div>
        </div>
      `;
    }).join('');
  }

  const topListElem = document.getElementById('topLocationsList');
  if (topListElem && state.topLocations) {
    if (state.topLocations.length === 0) {
      topListElem.innerHTML = '<span style="color:var(--text-muted); font-size:13px;">Данные по локациям появятся после завершения матчей.</span>';
    } else {
      const maxLocChests = Math.max(1, ...state.topLocations.map(l => l.chests));
      topListElem.innerHTML = state.topLocations.slice(0, 7).map(l => {
        const pct = Math.max(4, Math.round((l.chests / maxLocChests) * 100));
        const avg = l.matches > 0 ? (l.chests / l.matches).toFixed(1) : '0.0';
        return `
          <div style="background:var(--surface-well); border:1px solid var(--border); border-radius:8px; padding:10px 14px;">
            <div style="display:flex; justify-content:space-between; align-items:center; font-size:13px;">
              <span><b style="color:var(--text-main);">${l.name}</b> <span style="color:var(--text-ghost); font-size:11px;">&mdash; ${l.matches} матчей</span></span>
              <span><b style="color:var(--gold-bright); font-family:var(--mono);">${l.chests}</b> <span style="color:var(--text-ghost); font-family:var(--mono); font-size:11px;">(${avg} ср.)</span></span>
            </div>
            <div style="background:rgba(128,128,128,0.12); border-radius:4px; height:5px; margin-top:7px; overflow:hidden;">
              <div style="background:var(--gold); width:${pct}%; height:100%; border-radius:4px;"></div>
            </div>
          </div>
        `;
      }).join('');
    }
  }

  renderSelectedDayStats();
}

function selectDay(index) {
  selectedDayIndex = index;
  document.querySelectorAll('.chart-col').forEach((col, idx) => col.classList.toggle('selected', idx === index));
  renderSelectedDayStats();
}

function renderSelectedDayStats() {
  if (!weeklyDaysData || weeklyDaysData.length <= selectedDayIndex) return;
  const d = weeklyDaysData[selectedDayIndex];

  const titleElem = document.getElementById('dayDetailTitle');
  const dateElem = document.getElementById('dayDetailDate');
  if (titleElem) titleElem.innerText = 'Показатели за день';
  if (dateElem) dateElem.innerText = d.fullTitle;

  const hours = d.duration / 3600;
  const cph = hours > 0 ? (d.chests / hours).toFixed(1) : '0';
  document.getElementById('dayChestsPerHour').innerText = cph;

  const durHours = Math.floor(d.duration / 3600);
  const durMin = Math.floor((d.duration % 3600) / 60);
  document.getElementById('dayTimeInGame').innerText = durHours === 0 ? `${durMin} мин` : `${durHours} ч ${durMin} мин`;

  document.getElementById('dayAvgChests').innerText = d.matchCount > 0 ? (d.chests / d.matchCount).toFixed(1) : '0';
  document.getElementById('dayTotalChests').innerText = d.chests.toLocaleString();
  document.getElementById('dayMatchCount').innerText = `Сыграно каток: ${d.matchCount}`;

  const tbody = document.getElementById('dayMatchesTableBody');
  if (tbody) {
    if (!d.matches || d.matches.length === 0) {
      tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; color:var(--text-muted); padding:16px;">В этот день каток не было</td></tr>';
    } else {
      tbody.innerHTML = d.matches.slice().reverse().map(m => {
        const cpm = m.duration > 0 ? (m.chests / (m.duration / 60)).toFixed(1) : '0.0';
        const timeStr = m.date ? (m.date.split(' ')[1] || m.date) : '-';
        const cpmColor = parseFloat(cpm) >= 1.5 ? 'var(--green-bright)' : 'var(--gold-bright)';
        return `
          <tr>
            <td style="font-family:var(--mono);">${timeStr}</td>
            <td><span class="poi-pill">${m.mainLocation || 'Окрестности'}</span></td>
            <td style="font-family:var(--mono);">${fmtTime(m.duration)}</td>
            <td><b style="color:var(--gold-bright); font-family:var(--mono);">${m.chests}</b></td>
            <td style="font-family:var(--mono); color:${cpmColor}; font-weight:600;">${cpm} CPM</td>
          </tr>
        `;
      }).join('');
    }
  }
}

function onOpacityChange(val) {
  document.getElementById('opacityVal').innerText = val + '%';
  const prevBox = document.getElementById('hudPreviewBox');
  if (prevBox) {
    const a = val / 100;
    prevBox.style.background = `rgba(14, 17, 23, ${a})`;
    prevBox.style.borderColor = `rgba(217, 119, 6, ${a * 0.7})`;
  }
  api('setOverlayOpacity', val);
}

function onTextOpacityChange(val) {
  document.getElementById('textOpacityVal').innerText = val + '%';
  const prevTable = document.getElementById('hudPreviewTable');
  if (prevTable) prevTable.style.opacity = (val / 100);
  api('setOverlayTextOpacity', val);
}

async function toggleOnlyInGame(checked) {
  await api('setOverlayOnlyInGame', checked ? 'true' : 'false');
  showToast(checked ? 'Оверлей отображается только в игре' : 'Оверлей отображается всегда');
}

function fmtTime(sec) {
  if (!sec || sec < 0) return '00:00';
  const m = Math.floor(sec / 60);
  const s = sec % 60;
  return `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
}

async function saveRoom() {
  const code = document.getElementById('roomInput').value.trim();
  await api('setRoom', code);
  showToast('Код комнаты сохранён');
}

function genRoom() {
  const words = ['pirate', 'victory', 'tilted', 'chug', 'vault', 'storm', 'nexus', 'slurp'];
  const code = words[Math.floor(Math.random() * words.length)] + Math.floor(10 + Math.random() * 90);
  document.getElementById('roomInput').value = code;
  saveRoom();
}

function copyRoom() {
  const code = document.getElementById('roomInput').value.trim();
  if (code) {
    navigator.clipboard.writeText(code);
    showToast('Код скопирован');
  }
}

async function saveProfile() {
  const prof = document.getElementById('profileInput').value.trim();
  await api('setProfile', prof);
  showToast('Никнейм сохранён');
}

async function saveManual() {
  const val = parseInt(document.getElementById('manualInput').value);
  if (!isNaN(val)) {
    await api('addManual', val.toString());
    document.getElementById('manualInput').value = '';
    showToast('Сундуки добавлены');
  }
}

let pendingUpdateUrl = "";

function renderMarkdown(md) {
  if (!md) return '';
  return md
    .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
    .replace(/^###\s*(.*)$/gm, '<div style="font-weight:700; color:var(--gold-bright); margin-bottom:6px;">$1</div>')
    .replace(/^##\s*(.*)$/gm, '<div style="font-weight:700; font-size:13px; color:var(--gold-bright); margin-bottom:6px;">$1</div>')
    .replace(/\*\*(.*?)\*\*/g, '<b style="color:var(--text-main);">$1</b>')
    .replace(/^-\s+(.*)$/gm, '<div style="margin-left:4px; margin-bottom:4px; display:flex; gap:6px;"><span style="color:var(--gold-bright);">&bull;</span><span>$1</span></div>')
    .replace(/\n\n+/g, '<div style="height:6px;"></div>');
}

function renderUpdateNotice(u) {
  const box = document.getElementById('updateNoticeBox');
  if (!box) return;
  if (!u || !u.version) {
    box.style.display = 'none';
    pendingUpdateUrl = "";
    return;
  }
  pendingUpdateUrl = u.downloadUrl || "";
  document.getElementById('updateNoticeTitle').innerText = `Доступна новая версия: v${u.version}`;
  const mb = u.sizeBytes ? (u.sizeBytes / (1024 * 1024)).toFixed(1) + ' МБ' : '';
  document.getElementById('updateNoticeMeta').innerText = mb ? `Размер: ${mb}` : '';
  const changelogElem = document.getElementById('updateChangelog');
  if (u.changelog) {
    changelogElem.innerHTML = renderMarkdown(u.changelog);
    changelogElem.style.display = 'block';
  } else {
    changelogElem.style.display = 'none';
  }
  box.style.display = 'block';
}

async function checkUpdates(manual = false) {
  const statusElem = document.getElementById('updateCheckStatus');
  const btn = document.getElementById('checkUpdateBtn');
  if (statusElem) statusElem.innerText = 'Проверка обновлений...';
  if (btn) btn.disabled = true;

  try {
    const res = await api('checkUpdate');
    if (btn) btn.disabled = false;
    const hasUp = res && (res.hasUpdate || res.HasUpdate);
    const latestVer = res && (res.latestVersion || res.LatestVersion);
    const dUrl = res && (res.downloadUrl || res.DownloadUrl);
    const sBytes = res && (res.fileSizeBytes !== undefined ? res.fileSizeBytes : res.FileSizeBytes);
    const cl = res && (res.changelog || res.Changelog);
    const errMsg = res && (res.errorMessage || res.ErrorMessage || res.error || res.Error);

    if (hasUp) {
      if (statusElem) statusElem.innerText = `Найдено обновление v${latestVer}`;
      renderUpdateNotice({ version: latestVer, downloadUrl: dUrl, sizeBytes: sBytes, changelog: cl });
      showToast(`Доступна новая версия v${latestVer}`);
    } else if (errMsg) {
      if (statusElem) statusElem.innerText = `Ошибка: ${errMsg}`;
      if (manual) showToast(`Ошибка проверки: ${errMsg}`);
    } else {
      if (statusElem) statusElem.innerText = 'У вас установлена последняя версия';
      if (manual) showToast('У вас последняя версия');
    }
  } catch(e) {
    if (btn) btn.disabled = false;
    if (statusElem) statusElem.innerText = 'Ошибка соединения с GitHub';
  }
}

async function applyUpdate() {
  const btn = document.getElementById('applyUpdateBtn');
  if (btn) {
    btn.disabled = true;
    btn.innerText = 'Загрузка и замена...';
  }
  showToast('Скачивание обновления. Программа перезапустится через пару секунд.');
  try {
    await api('applyUpdate', pendingUpdateUrl);
  } catch(e) {
    showToast('Ошибка при установке обновления');
    if (btn) {
      btn.disabled = false;
      btn.innerText = 'Обновить сейчас';
    }
  }
}

async function saveUpdateRepo() {
  const repo = document.getElementById('updateRepoInput').value.trim();
  await api('setUpdateRepo', repo);
  showToast('Репозиторий обновлений сохранён');
}

async function toggleAutoUpdate(checked) {
  await api('toggleAutoCheckUpdates', checked ? 'true' : 'false');
  showToast(checked ? 'Автопроверка включена' : 'Автопроверка выключена');
}

pollState();
</script>
</body>
</html>
""";
}
