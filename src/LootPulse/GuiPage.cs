using System;
using System.Collections.Generic;

namespace LootPulse;

public static class GuiPage
{
    public static string Render(string token)
    {
        return HtmlTemplate.Replace("{{TOKEN}}", token);
    }

    private const string HtmlTemplate = """
<!DOCTYPE html>
<html lang="ru">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>LootPulse</title>
<style>
:root {
  --bg: #0c0e12;
  --surface: #14171f;
  --surface-hover: #1b202c;
  --border: rgba(255, 255, 255, 0.08);
  --border-active: rgba(229, 169, 60, 0.5);
  --text-main: #f3f4f6;
  --text-muted: #8b92a4;
  --gold: #e5a93c;
  --gold-glow: rgba(229, 169, 60, 0.15);
  --blue: #3b82f6;
  --green: #22c55e;
  --red: #ef4444;
  --font: "Segoe UI Variable Text", "Segoe UI", system-ui, -apple-system, sans-serif;
  --mono: Bahnschrift, "Segoe UI Mono", monospace;
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
}

/* Custom Frameless Titlebar */
#titlebar {
  height: 44px;
  background: #0f1117;
  border-bottom: 1px solid var(--border);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 16px;
  user-select: none;
  cursor: default;
}

.brand {
  display: flex;
  align-items: center;
  gap: 10px;
  font-weight: 700;
  font-size: 15px;
  letter-spacing: 0.5px;
}

.brand svg { width: 22px; height: 22px; fill: var(--gold); }
.brand .badge {
  font-size: 10px;
  background: var(--surface);
  border: 1px solid var(--border);
  padding: 2px 6px;
  border-radius: 4px;
  color: var(--text-muted);
}

.titlebar-right {
  display: flex;
  align-items: center;
  gap: 12px;
}

.room-badge {
  font-size: 12px;
  color: var(--text-muted);
  background: var(--surface);
  border: 1px solid var(--border);
  padding: 4px 10px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  gap: 6px;
}
.room-badge svg { width: 14px; height: 14px; fill: var(--text-muted); }

.win-controls {
  display: flex;
  align-items: center;
  gap: 4px;
}

.win-btn {
  width: 32px;
  height: 32px;
  background: transparent;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
  transition: background 150ms ease, color 150ms ease;
}
.win-btn:hover { background: var(--surface-hover); color: var(--text-main); }
.win-btn.close:hover { background: var(--red); color: #fff; }
.win-btn svg { width: 14px; height: 14px; fill: currentColor; }

/* Navigation Tab Bar */
#nav {
  display: flex;
  gap: 6px;
  padding: 10px 20px;
  background: #0f1117;
  border-bottom: 1px solid var(--border);
}

.tab-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  background: transparent;
  border: 1px solid transparent;
  color: var(--text-muted);
  font-size: 13px;
  font-weight: 600;
  padding: 7px 14px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 140ms ease;
}

.tab-btn:hover { background: var(--surface); color: var(--text-main); }
.tab-btn.active {
  background: var(--surface);
  border-color: var(--border);
  color: var(--gold);
}
.tab-btn svg { width: 16px; height: 16px; fill: currentColor; }

/* Main Content Views */
#main {
  flex: 1;
  overflow-y: auto;
  padding: 24px;
  position: relative;
}

.tab-view {
  display: none;
  flex-direction: column;
  gap: 20px;
  animation: fadeIn 180ms cubic-bezier(0.16, 1, 0.3, 1);
}
.tab-view.active { display: flex; }

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(6px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Grid & Cards */
.grid-2 {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 18px;
}

.card {
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  transition: border-color 150ms ease;
}
.card:hover { border-color: rgba(255, 255, 255, 0.14); }

.card-title {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: var(--text-muted);
  font-weight: 700;
}
.card-title svg { width: 16px; height: 16px; fill: var(--text-muted); }

.hero-number {
  font-family: var(--mono);
  font-size: 44px;
  font-weight: 700;
  line-height: 1;
  color: var(--text-main);
  font-variant-numeric: tabular-nums;
  display: flex;
  align-items: baseline;
  gap: 8px;
}

.hero-number .unit {
  font-size: 14px;
  color: var(--text-muted);
  font-family: var(--font);
  font-weight: 500;
}

.sub-stats {
  display: flex;
  align-items: center;
  gap: 16px;
  font-size: 13px;
  color: var(--text-muted);
  padding-top: 8px;
  border-top: 1px solid var(--border);
}
.sub-stats span { display: flex; align-items: center; gap: 4px; }
.sub-stats .highlight { color: var(--gold); font-weight: 600; font-family: var(--mono); }

/* Squad Roster */
.squad-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 14px;
  margin-top: 6px;
}

.teammate-card {
  background: #10131a;
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 14px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.teammate-card.me { border-color: var(--gold); }

.teammate-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.teammate-name {
  font-weight: 600;
  font-size: 14px;
  display: flex;
  align-items: center;
  gap: 6px;
}
.teammate-name svg { width: 14px; height: 14px; fill: var(--gold); }

.teammate-stats {
  display: flex;
  justify-content: space-between;
  font-size: 12px;
  color: var(--text-muted);
}
.teammate-stats b {
  color: var(--text-main);
  font-family: var(--mono);
  font-size: 13px;
}

/* History Table */
.table {
  width: 100%;
  border-collapse: collapse;
}
.table th {
  text-align: left;
  font-size: 12px;
  color: var(--text-muted);
  padding: 10px 14px;
  border-bottom: 1px solid var(--border);
  text-transform: uppercase;
}
.table td {
  padding: 12px 14px;
  border-bottom: 1px solid var(--border);
  font-size: 13px;
}
.table tr:hover td { background: var(--surface-hover); }

/* Form controls */
.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.form-label {
  font-size: 12px;
  font-weight: 600;
  color: var(--text-muted);
}
.input-row {
  display: flex;
  gap: 8px;
}
.input {
  flex: 1;
  background: #0a0c10;
  border: 1px solid var(--border);
  padding: 9px 12px;
  border-radius: 6px;
  color: #fff;
  font-size: 13px;
  font-family: var(--font);
  outline: none;
  transition: border-color 150ms ease;
}
.input:focus { border-color: var(--gold); }

.btn {
  background: var(--surface);
  border: 1px solid var(--border);
  color: var(--text-main);
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  font-size: 13px;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  transition: all 120ms ease;
}
.btn:hover { background: var(--surface-hover); border-color: rgba(255, 255, 255, 0.18); }
.btn:active { transform: scale(0.97); }
.btn.primary { background: var(--gold); color: #0c0e12; border-color: var(--gold); }
.btn.primary:hover { filter: brightness(1.1); }
.btn svg { width: 14px; height: 14px; fill: currentColor; }

.toast {
  position: fixed;
  bottom: 24px;
  right: 24px;
  background: #141822;
  border: 1px solid var(--gold);
  padding: 10px 16px;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 600;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.5);
  display: none;
  animation: toastIn 180ms ease;
}
@keyframes toastIn { from { transform: translateY(10px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }

/* Weekly Statistics & Charts */
.chart-container {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 12px;
  height: 220px;
  padding: 16px 12px 10px 12px;
  margin-top: 10px;
  background: #0f1117;
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
.chart-col:hover {
  background: rgba(255, 255, 255, 0.04);
}
.chart-col.selected {
  background: rgba(229, 169, 60, 0.08);
  border-color: var(--gold);
}

.chart-val {
  font-size: 13px;
  font-family: var(--mono);
  font-weight: 700;
  color: var(--text-muted);
  margin-bottom: 6px;
  transition: color 150ms ease;
}
.chart-col.selected .chart-val,
.chart-col:hover .chart-val {
  color: var(--gold);
}

.chart-bar-wrap {
  width: 100%;
  max-width: 44px;
  height: 130px;
  display: flex;
  align-items: flex-end;
  background: rgba(255, 255, 255, 0.03);
  border-radius: 4px;
  overflow: hidden;
}

.chart-bar {
  width: 100%;
  background: rgba(229, 169, 60, 0.35);
  border-radius: 4px 4px 0 0;
  transition: height 250ms cubic-bezier(0.16, 1, 0.3, 1), background 150ms ease;
  min-height: 3px;
}
.chart-col:hover .chart-bar {
  background: rgba(229, 169, 60, 0.65);
}
.chart-col.selected .chart-bar {
  background: var(--gold);
  box-shadow: 0 0 12px var(--gold-glow);
}

.chart-label {
  margin-top: 8px;
  font-size: 11px;
  font-weight: 600;
  color: var(--text-muted);
  text-align: center;
  line-height: 1.2;
}
.chart-col.selected .chart-label {
  color: var(--text-main);
  font-weight: 700;
}

.stat-box {
  background: #10131a;
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
  color: var(--text-muted);
  letter-spacing: 0.5px;
}
.stat-box .stat-val {
  font-family: var(--mono);
  font-size: 24px;
  font-weight: 700;
  color: var(--text-main);
}
.stat-box .stat-val.highlight {
  color: var(--gold);
}
.stat-box .stat-sub {
  font-size: 12px;
  color: var(--text-muted);
}
</style>
</head>
<body>

<!-- Frameless Titlebar -->
<div id="titlebar">
  <div class="brand">
    <svg viewBox="0 0 24 24"><path d="M4 6h16a2 2 0 0 1 2 2v3H2V8a2 2 0 0 1 2-2zm-2 7h20v5a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2v-5zm9-1v3h2v-3h-2z"/></svg>
    <span>LootPulse</span>
    <span class="badge" id="titlebarVersion">v2.0.0</span>
  </div>

  <div class="titlebar-right">
    <div class="room-badge" id="roomBadge">
      <svg viewBox="0 0 24 24"><path d="M16 11c1.66 0 3-1.34 3-3s-1.34-3-3-3-3 1.34-3 3 1.34 3 3 3zm-8 0c1.66 0 3-1.34 3-3S9.66 5 8 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z"/></svg>
      <span id="roomCodeText">Комната не задана</span>
    </div>

    <div class="win-controls">
      <button class="win-btn" onclick="api('wmin')"><svg viewBox="0 0 24 24"><path d="M5 11h14v2H5z"/></svg></button>
      <button class="win-btn" onclick="api('wmax')"><svg viewBox="0 0 24 24"><path d="M4 4h16v16H4V4zm2 2v12h12V6H6z"/></svg></button>
      <button class="win-btn close" onclick="api('wclose')"><svg viewBox="0 0 24 24"><path d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/></svg></button>
    </div>
  </div>
</div>

<!-- Tabs Navigation -->
<div id="nav">
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

<!-- Main Area -->
<div id="main">

  <!-- TAB: ГЛАВНАЯ -->
  <div id="tab-dashboard" class="tab-view active">
    <div class="grid-2">
      <div class="card">
        <div class="card-title">
          <span>Сундуков всего</span>
          <svg viewBox="0 0 24 24"><path d="M4 6h16a2 2 0 0 1 2 2v3H2V8a2 2 0 0 1 2-2zm-2 7h20v5a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2v-5zm9-1v3h2v-3h-2z"/></svg>
        </div>
        <div class="hero-number">
          <span id="totalChests">0</span>
          <span class="unit">добыто тобой</span>
        </div>
        <div class="sub-stats">
          <span>За сессию: <b id="sessionChests" class="highlight">+0</b></span>
          <span>Последний матч: <b id="lastMatchChestsVal" class="highlight">0</b></span>
        </div>
      </div>

      <div class="card">
        <div class="card-title">
          <span>Текущий матч</span>
          <svg viewBox="0 0 24 24"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 14H9V8h2v8zm4 0h-2V8h2v8z"/></svg>
        </div>
        <div class="hero-number">
          <span id="currentMatchChests">0</span>
          <span class="unit">сундуков</span>
        </div>
        <div class="sub-stats">
          <span>Время в матче: <b id="matchTimer" class="highlight">00:00</b></span>
        </div>
      </div>
    </div>

    <!-- Squad Members in App -->
    <div class="card">
      <div class="card-title">
        <span>Игроки в комнате</span>
        <span id="squadCount" style="color:var(--text-main); font-weight:600;">В сети: 1</span>
      </div>

      <div id="squadGrid" class="squad-grid">
        <!-- Rendered dynamically -->
      </div>
    </div>
  </div>

  <!-- TAB: ПАТИ -->
  <div id="tab-party" class="tab-view">
    <div class="card">
      <div class="card-title"><span>Комната</span></div>
      <p style="color:var(--text-muted); font-size:13px;">
        Введите одинаковое кодовое слово с друзьями, чтобы видеть сундуки друг друга в реальном времени.
      </p>

      <div class="form-group" style="margin-top:10px;">
        <label class="form-label">Код комнаты</label>
        <div class="input-row">
          <input type="text" id="roomInput" class="input" placeholder="Введите код комнаты...">
          <button class="btn primary" onclick="saveRoom()">Подключиться</button>
          <button class="btn" onclick="genRoom()">Случайный</button>
          <button class="btn" onclick="copyRoom()">Скопировать</button>
        </div>
      </div>
    </div>

    <div class="card">
      <div class="card-title"><span>Подключенные игроки</span></div>
      <div id="partyList" style="display:flex; flex-direction:column; gap:10px;">
        <span style="color:var(--text-muted);">Никого нет. Введите код комнаты.</span>
      </div>
    </div>
  </div>

  <!-- TAB: ИСТОРИЯ -->
  <div id="tab-history" class="tab-view">
    <div class="card">
      <div class="card-title"><span>История матчей</span></div>
      
      <div style="margin-top:4px;">
        <input type="text" id="historySearch" class="input" placeholder="Поиск по дате..." oninput="filterHistory()">
      </div>

      <table class="table" style="margin-top:10px;">
        <thead>
          <tr>
            <th>Дата</th>
            <th>Время</th>
            <th>Сундуков добыто</th>
            <th>Счёт Пати</th>
          </tr>
        </thead>
        <tbody id="historyTableBody">
          <tr><td colspan="4" style="text-align:center; color:var(--text-muted);">Матчей пока нет</td></tr>
        </tbody>
      </table>
    </div>
  </div>

  <!-- TAB: СТАТИСТИКА -->
  <div id="tab-stats" class="tab-view">
    <!-- Main Weekly Chest Chart Card -->
    <div class="card">
      <div class="card-title">
        <span>Сундуки по дням за неделю</span>
        <span id="statsWeekTotal" style="color:var(--gold); font-family:var(--mono); font-weight:700;">0 сундуков</span>
      </div>
      <p style="color:var(--text-muted); font-size:13px;">
        Нажмите на любой день недели на графике, чтобы посмотреть подробную статистику за этот день.
      </p>

      <!-- Weekly Bar Chart -->
      <div class="chart-container" id="weeklyChartContainer">
        <!-- Rendered dynamically -->
      </div>
    </div>

    <!-- Selected Day Detailed Stats Card -->
    <div class="card" id="dayDetailCard">
      <div class="card-title">
        <span id="dayDetailTitle">Показатели за день</span>
        <span id="dayDetailDate" style="color:var(--text-muted); font-family:var(--mono);">Сегодня</span>
      </div>

      <div style="display:grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap:14px; margin-top:4px;">
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

      <!-- Match list for the selected day -->
      <div style="margin-top:14px;">
        <div style="font-size:12px; font-weight:700; color:var(--text-muted); text-transform:uppercase; margin-bottom:8px;">Катки за этот день</div>
        <table class="table">
          <thead>
            <tr>
              <th>Время начала</th>
              <th>Длительность</th>
              <th>Сундуков добыто</th>
              <th>В минуту (CPM)</th>
            </tr>
          </thead>
          <tbody id="dayMatchesTableBody">
            <!-- Rendered dynamically -->
          </tbody>
        </table>
      </div>
    </div>
  </div>

  <!-- TAB: ОВЕРЛЕЙ -->
  <div id="tab-overlay" class="tab-view">
    <div class="card">
      <div class="card-title"><span>Игровой оверлей</span></div>
      <p style="color:var(--text-muted); font-size:13px;">
        Оверлей отображает таблицу сундуков поверх Fortnite (в режиме «Окно без рамки»).
        <br><b>Ctrl + Shift + O</b> — скрыть / показать оверлей.
        <br><b>Ctrl + Shift + L</b> — закрепить (клик сквозь окно) / разблокировать для перемещения мышью.
      </p>

      <div style="margin-top:16px;">
        <div style="font-size:12px; font-weight:700; color:var(--text-muted); text-transform:uppercase; margin-bottom:8px;">Вид таблицы в игре</div>
        <div id="hudPreviewBox" style="background:rgba(14,17,23,0.7); border:1px solid rgba(229,169,60,0.5); border-radius:8px; padding:12px 16px; width:410px; transition:background 150ms ease, border-color 150ms ease;">
          <table id="hudPreviewTable" style="width:100%; border-collapse:collapse; text-align:left; font-size:13px; opacity:1.0; transition:opacity 150ms ease;">
            <thead>
              <tr style="color:#8b92a4; font-size:11px; font-weight:700; border-bottom:1px solid rgba(255,255,255,0.15);">
                <th style="padding-bottom:6px;">Player</th>
                <th style="padding-bottom:6px;">Last</th>
                <th style="padding-bottom:6px;">All</th>
                <th style="padding-bottom:6px;">Session</th>
              </tr>
            </thead>
            <tbody id="overlayPrevTableBody">
              <!-- Rendered dynamically -->
            </tbody>
          </table>
        </div>

        <div class="form-group" style="margin-top:16px; max-width:410px;">
          <label class="form-label" style="display:flex; justify-content:space-between;">
            <span>Прозрачность фона (GUI)</span>
            <span id="opacityVal" style="color:var(--gold);">70%</span>
          </label>
          <input type="range" id="opacitySlider" min="0" max="100" value="70" style="width:100%; accent-color:var(--gold); cursor:pointer;" oninput="onOpacityChange(this.value)">
        </div>

        <div class="form-group" style="margin-top:12px; max-width:410px;">
          <label class="form-label" style="display:flex; justify-content:space-between;">
            <span>Прозрачность текста</span>
            <span id="textOpacityVal" style="color:var(--gold);">100%</span>
          </label>
          <input type="range" id="textOpacitySlider" min="10" max="100" value="100" style="width:100%; accent-color:var(--gold); cursor:pointer;" oninput="onTextOpacityChange(this.value)">
        </div>

        <div style="margin-top:14px; display:flex; align-items:center; gap:8px;">
          <input type="checkbox" id="onlyInGameCheckbox" style="accent-color:var(--gold); width:16px; height:16px; cursor:pointer;" onchange="toggleOnlyInGame(this.checked)">
          <label for="onlyInGameCheckbox" style="color:var(--text-main); font-size:13px; cursor:pointer;">
            Показывать только в игре
          </label>
        </div>
      </div>

      <div style="display:flex; gap:12px; margin-top:16px;">
        <button class="btn primary" onclick="api('toggleOverlay')">Скрыть / Показать</button>
        <button class="btn" onclick="api('toggleLock')">Закрепить / Разблокировать</button>
      </div>
    </div>
  </div>

  <!-- TAB: НАСТРОЙКИ -->
  <div id="tab-settings" class="tab-view">
    <div class="card">
      <div class="card-title"><span>Настройки профиля</span></div>

      <div class="form-group">
        <label class="form-label">Никнейм в игре (Epic Games)</label>
        <div class="input-row">
          <input type="text" id="profileInput" class="input" placeholder="Ваш ник в игре...">
          <button class="btn primary" onclick="saveProfile()">Сохранить</button>
        </div>
      </div>

      <div class="form-group" style="margin-top:12px;">
        <label class="form-label">Папка реплеев Fortnite</label>
        <div class="input-row">
          <input type="text" id="demosInput" class="input" readonly>
        </div>
      </div>

      <div class="form-group" style="margin-top:12px;">
        <label class="form-label">Добавить сундуки вручную</label>
        <div class="input-row">
          <input type="number" id="manualInput" class="input" placeholder="+100">
          <button class="btn" onclick="saveManual()">Добавить</button>
        </div>
      </div>
    </div>

    <!-- Updater Settings Card -->
    <div class="card" style="margin-top:16px;">
      <div class="card-title">
        <span>Обновление программы</span>
        <span class="badge" id="currentVersionBadge">v2.0.0</span>
      </div>
      <p style="color:var(--text-muted); font-size:13px;">
        Проверка и автоматическая установка обновлений программы напрямую из релизов GitHub.
      </p>

      <div class="form-group" style="margin-top:12px;">
        <label class="form-label">Репозиторий GitHub (owner/repo)</label>
        <div class="input-row">
          <input type="text" id="updateRepoInput" class="input" placeholder="например, fjdjdhidjfj/LootPulse">
          <button class="btn" onclick="saveUpdateRepo()">Сохранить</button>
        </div>
      </div>

      <div style="margin-top:14px; display:flex; align-items:center; gap:8px;">
        <input type="checkbox" id="autoUpdateCheckbox" style="accent-color:var(--gold); width:16px; height:16px; cursor:pointer;" onchange="toggleAutoUpdate(this.checked)">
        <label for="autoUpdateCheckbox" style="color:var(--text-main); font-size:13px; cursor:pointer;">
          Проверять обновления при запуске
        </label>
      </div>

      <div style="margin-top:16px; display:flex; gap:12px; align-items:center;">
        <button class="btn primary" id="checkUpdateBtn" onclick="checkUpdates(true)">Проверить обновление</button>
        <span id="updateCheckStatus" style="font-size:13px; color:var(--text-muted);"></span>
      </div>

      <div id="updateNoticeBox" style="display:none; margin-top:16px; background:#10131a; border:1px solid var(--gold); border-radius:8px; padding:14px 16px;">
        <div style="display:flex; justify-content:space-between; align-items:center;">
          <div>
            <div style="font-size:14px; font-weight:700; color:var(--gold);" id="updateNoticeTitle">Доступна новая версия</div>
            <div style="font-size:12px; color:var(--text-muted); margin-top:2px;" id="updateNoticeMeta"></div>
          </div>
          <button class="btn primary" id="applyUpdateBtn" onclick="applyUpdate()">Обновить сейчас</button>
        </div>
        <div id="updateChangelog" style="font-size:12px; color:var(--text-main); margin-top:10px; white-space:pre-wrap; border-top:1px solid var(--border); padding-top:8px;"></div>
      </div>
    </div>
  </div>

</div>

<div id="toast" class="toast"></div>

<script>
const TOKEN = "{{TOKEN}}";
let state = null;
let selectedDayIndex = 6;
let weeklyDaysData = [];

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
      render();
    }
  } catch(e) {}
  setTimeout(pollState, 1500);
}

function render() {
  if (!state) return;

  const roomText = state.roomCode ? `Комната: ${state.roomCode}` : 'Комната не задана';
  document.getElementById('roomCodeText').innerText = roomText;

  // Главная
  document.getElementById('totalChests').innerText = (state.totalChests || 0).toLocaleString();
  document.getElementById('sessionChests').innerText = `+${state.sessionChests || 0}`;
  document.getElementById('lastMatchChestsVal').innerText = state.lastMatchChests || 0;

  document.getElementById('currentMatchChests').innerText = state.currentMatchChests || 0;
  document.getElementById('matchTimer').innerText = fmtTime(state.matchSeconds);

  // Squad Grid
  const squadGrid = document.getElementById('squadGrid');
  squadGrid.innerHTML = '';

  // Local Player
  const myCard = document.createElement('div');
  myCard.className = 'teammate-card me';
  myCard.innerHTML = `
    <div class="teammate-header">
      <span class="teammate-name"><svg viewBox="0 0 24 24"><path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/></svg> ${state.profile || 'Вы'}</span>
      <span style="font-size:11px; color:var(--gold); font-weight:700;">ВЫ</span>
    </div>
    <div class="teammate-stats">
      <span>В матче: <b>${state.currentMatchChests || 0}</b></span>
      <span>За сессию: <b>${state.sessionChests || 0}</b></span>
    </div>
  `;
  squadGrid.appendChild(myCard);

  // Connected Peers
  if (state.peers && state.peers.length > 0) {
    document.getElementById('squadCount').innerText = `В сети: ${state.peers.length + 1}`;
    state.peers.forEach(p => {
      const card = document.createElement('div');
      card.className = 'teammate-card';
      card.innerHTML = `
        <div class="teammate-header">
          <span class="teammate-name"><svg viewBox="0 0 24 24"><path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/></svg> ${p.name}</span>
          <span style="font-size:11px; color:var(--green); font-weight:700;">ONLINE</span>
        </div>
        <div class="teammate-stats">
          <span>В матче: <b>${p.matchChests}</b></span>
          <span>За сессию: <b>${p.sessionChests}</b></span>
        </div>
      `;
      squadGrid.appendChild(card);
    });
  } else {
    document.getElementById('squadCount').innerText = 'В сети: 1 (Соло)';
  }

  // Party Tab List
  const partyList = document.getElementById('partyList');
  if (partyList) {
    if (state.peers && state.peers.length > 0) {
      partyList.innerHTML = state.peers.map(p => `
        <div style="background:#10131a; border:1px solid var(--border); border-radius:8px; padding:12px 16px; display:flex; justify-content:space-between; align-items:center;">
          <div>
            <div style="font-weight:600; color:#fff; font-size:14px;">${p.name}</div>
            <div style="font-size:12px; color:var(--text-muted); margin-top:2px;">Всего: ${p.allChests > 0 ? p.allChests.toLocaleString() : '-'} &bull; В матче: ${p.matchChests}</div>
          </div>
          <div style="text-align:right;">
            <div style="font-size:15px; font-weight:700; color:var(--gold);">${p.sessionChests} за сессию</div>
            <div style="font-size:11px; color:var(--green); font-weight:700;">ONLINE</div>
          </div>
        </div>
      `).join('');
    } else {
      partyList.innerHTML = '<span style="color:var(--text-muted);">Никого нет в комнате. Убедитесь, что друзья ввели тот же код.</span>';
    }
  }

  // History Table
  filterHistory();

  // Weekly Stats
  renderWeeklyStats();

  // HUD Table Preview
  const prevTbody = document.getElementById('overlayPrevTableBody');
  if (prevTbody) {
    const myName = (state.profile || 'Вы') + ' (Вы)';
    let rowsHtml = `
      <tr style="color:var(--gold); font-weight:700;">
        <td style="padding:6px 0;">${myName}</td>
        <td>${state.lastMatchChests || 0}</td>
        <td style="color:#fff;">${(state.totalChests || 0).toLocaleString()}</td>
        <td>${state.sessionChests || 0}</td>
      </tr>
    `;
    if (state.peers && state.peers.length > 0) {
      state.peers.forEach(p => {
        rowsHtml += `
          <tr style="color:#f3f4f6;">
            <td style="padding:4px 0;">${p.name}</td>
            <td style="color:var(--gold);">${p.lastChests !== undefined ? p.lastChests : (p.matchChests || 0)}</td>
            <td>${p.allChests > 0 ? p.allChests.toLocaleString() : '-'}</td>
            <td style="color:var(--gold);">${p.sessionChests}</td>
          </tr>
        `;
      });
    }
    prevTbody.innerHTML = rowsHtml;
  }

  // Inputs if not focused
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
      prevBox.style.borderColor = `rgba(229, 169, 60, ${a * 0.7})`;
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

  // Version & Update inputs
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
  if (autoBox && document.activeElement !== autoBox) {
    autoBox.checked = !!state.autoCheckUpdates;
  }

  if (state.availableUpdate) {
    renderUpdateNotice(state.availableUpdate);
  }
}

function filterHistory() {
  const tbody = document.getElementById('historyTableBody');
  if (!tbody || !state || !state.matches) return;

  const q = (document.getElementById('historySearch')?.value || '').trim().toLowerCase();
  let list = state.matches.slice().reverse();

  if (q) {
    list = list.filter(m => 
      (m.date && m.date.toLowerCase().includes(q)) || 
      (m.sessionId && m.sessionId.toLowerCase().includes(q))
    );
  }

  if (list.length === 0) {
    tbody.innerHTML = '<tr><td colspan="4" style="text-align:center; color:var(--text-muted); padding:16px;">Матчи не найдены</td></tr>';
    return;
  }

  tbody.innerHTML = list.slice(0, 50).map(m => `
    <tr>
      <td>${m.date}</td>
      <td>${fmtTime(m.duration)}</td>
      <td><b style="color:var(--gold);">${m.chests}</b></td>
      <td><b>${m.squadChests}</b></td>
    </tr>
  `).join('');
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
    if (i === 0) {
      label = `Сегодня (${dd}.${mm})`;
      fullTitle = `Сегодня (${fullDayNames[dayOfWeek]}, ${dd}.${mm})`;
    } else if (i === 1) {
      label = `Вчера (${dd}.${mm})`;
      fullTitle = `Вчера (${fullDayNames[dayOfWeek]}, ${dd}.${mm})`;
    }

    const dayMatches = matches.filter(m => m.dateIso === dateKey);
    const dayChests = dayMatches.reduce((acc, m) => acc + (m.chests || 0), 0);
    const dayDuration = dayMatches.reduce((acc, m) => acc + (m.duration || 0), 0);
    const matchCount = dayMatches.length;

    weekTotalChests += dayChests;

    weeklyDaysData.push({
      index: 6 - i,
      dateKey,
      label,
      fullTitle,
      matches: dayMatches,
      chests: dayChests,
      duration: dayDuration,
      matchCount
    });
  }

  const weekTotalElem = document.getElementById('statsWeekTotal');
  if (weekTotalElem) weekTotalElem.innerText = `${weekTotalChests.toLocaleString()} сундуков`;

  const container = document.getElementById('weeklyChartContainer');
  if (container) {
    const maxChests = Math.max(1, ...weeklyDaysData.map(d => d.chests));
    container.innerHTML = weeklyDaysData.map((d, idx) => {
      const pct = Math.max(3, Math.round((d.chests / maxChests) * 100));
      const isSelected = (idx === selectedDayIndex);
      return `
        <div class="chart-col ${isSelected ? 'selected' : ''}" onclick="selectDay(${idx})">
          <div class="chart-val">${d.chests}</div>
          <div class="chart-bar-wrap">
            <div class="chart-bar" style="height:${pct}%;"></div>
          </div>
          <div class="chart-label">${d.label}</div>
        </div>
      `;
    }).join('');
  }

  renderSelectedDayStats();
}

function selectDay(index) {
  selectedDayIndex = index;
  document.querySelectorAll('.chart-col').forEach((col, idx) => {
    col.classList.toggle('selected', idx === index);
  });
  renderSelectedDayStats();
}

function renderSelectedDayStats() {
  if (!weeklyDaysData || weeklyDaysData.length <= selectedDayIndex) return;
  const d = weeklyDaysData[selectedDayIndex];

  const titleElem = document.getElementById('dayDetailTitle');
  const dateElem = document.getElementById('dayDetailDate');
  if (titleElem) titleElem.innerText = 'Показатели за день';
  if (dateElem) dateElem.innerText = d.fullTitle;

  // 1. Сундуков в час
  const hours = d.duration / 3600;
  const cph = hours > 0 ? (d.chests / hours).toFixed(1) : '0';
  const cphElem = document.getElementById('dayChestsPerHour');
  if (cphElem) cphElem.innerText = cph;

  // 2. Время в игре (в катках)
  const durHours = Math.floor(d.duration / 3600);
  const durMin = Math.floor((d.duration % 3600) / 60);
  let durStr = `${durHours} ч ${durMin} мин`;
  if (durHours === 0) durStr = `${durMin} мин`;
  const timeElem = document.getElementById('dayTimeInGame');
  if (timeElem) timeElem.innerText = durStr;

  // 3. В среднем за катку
  const avg = d.matchCount > 0 ? (d.chests / d.matchCount).toFixed(1) : '0';
  const avgElem = document.getElementById('dayAvgChests');
  if (avgElem) avgElem.innerText = avg;

  // 4. Всего сундуков и каток
  const totalElem = document.getElementById('dayTotalChests');
  if (totalElem) totalElem.innerText = d.chests.toLocaleString();
  const countElem = document.getElementById('dayMatchCount');
  if (countElem) countElem.innerText = `Сыграно каток: ${d.matchCount}`;

  // Matches table
  const tbody = document.getElementById('dayMatchesTableBody');
  if (tbody) {
    if (!d.matches || d.matches.length === 0) {
      tbody.innerHTML = '<tr><td colspan="4" style="text-align:center; color:var(--text-muted); padding:16px;">В этот день каток не было</td></tr>';
    } else {
      tbody.innerHTML = d.matches.slice().reverse().map(m => {
        const cpm = m.duration > 0 ? (m.chests / (m.duration / 60)).toFixed(1) : '0';
        const timeStr = m.date ? (m.date.split(' ')[1] || m.date) : '-';
        return `
          <tr>
            <td>${timeStr}</td>
            <td>${fmtTime(m.duration)}</td>
            <td><b style="color:var(--gold);">${m.chests}</b></td>
            <td>${cpm}</td>
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
    prevBox.style.borderColor = `rgba(229, 169, 60, ${a * 0.7})`;
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
    changelogElem.innerText = u.changelog;
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
      renderUpdateNotice({
        version: latestVer,
        downloadUrl: dUrl,
        sizeBytes: sBytes,
        changelog: cl
      });
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
