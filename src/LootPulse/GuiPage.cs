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

.chest-breakdown {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 6px;
}

.chest-badge {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  padding: 3px 8px;
  border-radius: 5px;
  font-family: var(--mono);
  font-size: 12px;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
  border: 1px solid var(--border);
  background: var(--surface-well);
  color: var(--text-main);
}
.chest-badge img, .chest-icon-lg {
  object-fit: contain;
  flex-shrink: 0;
  -webkit-mask-image: radial-gradient(circle, #000 52%, transparent 95%);
  mask-image: radial-gradient(circle, #000 52%, transparent 95%);
}
.chest-badge img { width: 40px; height: 40px; }
.chest-icon-lg { width: 44px; height: 44px; }
tr.clickable-row { cursor: pointer; }

/* Match Analytics Modal */
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(5, 7, 10, 0.72);
  backdrop-filter: blur(4px);
  display: none;
  align-items: center;
  justify-content: center;
  z-index: 900;
  padding: 24px;
}
.modal-backdrop.open { display: flex; }
.modal-card {
  background: var(--surface);
  border: 1px solid var(--border-hover);
  border-radius: 12px;
  width: 100%;
  max-width: 720px;
  max-height: 85vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  box-shadow: 0 20px 48px rgba(0, 0, 0, 0.45);
  animation: fadeIn 160ms cubic-bezier(0.16, 1, 0.3, 1);
}
.modal-header {
  padding: 16px 20px;
  background: var(--surface-1);
  border-bottom: 1px solid var(--border);
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.modal-body {
  padding: 20px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.timeline-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 6px 12px;
  background: var(--surface-well);
  border: 1px solid var(--border);
  border-radius: 6px;
  font-size: 13px;
}
.chest-badge.normal { border-color: rgba(217, 119, 6, 0.35); background: rgba(217, 119, 6, 0.10); color: var(--gold-bright); }
.chest-badge.rare { border-color: rgba(56, 189, 248, 0.35); background: rgba(56, 189, 248, 0.10); color: #38BDF8; }
.chest-badge.weapon { border-color: rgba(245, 158, 11, 0.38); background: rgba(245, 158, 11, 0.12); color: #FBBF24; }
.chest-badge.buried { border-color: rgba(202, 138, 4, 0.35); background: rgba(202, 138, 4, 0.10); color: #EAB308; }
.chest-badge.ghost { border-color: rgba(226, 232, 240, 0.35); background: rgba(226, 232, 240, 0.10); color: #E2E8F0; }

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
        <div class="stat-label">Особых сундуков</div>
        <div class="stat-val" style="color:#38BDF8;" id="histSpecialChests">0</div>
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
        <input type="text" id="historySearch" class="input" style="width:100%;" placeholder="Поиск по дате или ID..." oninput="filterHistory()">
        <div id="historyFilterChips" class="filter-chips"></div>
      </div>

      <table class="table">
        <thead>
          <tr>
            <th>Дата</th>
            <th>Добыча по типам</th>
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
                <th>Добыча по типам</th>
                <th>Длительность</th>
                <th>Сундуков добыто</th>
                <th>В минуту (CPM)</th>
              </tr>
            </thead>
            <tbody id="dayMatchesTableBody"></tbody>
          </table>
        </div>
      </div>

      <!-- Overall Chest Types Card -->
      <div class="card" id="chestTotalsCard">
        <div class="card-title">
          <span>Разбивка по типам сундуков</span>
          <span class="mono-pill">по всем матчам</span>
        </div>
        <p style="color:var(--text-muted); font-size:13px;">
          Распределение открытых и разрушенных сундуков по их категориям.
        </p>
        <div id="chestTotalsList" style="display:flex; flex-direction:column; gap:8px;"></div>
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

<!-- Match Detail Modal -->
<div id="matchModal" class="modal-backdrop" onclick="if (event.target === this) closeMatchModal()">
  <div class="modal-card">
    <div class="modal-header">
      <div>
        <div style="font-size:15px; font-weight:700; color:var(--text-main);" id="modalMatchTitle">Аналитика матча</div>
        <div style="font-size:11px; color:var(--text-ghost); font-family:var(--mono);" id="modalMatchSub"></div>
      </div>
      <button class="win-btn" onclick="closeMatchModal()" title="Закрыть">
        <svg viewBox="0 0 24 24"><path d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/></svg>
      </button>
    </div>
    <div class="modal-body" id="modalMatchBody"></div>
  </div>
</div>

<div id="toast" class="toast"></div>

<script>
const TOKEN = "{{TOKEN}}";
let state = null;
let selectedDayIndex = 6;
let weeklyDaysData = [];
let activeChestFilter = '';

const CHEST_ORDER = ['normal', 'rare', 'weapon', 'buried', 'ghost'];
const CHEST_META = {
  normal: { name: 'Обычный сундук', img: 'data:image/png;base64,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' },
  rare: { name: 'Rare сундук', img: 'data:image/png;base64,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' },
  weapon: { name: 'Сундук-пушка', img: 'data:image/webp;base64,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' },
  buried: { name: 'Зарытый сундук', img: 'data:image/png;base64,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' },
  ghost: { name: 'Сундук духов', img: 'data:image/png;base64,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YXRvcj4KIDwvcmRmOkRlc2NyaXB0aW9uPgo8L3JkZjpSREY+CjwveDp4bXBtZXRhPgogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAo8P3hwYWNrZXQgZW5kPSd3Jz8+AA==' }
};

function renderChestBadges(breakdown, fallbackTotal) {
  const b = breakdown || {};
  const entries = CHEST_ORDER.filter(k => (b[k] || 0) > 0).map(k => [k, b[k]]);
  if (entries.length === 0 && fallbackTotal > 0) {
    entries.push(['normal', fallbackTotal]);
  }
  if (entries.length === 0) {
    return '<span style="color:var(--text-ghost); font-size:12px;">—</span>';
  }
  return `<div class="chest-breakdown">${entries.map(([k, cnt]) => {
    const meta = CHEST_META[k] || CHEST_META.normal;
    return `<span class="chest-badge ${k}" title="${meta.name}: ${cnt}"><img src="${meta.img}" alt="${meta.name}"><span>&times;${cnt}</span></span>`;
  }).join('')}</div>`;
}

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

  const specialEl = document.getElementById('histSpecialChests');
  if (specialEl) {
    const specialSum = (state.chestTotals || [])
      .filter(t => t.type !== 'normal')
      .reduce((a, t) => a + (t.count || 0), 0);
    specialEl.innerText = specialSum.toLocaleString();
  }

  const chipsEl = document.getElementById('historyFilterChips');
  if (chipsEl) {
    const filters = [
      { key: '', label: 'Все матчи' },
      { key: 'rare', label: 'Rare' },
      { key: 'weapon', label: 'Сундук-пушка' },
      { key: 'buried', label: 'Зарытый' },
      { key: 'ghost', label: 'Сундук духов' }
    ];
    chipsEl.innerHTML = filters.map(f =>
      `<button class="chip-btn ${activeChestFilter === f.key ? 'active' : ''}" onclick="setChestFilter('${f.key}')">${f.label}</button>`
    ).join('');
  }
}

function setChestFilter(type) {
  activeChestFilter = type;
  renderHistorySummary();
  filterHistory();
}

function filterHistory() {
  const tbody = document.getElementById('historyTableBody');
  if (!tbody || !state || !state.matches) return;

  const q = (document.getElementById('historySearch')?.value || '').trim().toLowerCase();
  let list = state.matches.slice().reverse();

  if (activeChestFilter) {
    list = list.filter(m => ((m.chestBreakdown || {})[activeChestFilter] || 0) > 0);
  }
  if (q) {
    list = list.filter(m =>
      (m.date && m.date.toLowerCase().includes(q)) ||
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
      <tr class="clickable-row" onclick="openMatchModal(${m.timestamp})">
        <td style="font-family:var(--mono);">${m.date}</td>
        <td>${renderChestBadges(m.chestBreakdown, m.chests)}</td>
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

  const totalsListElem = document.getElementById('chestTotalsList');
  if (totalsListElem && state.chestTotals) {
    if (state.chestTotals.length === 0) {
      totalsListElem.innerHTML = '<span style="color:var(--text-muted); font-size:13px;">Данные по сундукам появятся после завершения матчей.</span>';
    } else {
      const grandTotal = Math.max(1, state.chestTotals.reduce((a, t) => a + (t.count || 0), 0));
      totalsListElem.innerHTML = state.chestTotals.map(t => {
        const meta = CHEST_META[t.type] || CHEST_META.normal;
        const pct = Math.max(2, Math.round((t.count / grandTotal) * 100));
        return `
          <div style="background:var(--surface-well); border:1px solid var(--border); border-radius:8px; padding:10px 14px;">
            <div style="display:flex; justify-content:space-between; align-items:center; font-size:13px;">
              <span style="display:inline-flex; align-items:center; gap:10px;">
                <img class="chest-icon-lg" src="${meta.img}" alt="${meta.name}">
                <b style="color:var(--text-main);">${meta.name}</b>
                <span style="color:var(--text-ghost); font-size:11px;">&mdash; ${t.matches} матчей</span>
              </span>
              <span><b style="color:var(--gold-bright); font-family:var(--mono);">${t.count.toLocaleString()}</b> <span style="color:var(--text-ghost); font-family:var(--mono); font-size:11px;">(${pct}%)</span></span>
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
          <tr class="clickable-row" onclick="openMatchModal(${m.timestamp})">
            <td style="font-family:var(--mono);">${timeStr}</td>
            <td>${renderChestBadges(m.chestBreakdown, m.chests)}</td>
            <td style="font-family:var(--mono);">${fmtTime(m.duration)}</td>
            <td><b style="color:var(--gold-bright); font-family:var(--mono);">${m.chests}</b></td>
            <td style="font-family:var(--mono); color:${cpmColor}; font-weight:600;">${cpm} CPM</td>
          </tr>
        `;
      }).join('');
    }
  }
}

function openMatchModal(ts) {
  if (!state || !state.matches) return;
  const m = state.matches.find(x => x.timestamp === ts);
  if (!m) return;

  const modal = document.getElementById('matchModal');
  const title = document.getElementById('modalMatchTitle');
  const sub = document.getElementById('modalMatchSub');
  const body = document.getElementById('modalMatchBody');
  if (!modal || !body) return;

  const cpm = m.duration > 0 ? (m.chests / (m.duration / 60)).toFixed(1) : '0.0';
  const events = m.events || [];
  const firstT = events.length > 0 ? fmtTime(events[0].t) : '—';
  const avgInt = events.length > 1 ? Math.round((events[events.length - 1].t - events[0].t) / (events.length - 1)) + ' сек' : '—';

  if (title) title.innerText = `Матч от ${m.date}`;
  if (sub) sub.innerText = `SESSION: ${m.sessionId || '—'} // ДЛИТЕЛЬНОСТЬ: ${fmtTime(m.duration)}`;

  const timelineHtml = events.length === 0
    ? '<span style="color:var(--text-muted); font-size:13px;">Подробный лог событий отсутствует.</span>'
    : events.map((ev, idx) => {
        const meta = CHEST_META[ev.type] || CHEST_META.normal;
        const statusPill = ev.d
          ? '<span class="mono-pill" style="color:var(--red); border-color:rgba(220,38,38,0.35);">СЛОМАН</span>'
          : '<span class="mono-pill green">ОТКРЫТ</span>';
        return `
          <div class="timeline-item">
            <div style="display:flex; align-items:center; gap:10px; flex-wrap:wrap;">
              <span style="font-family:var(--mono); color:var(--text-ghost); width:26px;">#${idx + 1}</span>
              <span style="font-family:var(--mono); color:var(--gold-bright); font-weight:700;">${fmtTime(ev.t)}</span>
              <img class="chest-icon-lg" style="width:32px; height:32px;" src="${meta.img}" alt="${meta.name}">
              <span style="color:var(--text-main); font-weight:600;">${meta.name}</span>
              <span class="mono-pill">${ev.loc || 'Окрестности'}</span>
            </div>
            ${statusPill}
          </div>
        `;
      }).join('');

  const locs = m.locations || {};
  const locEntries = Object.entries(locs).sort((a, b) => b[1] - a[1]);
  const totalLocChests = Math.max(1, locEntries.reduce((acc, [, c]) => acc + c, 0));
  const locationsHtml = locEntries.length === 0
    ? '<span style="color:var(--text-muted); font-size:13px;">Нет данных по локациям для этого матча.</span>'
    : locEntries.map(([locName, cnt]) => {
        const pct = Math.max(4, Math.round((cnt / totalLocChests) * 100));
        return `
          <div style="background:var(--surface-well); border:1px solid var(--border); border-radius:8px; padding:8px 12px;">
            <div style="display:flex; justify-content:space-between; align-items:center; font-size:13px;">
              <b style="color:var(--text-main);">${locName}</b>
              <span><b style="color:var(--gold-bright); font-family:var(--mono);">${cnt}</b> <span style="color:var(--text-ghost); font-family:var(--mono); font-size:11px;">(${pct}%)</span></span>
            </div>
            <div style="background:rgba(128,128,128,0.12); border-radius:4px; height:5px; margin-top:6px; overflow:hidden;">
              <div style="background:var(--gold); width:${pct}%; height:100%; border-radius:4px;"></div>
            </div>
          </div>
        `;
      }).join('');

  body.innerHTML = `
    <div style="display:grid; grid-template-columns: repeat(4, 1fr); gap:10px;">
      <div class="stat-box">
        <div class="stat-label">Всего сундуков</div>
        <div class="stat-val highlight">${m.chests}</div>
        <div class="stat-sub">${m.opened || 0} откр. / ${m.destroyed || 0} слом.</div>
      </div>
      <div class="stat-box">
        <div class="stat-label">Темп (CPM)</div>
        <div class="stat-val">${cpm}</div>
        <div class="stat-sub">ср. интервал: ${avgInt}</div>
      </div>
      <div class="stat-box">
        <div class="stat-label">Первый сундук</div>
        <div class="stat-val">${firstT}</div>
        <div class="stat-sub">от старта матча</div>
      </div>
      <div class="stat-box">
        <div class="stat-label">Счёт Пати</div>
        <div class="stat-val">${m.squadChests}</div>
        <div class="stat-sub">${m.squad && m.squad.length ? m.squad.join(', ') : 'Соло'}</div>
      </div>
    </div>

    <div>
      <div style="font-size:11px; font-weight:700; color:var(--text-ghost); text-transform:uppercase; letter-spacing:0.6px; margin-bottom:8px;">Сундуки в этом матче</div>
      ${renderChestBadges(m.chestBreakdown, m.chests)}
    </div>

    <div>
      <div style="font-size:11px; font-weight:700; color:var(--text-ghost); text-transform:uppercase; letter-spacing:0.6px; margin-bottom:8px;">Хронология лута (${events.length})</div>
      <div style="display:flex; flex-direction:column; gap:6px;">
        ${timelineHtml}
      </div>
    </div>

    <div>
      <div style="font-size:11px; font-weight:700; color:var(--text-ghost); text-transform:uppercase; letter-spacing:0.6px; margin-bottom:8px;">Аналитика по локациям</div>
      <div style="display:flex; flex-direction:column; gap:6px;">
        ${locationsHtml}
      </div>
    </div>
  `;

  modal.classList.add('open');
}

function closeMatchModal() {
  const modal = document.getElementById('matchModal');
  if (modal) modal.classList.remove('open');
}

document.addEventListener('keydown', (e) => {
  if (e.key === 'Escape') closeMatchModal();
});

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
