"use strict";
(() => {
  // src/api.ts
  var API_BASE = "/api";
  async function parseJson(r) {
    if (!r.ok) {
      const text = await r.text();
      throw new Error(text || r.statusText || String(r.status));
    }
    return r.json();
  }
  async function getJson(path) {
    const r = await fetch(`${API_BASE}${path}`, {
      headers: { Accept: "application/json" }
    });
    return parseJson(r);
  }
  function tournamentsByDate(dateIso) {
    return getJson(
      `/Tournaments/by-date/${dateIso}/matches`
    );
  }
  function matchById(id) {
    return getJson(`/Matches/${id}`);
  }
  function tournamentById(id) {
    return getJson(`/Tournaments/${id}`);
  }
  function teamStatsByTournament(tournamentId) {
    return getJson(
      `/TeamStats/tournaments/${tournamentId}`
    );
  }
  function tournamentMatchesResult(tournamentId, page, pageSize) {
    const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    return getJson(
      `/tournaments/${tournamentId}/matches/result?${q}`
    );
  }
  function tournamentMatchesCalendar(tournamentId, page, pageSize) {
    const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    return getJson(
      `/tournaments/${tournamentId}/matches/calendar?${q}`
    );
  }
  function teamCalendar(teamId, page, pageSize) {
    const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    return getJson(
      `/Teams/${teamId}/calendar?${q}`
    );
  }
  function teamResults(teamId, page, pageSize) {
    const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    return getJson(
      `/Teams/${teamId}/results?${q}`
    );
  }
  function playersByTeam(teamId) {
    return getJson(`/Players?teamId=${teamId}`);
  }
  function playerById(id) {
    return getJson(`/Players/${id}`);
  }

  // src/nav.ts
  function go(path) {
    window.history.pushState({}, "", path);
    window.dispatchEvent(new PopStateEvent("popstate"));
  }

  // src/ui/utils.ts
  function escapeHtml(s) {
    return s.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#039;");
  }
  var FINISHED = "\u0417\u0430\u043A\u043E\u043D\u0447\u0435\u043D";
  function isMatchFinished(status) {
    return status === FINISHED;
  }
  function formatGoalTime(seconds) {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${String(m).padStart(2, "0")}:${String(s).padStart(2, "0")}`;
  }
  function formatTimeOnly(iso) {
    const d = new Date(iso);
    return new Intl.DateTimeFormat("ru-RU", {
      hour: "2-digit",
      minute: "2-digit"
    }).format(d);
  }
  function toDateOnlyIso(d) {
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, "0");
    const day = String(d.getDate()).padStart(2, "0");
    return `${y}-${m}-${day}`;
  }
  function parseDateOnlyIso(s) {
    const [y, m, d] = s.split("-").map(Number);
    return new Date(y, m - 1, d);
  }
  function weekdayShort(d) {
    return new Intl.DateTimeFormat("ru-RU", { weekday: "short" }).format(d);
  }
  function dayMonthLabel(d) {
    return new Intl.DateTimeFormat("ru-RU", { day: "2-digit", month: "2-digit" }).format(d);
  }
  function addDays(d, n) {
    const x = new Date(d);
    x.setDate(x.getDate() + n);
    return x;
  }
  function placeholderImg(alt, className = "media-block__img") {
    return `<img class="${escapeHtml(className)}" src="/assets/placeholder.svg" alt="${escapeHtml(alt)}" width="160" height="160" loading="lazy" />`;
  }
  function el(html) {
    const t = document.createElement("template");
    t.innerHTML = html.trim();
    const n = t.content.firstChild;
    if (!n || !(n instanceof HTMLElement)) throw new Error("el: invalid html");
    return n;
  }

  // src/components/matchRow.ts
  function renderMatchRow(m, opts) {
    const finished = isMatchFinished(m.status);
    const rowClass = finished ? "match-row match-row--done" : "match-row match-row--live";
    const ot = finished && m.isOvertime && opts?.showOtForFinished !== false ? '<span class="match-row__ot" title="\u041E\u0432\u0435\u0440\u0442\u0430\u0439\u043C">\u041E\u0422</span>' : "";
    const score = `${m.homeTeamScore} : ${m.awayTeamScore}`;
    return `
    <div class="${rowClass}" data-nav-match="${m.id}">
      <div class="match-row__teams">
        <a class="match-row__team" href="/team/${m.homeTeam.id}" data-app-link>${escapeHtml(m.homeTeam.name)}</a>
        <span class="match-row__score">${escapeHtml(score)}${ot}</span>
        <a class="match-row__team match-row__team--away" href="/team/${m.awayTeam.id}" data-app-link>${escapeHtml(m.awayTeam.name)}</a>
      </div>
      <div class="match-row__meta">
        <span class="match-row__time">${escapeHtml(new Date(m.scheduleAt).toLocaleTimeString("ru-RU", { hour: "2-digit", minute: "2-digit" }))}</span>
        <span class="match-row__status">${escapeHtml(m.status)}</span>
      </div>
    </div>
  `;
  }
  function bindMatchRowClicks(container) {
    container.querySelectorAll("[data-nav-match]").forEach((row) => {
      row.addEventListener("click", (e) => {
        const t = e.target;
        if (t?.closest("a[data-app-link]")) return;
        const id = row.dataset.navMatch;
        if (id) {
          e.preventDefault();
          go(`/match/${id}`);
        }
      });
    });
  }

  // src/views/home.ts
  function isSameDay(a, b) {
    return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
  }
  async function renderHome(root, query) {
    const today = /* @__PURE__ */ new Date();
    const dateStr = query.get("date");
    const center = dateStr ? parseDateOnlyIso(dateStr) : today;
    const iso = toDateOnlyIso(center);
    const data = await tournamentsByDate(iso);
    const stripDays = [];
    for (let i = -4; i <= 4; i++) stripDays.push(addDays(center, i));
    const stripHtml = stripDays.map((d) => {
      const active = isSameDay(d, center) ? " date-strip__btn--active" : "";
      const label = isSameDay(d, today) ? `\u0421\u0435\u0433\u043E\u0434\u043D\u044F ${dayMonthLabel(d)}` : `${weekdayShort(d)} ${dayMonthLabel(d)}`;
      const di = toDateOnlyIso(d);
      return `<button type="button" class="date-strip__btn${active}" data-date="${di}">${escapeHtml(label)}</button>`;
    }).join("");
    const blocks = data.map((t) => {
      const matchesHtml = t.matches.map((m) => renderMatchRow(m)).join("");
      return `
        <details class="tournament-block" open>
          <summary class="tournament-block__summary">
            <span class="tournament-block__chevron" aria-hidden="true"></span>
            <a class="tournament-block__title" href="/tournament/${t.id}" data-app-link>${escapeHtml(t.name)}</a>
          </summary>
          <div class="tournament-block__body">
            ${matchesHtml || '<p class="muted">\u041D\u0435\u0442 \u043C\u0430\u0442\u0447\u0435\u0439 \u043D\u0430 \u044D\u0442\u0443 \u0434\u0430\u0442\u0443.</p>'}
          </div>
        </details>
      `;
    }).join("");
    root.replaceChildren(
      el(`
      <section class="page page--home">
        <div class="page-head">
          <h1 class="page-title">\u041C\u0430\u0442\u0447\u0438</h1>
          <p class="page-sub">\u0422\u0443\u0440\u043D\u0438\u0440\u044B \u0438 \u0438\u0433\u0440\u044B \u043D\u0430 \u0432\u044B\u0431\u0440\u0430\u043D\u043D\u044B\u0439 \u0434\u0435\u043D\u044C</p>
        </div>
        <div class="date-toolbar">
          <div class="date-strip">${stripHtml}</div>
          <label class="date-any">
            <span class="date-any__label">\u0414\u0440\u0443\u0433\u0430\u044F \u0434\u0430\u0442\u0430</span>
            <input class="date-any__input" type="date" value="${escapeHtml(iso)}" max="2099-12-31" min="1990-01-01" />
          </label>
        </div>
        <div class="tournament-list">
          ${blocks || '<p class="muted panel">\u041D\u0430 \u044D\u0442\u0443 \u0434\u0430\u0442\u0443 \u043D\u0435\u0442 \u0437\u0430\u043F\u043B\u0430\u043D\u0438\u0440\u043E\u0432\u0430\u043D\u043D\u044B\u0445 \u043C\u0430\u0442\u0447\u0435\u0439.</p>'}
        </div>
      </section>
    `)
    );
    const section = root.querySelector(".page--home");
    if (!section) return;
    section.querySelectorAll(".date-strip__btn").forEach((btn) => {
      btn.addEventListener("click", () => {
        const d = btn.dataset.date;
        if (d) go(`/?date=${d}`);
      });
    });
    const any = section.querySelector(".date-any__input");
    any?.addEventListener("change", () => {
      if (any.value) go(`/?date=${any.value}`);
    });
    bindMatchRowClicks(section);
  }

  // src/views/match.ts
  function periodTitle(period, periodsCount) {
    if (period <= periodsCount) return `${period}-\u0439 \u043F\u0435\u0440\u0438\u043E\u0434`;
    return "\u041E\u0432\u0435\u0440\u0442\u0430\u0439\u043C";
  }
  function sortGoals(goals) {
    return [...goals].sort((a, b) => a.period - b.period || a.time - b.time);
  }
  function buildRunningScores(match) {
    const key = (g) => `${g.period}-${g.time}-${g.goalScorerId.id}`;
    const map = /* @__PURE__ */ new Map();
    let h = 0;
    let aw = 0;
    const homeId = match.homeTeam.id;
    for (const g of sortGoals(match.goals)) {
      if (g.scoringTeamId.id === homeId) h++;
      else aw++;
      map.set(key(g), `${h}:${aw}`);
    }
    return map;
  }
  async function renderMatchPage(root, id) {
    const match = await matchById(id);
    const periodsCount = match.rules?.matchDurationRules?.periodsCount ?? 3;
    const scoreMap = buildRunningScores(match);
    const goalsByPeriod = /* @__PURE__ */ new Map();
    for (const g of match.goals) {
      const list = goalsByPeriod.get(g.period) ?? [];
      list.push(g);
      goalsByPeriod.set(g.period, list);
    }
    for (const [, list] of goalsByPeriod) {
      list.sort((a, b) => a.time - b.time);
    }
    const periodNumbers = /* @__PURE__ */ new Set();
    for (let p = 1; p <= periodsCount; p++) periodNumbers.add(p);
    for (const g of match.goals) {
      if (g.period > periodsCount) periodNumbers.add(g.period);
    }
    const sortedPeriods = [...periodNumbers].sort((a, b) => a - b);
    const goalKey = (g) => `${g.period}-${g.time}-${g.goalScorerId.id}`;
    const overviewBlocks = sortedPeriods.map((pNum) => {
      const title = periodTitle(pNum, periodsCount);
      const goals = goalsByPeriod.get(pNum) ?? [];
      const body = goals.length === 0 ? '<p class="period-block__empty">\u2014</p>' : goals.map((g) => {
        const scoreAfter = scoreMap.get(goalKey(g)) ?? "";
        const assists = [];
        if (g.firstAssistId) assists.push(`${g.firstAssistId.name} ${g.firstAssistId.surname}`);
        if (g.secondAssistId) assists.push(`${g.secondAssistId.name} ${g.secondAssistId.surname}`);
        const detailParts = [g.strengthType, g.netType].filter(Boolean);
        const detail = detailParts.length ? detailParts.join(" \xB7 ") : "";
        const assistsHtml = assists.length > 0 ? `<div class="goal-line__assists">\u0410\u0441\u0441\u0438\u0441\u0442\u0435\u043D\u0442\u044B: ${assists.map((x) => escapeHtml(x)).join(", ")}</div>` : "";
        return `
                  <article class="goal-line">
                    <div class="goal-line__row">
                      <span class="goal-line__time">${formatGoalTime(g.time)}</span>
                      <span class="goal-line__score">${escapeHtml(scoreAfter)}</span>
                      <span class="goal-line__scorer">
                        <a href="/player/${g.goalScorerId.id}" data-app-link>${escapeHtml(`${g.goalScorerId.name} ${g.goalScorerId.surname}`)}</a>
                      </span>
                    </div>
                    ${detail ? `<div class="goal-line__detail">${escapeHtml(detail)}</div>` : ""}
                    ${assistsHtml}
                  </article>
                `;
      }).join("");
      return `
        <section class="period-block">
          <h3 class="period-block__title">${escapeHtml(title)}</h3>
          ${body}
        </section>
      `;
    }).join("");
    const roster = (players) => players.map(
      (pl) => `
      <li class="roster-item">
        <a href="/player/${pl.id}" data-app-link class="roster-item__link">
          <span class="roster-item__num">${escapeHtml(pl.position)}</span>
          <span class="roster-item__name">${escapeHtml(`${pl.name} ${pl.surname}`)}</span>
        </a>
      </li>
    `
    ).join("");
    root.replaceChildren(
      el(`
      <section class="page page--match" data-tabs>
        <div class="match-hero">
          <div class="match-hero__side">
            ${placeholderImg(match.homeTeam.name)}
            <a class="match-hero__team" href="/team/${match.homeTeam.id}" data-app-link>${escapeHtml(match.homeTeam.name)}</a>
          </div>
          <div class="match-hero__center">
            <div class="match-hero__score">${match.homeTeamScore} : ${match.awayTeamScore}</div>
            ${match.isOvertime ? '<div class="badge">\u041E\u0422</div>' : ""}
            <div class="match-hero__time">${escapeHtml(formatTimeOnly(match.scheduleAt))}</div>
            <div class="match-hero__status">${escapeHtml(match.status)}</div>
            <a class="muted-link" href="/tournament/${match.tournament.id}" data-app-link>${escapeHtml(match.tournament.name)}</a>
          </div>
          <div class="match-hero__side">
            ${placeholderImg(match.awayTeam.name)}
            <a class="match-hero__team" href="/team/${match.awayTeam.id}" data-app-link>${escapeHtml(match.awayTeam.name)}</a>
          </div>
        </div>

        <div class="tabs">
          <button type="button" class="tabs__btn tabs__btn--active" data-tab="overview">\u041E\u0431\u0437\u043E\u0440</button>
          <button type="button" class="tabs__btn" data-tab="rosters">\u0421\u043E\u0441\u0442\u0430\u0432\u044B</button>
        </div>

        <div class="tabs__panel tabs__panel--active" data-panel="overview">
          <div class="periods">${overviewBlocks}</div>
        </div>
        <div class="tabs__panel" data-panel="rosters" hidden>
          <div class="roster-grid">
            <div class="roster-col">
              <h3>${escapeHtml(match.homeTeam.name)}</h3>
              <ul class="roster-list">${roster(match.homeTeamRoster)}</ul>
            </div>
            <div class="roster-col">
              <h3>${escapeHtml(match.awayTeam.name)}</h3>
              <ul class="roster-list">${roster(match.awayTeamRoster)}</ul>
            </div>
          </div>
        </div>
      </section>
    `)
    );
    setupTabs(root.querySelector("[data-tabs]"));
  }
  function setupTabs(host) {
    if (!host) return;
    const btns = host.querySelectorAll(".tabs__btn");
    const panels = host.querySelectorAll(".tabs__panel");
    btns.forEach((btn) => {
      btn.addEventListener("click", () => {
        const tab = btn.dataset.tab;
        btns.forEach((b) => b.classList.toggle("tabs__btn--active", b === btn));
        panels.forEach((p) => {
          const on = p.dataset.panel === tab;
          p.classList.toggle("tabs__panel--active", on);
          if (on) p.removeAttribute("hidden");
          else p.setAttribute("hidden", "");
        });
      });
    });
  }

  // src/views/tournament.ts
  var PAGE_SIZE = 200;
  function numCell(v) {
    if (v === null) return "\u2014";
    return String(v);
  }
  function comparePrimary(a, b, key, asc) {
    const s = asc ? 1 : -1;
    switch (key) {
      case "teamName":
        return a.teamName.localeCompare(b.teamName, "ru") * s;
      case "games":
        return (a.games - b.games) * s;
      case "regularWins":
        return (a.regularWins - b.regularWins) * s;
      case "otWins":
        return (a.otWins - b.otWins) * s;
      case "otLosses":
        return (a.otLosses - b.otLosses) * s;
      case "regularLosses":
        return (a.regularLosses - b.regularLosses) * s;
      case "draws":
        return (a.draws - b.draws) * s;
      case "points":
        return (a.points - b.points) * s;
      default:
        return 0;
    }
  }
  function tieBreak(a, b) {
    if (b.points !== a.points) return b.points - a.points;
    if (b.regularWins !== a.regularWins) return b.regularWins - a.regularWins;
    if (b.otWins !== a.otWins) return b.otWins - a.otWins;
    return a.teamName.localeCompare(b.teamName, "ru");
  }
  function sortRows(rows, key, asc) {
    return [...rows].sort((a, b) => {
      const p = comparePrimary(a, b, key, asc);
      if (p !== 0) return p;
      return tieBreak(a, b);
    });
  }
  function renderTableBody(rows) {
    return rows.map((r, idx) => {
      return `
      <tr>
        <td>${idx + 1}</td>
        <td><a href="/team/${r.teamId}" data-app-link>${escapeHtml(r.teamName)}</a></td>
        <td>${r.games}</td>
        <td>${r.regularWins}</td>
        <td>${r.otWins}</td>
        <td>${r.otLosses}</td>
        <td>${r.regularLosses}</td>
        <td>${r.draws}</td>
        <td>${numCell(null)}</td>
        <td>${numCell(null)}</td>
        <td>${numCell(null)}</td>
        <td class="col-points">${r.points}</td>
      </tr>`;
    }).join("");
  }
  async function renderTournamentPage(root, id) {
    const [t, stats, finished, schedule] = await Promise.all([
      tournamentById(id),
      teamStatsByTournament(id),
      tournamentMatchesResult(id, 1, PAGE_SIZE),
      tournamentMatchesCalendar(id, 1, PAGE_SIZE)
    ]);
    let sortKey = "points";
    let sortAsc = false;
    const resort = () => sortRows(stats, sortKey, sortAsc);
    const tableHtml = (rows) => `
    <div class="table-wrap">
      <table class="data-table" data-sort-table>
        <thead>
          <tr>
            <th>#</th>
            <th data-sort="teamName">\u041A\u043E\u043C\u0430\u043D\u0434\u0430</th>
            <th data-sort="games">\u0418</th>
            <th data-sort="regularWins" title="\u041F\u043E\u0431\u0435\u0434\u044B \u0432 \u043E\u0441\u043D\u043E\u0432\u043D\u043E\u0435 \u0432\u0440\u0435\u043C\u044F">\u0412</th>
            <th data-sort="otWins" title="\u041F\u043E\u0431\u0435\u0434\u044B \u0432 \u043E\u0432\u0435\u0440\u0442\u0430\u0439\u043C\u0435">\u0412\u041E</th>
            <th data-sort="otLosses" title="\u041F\u043E\u0440\u0430\u0436\u0435\u043D\u0438\u044F \u0432 \u043E\u0432\u0435\u0440\u0442\u0430\u0439\u043C\u0435">\u041F\u041E</th>
            <th data-sort="regularLosses" title="\u041F\u043E\u0440\u0430\u0436\u0435\u043D\u0438\u044F \u0432 \u043E\u0441\u043D\u043E\u0432\u043D\u043E\u0435 \u0432\u0440\u0435\u043C\u044F">\u041F</th>
            <th data-sort="draws">\u041D</th>
            <th title="\u0417\u0430\u0431\u0438\u0442\u043E (\u043F\u043E\u043A\u0430 \u043D\u0435\u0442 \u0432 API)">\u0417</th>
            <th title="\u041F\u0440\u043E\u043F\u0443\u0449\u0435\u043D\u043E (\u043F\u043E\u043A\u0430 \u043D\u0435\u0442 \u0432 API)">\u041F\u0440</th>
            <th title="\u0420\u0430\u0437\u043D\u0438\u0446\u0430 (\u043F\u043E\u043A\u0430 \u043D\u0435\u0442 \u0432 API)">\u0420</th>
            <th data-sort="points">\u041E</th>
          </tr>
        </thead>
        <tbody>${renderTableBody(rows)}</tbody>
      </table>
    </div>
    <p class="table-hint muted">\u041A\u043E\u043B\u043E\u043D\u043A\u0438 \xAB\u0417\xBB, \xAB\u041F\xBB, \xAB\u0420\xBB (\u0437\u0430\u0431\u0438\u0442\u043E / \u043F\u0440\u043E\u043F\u0443\u0449\u0435\u043D\u043E / \u0440\u0430\u0437\u043D\u0438\u0446\u0430) \u043F\u043E\u043A\u0430 \u043D\u0435\u0434\u043E\u0441\u0442\u0443\u043F\u043D\u044B \u0432 API \u2014 \u0437\u0430\u0440\u0435\u0437\u0435\u0440\u0432\u0438\u0440\u043E\u0432\u0430\u043D\u044B \u043F\u043E\u0434 \u0431\u0443\u0434\u0443\u0449\u0438\u0435 \u0434\u0430\u043D\u043D\u044B\u0435.</p>
  `;
    const resultsHtml = finished.map((m) => renderMatchRow(m)).join("") || '<p class="muted">\u041D\u0435\u0442 \u0437\u0430\u0432\u0435\u0440\u0448\u0451\u043D\u043D\u044B\u0445 \u043C\u0430\u0442\u0447\u0435\u0439.</p>';
    const calHtml = schedule.map((m) => renderMatchRow(m)).join("") || '<p class="muted">\u041D\u0435\u0442 \u0437\u0430\u043F\u043B\u0430\u043D\u0438\u0440\u043E\u0432\u0430\u043D\u043D\u044B\u0445 \u043C\u0430\u0442\u0447\u0435\u0439.</p>';
    root.replaceChildren(
      el(`
      <section class="page page--tournament" data-tournament-tabs="${id}">
        <div class="entity-head">
          ${placeholderImg(t.name)}
          <div>
            <h1 class="page-title">${escapeHtml(t.name)}</h1>
            <p class="page-sub">${escapeHtml(t.status)}</p>
          </div>
        </div>

        <div class="tabs">
          <button type="button" class="tabs__btn tabs__btn--active" data-tab="table">\u0422\u0430\u0431\u043B\u0438\u0446\u0430</button>
          <button type="button" class="tabs__btn" data-tab="results">\u0420\u0435\u0437\u0443\u043B\u044C\u0442\u0430\u0442\u044B</button>
          <button type="button" class="tabs__btn" data-tab="calendar">\u041A\u0430\u043B\u0435\u043D\u0434\u0430\u0440\u044C</button>
        </div>

        <div class="tabs__panel tabs__panel--active" data-panel="table">${tableHtml(resort())}</div>
        <div class="tabs__panel" data-panel="results" hidden><div class="match-stack" data-result-matches>${resultsHtml}</div></div>
        <div class="tabs__panel" data-panel="calendar" hidden><div class="match-stack" data-cal-matches>${calHtml}</div></div>
      </section>
    `)
    );
    const host = root.querySelector("[data-tournament-tabs]");
    if (!host) return;
    const btns = host.querySelectorAll(".tabs__btn");
    const panels = host.querySelectorAll(".tabs__panel");
    btns.forEach((btn) => {
      btn.addEventListener("click", () => {
        const tab = btn.dataset.tab;
        btns.forEach((b) => b.classList.toggle("tabs__btn--active", b === btn));
        panels.forEach((p) => {
          const on = p.dataset.panel === tab;
          p.classList.toggle("tabs__panel--active", on);
          if (on) p.removeAttribute("hidden");
          else p.setAttribute("hidden", "");
        });
      });
    });
    const tbody = host.querySelector("tbody");
    host.querySelector("[data-sort-table]")?.addEventListener("click", (e) => {
      const th = e.target.closest("th[data-sort]");
      if (!th || !tbody) return;
      const k = th.dataset.sort;
      if (sortKey === k) sortAsc = !sortAsc;
      else {
        sortKey = k;
        sortAsc = k === "teamName";
      }
      tbody.innerHTML = renderTableBody(resort());
    });
    bindMatchRowClicks(host);
  }

  // src/views/team.ts
  var PAGE_SIZE2 = 200;
  async function teamMeta(id) {
    return getJson(`/Teams/${id}`);
  }
  function renderTournamentGroups(blocks) {
    if (blocks.length === 0) return '<p class="muted">\u041D\u0435\u0442 \u043C\u0430\u0442\u0447\u0435\u0439.</p>';
    return blocks.map((t) => {
      const rows = t.matches.map((m) => renderMatchRow(m)).join("");
      return `
        <section class="team-tblock">
          <h3 class="team-tblock__title"><a href="/tournament/${t.id}" data-app-link>${escapeHtml(t.name)}</a></h3>
          <div class="match-stack">${rows}</div>
        </section>
      `;
    }).join("");
  }
  async function renderTeamPage(root, id) {
    const team = await teamMeta(id);
    const [cal, res, roster] = await Promise.all([
      teamCalendar(id, 1, PAGE_SIZE2).catch(() => []),
      teamResults(id, 1, PAGE_SIZE2).catch(() => []),
      playersByTeam(id)
    ]);
    root.replaceChildren(
      el(`
      <section class="page page--team" data-team-tabs>
        <div class="entity-head">
          ${placeholderImg(team.name)}
          <div>
            <h1 class="page-title">${escapeHtml(team.name)}</h1>
          </div>
        </div>

        <div class="tabs">
          <button type="button" class="tabs__btn tabs__btn--active" data-tab="calendar">\u041A\u0430\u043B\u0435\u043D\u0434\u0430\u0440\u044C</button>
          <button type="button" class="tabs__btn" data-tab="results">\u0420\u0435\u0437\u0443\u043B\u044C\u0442\u0430\u0442\u044B</button>
          <button type="button" class="tabs__btn" data-tab="roster">\u0421\u043E\u0441\u0442\u0430\u0432</button>
        </div>

        <div class="tabs__panel tabs__panel--active" data-panel="calendar">
          ${renderTournamentGroups(cal)}
        </div>
        <div class="tabs__panel" data-panel="results" hidden>
          ${renderTournamentGroups(res)}
        </div>
        <div class="tabs__panel" data-panel="roster" hidden>
          <ul class="player-cards">
            ${roster.map(
        (p) => `
              <li class="player-cards__item">
                <a href="/player/${p.id}" data-app-link class="player-cards__link">
                  ${placeholderImg(`${p.name} ${p.surname}`, "player-cards__img")}
                  <span class="player-cards__name">${escapeHtml(`${p.name} ${p.surname}`)}</span>
                  <span class="player-cards__meta">${escapeHtml(p.position)}</span>
                </a>
              </li>`
      ).join("")}
          </ul>
          ${roster.length === 0 ? '<p class="muted">\u0418\u0433\u0440\u043E\u043A\u0438 \u043D\u0435 \u043D\u0430\u0439\u0434\u0435\u043D\u044B.</p>' : ""}
        </div>
      </section>
    `)
    );
    const host = root.querySelector("[data-team-tabs]");
    if (!host) return;
    const btns = host.querySelectorAll(".tabs__btn");
    const panels = host.querySelectorAll(".tabs__panel");
    btns.forEach((btn) => {
      btn.addEventListener("click", () => {
        const tab = btn.dataset.tab;
        btns.forEach((b) => b.classList.toggle("tabs__btn--active", b === btn));
        panels.forEach((p) => {
          const on = p.dataset.panel === tab;
          p.classList.toggle("tabs__panel--active", on);
          if (on) p.removeAttribute("hidden");
          else p.setAttribute("hidden", "");
        });
      });
    });
    bindMatchRowClicks(host);
  }

  // src/views/player.ts
  async function renderPlayerPage(root, id) {
    const p = await playerById(id);
    root.replaceChildren(
      el(`
      <section class="page page--player">
        <div class="entity-head entity-head--player">
          ${placeholderImg(`${p.name} ${p.surname}`, "entity-head__photo")}
          <div>
            <h1 class="page-title">${escapeHtml(`${p.name} ${p.surname}`)}</h1>
            <p class="page-sub">${escapeHtml(p.position)}</p>
            <p class="page-sub">
              \u041A\u043E\u043C\u0430\u043D\u0434\u0430:
              <a href="/team/${p.teamId}" data-app-link>${escapeHtml(p.teamName)}</a>
            </p>
          </div>
        </div>
      </section>
    `)
    );
  }

  // src/router.ts
  function parsePath(pathname, search) {
    const q = new URLSearchParams(search);
    const parts = pathname.split("/").filter(Boolean);
    if (parts.length === 0) return { name: "home", query: q };
    if (parts[0] === "match" && parts[1]) {
      const id = Number(parts[1]);
      if (Number.isFinite(id)) return { name: "match", id };
    }
    if (parts[0] === "tournament" && parts[1]) {
      const id = Number(parts[1]);
      if (Number.isFinite(id)) return { name: "tournament", id };
    }
    if (parts[0] === "team" && parts[1]) {
      const id = Number(parts[1]);
      if (Number.isFinite(id)) return { name: "team", id };
    }
    if (parts[0] === "player" && parts[1]) {
      const id = Number(parts[1]);
      if (Number.isFinite(id)) return { name: "player", id };
    }
    return { name: "notfound" };
  }
  function renderNotFound(root) {
    root.replaceChildren(
      el(`<section class="panel"><h1>\u0421\u0442\u0440\u0430\u043D\u0438\u0446\u0430 \u043D\u0435 \u043D\u0430\u0439\u0434\u0435\u043D\u0430</h1><p><a href="/" data-app-link>\u041D\u0430 \u0433\u043B\u0430\u0432\u043D\u0443\u044E</a></p></section>`)
    );
  }
  async function renderRoute(route, root) {
    root.innerHTML = '<div class="loading">\u0417\u0430\u0433\u0440\u0443\u0437\u043A\u0430\u2026</div>';
    try {
      switch (route.name) {
        case "home":
          await renderHome(root, route.query);
          break;
        case "match":
          await renderMatchPage(root, route.id);
          break;
        case "tournament":
          await renderTournamentPage(root, route.id);
          break;
        case "team":
          await renderTeamPage(root, route.id);
          break;
        case "player":
          await renderPlayerPage(root, route.id);
          break;
        default:
          renderNotFound(root);
      }
    } catch (e) {
      const msg = e instanceof Error ? e.message : String(e);
      root.innerHTML = "";
      root.appendChild(
        el(`<section class="panel panel--error"><h1>\u041E\u0448\u0438\u0431\u043A\u0430</h1><p>${escapeAttr(msg)}</p><p><a href="/" data-app-link>\u041D\u0430 \u0433\u043B\u0430\u0432\u043D\u0443\u044E</a></p></section>`)
      );
    } finally {
      window.scrollTo(0, 0);
    }
  }
  function escapeAttr(s) {
    return s.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/"/g, "&quot;");
  }
  function initRouter() {
    const root = document.getElementById("app");
    if (!root) return;
    const run = () => {
      const route = parsePath(window.location.pathname, window.location.search);
      void renderRoute(route, root);
    };
    window.addEventListener("popstate", run);
    document.addEventListener("click", (e) => {
      const target = e.target;
      const a = target?.closest("a[data-app-link]");
      if (!a || !a.href) return;
      const url = new URL(a.href);
      if (url.origin !== window.location.origin) return;
      if (e.ctrlKey || e.metaKey || e.shiftKey || e.altKey) return;
      e.preventDefault();
      e.stopPropagation();
      go(url.pathname + url.search);
    });
    run();
  }

  // src/main.ts
  initRouter();
})();
//# sourceMappingURL=bundle.js.map
