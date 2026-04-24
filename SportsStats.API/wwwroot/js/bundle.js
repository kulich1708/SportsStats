"use strict";
(() => {
  // src/api.ts
  var API_BASE = "/api";
  function extractServerError(raw) {
    if (!raw) return "";
    try {
      const parsed = JSON.parse(raw);
      if (typeof parsed.error === "string" && parsed.error.trim()) return parsed.error;
      if (typeof parsed.message === "string" && parsed.message.trim()) return parsed.message;
      if (typeof parsed.details === "string" && parsed.details.trim()) return parsed.details;
    } catch {
    }
    return raw;
  }
  function logApiError(r, raw, message) {
    console.error("[API ERROR]", {
      url: r.url,
      status: r.status,
      statusText: r.statusText,
      message,
      raw
    });
  }
  async function parseJson(r) {
    if (!r.ok) {
      const raw = await r.text();
      const message = extractServerError(raw) || r.statusText || String(r.status);
      logApiError(r, raw, message);
      throw new Error(`HTTP ${r.status}: ${message}`);
    }
    return r.json();
  }
  async function getJson(path) {
    const r = await fetch(`${API_BASE}${path}`, {
      headers: { Accept: "application/json" }
    });
    return parseJson(r);
  }
  async function postJson(path, body) {
    const r = await fetch(`${API_BASE}${path}`, {
      method: "POST",
      headers: { "Content-Type": "application/json", Accept: "application/json" },
      body: body === void 0 ? void 0 : JSON.stringify(body)
    });
    if (r.status === 204) return void 0;
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
  function teamById(id) {
    return getJson(`/Teams/${id}`);
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
    return getJson(`/Players/by-team?teamId=${teamId}`);
  }
  function playerById(id) {
    return getJson(`/Players/${id}`);
  }
  function playerPositions() {
    return getJson("/Players/positions");
  }
  function tournamentsPaged(page, pageSize, search = "") {
    const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    if (search) q.set("search", search);
    return getJson(`/Tournaments?${q}`);
  }
  function teamsPaged(page, pageSize, search = "") {
    const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    if (search) q.set("search", search);
    return getJson(`/Teams?${q}`);
  }
  function playersPaged(page, pageSize, search = "") {
    const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    if (search) q.set("search", search);
    return getJson(`/Players?${q}`);
  }
  function teamByTournament(tournamentId) {
    return getJson(`/Teams/by-tournament?tournamentId=${tournamentId}`);
  }
  function addPlayersToRoster(matchId, teamId, playerIds) {
    return postJson(`/Matches/${matchId}/roster`, {
      playerIds,
      teamId
    });
  }
  function startMatch(matchId) {
    return postJson(`/Matches/${matchId}/start`, null);
  }
  function finishMatch(matchId) {
    return postJson(`/Matches/${matchId}/finish`, null);
  }
  function addGoal(matchId, dto) {
    return postJson(`/Matches/${matchId}/goals`, dto);
  }
  var strengthTypeWire = {
    EvenStrength: 0,
    PowerPlay: 1,
    Shorthanded: 2
  };
  function fillGoal(matchId, goalId, dto) {
    const body = {
      scorerId: dto.scorerId,
      firstAssistId: dto.firstAssistId,
      secondAssistId: dto.secondAssistId,
      strengthType: strengthTypeWire[dto.strengthType],
      netType: dto.netType === "EmptyNet" ? 0 : null
    };
    return postJson(`/Matches/${matchId}/goals/${goalId}`, body);
  }
  function tournamentChangeGeneralInfo(id, dto) {
    return postJson(`/Tournaments/${id}/general/change`, dto);
  }
  function tournamentSetRules(id, rules) {
    return postJson(`/Tournaments/${id}/rules/set`, rules);
  }
  function tournamentRegistrationStep(id) {
    return postJson(`/Tournaments/${id}/registration`, null);
  }
  function tournamentStart(id) {
    return postJson(`/Tournaments/${id}/start`, null);
  }
  function tournamentFinish(id) {
    return postJson(`/Tournaments/${id}/finish`, null);
  }
  function tournamentSetRegistrationTeams(tournamentId, teamIds) {
    return postJson(`/Tournaments/${tournamentId}/teams`, teamIds);
  }
  function teamChangeGeneralInfo(id, dto) {
    return postJson(`/Teams/${id}/general/change`, dto);
  }
  function playerChangeGeneralInfo(id, dto) {
    return postJson(`/Players/${id}/general/change`, dto);
  }
  function createTournament(name) {
    return postJson(`/Tournaments`, { name });
  }
  function createTeam(name) {
    return postJson(`/Teams`, { name });
  }
  function createPlayer(dto) {
    return postJson(`/Players`, dto);
  }
  function createTournamentMatch(tournamentId, dto) {
    return postJson(`/tournaments/${tournamentId}/matches`, dto);
  }
  function matchChangeGeneralInfo(id, dto) {
    return postJson(`/Matches/${id}/general`, dto);
  }

  // src/appMode.ts
  var ADMIN_PREFIX = "/admin";
  function isAdminPath(pathname) {
    return pathname === ADMIN_PREFIX || pathname.startsWith(`${ADMIN_PREFIX}/`);
  }
  function stripAdminPrefix(pathname) {
    let p = pathname;
    while (p === ADMIN_PREFIX || p.startsWith(`${ADMIN_PREFIX}/`)) {
      p = p.slice(ADMIN_PREFIX.length);
      if (p.length === 0) return "/";
    }
    return p;
  }
  function withMode(path, admin) {
    if (!admin) return path;
    if (path === ADMIN_PREFIX || path.startsWith(`${ADMIN_PREFIX}/`)) return path;
    if (path === "/") return ADMIN_PREFIX;
    return `${ADMIN_PREFIX}${path}`;
  }

  // src/nav.ts
  function go(path, admin = false) {
    const target = withMode(path, admin);
    window.history.pushState({}, "", target);
    window.dispatchEvent(new PopStateEvent("popstate"));
  }
  function currentAdminMode() {
    return window.location.pathname === "/admin" || window.location.pathname.startsWith("/admin/");
  }
  function goSameMode(path) {
    go(path, currentAdminMode());
  }
  function urlFor(path, admin) {
    return withMode(path, admin);
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
  function dtoPhotoSrc(photo, photoMime) {
    if (!photo || !photoMime) return "/assets/placeholder.svg";
    return `data:${photoMime};base64,${photo}`;
  }
  function dtoImg(alt, photo, photoMime, className = "media-block__img") {
    return `<img class="${escapeHtml(className)}" src="${escapeHtml(dtoPhotoSrc(photo, photoMime))}" alt="${escapeHtml(alt)}" loading="lazy" />`;
  }
  async function fileToBase64(file) {
    const b = await file.arrayBuffer();
    let binary = "";
    const bytes = new Uint8Array(b);
    const chunk = 32768;
    for (let i = 0; i < bytes.length; i += chunk) {
      binary += String.fromCharCode(...bytes.slice(i, i + chunk));
    }
    return btoa(binary);
  }
  function el(html) {
    const t = document.createElement("template");
    t.innerHTML = html.trim();
    const n = t.content.firstChild;
    if (!n || !(n instanceof HTMLElement)) throw new Error("el: invalid html");
    return n;
  }
  function citizenshipBadgeHtml(c) {
    if (!c || !c.name?.trim() && !c.photo) return "";
    const alt = c.name.trim() || "\u0413\u0440\u0430\u0436\u0434\u0430\u043D\u0441\u0442\u0432\u043E";
    return `<span class="citizenship-inline" title="${escapeHtml(c.name)}">${dtoImg(alt, c.photo, c.photoMime, "citizenship-flag-img")}</span>`;
  }
  function iconSvgEdit() {
    return `<svg class="icon-svg" width="16" height="16" viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zm2.92 2.83H5v-.92l9.06-9.06.92.92L5.92 20.08zM20.71 7.04a1 1 0 0 0 0-1.41l-2.34-2.34a1 1 0 0 0-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/></svg>`;
  }
  function iconSvgTrash() {
    return `<svg class="icon-svg" width="16" height="16" viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M9 3V2h6v1h5v2H4V3h5zm1 5h2v9h-2V8zm4 0h2v9h-2V8zM6 6h12l-1 14H7L6 6z"/></svg>`;
  }
  function photoMediaFieldHtml(label, previewInnerHtml) {
    return `
    <div class="media-field">
      <div class="media-field__toolbar">
        <span class="media-field__label">${escapeHtml(label)}</span>
        <input type="file" accept="image/*" class="media-field__file" data-photo-file hidden />
        <button type="button" class="icon-btn icon-btn--tight" data-photo-pick title="\u0412\u044B\u0431\u0440\u0430\u0442\u044C \u0438\u0437\u043E\u0431\u0440\u0430\u0436\u0435\u043D\u0438\u0435" aria-label="\u0412\u044B\u0431\u0440\u0430\u0442\u044C \u0438\u0437\u043E\u0431\u0440\u0430\u0436\u0435\u043D\u0438\u0435">${iconSvgEdit()}</button>
        <button type="button" class="icon-btn icon-btn--tight icon-btn--danger" data-photo-clear title="\u0423\u0434\u0430\u043B\u0438\u0442\u044C \u0438\u0437\u043E\u0431\u0440\u0430\u0436\u0435\u043D\u0438\u0435" aria-label="\u0423\u0434\u0430\u043B\u0438\u0442\u044C \u0438\u0437\u043E\u0431\u0440\u0430\u0436\u0435\u043D\u0438\u0435">${iconSvgTrash()}</button>
      </div>
      <div class="media-field__preview" data-photo-preview>${previewInnerHtml}</div>
    </div>`;
  }
  function mountPhotoMediaField(root, opts) {
    const input = root.querySelector("[data-photo-file]");
    const preview = root.querySelector("[data-photo-preview]");
    const pick = root.querySelector("[data-photo-pick]");
    const clear = root.querySelector("[data-photo-clear]");
    const sync = () => {
      preview.innerHTML = opts.getPreviewHtml();
    };
    pick.addEventListener("click", () => input.click());
    input.addEventListener("change", async () => {
      const f = input.files?.[0];
      if (!f) return;
      await opts.onPickFile(f);
      sync();
      input.value = "";
    });
    clear.addEventListener("click", () => {
      opts.onClear();
      input.value = "";
      sync();
    });
  }

  // src/components/matchRow.ts
  function renderMatchRow(m, opts) {
    const finished = isMatchFinished(m.status.description);
    const rowClass = finished ? "match-row match-row--done" : "match-row match-row--live";
    const ot = finished && m.isOvertime && opts?.showOtForFinished !== false ? '<span class="match-row__ot" title="\u041E\u0432\u0435\u0440\u0442\u0430\u0439\u043C">\u041E\u0422</span>' : "";
    const score = `${m.homeTeamScore} : ${m.awayTeamScore}`;
    return `
    <div class="${rowClass}" data-nav-match="${m.id}">
      <div class="match-row__teams">
        <div class="match-row__team-col match-row__team-col--home">
          <a class="match-row__team-link entity-link" href="${urlFor(`/team/${m.homeTeam.id}`, !!opts?.admin)}" data-app-link>
            ${dtoImg(m.homeTeam.name, m.homeTeam.photo, m.homeTeam.photoMime, "mini-photo")}
            <span class="match-row__team-text">${escapeHtml(m.homeTeam.name)}</span>
          </a>
        </div>
        <div class="match-row__score-cell">
          <span class="match-row__score-wrap">
            <span class="match-row__score">${escapeHtml(score)}</span>${ot}
          </span>
        </div>
        <div class="match-row__team-col match-row__team-col--away">
          <a class="match-row__team-link entity-link" href="${urlFor(`/team/${m.awayTeam.id}`, !!opts?.admin)}" data-app-link>
            ${dtoImg(m.awayTeam.name, m.awayTeam.photo, m.awayTeam.photoMime, "mini-photo")}
            <span class="match-row__team-text">${escapeHtml(m.awayTeam.name)}</span>
          </a>
        </div>
      </div>
      <div class="match-row__meta">
        <span class="match-row__time">${escapeHtml(new Date(m.scheduleAt).toLocaleTimeString("ru-RU", { hour: "2-digit", minute: "2-digit" }))}</span>
        <span class="match-row__status">${escapeHtml(m.status.description)}</span>
        ${opts?.showEdit ? `<a class="icon-btn" href="${urlFor(`/admin/edit/match/${m.id}`, true)}" data-app-link title="\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u0442\u044C \u043C\u0430\u0442\u0447">\u270E</a>` : ""}
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
          goSameMode(`/match/${id}`);
        }
      });
    });
  }

  // src/views/home.ts
  function bindHomeAccordions(host) {
    host.querySelectorAll("[data-accordion]").forEach((block) => {
      const summary = block.querySelector("[data-accordion-summary]");
      const body = block.querySelector("[data-accordion-body]");
      if (!summary || !body) return;
      const apply = (open) => {
        block.classList.toggle("is-open", open);
        body.classList.toggle("is-open", open);
        summary.setAttribute("aria-expanded", open ? "true" : "false");
      };
      apply(block.classList.contains("is-open"));
      const toggle = (e) => {
        if (e) {
          const t = e.target;
          if (t?.closest("a[data-app-link], button, input, textarea, select")) return;
        }
        apply(!block.classList.contains("is-open"));
      };
      summary.addEventListener("click", (e) => toggle(e));
      summary.addEventListener("keydown", (e) => {
        if (e.key === "Enter" || e.key === " ") {
          e.preventDefault();
          toggle();
        }
      });
    });
  }
  function isSameDay(a, b) {
    return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
  }
  async function renderHome(root, query, admin) {
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
      const matchesHtml = t.matches.map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 })).join("");
      return `
        <section class="tournament-block tournament-block--accordion is-open" data-accordion>
          <div class="tournament-block__head" data-accordion-summary tabindex="0" role="button" aria-expanded="true">
            <span class="tournament-block__chevron" aria-hidden="true"></span>
            ${dtoImg(t.name, t.photo, t.photoMime, "mini-photo")}
            <span class="tournament-block__title-wrap">
              <a class="tournament-block__title" href="${urlFor(`/tournament/${t.id}`, admin)}" data-app-link>${escapeHtml(t.name)}</a>
            </span>
            ${admin ? `<a class="icon-btn" href="${urlFor(`/admin/edit/tournament/${t.id}`, true)}" data-app-link title="\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u0442\u044C \u0442\u0443\u0440\u043D\u0438\u0440">\u270E</a>` : ""}
          </div>
          <div class="tournament-block__collapsible is-open" data-accordion-body>
            <div class="tournament-block__collapsible-inner">
              <div class="tournament-block__body">
                ${matchesHtml || '<p class="muted">\u041D\u0435\u0442 \u043C\u0430\u0442\u0447\u0435\u0439 \u043D\u0430 \u044D\u0442\u0443 \u0434\u0430\u0442\u0443.</p>'}
              </div>
            </div>
          </div>
        </section>
      `;
    }).join("");
    root.replaceChildren(
      el(`
      <section class="page page--home">
        <div class="page-head">
          <h1 class="page-title">\u041C\u0430\u0442\u0447\u0438</h1>
          <p class="page-sub">\u0422\u0443\u0440\u043D\u0438\u0440\u044B \u0438 \u0438\u0433\u0440\u044B \u043D\u0430 \u0432\u044B\u0431\u0440\u0430\u043D\u043D\u044B\u0439 \u0434\u0435\u043D\u044C</p>
          ${admin ? '<p class="muted">\u0420\u0435\u0436\u0438\u043C \u0430\u0434\u043C\u0438\u043D\u0438\u0441\u0442\u0440\u0430\u0442\u043E\u0440\u0430</p>' : ""}
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
        if (d) goSameMode(`/?date=${d}`);
      });
    });
    const any = section.querySelector(".date-any__input");
    any?.addEventListener("change", () => {
      if (any.value) goSameMode(`/?date=${any.value}`);
    });
    bindMatchRowClicks(section);
    bindHomeAccordions(section);
  }

  // src/ui/modal.ts
  function openModal(title, contentHtml, opts) {
    const dialogCls = ["app-modal__dialog", opts?.wide && "app-modal__dialog--wide", opts?.tall && "app-modal__dialog--tall"].filter(Boolean).join(" ");
    const root = el(`
    <div class="app-modal app-modal--open">
      <div class="app-modal__backdrop" data-close></div>
      <div class="${dialogCls}">
        <header class="app-modal__head">
          <h3>${title}</h3>
          <button class="icon-btn" type="button" data-close title="\u0417\u0430\u043A\u0440\u044B\u0442\u044C">\xD7</button>
        </header>
        <div class="app-modal__body">${contentHtml}</div>
      </div>
    </div>
  `);
    const body = root.querySelector(".app-modal__body");
    const close = () => {
      root.remove();
      document.body.classList.remove("no-scroll");
    };
    root.querySelectorAll("[data-close]").forEach((x) => x.addEventListener("click", close));
    document.body.classList.add("no-scroll");
    document.body.appendChild(root);
    return { root, body, close };
  }

  // src/ui/errors.ts
  var STACK_ID = "app-error-toast-stack";
  var INVALID_CLASS = "search-input--invalid";
  function ensureStack() {
    let stack = document.getElementById(STACK_ID);
    if (!stack) {
      stack = document.createElement("div");
      stack.id = STACK_ID;
      stack.className = "app-error-toast-stack";
      stack.setAttribute("aria-live", "polite");
      document.body.appendChild(stack);
    }
    return stack;
  }
  var DEFAULT_TOAST_MS = 5e3;
  function defaultDurationMs() {
    return DEFAULT_TOAST_MS;
  }
  function pushToast(className, message, durationMs) {
    const stack = ensureStack();
    const toast = document.createElement("div");
    toast.className = className;
    toast.textContent = message;
    stack.appendChild(toast);
    window.setTimeout(() => {
      toast.remove();
      if (!stack.childElementCount) {
        stack.remove();
      }
    }, durationMs);
  }
  function clearInvalidHighlightsIn(root) {
    root.querySelectorAll(`.${INVALID_CLASS}`).forEach((el2) => {
      el2.classList.remove(INVALID_CLASS);
    });
  }
  function showValidationToast(options) {
    const { message, highlightInputs = [] } = options;
    const toHighlight = highlightInputs.filter((el2) => Boolean(el2));
    for (const el2 of toHighlight) {
      el2.classList.add(INVALID_CLASS);
    }
    const durationMs = options.durationMs ?? defaultDurationMs();
    pushToast("app-error-toast", message, durationMs);
  }
  function showSuccessToast(options) {
    const durationMs = options.durationMs ?? defaultDurationMs();
    pushToast("app-success-toast", options.message, durationMs);
  }
  function showErrorToast(options) {
    const durationMs = options.durationMs ?? defaultDurationMs();
    pushToast("app-error-toast", options.message, durationMs);
  }

  // src/ui/entitySelect.ts
  var entitySelectRadioSeq = 0;
  function mountEntitySelect(host, options) {
    const pageSize = options.pageSize ?? 40;
    const radioGroupId = ++entitySelectRadioSeq;
    const showMore = options.showMore !== false;
    const searchable = options.searchable !== false;
    let open = false;
    let page = 1;
    let search = "";
    let loaded = [];
    let selected = options.initialSelected ? [...options.initialSelected] : [];
    host.replaceChildren(el(`
    <div class="entity-select" data-es>
      <div class="entity-select__trigger-wrap">
        <span class="entity-select__trigger-thumb" data-es-trigger-thumb aria-hidden="true"></span>
        <input class="entity-select__trigger" data-es-trigger ${searchable ? "" : "readonly"} />
      </div>
      <div class="entity-select__menu" data-es-menu>
        <div class="entity-select__list" data-es-list></div>
        ${showMore ? '<button type="button" class="action-btn action-btn--ghost" data-es-more>\u041F\u043E\u043A\u0430\u0437\u0430\u0442\u044C \u0435\u0449\u0451</button>' : ""}
      </div>
    </div>
  `));
    const root = host.querySelector("[data-es]");
    const trigger = host.querySelector("[data-es-trigger]");
    const thumb = host.querySelector("[data-es-trigger-thumb]");
    const list = host.querySelector("[data-es-list]");
    const more = host.querySelector("[data-es-more]");
    trigger.placeholder = options.placeholder;
    const renderTrigger = () => {
      const one = selected.length === 1 ? selected[0] : null;
      if (thumb) {
        if (open || !one?.photo) thumb.innerHTML = "";
        else thumb.innerHTML = dtoImg(one.name, one.photo, one.photoMime, "entity-select__thumb-img");
      }
      if (selected.length === 0) {
        if (!open) trigger.value = "";
        return;
      }
      if (!open) trigger.value = selected.map((x) => x.name).join(", ");
    };
    const isSelected = (id) => selected.some((x) => x.id === id);
    const renderList = () => {
      list.innerHTML = loaded.map((x) => `
        <label class="entity-select__item">
          <input type="${options.multiple ? "checkbox" : "radio"}" name="entity-select-${radioGroupId}" value="${x.id}" ${isSelected(x.id) ? "checked" : ""} />
          ${dtoImg(x.name, x.photo, x.photoMime, "entity-select__photo")}
          <span>
            <strong>${escapeHtml(x.name)}</strong>${citizenshipBadgeHtml(x.citizenship ?? null)}
            ${x.sub ? `<small class="muted">${escapeHtml(x.sub)}</small>` : ""}
          </span>
        </label>
      `).join("");
    };
    const loadPage = async (reset) => {
      if (reset) {
        page = 1;
        loaded = [];
      }
      const effectiveSearch = searchable ? search : "";
      const chunk = await options.load({ page, pageSize, search: effectiveSearch });
      loaded = reset ? chunk : [...loaded, ...chunk];
      renderList();
    };
    const openMenu = () => {
      if (open) return;
      open = true;
      root.classList.add("is-open");
      if (searchable) {
        trigger.value = search;
        trigger.focus();
      } else {
        trigger.focus();
      }
      renderTrigger();
    };
    const closeMenu = () => {
      if (!open) return;
      open = false;
      root.classList.remove("is-open");
      search = "";
      renderTrigger();
    };
    const syncSelectionFromInputs = () => {
      const checked = list.querySelectorAll("input:checked");
      if (options.multiple) {
        selected = loaded.filter((x) => Array.from(checked).some((c) => Number(c.value) === x.id));
      } else {
        const one = checked[0];
        selected = one ? loaded.filter((x) => x.id === Number(one.value)) : [];
        closeMenu();
      }
      renderTrigger();
      options.onChange?.(selected);
    };
    trigger.addEventListener("click", () => {
      openMenu();
    });
    let timer3 = 0;
    trigger.addEventListener("input", () => {
      if (!open || !searchable) return;
      if (timer3) window.clearTimeout(timer3);
      timer3 = window.setTimeout(async () => {
        search = trigger.value.trim();
        await loadPage(true);
      }, 300);
    });
    list.addEventListener("change", syncSelectionFromInputs);
    more?.addEventListener("click", async () => {
      page += 1;
      await loadPage(false);
    });
    document.addEventListener("click", (e) => {
      if (!open) return;
      const target = e.target;
      if (target && root.contains(target)) return;
      closeMenu();
    });
    void loadPage(true);
    renderTrigger();
    return {
      getSelected: () => [...selected],
      setSelected: (items) => {
        selected = [...items];
        renderTrigger();
        renderList();
        options.onChange?.(selected);
      },
      reload: () => loadPage(true)
    };
  }

  // src/views/match.ts
  function periodTitle(period, periodsCount) {
    if (period <= periodsCount) return `${period}-\u0439 \u043F\u0435\u0440\u0438\u043E\u0434`;
    const otIndex = period - periodsCount;
    return `${otIndex}-\u0439 \u043E\u0432\u0435\u0440\u0442\u0430\u0439\u043C`;
  }
  function mergeDisplayedPeriods(match, periodsCount, goals) {
    const periods = new Set(Array.from({ length: periodsCount }, (_, i) => i + 1));
    goals.forEach((g) => periods.add(g.period));
    const tr = match.rules?.matchTimeRules;
    const ot = tr?.overtimeRules;
    if (tr?.hasOvertime && ot) {
      const otGoalPeriods = goals.map((g) => g.period).filter((pp) => pp > periodsCount);
      if (otGoalPeriods.length > 0) {
        const maxFromGoals = Math.max(...otGoalPeriods);
        let hi;
        if (ot.goalEndsOvertime) {
          hi = maxFromGoals;
        } else {
          const cap = ot.overtimesCount != null ? periodsCount + ot.overtimesCount : maxFromGoals;
          hi = Math.max(cap, maxFromGoals);
        }
        for (let pp = periodsCount + 1; pp <= hi; pp++) periods.add(pp);
      }
    }
    return [...periods].sort((a, b) => a - b);
  }
  var STRENGTH_FROM_API = {
    "\u0412 \u0440\u0430\u0432\u043D\u044B\u0445 \u0441\u043E\u0441\u0442\u0430\u0432\u0430\u0445": "EvenStrength",
    "\u0412 \u0431\u043E\u043B\u044C\u0448\u0438\u043D\u0441\u0442\u0432\u0435": "PowerPlay",
    "\u0412 \u043C\u0435\u043D\u044C\u0448\u0435\u043D\u0441\u0442\u0432\u0435": "Shorthanded",
    EvenStrength: "EvenStrength",
    PowerPlay: "PowerPlay",
    Shorthanded: "Shorthanded"
  };
  function strengthFromGoalApi(text) {
    const t = (text ?? "").trim();
    if (!t) return "EvenStrength";
    const mapped = STRENGTH_FROM_API[t];
    if (mapped) return mapped;
    const low = t.toLowerCase();
    if (low === "evenstrength" || t === "0") return "EvenStrength";
    if (low === "powerplay" || t === "1") return "PowerPlay";
    if (low === "shorthanded" || t === "2") return "Shorthanded";
    return "EvenStrength";
  }
  function emptyNetFromGoalApi(text) {
    const t = (text ?? "").trim();
    if (!t) return false;
    if (t === "\u0412 \u043F\u0443\u0441\u0442\u044B\u0435 \u0432\u043E\u0440\u043E\u0442\u0430") return true;
    const low = t.toLowerCase();
    if (low === "emptynet" || t === "0") return true;
    return false;
  }
  function sortGoals(goals) {
    return [...goals].sort((a, b) => a.period - b.period || a.time - b.time || (a.id ?? 0) - (b.id ?? 0));
  }
  function scoreAfterGoal(match) {
    const key = (g) => String(g.id);
    const map = /* @__PURE__ */ new Map();
    let h = 0;
    let a = 0;
    for (const g of sortGoals(match.goals)) {
      if (g.scoringTeamId.id === match.homeTeam.id) h += 1;
      else a += 1;
      map.set(key(g), `${h}:${a}`);
    }
    return map;
  }
  function setupTabs(host) {
    if (!host) return;
    const btns = host.querySelectorAll(".tabs__btn");
    const panels = host.querySelectorAll(".tabs__panel");
    btns.forEach((btn) => btn.addEventListener("click", () => {
      const tab = btn.dataset.tab;
      btns.forEach((b) => b.classList.toggle("tabs__btn--active", b === btn));
      panels.forEach((p) => {
        const on = p.dataset.panel === tab;
        p.classList.toggle("tabs__panel--active", on);
        if (on) p.removeAttribute("hidden");
        else p.setAttribute("hidden", "");
      });
    }));
  }
  async function renderMatchPage(root, id, admin) {
    const match = await matchById(id);
    const periodsCount = match.rules?.matchTimeRules?.periodsCount ?? 3;
    const scoreMap = scoreAfterGoal(match);
    const goals = sortGoals(match.goals);
    const goalKey = (g) => String(g.id);
    const periods = mergeDisplayedPeriods(match, periodsCount, goals);
    root.replaceChildren(
      el(`
      <section class="page page--match" data-tabs>
        <div class="match-hero">
          <div class="match-hero__side">
            <a href="${urlFor(`/team/${match.homeTeam.id}`, admin)}" data-app-link>${dtoImg(match.homeTeam.name, match.homeTeam.photo, match.homeTeam.photoMime)}</a>
            <a class="match-hero__team" href="${urlFor(`/team/${match.homeTeam.id}`, admin)}" data-app-link>${escapeHtml(match.homeTeam.name)}</a>
          </div>
          <div class="match-hero__center">
            <div class="match-hero__score">${match.homeTeamScore} : ${match.awayTeamScore}</div>
            ${match.isOvertime ? '<div class="badge">\u041E\u0422</div>' : ""}
            <div class="match-hero__time">${escapeHtml(formatTimeOnly(match.scheduleAt))}</div>
            <div class="match-hero__status">${escapeHtml(match.status.description)}</div>
            <a class="muted-link" href="${urlFor(`/tournament/${match.tournament.id}`, admin)}" data-app-link>${escapeHtml(match.tournament.name)}</a>
          </div>
          <div class="match-hero__side">
            <a href="${urlFor(`/team/${match.awayTeam.id}`, admin)}" data-app-link>${dtoImg(match.awayTeam.name, match.awayTeam.photo, match.awayTeam.photoMime)}</a>
            <a class="match-hero__team" href="${urlFor(`/team/${match.awayTeam.id}`, admin)}" data-app-link>${escapeHtml(match.awayTeam.name)}</a>
          </div>
        </div>
        ${admin ? `<div class="action-row">
          ${match.status.code === 0 ? `<a class="action-btn" href="${urlFor(`/admin/edit/match/${id}`, true)}" data-app-link>\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u0442\u044C \u043C\u0430\u0442\u0447</a>` : ""}
          ${match.status.code < 2 ? `<button type="button" class="action-btn" data-next-status>${escapeHtml(match.status.nextActionDescription)}</button>` : ""}
          ${match.status.code === 1 ? `<button type="button" class="action-btn" data-add-goal>\u0414\u043E\u0431\u0430\u0432\u0438\u0442\u044C \u0433\u043E\u043B</button>` : ""}
        </div>` : ""}
        <div class="tabs"><button type="button" class="tabs__btn tabs__btn--active" data-tab="overview">\u041E\u0431\u0437\u043E\u0440</button><button type="button" class="tabs__btn" data-tab="rosters">\u0421\u043E\u0441\u0442\u0430\u0432\u044B</button></div>
        <div class="tabs__panel tabs__panel--active" data-panel="overview">
          <div class="periods">
            ${periods.map((p) => {
        const pg = goals.filter((g) => g.period === p);
        if (pg.length === 0) return `<section class="period-block"><h3 class="period-block__title">${periodTitle(p, periodsCount)}</h3><p class="period-block__empty">\u2014</p></section>`;
        return `<section class="period-block"><h3 class="period-block__title">${periodTitle(p, periodsCount)}</h3>${pg.map((g) => `
                <article class="goal-line">
                  <div class="goal-line__row"><span class="goal-line__time">${formatGoalTime(g.time)}</span><span class="goal-line__score">${scoreMap.get(goalKey(g)) ?? ""}</span>
                    <span class="goal-line__scorer"><a href="${urlFor(`/player/${g.goalScorerId.id}`, admin)}" data-app-link>${escapeHtml(`${g.goalScorerId.name} ${g.goalScorerId.surname}`)}</a>${citizenshipBadgeHtml(g.goalScorerId.citizenship ?? null)}</span>
                    ${admin ? `<button type="button" class="icon-btn" data-goal-id="${g.id}" title="\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u0442\u044C \u0433\u043E\u043B">\u270E</button>` : ""}
                  </div>
                  <div class="goal-line__detail">${escapeHtml([g.strengthType, g.netType].filter(Boolean).join(" \xB7 "))}</div>
                  <div class="goal-line__assists">${g.firstAssistId || g.secondAssistId ? `\u0410\u0441\u0441\u0438\u0441\u0442\u0435\u043D\u0442\u044B: ${[g.firstAssistId, g.secondAssistId].filter(Boolean).map((x) => `${x.name} ${x.surname}`).map(escapeHtml).join(", ")}` : ""}</div>
                </article>`).join("")}</section>`;
      }).join("")}
          </div>
        </div>
        <div class="tabs__panel" data-panel="rosters" hidden>
          <div class="roster-grid">
            <div class="roster-col"><h3>${escapeHtml(match.homeTeam.name)}</h3><ul class="roster-list" data-roster-home></ul></div>
            <div class="roster-col"><h3>${escapeHtml(match.awayTeam.name)}</h3><ul class="roster-list" data-roster-away></ul></div>
          </div>
        </div>
      </section>
    `)
    );
    setupTabs(root.querySelector("[data-tabs]"));
    const rosterHome = root.querySelector("[data-roster-home]");
    const rosterAway = root.querySelector("[data-roster-away]");
    const homePlayers = match.homeTeamRoster;
    const awayPlayers = match.awayTeamRoster;
    const renderRoster = (players, host) => {
      if (!host) return;
      host.innerHTML = players.map((pl) => `
      <li class="roster-item">
        <label class="roster-item__link">
          ${dtoImg(`${pl.name} ${pl.surname}`, pl.photo, pl.photoMime, "mini-photo player-cards__img--round")}
          <span class="roster-item__name"><a href="${urlFor(`/player/${pl.id}`, admin)}" data-app-link>${escapeHtml(`${pl.name} ${pl.surname}`)}</a>${citizenshipBadgeHtml(pl.citizenship ?? null)} \xB7 ${escapeHtml(pl.position.name)}</span>
        </label>
      </li>
    `).join("");
    };
    renderRoster(homePlayers, rosterHome);
    renderRoster(awayPlayers, rosterAway);
    if (admin) {
      root.querySelector("[data-next-status]")?.addEventListener("click", async () => {
        try {
          if (match.status.code === 0) {
            await startMatch(id);
            showSuccessToast({ message: "\u041C\u0430\u0442\u0447 \u043D\u0430\u0447\u0430\u0442." });
          } else if (match.status.code === 1) {
            await finishMatch(id);
            showSuccessToast({ message: "\u041C\u0430\u0442\u0447 \u0437\u0430\u0432\u0435\u0440\u0448\u0451\u043D." });
          }
          window.dispatchEvent(new PopStateEvent("popstate"));
        } catch (e) {
          showErrorToast({ message: e instanceof Error ? e.message : String(e) });
        }
      });
      const openGoalDialog = async (editGoal) => {
        const ASSIST_NONE_ID = -1;
        const assistNoneItem = () => ({
          id: ASSIST_NONE_ID,
          name: "\u0411\u0435\u0437 \u0430\u0441\u0441\u0438\u0441\u0442\u0435\u043D\u0442\u0430",
          sub: null,
          photo: null,
          photoMime: null
        });
        const playerToItem = (p) => ({
          id: p.id,
          name: `${p.name} ${p.surname}`,
          sub: p.position.name,
          photo: p.photo ?? null,
          photoMime: p.photoMime ?? null,
          citizenship: p.citizenship ?? null
        });
        const initialTeamId = editGoal?.scoringTeamId.id ?? match.homeTeam.id;
        let teamId = initialTeamId;
        const strength = editGoal ? strengthFromGoalApi(editGoal.strengthType) : "EvenStrength";
        const emptyNetChecked = editGoal ? emptyNetFromGoalApi(editGoal.netType) : false;
        const modal = openModal(
          editGoal ? "\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u043D\u0438\u0435 \u0433\u043E\u043B\u0430" : "\u0414\u043E\u0431\u0430\u0432\u043B\u0435\u043D\u0438\u0435 \u0433\u043E\u043B\u0430",
          `<div class="goal-dialog">
          ${editGoal ? `<div class="goal-dialog__readonly-meta muted">
            <p class="goal-dialog__readonly-line"><strong>${escapeHtml(editGoal.scoringTeamId.name)}</strong> \xB7 ${escapeHtml(periodTitle(editGoal.period, periodsCount))} \xB7 ${formatGoalTime(editGoal.time)}</p>
          </div>` : `<div class="goal-dialog__time-row">
            <div class="field-stack">
              <span class="field-stack__label">\u041F\u0435\u0440\u0438\u043E\u0434</span>
              <input class="field-stack__input search-input" required type="number" min="1" max="${periodsCount + 5}" value="1" data-period />
            </div>
            <div class="field-stack">
              <span class="field-stack__label">\u041C\u0438\u043D\u0443\u0442\u044B</span>
              <input class="field-stack__input search-input" required type="number" min="0" max="59" value="0" data-min />
            </div>
            <div class="field-stack">
              <span class="field-stack__label">\u0421\u0435\u043A\u0443\u043D\u0434\u044B</span>
              <input class="field-stack__input search-input" required type="number" min="0" max="59" value="0" data-sec />
            </div>
          </div>
          <div>
            <div class="goal-dialog__section-label">\u041A\u043E\u043C\u0430\u043D\u0434\u0430</div>
            <div class="goal-dialog__teams" role="group" aria-label="\u041A\u043E\u043C\u0430\u043D\u0434\u0430">
              <button type="button" class="goal-dialog__team-tile${initialTeamId === match.homeTeam.id ? " goal-dialog__team-tile--active" : ""}" data-team-tile="home">
                ${dtoImg(match.homeTeam.name, match.homeTeam.photo, match.homeTeam.photoMime, "goal-dialog__team-photo")}
                <span class="goal-dialog__team-name">${escapeHtml(match.homeTeam.name)}</span>
              </button>
              <button type="button" class="goal-dialog__team-tile${initialTeamId === match.awayTeam.id ? " goal-dialog__team-tile--active" : ""}" data-team-tile="away">
                ${dtoImg(match.awayTeam.name, match.awayTeam.photo, match.awayTeam.photoMime, "goal-dialog__team-photo")}
                <span class="goal-dialog__team-name">${escapeHtml(match.awayTeam.name)}</span>
              </button>
            </div>
          </div>`}
          <div class="goal-dialog__field">
            <span class="goal-dialog__label">\u0410\u0432\u0442\u043E\u0440</span>
            <div data-scorer></div>
          </div>
          <div class="goal-dialog__field">
            <span class="goal-dialog__label">1-\u0439 \u0430\u0441\u0441\u0438\u0441\u0442\u0435\u043D\u0442</span>
            <div data-a1></div>
          </div>
          <div class="goal-dialog__field">
            <span class="goal-dialog__label">2-\u0439 \u0430\u0441\u0441\u0438\u0441\u0442\u0435\u043D\u0442</span>
            <div data-a2></div>
          </div>
          <div class="goal-types goal-dialog__goal-types">
            <label class="goal-types__choice"><input type="radio" name="strengthType" value="EvenStrength" ${strength === "EvenStrength" ? "checked" : ""} /> \u0412 \u0440\u0430\u0432\u043D\u044B\u0445</label>
            <label class="goal-types__choice"><input type="radio" name="strengthType" value="PowerPlay" ${strength === "PowerPlay" ? "checked" : ""} /> \u0412 \u0431\u043E\u043B\u044C\u0448\u0438\u043D\u0441\u0442\u0432\u0435</label>
            <label class="goal-types__choice"><input type="radio" name="strengthType" value="Shorthanded" ${strength === "Shorthanded" ? "checked" : ""} /> \u0412 \u043C\u0435\u043D\u044C\u0448\u0438\u043D\u0441\u0442\u0432\u0435</label>
            <label class="goal-types__choice"><input type="checkbox" data-empty-net ${emptyNetChecked ? "checked" : ""} /> \u0412 \u043F\u0443\u0441\u0442\u044B\u0435 \u0432\u043E\u0440\u043E\u0442\u0430</label>
          </div>
          <div class="action-row goal-dialog__actions"><button class="action-btn" type="button" data-save-goal>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
        </div>`,
          { tall: true, wide: true }
        );
        const getRosterByTeam = (tid) => tid === match.homeTeam.id ? match.homeTeamRoster : match.awayTeamRoster;
        const homeTile = modal.body.querySelector('[data-team-tile="home"]');
        const awayTile = modal.body.querySelector('[data-team-tile="away"]');
        const setTeamTiles = () => {
          if (!homeTile || !awayTile) return;
          homeTile.classList.toggle("goal-dialog__team-tile--active", teamId === match.homeTeam.id);
          awayTile.classList.toggle("goal-dialog__team-tile--active", teamId === match.awayTeam.id);
        };
        const scorerHost = modal.body.querySelector("[data-scorer]");
        const a1Host = modal.body.querySelector("[data-a1]");
        const a2Host = modal.body.querySelector("[data-a2]");
        const peer = { scorer: null, a1: null, a2: null };
        const reloadExcept = (k) => {
          ["scorer", "a1", "a2"].forEach((key) => {
            if (key === k) return;
            void peer[key]?.reload();
          });
        };
        const clearPickerHosts = () => {
          scorerHost.innerHTML = "";
          a1Host.innerHTML = "";
          a2Host.innerHTML = "";
        };
        const mountPickers = () => {
          clearPickerHosts();
          peer.scorer = peer.a1 = peer.a2 = null;
          const roster = getRosterByTeam(teamId);
          const matchesSearch = (pl, search) => `${pl.name} ${pl.surname}`.toLowerCase().includes(search.trim().toLowerCase());
          const buildPlayers = (search, exclude) => roster.filter((pl) => !exclude.has(pl.id)).filter((pl) => matchesSearch(pl, search)).map(playerToItem);
          const initialScorer = editGoal && editGoal.scoringTeamId.id === teamId ? [playerToItem(editGoal.goalScorerId)] : [];
          const initialA1 = editGoal && editGoal.scoringTeamId.id === teamId && editGoal.firstAssistId ? [playerToItem(editGoal.firstAssistId)] : [];
          const initialA2 = editGoal && editGoal.scoringTeamId.id === teamId && editGoal.secondAssistId ? [playerToItem(editGoal.secondAssistId)] : [];
          peer.scorer = mountEntitySelect(scorerHost, {
            placeholder: "\u0412\u044B\u0431\u0435\u0440\u0438\u0442\u0435 \u0438\u0433\u0440\u043E\u043A\u0430",
            multiple: false,
            showMore: false,
            initialSelected: initialScorer,
            load: async ({ search }) => {
              const ex = /* @__PURE__ */ new Set();
              const a1v = peer.a1?.getSelected()[0]?.id;
              const a2v = peer.a2?.getSelected()[0]?.id;
              if (a1v && a1v !== ASSIST_NONE_ID) ex.add(a1v);
              if (a2v && a2v !== ASSIST_NONE_ID) ex.add(a2v);
              return buildPlayers(search, ex);
            },
            onChange: () => reloadExcept("scorer")
          });
          peer.a1 = mountEntitySelect(a1Host, {
            placeholder: "\u0418\u0433\u0440\u043E\u043A \u0438\u043B\u0438 \u0431\u0435\u0437 \u0430\u0441\u0441\u0438\u0441\u0442\u0435\u043D\u0442\u0430",
            multiple: false,
            showMore: false,
            initialSelected: initialA1,
            load: async ({ search }) => {
              const ex = /* @__PURE__ */ new Set();
              const sv = peer.scorer?.getSelected()[0]?.id;
              const a2v = peer.a2?.getSelected()[0]?.id;
              if (sv) ex.add(sv);
              if (a2v && a2v !== ASSIST_NONE_ID) ex.add(a2v);
              return [assistNoneItem(), ...buildPlayers(search, ex)];
            },
            onChange: (sel) => {
              if (sel[0]?.id === ASSIST_NONE_ID) peer.a2?.setSelected([assistNoneItem()]);
              reloadExcept("a1");
            }
          });
          peer.a2 = mountEntitySelect(a2Host, {
            placeholder: "\u0418\u0433\u0440\u043E\u043A \u0438\u043B\u0438 \u0431\u0435\u0437 \u0430\u0441\u0441\u0438\u0441\u0442\u0435\u043D\u0442\u0430",
            multiple: false,
            showMore: false,
            initialSelected: initialA2,
            load: async ({ search }) => {
              const ex = /* @__PURE__ */ new Set();
              const sv = peer.scorer?.getSelected()[0]?.id;
              const a1v = peer.a1?.getSelected()[0]?.id;
              if (sv) ex.add(sv);
              if (a1v && a1v !== ASSIST_NONE_ID) ex.add(a1v);
              return [assistNoneItem(), ...buildPlayers(search, ex)];
            },
            onChange: () => reloadExcept("a2")
          });
          setTeamTiles();
        };
        mountPickers();
        const bindTeamTile = (btn, id2) => {
          btn.addEventListener("click", () => {
            if (teamId === id2) return;
            teamId = id2;
            setTeamTiles();
            mountPickers();
          });
        };
        if (homeTile && awayTile) {
          bindTeamTile(homeTile, match.homeTeam.id);
          bindTeamTile(awayTile, match.awayTeam.id);
        }
        modal.body.querySelector("[data-save-goal]")?.addEventListener("click", async () => {
          const period = editGoal ? editGoal.period : Number(modal.body.querySelector("[data-period]").value);
          const time = editGoal ? editGoal.time : Number(modal.body.querySelector("[data-min]").value) * 60 + Number(modal.body.querySelector("[data-sec]").value);
          const scorerId = peer.scorer.getSelected()[0]?.id;
          if (!scorerId) {
            showErrorToast({ message: "\u0423\u043A\u0430\u0436\u0438\u0442\u0435 \u0430\u0432\u0442\u043E\u0440\u0430 \u0433\u043E\u043B\u0430." });
            return;
          }
          const rawA1 = peer.a1.getSelected()[0]?.id;
          const rawA2 = peer.a2.getSelected()[0]?.id;
          const a1 = rawA1 && rawA1 !== ASSIST_NONE_ID ? rawA1 : null;
          const a2 = rawA2 && rawA2 !== ASSIST_NONE_ID ? rawA2 : null;
          const strengthType = modal.body.querySelector('input[name="strengthType"]:checked')?.value || "EvenStrength";
          const netType = modal.body.querySelector("[data-empty-net]").checked ? "EmptyNet" : null;
          try {
            if (!editGoal) {
              const goalId = await addGoal(id, { scoringTeamId: teamId, goalScorerId: scorerId, period, time });
              await fillGoal(id, goalId, { scorerId, firstAssistId: a1, secondAssistId: a2, strengthType, netType });
              showSuccessToast({ message: "\u0413\u043E\u043B \u0434\u043E\u0431\u0430\u0432\u043B\u0435\u043D \u0438 \u0441\u043E\u0445\u0440\u0430\u043D\u0451\u043D." });
            } else {
              const editGoalId = editGoal.id;
              if (editGoalId === void 0) {
                showErrorToast({ message: "\u0423 \u0433\u043E\u043B\u0430 \u043D\u0435\u0442 \u0438\u0434\u0435\u043D\u0442\u0438\u0444\u0438\u043A\u0430\u0442\u043E\u0440\u0430 \u2014 \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u0438\u0435 \u043D\u0435\u0432\u043E\u0437\u043C\u043E\u0436\u043D\u043E." });
                return;
              }
              await fillGoal(id, editGoalId, { scorerId, firstAssistId: a1, secondAssistId: a2, strengthType, netType });
              showSuccessToast({ message: "\u0421\u0432\u0435\u0434\u0435\u043D\u0438\u044F \u043E \u0433\u043E\u043B\u0435 \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u044B." });
            }
            modal.close();
            window.dispatchEvent(new PopStateEvent("popstate"));
          } catch (e) {
            showErrorToast({ message: e instanceof Error ? e.message : String(e) });
          }
        });
      };
      root.querySelector("[data-add-goal]")?.addEventListener("click", () => {
        void openGoalDialog();
      });
      root.querySelectorAll("[data-goal-id]").forEach((btn) => {
        btn.addEventListener("click", () => {
          const gid = Number(btn.dataset.goalId);
          const g = goals.find((x) => x.id === gid);
          if (g) void openGoalDialog(g);
        });
      });
    }
  }

  // src/views/tournament.ts
  var PAGE_SIZE = 40;
  function safeTeamName(name) {
    return (name ?? "").trim();
  }
  function comparePrimary(a, b, key, asc) {
    const s = asc ? 1 : -1;
    switch (key) {
      case "teamName":
        return safeTeamName(a.teamName).localeCompare(safeTeamName(b.teamName), "ru") * s;
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
    return safeTeamName(a.teamName).localeCompare(safeTeamName(b.teamName), "ru");
  }
  function sortRows(rows, key, asc) {
    return [...rows].sort((a, b) => {
      const p = comparePrimary(a, b, key, asc);
      if (p !== 0) return p;
      return tieBreak(a, b);
    });
  }
  function renderTableBody(rows, teams, admin) {
    return rows.map((r, idx) => {
      const team = teams.get(r.teamId);
      const name = safeTeamName(r.teamName) || "-";
      return `
      <tr>
        <td>${idx + 1}</td>
        <td class="data-table__team-cell">
          <a class="entity-link" href="${urlFor(`/team/${r.teamId}`, admin)}" data-app-link>
            ${dtoImg(name, team?.photo, team?.photoMime, "mini-photo")}
            <span>${escapeHtml(name)}</span>
          </a>
          ${admin ? `<a class="icon-btn" href="${urlFor(`/admin/edit/team/${r.teamId}`, true)}" data-app-link title="\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u0442\u044C \u043A\u043E\u043C\u0430\u043D\u0434\u0443">\u270E</a>` : ""}
        </td>
        <td>${r.games}</td>
        <td>${r.regularWins}</td>
        <td>${r.otWins}</td>
        <td>${r.otLosses}</td>
        <td>${r.regularLosses}</td>
        <td>${r.draws}</td>
        <td>\u2014</td>
        <td>\u2014</td>
        <td>\u2014</td>
        <td class="col-points">${r.points}</td>
      </tr>`;
    }).join("");
  }
  function combineLocalDateTimeForMatch(date, time) {
    const dateT = date.trim();
    const timeT = time.trim();
    if (!dateT || !timeT) return null;
    const dt = /* @__PURE__ */ new Date(`${dateT}T${timeT}`);
    if (Number.isNaN(dt.getTime())) return null;
    return dt.toISOString();
  }
  function openCreateMatchModal(tournamentId, allTeams) {
    const toItems = (excludeId) => allTeams.filter((x) => excludeId ? x.id !== excludeId : true).map((x) => ({
      id: x.id,
      name: x.name,
      sub: x.city,
      photo: x.photo ?? null,
      photoMime: x.photoMime ?? null
    }));
    let excludedHome;
    let excludedAway;
    const modal = openModal(
      "\u0421\u043E\u0437\u0434\u0430\u043D\u0438\u0435 \u043C\u0430\u0442\u0447\u0430",
      `
      <div class="grid-2">
        <div><span>\u0425\u043E\u0437\u044F\u0435\u0432\u0430</span><div data-home-select></div></div>
        <div><span>\u0413\u043E\u0441\u0442\u0438</span><div data-away-select></div></div>
        <div class="stack-field"><span class="stack-field__label">\u0414\u0430\u0442\u0430</span><input class="search-input" data-match-date type="date" /></div>
        <div class="stack-field"><span class="stack-field__label">\u0412\u0440\u0435\u043C\u044F</span><input class="search-input" data-match-time type="time" /></div>
      </div>
      <div class="action-row"><button class="action-btn" type="button" data-create-match-submit>\u0421\u043E\u0437\u0434\u0430\u0442\u044C</button></div>
    `,
      { tall: true }
    );
    const homeHost = modal.body.querySelector("[data-home-select]");
    const awayHost = modal.body.querySelector("[data-away-select]");
    let homePicker;
    let awayPicker;
    homePicker = mountEntitySelect(homeHost, {
      placeholder: "\u041A\u043E\u043C\u0430\u043D\u0434\u0430",
      multiple: false,
      showMore: false,
      load: async ({ search }) => {
        const q = search.toLowerCase();
        return toItems(excludedAway).filter((x) => x.name.toLowerCase().includes(q));
      },
      onChange: () => {
        excludedHome = homePicker.getSelected()[0]?.id;
        void awayPicker.reload();
      }
    });
    awayPicker = mountEntitySelect(awayHost, {
      placeholder: "\u041A\u043E\u043C\u0430\u043D\u0434\u0430",
      multiple: false,
      showMore: false,
      load: async ({ search }) => {
        const q = search.toLowerCase();
        return toItems(excludedHome).filter((x) => x.name.toLowerCase().includes(q));
      },
      onChange: () => {
        excludedAway = awayPicker.getSelected()[0]?.id;
        void homePicker.reload();
      }
    });
    const dateInput = modal.body.querySelector("[data-match-date]");
    const timeInput = modal.body.querySelector("[data-match-time]");
    modal.body.querySelector("[data-create-match-submit]")?.addEventListener("click", async () => {
      const homeId = homePicker.getSelected()[0]?.id;
      const awayId = awayPicker.getSelected()[0]?.id;
      const homeTrigger = homeHost.querySelector(".entity-select__trigger");
      const awayTrigger = awayHost.querySelector(".entity-select__trigger");
      const bad = [];
      if (!homeId && homeTrigger) bad.push(homeTrigger);
      if (!awayId && awayTrigger) bad.push(awayTrigger);
      if (!dateInput.value.trim()) bad.push(dateInput);
      if (!timeInput.value.trim()) bad.push(timeInput);
      if (bad.length) {
        showValidationToast({
          message: "\u041F\u0440\u043E\u0432\u0435\u0440\u044C\u0442\u0435 \u043F\u0440\u0430\u0432\u0438\u043B\u044C\u043D\u043E\u0441\u0442\u044C \u0437\u0430\u043F\u043E\u043B\u043D\u0435\u043D\u0438\u044F \u043F\u043E\u043B\u0435\u0439: \u0432\u044B\u0431\u0435\u0440\u0438\u0442\u0435 \u043E\u0431\u0435 \u043A\u043E\u043C\u0430\u043D\u0434\u044B \u0438 \u0443\u043A\u0430\u0436\u0438\u0442\u0435 \u0434\u0430\u0442\u0443 \u0438 \u0432\u0440\u0435\u043C\u044F.",
          highlightInputs: bad
        });
        return;
      }
      if (homeId === awayId) {
        showValidationToast({
          message: "\u041D\u0435\u043B\u044C\u0437\u044F \u0432\u044B\u0431\u0440\u0430\u0442\u044C \u043E\u0434\u043D\u0443 \u0438 \u0442\u0443 \u0436\u0435 \u043A\u043E\u043C\u0430\u043D\u0434\u0443 \u0445\u043E\u0437\u044F\u0435\u0432 \u0438 \u0433\u043E\u0441\u0442\u0435\u0439.",
          highlightInputs: [homeTrigger, awayTrigger].filter((x) => Boolean(x))
        });
        return;
      }
      const scheduledAt = combineLocalDateTimeForMatch(dateInput.value, timeInput.value);
      if (!scheduledAt) {
        showValidationToast({
          message: "\u041F\u0440\u043E\u0432\u0435\u0440\u044C\u0442\u0435 \u043F\u0440\u0430\u0432\u0438\u043B\u044C\u043D\u043E\u0441\u0442\u044C \u0437\u0430\u043F\u043E\u043B\u043D\u0435\u043D\u0438\u044F \u043F\u043E\u043B\u0435\u0439: \u0443\u043A\u0430\u0436\u0438\u0442\u0435 \u043A\u043E\u0440\u0440\u0435\u043A\u0442\u043D\u0443\u044E \u0434\u0430\u0442\u0443 \u0438 \u0432\u0440\u0435\u043C\u044F.",
          highlightInputs: [dateInput, timeInput]
        });
        return;
      }
      try {
        const newId = await createTournamentMatch(tournamentId, {
          homeTeamId: homeId,
          awayTeamId: awayId,
          scheduledAt
        });
        showSuccessToast({ message: "\u041C\u0430\u0442\u0447 \u0442\u0443\u0440\u043D\u0438\u0440\u0430 \u0441\u043E\u0437\u0434\u0430\u043D." });
        modal.close();
        goSameMode(`/admin/edit/match/${newId}`);
      } catch (e) {
        showErrorToast({ message: e instanceof Error ? e.message : String(e) });
      }
    });
  }
  async function renderTournamentPage(root, id, admin) {
    const [t, stats, teams] = await Promise.all([tournamentById(id), teamStatsByTournament(id), teamByTournament(id)]);
    const teamMap = new Map(teams.map((x) => [x.id, x]));
    let resultPage = 1;
    let calendarPage = 1;
    let finished = await tournamentMatchesResult(id, resultPage, PAGE_SIZE);
    let schedule = await tournamentMatchesCalendar(id, calendarPage, PAGE_SIZE);
    let sortKey = "points";
    let sortAsc = false;
    const resort = () => sortRows(stats, sortKey, sortAsc);
    const tableHtml = (rows) => `
    <div class="table-wrap">
      <table class="data-table" data-sort-table>
        <thead>
          <tr><th>#</th><th data-sort="teamName">\u041A\u043E\u043C\u0430\u043D\u0434\u0430</th><th data-sort="games">\u0418</th><th data-sort="regularWins">\u0412</th><th data-sort="otWins">\u0412\u041E</th><th data-sort="otLosses">\u041F\u041E</th><th data-sort="regularLosses">\u041F</th><th data-sort="draws">\u041D</th><th>\u0417</th><th>\u041F\u0440</th><th>\u0420</th><th data-sort="points">\u041E</th></tr>
        </thead>
        <tbody>${renderTableBody(rows, teamMap, admin)}</tbody>
      </table>
    </div>
  `;
    root.replaceChildren(
      el(`
      <section class="page page--tournament" data-tournament-tabs="${id}">
        <div class="entity-head">
          ${dtoImg(t.name, t.photo, t.photoMime)}
          <div><h1 class="page-title">${escapeHtml(t.name)}</h1><p class="page-sub">${escapeHtml(t.status.description)}</p>
            ${admin ? `<div class="action-row"><a class="action-btn" href="${urlFor(`/admin/edit/tournament/${t.id}`, true)}" data-app-link>\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u0442\u044C \u0442\u0443\u0440\u043D\u0438\u0440</a>${t.status.code === 2 ? `<button type="button" class="action-btn" data-create-match>\u0421\u043E\u0437\u0434\u0430\u0442\u044C \u043C\u0430\u0442\u0447</button>` : ""}</div>` : ""}
          </div>
        </div>
        <div class="tabs"><button type="button" class="tabs__btn tabs__btn--active" data-tab="table">\u0422\u0430\u0431\u043B\u0438\u0446\u0430</button><button type="button" class="tabs__btn" data-tab="results">\u0420\u0435\u0437\u0443\u043B\u044C\u0442\u0430\u0442\u044B</button><button type="button" class="tabs__btn" data-tab="calendar">\u041A\u0430\u043B\u0435\u043D\u0434\u0430\u0440\u044C</button></div>
        <div class="tabs__panel tabs__panel--active" data-panel="table">${tableHtml(resort())}</div>
        <div class="tabs__panel" data-panel="results" hidden><div class="match-stack" data-result-matches>${finished.map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 })).join("")}</div><button class="action-btn action-btn--ghost" data-more-results>\u041F\u043E\u043A\u0430\u0437\u0430\u0442\u044C \u0431\u043E\u043B\u044C\u0448\u0435</button></div>
        <div class="tabs__panel" data-panel="calendar" hidden><div class="match-stack" data-cal-matches>${schedule.map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 })).join("")}</div><button class="action-btn action-btn--ghost" data-more-calendar>\u041F\u043E\u043A\u0430\u0437\u0430\u0442\u044C \u0431\u043E\u043B\u044C\u0448\u0435</button></div>
      </section>
    `)
    );
    const host = root.querySelector("[data-tournament-tabs]");
    if (!host) return;
    const btns = host.querySelectorAll(".tabs__btn");
    const panels = host.querySelectorAll(".tabs__panel");
    btns.forEach((btn) => btn.addEventListener("click", () => {
      const tab = btn.dataset.tab;
      btns.forEach((b) => b.classList.toggle("tabs__btn--active", b === btn));
      panels.forEach((p) => {
        const on = p.dataset.panel === tab;
        p.classList.toggle("tabs__panel--active", on);
        if (on) p.removeAttribute("hidden");
        else p.setAttribute("hidden", "");
      });
    }));
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
      tbody.innerHTML = renderTableBody(resort(), teamMap, admin);
    });
    bindMatchRowClicks(host);
    root.querySelector("[data-create-match]")?.addEventListener("click", () => {
      openCreateMatchModal(id, teams);
    });
    host.querySelector("[data-more-results]")?.addEventListener("click", async () => {
      resultPage += 1;
      const chunk = await tournamentMatchesResult(id, resultPage, PAGE_SIZE);
      const wrap = host.querySelector("[data-result-matches]");
      if (!wrap || chunk.length === 0) return;
      wrap.insertAdjacentHTML("beforeend", chunk.map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 })).join(""));
      bindMatchRowClicks(wrap);
      finished = [...finished, ...chunk];
    });
    host.querySelector("[data-more-calendar]")?.addEventListener("click", async () => {
      calendarPage += 1;
      const chunk = await tournamentMatchesCalendar(id, calendarPage, PAGE_SIZE);
      const wrap = host.querySelector("[data-cal-matches]");
      if (!wrap || chunk.length === 0) return;
      wrap.insertAdjacentHTML("beforeend", chunk.map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 })).join(""));
      bindMatchRowClicks(wrap);
      schedule = [...schedule, ...chunk];
    });
  }

  // src/views/team.ts
  var PAGE_SIZE2 = 40;
  async function teamMeta(id) {
    return getJson(`/Teams/${id}`);
  }
  function renderTournamentGroups(blocks, admin) {
    if (blocks.length === 0) return '<p class="muted">\u041D\u0435\u0442 \u043C\u0430\u0442\u0447\u0435\u0439.</p>';
    return blocks.map((t) => `
      <section class="team-tblock">
        <h3 class="team-tblock__title">
          <a class="entity-link" href="${urlFor(`/tournament/${t.id}`, admin)}" data-app-link>
            ${dtoImg(t.name, t.photo, t.photoMime, "mini-photo")}
            <span>${escapeHtml(t.name)}</span>
          </a>
          
        </h3>
        <div class="match-stack">${t.matches.map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 })).join("")}</div>
      </section>
    `).join("");
  }
  async function renderTeamPage(root, id, admin) {
    const team = await teamMeta(id);
    let calPage = 1;
    let resPage = 1;
    let [cal, res, roster] = await Promise.all([
      teamCalendar(id, calPage, PAGE_SIZE2).catch(() => []),
      teamResults(id, resPage, PAGE_SIZE2).catch(() => []),
      playersByTeam(id)
    ]);
    root.replaceChildren(
      el(`
      <section class="page page--team" data-team-tabs>
        <div class="entity-head">
          ${dtoImg(team.name, team.photo, team.photoMime)}
          <div>
            <h1 class="page-title">${escapeHtml(team.name)}</h1>
            ${team.city ? `<p class="page-sub">${escapeHtml(team.city)}</p>` : ""}
            ${admin ? `<div class="action-row"><a class="action-btn" href="${urlFor(`/admin/edit/team/${team.id}`, true)}" data-app-link>\u0418\u0437\u043C\u0435\u043D\u0438\u0442\u044C \u043A\u043E\u043C\u0430\u043D\u0434\u0443</a></div>` : ""}
          </div>
        </div>

        <div class="tabs">
          <button type="button" class="tabs__btn tabs__btn--active" data-tab="calendar">\u041A\u0430\u043B\u0435\u043D\u0434\u0430\u0440\u044C</button>
          <button type="button" class="tabs__btn" data-tab="results">\u0420\u0435\u0437\u0443\u043B\u044C\u0442\u0430\u0442\u044B</button>
          <button type="button" class="tabs__btn" data-tab="roster">\u0421\u043E\u0441\u0442\u0430\u0432</button>
        </div>

        <div class="tabs__panel tabs__panel--active" data-panel="calendar">${renderTournamentGroups(cal, admin)}<button class="action-btn action-btn--ghost" data-more-cal>\u041F\u043E\u043A\u0430\u0437\u0430\u0442\u044C \u0431\u043E\u043B\u044C\u0448\u0435</button></div>
        <div class="tabs__panel" data-panel="results" hidden>${renderTournamentGroups(res, admin)}<button class="action-btn action-btn--ghost" data-more-res>\u041F\u043E\u043A\u0430\u0437\u0430\u0442\u044C \u0431\u043E\u043B\u044C\u0448\u0435</button></div>
        <div class="tabs__panel" data-panel="roster" hidden>
          <ul class="player-cards">
            ${roster.map((p) => `
              <li class="player-cards__item">
                <a href="${urlFor(`/player/${p.id}`, admin)}" data-app-link class="player-cards__link">
                  ${dtoImg(`${p.name} ${p.surname}`, p.photo, p.photoMime, "player-cards__img player-cards__img--round")}
                  <span class="player-cards__name">${escapeHtml(`${p.name} ${p.surname}`)}</span>${citizenshipBadgeHtml(p.citizenship ?? null)}
                  <span class="player-cards__meta">${escapeHtml(p.position.name)}</span>
                </a>
                ${admin ? `<a class="icon-btn" href="${urlFor(`/admin/edit/player/${p.id}`, true)}" data-app-link title="\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u0442\u044C \u0438\u0433\u0440\u043E\u043A\u0430">\u270E</a>` : ""}
              </li>`).join("")}
          </ul>
        </div>
      </section>
    `)
    );
    const host = root.querySelector("[data-team-tabs]");
    if (!host) return;
    const btns = host.querySelectorAll(".tabs__btn");
    const panels = host.querySelectorAll(".tabs__panel");
    btns.forEach((btn) => btn.addEventListener("click", () => {
      const tab = btn.dataset.tab;
      btns.forEach((b) => b.classList.toggle("tabs__btn--active", b === btn));
      panels.forEach((p) => {
        const on = p.dataset.panel === tab;
        p.classList.toggle("tabs__panel--active", on);
        if (on) p.removeAttribute("hidden");
        else p.setAttribute("hidden", "");
      });
    }));
    bindMatchRowClicks(host);
    host.querySelector("[data-more-cal]")?.addEventListener("click", async () => {
      calPage += 1;
      const chunk = await teamCalendar(id, calPage, PAGE_SIZE2);
      if (chunk.length === 0) return;
      cal = [...cal, ...chunk];
      const panel = host.querySelector('[data-panel="calendar"]');
      if (!panel) return;
      panel.insertAdjacentHTML("afterbegin", renderTournamentGroups(chunk, admin));
      bindMatchRowClicks(panel);
    });
    host.querySelector("[data-more-res]")?.addEventListener("click", async () => {
      resPage += 1;
      const chunk = await teamResults(id, resPage, PAGE_SIZE2);
      if (chunk.length === 0) return;
      res = [...res, ...chunk];
      const panel = host.querySelector('[data-panel="results"]');
      if (!panel) return;
      panel.insertAdjacentHTML("afterbegin", renderTournamentGroups(chunk, admin));
      bindMatchRowClicks(panel);
    });
  }

  // src/views/player.ts
  async function renderPlayerPage(root, id, admin) {
    const p = await playerById(id);
    root.replaceChildren(
      el(`
      <section class="page page--player">
        <div class="entity-head entity-head--player">
          ${dtoImg(`${p.name} ${p.surname}`, p.photo, p.photoMime, "entity-head__photo player-cards__img--round")}
          <div class="entity-head__body">
            <h1 class="page-title">${escapeHtml(`${p.name} ${p.surname}`)}</h1>
            <p class="page-sub">${escapeHtml(p.position.name)}${p.number ? ` \u2014 #${p.number}` : ""}</p>
            ${p.birthday ? `<p class="page-sub">\u0414\u0430\u0442\u0430 \u0440\u043E\u0436\u0434\u0435\u043D\u0438\u044F: ${escapeHtml(new Date(p.birthday).toLocaleDateString("ru-RU"))}</p>` : ""}
            <p class="page-sub">
              \u041A\u043E\u043C\u0430\u043D\u0434\u0430:
              ${p.teamId ? `<a class="entity-link" href="${urlFor(`/team/${p.teamId}`, admin)}" data-app-link>${escapeHtml(p.teamName ?? "-")}</a>` : '<span class="muted">\u0411\u0435\u0437 \u043A\u043E\u043C\u0430\u043D\u0434\u044B</span>'}
            </p>
            ${p.citizenship ? `<p class="page-sub page-sub--citizenship"><span class="page-sub__citizenship-label">\u0413\u0440\u0430\u0436\u0434\u0430\u043D\u0441\u0442\u0432\u043E:</span><span class="page-sub__citizenship-value">${citizenshipBadgeHtml(p.citizenship)}<span class="page-sub__citizenship-name">${escapeHtml(p.citizenship.name)}</span></span></p>` : ""}
            ${admin ? `<div class="action-row"><a class="action-btn" href="${urlFor(`/admin/edit/player/${p.id}`, true)}" data-app-link>\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u0442\u044C \u0438\u0433\u0440\u043E\u043A\u0430</a></div>` : ""}
          </div>
        </div>
      </section>
    `)
    );
  }

  // src/views/tournamentsList.ts
  var PAGE_SIZE3 = 40;
  var timer = 0;
  function openCreateTournamentModal() {
    const state = {
      name: "",
      photo: null,
      photoMime: null
    };
    const modal = openModal(
      "\u0421\u043E\u0437\u0434\u0430\u043D\u0438\u0435 \u0442\u0443\u0440\u043D\u0438\u0440\u0430",
      `
      <div class="action-row"><button class="action-btn" data-create-top>\u0421\u043E\u0437\u0434\u0430\u0442\u044C</button></div>
      <div class="panel">
        <div class="stack-field"><span class="stack-field__label">\u041D\u0430\u0437\u0432\u0430\u043D\u0438\u0435</span><input class="search-input" data-name /></div>
        <div data-tournament-photo>${photoMediaFieldHtml("\u0424\u043E\u0442\u043E", dtoImg("\u0422\u0443\u0440\u043D\u0438\u0440", null, null))}</div>
      </div>
      <div class="action-row"><button class="action-btn" data-create-bottom>\u0421\u043E\u0437\u0434\u0430\u0442\u044C</button></div>
    `
    );
    const nameInput = modal.body.querySelector("[data-name]");
    const photoRoot = modal.body.querySelector("[data-tournament-photo]");
    const showNameError = () => {
      showValidationToast({
        message: "\u041F\u0440\u043E\u0432\u0435\u0440\u044C\u0442\u0435 \u043F\u0440\u0430\u0432\u0438\u043B\u044C\u043D\u043E\u0441\u0442\u044C \u0437\u0430\u043F\u043E\u043B\u043D\u0435\u043D\u0438\u044F \u043F\u043E\u043B\u0435\u0439: \u043D\u0430\u0437\u0432\u0430\u043D\u0438\u0435 \u0442\u0443\u0440\u043D\u0438\u0440\u0430 \u043D\u0435 \u0434\u043E\u043B\u0436\u043D\u043E \u0431\u044B\u0442\u044C \u043F\u0443\u0441\u0442\u044B\u043C.",
        highlightInputs: [nameInput]
      });
    };
    nameInput.addEventListener("input", () => {
      state.name = nameInput.value.trim();
      clearInvalidHighlightsIn(modal.body);
    });
    mountPhotoMediaField(photoRoot, {
      getPreviewHtml: () => dtoImg(state.name || "\u0422\u0443\u0440\u043D\u0438\u0440", state.photo, state.photoMime),
      onPickFile: async (file) => {
        state.photo = await fileToBase64(file);
        state.photoMime = file.type;
      },
      onClear: () => {
        state.photo = null;
        state.photoMime = null;
      }
    });
    const createNow = async () => {
      const name = nameInput.value.trim();
      if (!name) {
        showNameError();
        return;
      }
      const tournamentId = await createTournament(name);
      await tournamentChangeGeneralInfo(tournamentId, {
        name,
        photo: state.photo,
        photoMime: state.photoMime
      });
      showSuccessToast({ message: "\u0422\u0443\u0440\u043D\u0438\u0440 \u0441\u043E\u0437\u0434\u0430\u043D, \u043E\u0441\u043D\u043E\u0432\u043D\u0430\u044F \u0438\u043D\u0444\u043E\u0440\u043C\u0430\u0446\u0438\u044F \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u0430." });
      modal.close();
      goSameMode(`/admin/edit/tournament/${tournamentId}`);
    };
    modal.body.querySelectorAll("[data-create-top], [data-create-bottom]").forEach((b) => b.addEventListener("click", () => void createNow()));
  }
  function tournamentRow(t) {
    return `
    <article class="list-row">
      <div class="list-row__lead">
        <a class="entity-link" href="${urlFor(`/tournament/${t.id}`, true)}" data-app-link>${dtoImg(t.name, t.photo, t.photoMime, "mini-photo")}<span>${escapeHtml(t.name)}</span></a>
        <span class="muted list-row__meta">${escapeHtml(t.status.description)}</span>
      </div>
      <a class="icon-btn" href="${urlFor(`/admin/edit/tournament/${t.id}`, true)}" data-app-link>\u270E</a>
    </article>`;
  }
  async function renderAdminTournamentsPage(root, query) {
    let page = Number(query.get("page") || 1);
    let search = query.get("search") || "";
    let rows = await tournamentsPaged(page, PAGE_SIZE3, search);
    const render = () => rows.map((t) => tournamentRow(t)).join("");
    root.replaceChildren(el(`
    <section class="page"><div class="page-head page-head--row"><h1 class="page-title">\u0422\u0443\u0440\u043D\u0438\u0440\u044B</h1><button type="button" class="action-btn" data-create-tournament>\u0421\u043E\u0437\u0434\u0430\u0442\u044C \u0442\u0443\u0440\u043D\u0438\u0440</button></div>
      <div class="filters-row">
        <input class="search-input" placeholder="\u041F\u043E\u0438\u0441\u043A \u0442\u0443\u0440\u043D\u0438\u0440\u043E\u0432" value="${escapeHtml(search)}" data-search />
      </div>
      <div class="list-wrap" data-list>${render()}</div>
      <button class="action-btn action-btn--ghost" data-more>\u041F\u043E\u043A\u0430\u0437\u0430\u0442\u044C \u0431\u043E\u043B\u044C\u0448\u0435</button>
    </section>`));
    root.querySelector("[data-create-tournament]")?.addEventListener("click", () => openCreateTournamentModal());
    if (query.get("create") === "1") {
      const next = new URLSearchParams(query);
      next.delete("create");
      const nextSearch = next.toString();
      window.history.replaceState({}, "", `/admin/tournaments${nextSearch ? `?${nextSearch}` : ""}`);
      openCreateTournamentModal();
    }
    const inp = root.querySelector("[data-search]");
    const list = root.querySelector("[data-list]");
    inp?.addEventListener("input", () => {
      if (timer) window.clearTimeout(timer);
      timer = window.setTimeout(async () => {
        search = inp.value.trim();
        page = 1;
        rows = await tournamentsPaged(page, PAGE_SIZE3, search);
        if (list) list.innerHTML = render();
      }, 700);
    });
    root.querySelector("[data-more]")?.addEventListener("click", async () => {
      page += 1;
      const chunk = await tournamentsPaged(page, PAGE_SIZE3, search);
      if (chunk.length === 0) return;
      rows = [...rows, ...chunk];
      if (list) list.insertAdjacentHTML("beforeend", chunk.map((t) => tournamentRow(t)).join(""));
    });
  }

  // src/views/teamsList.ts
  var PAGE_SIZE4 = 40;
  var timer2 = 0;
  function teamRow(t) {
    return `
    <article class="list-row">
      <div class="list-row__lead">
        <a class="entity-link" href="${urlFor(`/team/${t.id}`, true)}" data-app-link>${dtoImg(t.name, t.photo, t.photoMime, "mini-photo")}<span>${escapeHtml(t.name)}</span></a>
        <span class="muted list-row__meta">${escapeHtml(t.city ?? "\u2014")}</span>
      </div>
      <a class="icon-btn" href="${urlFor(`/admin/edit/team/${t.id}`, true)}" data-app-link>\u270E</a>
    </article>`;
  }
  async function renderAdminTeamsPage(root, query) {
    let page = Number(query.get("page") || 1);
    let search = query.get("search") || "";
    let rows = await teamsPaged(page, PAGE_SIZE4, search);
    const render = () => rows.map((t) => teamRow(t)).join("");
    root.replaceChildren(el(`
    <section class="page"><div class="page-head page-head--row"><h1 class="page-title">\u041A\u043E\u043C\u0430\u043D\u0434\u044B</h1><a class="action-btn" href="${urlFor("/admin/edit/team/new", true)}" data-app-link>\u0414\u043E\u0431\u0430\u0432\u0438\u0442\u044C \u043A\u043E\u043C\u0430\u043D\u0434\u0443</a></div>
      <div class="filters-row">
        <input class="search-input" placeholder="\u041F\u043E\u0438\u0441\u043A \u043A\u043E\u043C\u0430\u043D\u0434\u044B" value="${escapeHtml(search)}" data-search />
      </div>
      <div class="list-wrap" data-list>${render()}</div>
      <button class="action-btn action-btn--ghost" data-more>\u041F\u043E\u043A\u0430\u0437\u0430\u0442\u044C \u0431\u043E\u043B\u044C\u0448\u0435</button>
    </section>`));
    const inp = root.querySelector("[data-search]");
    const list = root.querySelector("[data-list]");
    inp?.addEventListener("input", () => {
      if (timer2) window.clearTimeout(timer2);
      timer2 = window.setTimeout(async () => {
        search = inp.value.trim();
        page = 1;
        rows = await teamsPaged(page, PAGE_SIZE4, search);
        if (list) list.innerHTML = render();
      }, 700);
    });
    root.querySelector("[data-more]")?.addEventListener("click", async () => {
      page += 1;
      const chunk = await teamsPaged(page, PAGE_SIZE4, search);
      if (chunk.length === 0) return;
      rows = [...rows, ...chunk];
      if (list) list.insertAdjacentHTML("beforeend", chunk.map((t) => teamRow(t)).join(""));
    });
  }

  // src/views/playersList.ts
  var PAGE_SIZE5 = 40;
  async function renderAdminPlayersPage(root, query) {
    let page = Number(query.get("page") || 1);
    let search = query.get("search") || "";
    let selectedTeamIds = [];
    let rows = await playersPaged(page, PAGE_SIZE5, search);
    const renderRows = (items) => items.map((p) => `
      <article class="list-row">
        <a class="entity-link" href="${urlFor(`/player/${p.id}`, true)}" data-app-link>
          ${dtoImg(`${p.name} ${p.surname}`, p.photo, p.photoMime, "mini-photo player-cards__img--round")}
          <span>${escapeHtml(`${p.name} ${p.surname}`)}</span>${citizenshipBadgeHtml(p.citizenship ?? null)}
        </a>
        <span class="muted">${escapeHtml(`${p.teamName ?? "\u0411\u0435\u0437 \u043A\u043E\u043C\u0430\u043D\u0434\u044B"} \xB7 ${p.position.name}`)}</span>
        <a class="icon-btn" href="${urlFor(`/admin/edit/player/${p.id}`, true)}" data-app-link>\u270E</a>
      </article>`).join("");
    root.replaceChildren(
      el(`
      <section class="page">
        <div class="page-head page-head--row">
          <h1 class="page-title">\u0418\u0433\u0440\u043E\u043A\u0438</h1>
          <a class="action-btn" href="${urlFor("/admin/edit/player/new", true)}" data-app-link>\u0414\u043E\u0431\u0430\u0432\u0438\u0442\u044C \u0438\u0433\u0440\u043E\u043A\u0430</a>
        </div>
        <div class="filters-row">
          <input class="search-input" placeholder="\u041F\u043E\u0438\u0441\u043A \u0438\u0433\u0440\u043E\u043A\u0430" value="${escapeHtml(search)}" data-search />
          <div class="teams-filter-host" data-team-filter></div>
        </div>
        <div class="list-wrap" data-list>${renderRows(rows)}</div>
        <button class="action-btn action-btn--ghost" data-more>\u041F\u043E\u043A\u0430\u0437\u0430\u0442\u044C \u0431\u043E\u043B\u044C\u0448\u0435</button>
      </section>
    `)
    );
    const list = root.querySelector("[data-list]");
    const searchInput = root.querySelector("[data-search]");
    const teamFilterHost = root.querySelector("[data-team-filter]");
    const moreBtn = root.querySelector("[data-more]");
    mountEntitySelect(teamFilterHost, {
      placeholder: "\u0424\u0438\u043B\u044C\u0442\u0440 \u043F\u043E \u043A\u043E\u043C\u0430\u043D\u0434\u0430\u043C",
      multiple: true,
      load: async ({ page: p, pageSize, search: s }) => {
        const items = await teamsPaged(p, pageSize, s);
        return items.map((x) => ({ id: x.id, name: x.name, sub: x.city, photo: x.photo, photoMime: x.photoMime }));
      },
      onChange: async (selected) => {
        selectedTeamIds = selected.map((x) => x.id);
        page = 1;
        if (selectedTeamIds.length === 0) {
          rows = await playersPaged(page, PAGE_SIZE5, search);
        } else {
          const chunks = await Promise.all(selectedTeamIds.map((teamId) => playersByTeam(teamId)));
          const map = /* @__PURE__ */ new Map();
          chunks.flat().forEach((pl) => map.set(pl.id, pl));
          rows = [...map.values()];
        }
        list.innerHTML = renderRows(rows);
      }
    });
    let timer3 = 0;
    searchInput.addEventListener("input", () => {
      if (timer3) window.clearTimeout(timer3);
      timer3 = window.setTimeout(async () => {
        search = searchInput.value.trim();
        page = 1;
        if (selectedTeamIds.length > 0) {
          const chunks = await Promise.all(selectedTeamIds.map((teamId) => playersByTeam(teamId)));
          const all = chunks.flat();
          const q = search.toLowerCase();
          rows = all.filter((p) => `${p.name} ${p.surname} ${p.teamName ?? ""}`.toLowerCase().includes(q));
        } else {
          rows = await playersPaged(page, PAGE_SIZE5, search);
        }
        list.innerHTML = renderRows(rows);
      }, 700);
    });
    moreBtn.addEventListener("click", async () => {
      if (selectedTeamIds.length > 0) return;
      page += 1;
      const chunk = await playersPaged(page, PAGE_SIZE5, search);
      if (chunk.length === 0) return;
      rows = [...rows, ...chunk];
      list.insertAdjacentHTML("beforeend", renderRows(chunk));
    });
  }

  // src/views/editTournament.ts
  async function renderEditTournamentPage(root, idOrNew) {
    const isNew = idOrNew === "new";
    const id = isNew ? null : Number(idOrNew);
    const t = id ? await tournamentById(id) : {
      id: 0,
      name: "",
      photo: null,
      photoMime: null,
      startedAt: null,
      finishedAt: null,
      status: { code: 0, description: "\u041D\u0430\u0447\u0430\u0442", nextActionDescription: "\u041E\u0442\u043A\u044B\u0442\u044C \u0440\u0435\u0433\u0438\u0441\u0442\u0440\u0430\u0446\u0438\u044E" },
      tournamentRules: null,
      teams: []
    };
    let general = { name: t.name, photo: t.photo, photoMime: t.photoMime };
    let rules = t.tournamentRules ? structuredClone(t.tournamentRules) : null;
    const statusCode = t.status.code;
    const persistedTeams = id ? await teamByTournament(id) : [];
    let currentTeams = [...persistedTeams];
    const dirty = { general: false, rules: false, teams: false };
    const setDirty = (k, v) => {
      dirty[k] = v;
      root.querySelectorAll(`[data-save-${k}]`).forEach((b) => {
        b.disabled = !dirty[k];
      });
      root.querySelectorAll("[data-save-all]").forEach((b) => {
        b.disabled = !(dirty.general || dirty.rules || dirty.teams);
      });
    };
    const statusControlsVisible = Boolean(id) && statusCode < 3;
    const statusControlsHtml = statusControlsVisible ? `
      <div class="panel">
        <p class="muted">${escapeHtml(t.status.description)}</p>
        <button type="button" class="action-btn" data-next-status>${escapeHtml(t.status.nextActionDescription)}</button>
      </div>` : "";
    const viewLink = id !== null ? `<a class="action-btn action-btn--ghost" href="${urlFor(`/tournament/${id}`, true)}" data-app-link>\u041A \u043F\u0440\u043E\u0441\u043C\u043E\u0442\u0440\u0443 \u0442\u0443\u0440\u043D\u0438\u0440\u0430</a>` : "";
    root.replaceChildren(el(`
    <section class="page page--edit-tournament" data-tabs>
      <div class="page-head page-head--row">
        <h1 class="page-title">${isNew ? "\u0421\u043E\u0437\u0434\u0430\u043D\u0438\u0435 \u0442\u0443\u0440\u043D\u0438\u0440\u0430" : "\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u043D\u0438\u0435 \u0442\u0443\u0440\u043D\u0438\u0440\u0430"}</h1>
        ${viewLink}
      </div>
      ${statusControlsHtml}
      <div class="action-row"><button class="action-btn" data-save-all disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C \u0432\u0441\u0451</button></div>
      <div class="tabs"><button type="button" class="tabs__btn tabs__btn--active" data-tab="general">\u041E\u0431\u0449\u0430\u044F \u0438\u043D\u0444\u043E\u0440\u043C\u0430\u0446\u0438\u044F</button><button type="button" class="tabs__btn" data-tab="rules">\u041F\u0440\u0430\u0432\u0438\u043B\u0430</button><button type="button" class="tabs__btn" data-tab="teams">\u0417\u0430\u044F\u0432\u043A\u0430 \u043A\u043E\u043C\u0430\u043D\u0434</button></div>
      <div class="tabs__panel tabs__panel--active" data-panel="general">
        <div class="action-row"><button class="action-btn" data-save-general disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
        <div class="panel">
          <div class="stack-field"><span class="stack-field__label">\u041D\u0430\u0437\u0432\u0430\u043D\u0438\u0435</span><input class="search-input" data-name value="${escapeHtml(general.name)}" /></div>
          <div data-tournament-photo>${photoMediaFieldHtml("\u0424\u043E\u0442\u043E", dtoImg(general.name || "\u0422\u0443\u0440\u043D\u0438\u0440", general.photo, general.photoMime))}</div>
        </div>
        <div class="action-row"><button class="action-btn" data-save-general disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
      </div>
      <div class="tabs__panel" data-panel="rules" hidden>
        <div class="action-row"><button class="action-btn" data-save-rules ${statusCode !== 0 ? "disabled" : ""}>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
        <div class="panel" data-rules-host></div>
        <div class="action-row"><button class="action-btn" data-save-rules ${statusCode !== 0 ? "disabled" : ""}>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
      </div>
      <div class="tabs__panel" data-panel="teams" hidden>
        <div class="action-row"><button class="action-btn" data-save-teams disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
        <div class="panel">
          ${statusCode === 0 ? '<p class="muted">\u0421\u043D\u0430\u0447\u0430\u043B\u0430 \u043D\u0430\u0434\u043E \u043E\u0442\u043A\u0440\u044B\u0442\u044C \u0437\u0430\u044F\u0432\u043A\u0443 \u043A\u043E\u043C\u0430\u043D\u0434</p>' : ""}
          ${statusCode === 1 ? '<button class="action-btn" data-add-team>\u0414\u043E\u0431\u0430\u0432\u0438\u0442\u044C \u043A\u043E\u043C\u0430\u043D\u0434\u0443</button>' : ""}
          <div class="list-wrap" data-teams-list></div>
        </div>
        <div class="action-row"><button class="action-btn" data-save-teams disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
      </div>
    </section>
  `));
    const setupTabs3 = () => {
      const host = root.querySelector("[data-tabs]");
      const btns = host.querySelectorAll(".tabs__btn");
      const panels = host.querySelectorAll(".tabs__panel");
      btns.forEach((btn) => btn.addEventListener("click", () => {
        const tab = btn.dataset.tab;
        btns.forEach((b) => b.classList.toggle("tabs__btn--active", b === btn));
        panels.forEach((p) => {
          const on = p.dataset.panel === tab;
          p.classList.toggle("tabs__panel--active", on);
          if (on) p.removeAttribute("hidden");
          else p.setAttribute("hidden", "");
        });
      }));
    };
    setupTabs3();
    const nameInput = root.querySelector("[data-name]");
    nameInput.addEventListener("input", () => {
      setDirty("general", true);
      const pv = photoRoot.querySelector("[data-photo-preview]");
      if (pv) pv.innerHTML = dtoImg(general.name || "\u0422\u0443\u0440\u043D\u0438\u0440", general.photo, general.photoMime);
    });
    const photoRoot = root.querySelector("[data-tournament-photo]");
    mountPhotoMediaField(photoRoot, {
      getPreviewHtml: () => dtoImg(general.name || "\u0422\u0443\u0440\u043D\u0438\u0440", general.photo, general.photoMime),
      onPickFile: async (file) => {
        general.photo = await fileToBase64(file);
        general.photoMime = file.type;
        setDirty("general", true);
      },
      onClear: () => {
        general.photo = null;
        general.photoMime = null;
        setDirty("general", true);
      }
    });
    const saveGeneral = async () => {
      const name = nameInput.value.trim();
      if (!name) {
        showErrorToast({ message: "\u041D\u0430\u0437\u0432\u0430\u043D\u0438\u0435 \u0442\u0443\u0440\u043D\u0438\u0440\u0430 \u043D\u0435 \u043C\u043E\u0436\u0435\u0442 \u0431\u044B\u0442\u044C \u043F\u0443\u0441\u0442\u044B\u043C" });
        return;
      }
      let tournamentId = id;
      const createdNow = !tournamentId;
      if (!tournamentId) tournamentId = await createTournament(name);
      await tournamentChangeGeneralInfo(tournamentId, { name, photo: general.photo, photoMime: general.photoMime });
      showSuccessToast({
        message: createdNow ? "\u0422\u0443\u0440\u043D\u0438\u0440 \u0441\u043E\u0437\u0434\u0430\u043D, \u043E\u0441\u043D\u043E\u0432\u043D\u0430\u044F \u0438\u043D\u0444\u043E\u0440\u043C\u0430\u0446\u0438\u044F \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u0430." : "\u041E\u0441\u043D\u043E\u0432\u043D\u0430\u044F \u0438\u043D\u0444\u043E\u0440\u043C\u0430\u0446\u0438\u044F \u0442\u0443\u0440\u043D\u0438\u0440\u0430 \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u0430."
      });
      setDirty("general", false);
      if (isNew) goSameMode(`/admin/edit/tournament/${tournamentId}`);
    };
    root.querySelectorAll("[data-save-general]").forEach((b) => b.addEventListener("click", () => void saveGeneral()));
    const rulesHost = root.querySelector("[data-rules-host]");
    const renderRules = () => {
      const disabled = statusCode !== 0 ? "disabled" : "";
      const time = rules?.matchTimeRules;
      const roster = rules?.matchRosterRules;
      const points = rules?.matchPointsRules;
      const hasOvertime = time?.hasOvertime ?? false;
      const hasDraw = time?.isDrawPossible ?? false;
      rulesHost.innerHTML = `
      <div class="rules-block">
        <h3>\u0414\u043B\u0438\u0442\u0435\u043B\u044C\u043D\u043E\u0441\u0442\u044C \u043C\u0430\u0442\u0447\u0430</h3>
        <div class="grid-2">
          <label class="stack-field">\u041F\u0435\u0440\u0438\u043E\u0434\u043E\u0432 <input ${disabled} class="search-input" data-r="periodsCount" type="number" value="${time?.periodsCount ?? ""}" /></label>
          <label class="stack-field">\u0414\u043B\u0438\u0442\u0435\u043B\u044C\u043D\u043E\u0441\u0442\u044C \u043F\u0435\u0440\u0438\u043E\u0434\u0430 (\u0441\u0435\u043A) <input ${disabled} class="search-input" data-r="periodDurationSeconds" type="number" value="${time?.periodDurationSeconds ?? ""}" /></label>
        </div>
        <div class="rules-toggle-row">
          <label class="rules-checkbox"><input ${disabled} data-rb="hasOvertime" type="checkbox" ${hasOvertime ? "checked" : ""} /> \u0415\u0441\u0442\u044C \u043E\u0432\u0435\u0440\u0442\u0430\u0439\u043C</label>
        </div>
        <div class="rules-collapsible ${hasOvertime ? "is-open" : ""}" data-overtime-fields><div class="rules-collapsible__inner"><div class="grid-2">
          <label class="stack-field">\u041A\u043E\u043B\u0438\u0447\u0435\u0441\u0442\u0432\u043E \u043E\u0432\u0435\u0440\u0442\u0430\u0439\u043C\u043E\u0432 <input ${disabled} class="search-input" data-r="overtimesCount" type="number" value="${time?.overtimeRules?.overtimesCount ?? ""}" /></label>
          <label class="stack-field">\u0414\u043B\u0438\u0442\u0435\u043B\u044C\u043D\u043E\u0441\u0442\u044C \u043E\u0432\u0435\u0440\u0442\u0430\u0439\u043C\u0430 (\u0441\u0435\u043A) <input ${disabled} class="search-input" data-r="overtimeDurationSeconds" type="number" value="${time?.overtimeRules?.overtimeDurationSeconds ?? ""}" /></label>
          <label class="rules-checkbox"><input ${disabled} data-rb="goalEndsOvertime" type="checkbox" ${time?.overtimeRules?.goalEndsOvertime ? "checked" : ""} /> \u0413\u043E\u043B \u0437\u0430\u0432\u0435\u0440\u0448\u0430\u0435\u0442 \u043E\u0432\u0435\u0440\u0442\u0430\u0439\u043C</label>
        </div></div></div>
      </div>
      <div class="rules-block">
        <h3>\u041F\u0440\u0430\u0432\u0438\u043B\u0430 \u0437\u0430\u044F\u0432\u043A\u0438 \u043D\u0430 \u043C\u0430\u0442\u0447</h3>
        <div class="grid-2">
          <label class="stack-field">\u041C\u0438\u043D\u0438\u043C\u0443\u043C \u0438\u0433\u0440\u043E\u043A\u043E\u0432 <input ${disabled} class="search-input" data-r="minPlayers" type="number" value="${roster?.minPlayers ?? ""}" /></label>
          <label class="stack-field">\u041C\u0430\u043A\u0441\u0438\u043C\u0443\u043C \u0438\u0433\u0440\u043E\u043A\u043E\u0432 <input ${disabled} class="search-input" data-r="maxPlayers" type="number" value="${roster?.maxPlayers ?? ""}" /></label>
          <label class="stack-field">\u041C\u0438\u043D\u0438\u043C\u0443\u043C \u043D\u0430\u043F\u0430\u0434\u0430\u044E\u0449\u0438\u0445 <input ${disabled} class="search-input" data-r="minForwards" type="number" value="${roster?.minForwards ?? ""}" /></label>
          <label class="stack-field">\u041C\u0430\u043A\u0441\u0438\u043C\u0443\u043C \u043D\u0430\u043F\u0430\u0434\u0430\u044E\u0449\u0438\u0445 <input ${disabled} class="search-input" data-r="maxForwards" type="number" value="${roster?.maxForwards ?? ""}" /></label>
          <label class="stack-field">\u041C\u0438\u043D\u0438\u043C\u0443\u043C \u0437\u0430\u0449\u0438\u0442\u043D\u0438\u043A\u043E\u0432 <input ${disabled} class="search-input" data-r="minDefensemans" type="number" value="${roster?.minDefensemans ?? ""}" /></label>
          <label class="stack-field">\u041C\u0430\u043A\u0441\u0438\u043C\u0443\u043C \u0437\u0430\u0449\u0438\u0442\u043D\u0438\u043A\u043E\u0432 <input ${disabled} class="search-input" data-r="maxDefensemans" type="number" value="${roster?.maxDefensemans ?? ""}" /></label>
          <label class="stack-field">\u041C\u0438\u043D\u0438\u043C\u0443\u043C \u0432\u0440\u0430\u0442\u0430\u0440\u0435\u0439 <input ${disabled} class="search-input" data-r="minGoalies" type="number" value="${roster?.minGoalies ?? ""}" /></label>
          <label class="stack-field">\u041C\u0430\u043A\u0441\u0438\u043C\u0443\u043C \u0432\u0440\u0430\u0442\u0430\u0440\u0435\u0439 <input ${disabled} class="search-input" data-r="maxGoalies" type="number" value="${roster?.maxGoalies ?? ""}" /></label>
        </div>
      </div>
      <div class="rules-block">
        <h3>\u041F\u0440\u0430\u0432\u0438\u043B\u0430 \u043D\u0430\u0447\u0438\u0441\u043B\u0435\u043D\u0438\u044F \u043E\u0447\u043A\u043E\u0432</h3>
        <div class="grid-2">
          <label class="stack-field">\u041E\u0447\u043A\u0438 \u0437\u0430 \u043F\u043E\u0431\u0435\u0434\u0443 <input ${disabled} class="search-input" data-r="winPoints" type="number" value="${points?.winPoints ?? ""}" /></label>
          <label class="stack-field">\u041E\u0447\u043A\u0438 \u0437\u0430 \u043F\u043E\u0440\u0430\u0436\u0435\u043D\u0438\u0435 <input ${disabled} class="search-input" data-r="lossPoints" type="number" value="${points?.lossPoints ?? ""}" /></label>
        </div>
        <div class="rules-collapsible ${hasOvertime ? "is-open" : ""}" data-ot-points-fields><div class="rules-collapsible__inner"><div class="grid-2">
          <label class="stack-field">\u041E\u0447\u043A\u0438 \u0437\u0430 \u043F\u043E\u0431\u0435\u0434\u0443 \u0432 \u043E\u0432\u0435\u0440\u0442\u0430\u0439\u043C\u0435 <input ${disabled} class="search-input" data-r="otWinPoints" type="number" value="${points?.otWinPoints ?? ""}" /></label>
          <label class="stack-field">\u041E\u0447\u043A\u0438 \u0437\u0430 \u043F\u043E\u0440\u0430\u0436\u0435\u043D\u0438\u0435 \u0432 \u043E\u0432\u0435\u0440\u0442\u0430\u0439\u043C\u0435 <input ${disabled} class="search-input" data-r="otLossPoints" type="number" value="${points?.otLossPoints ?? ""}" /></label>
        </div></div></div>
        <div class="rules-toggle-row">
          <label class="rules-checkbox"><input ${disabled} data-rb="hasDraw" type="checkbox" ${hasDraw ? "checked" : ""} /> \u0415\u0441\u0442\u044C \u043D\u0438\u0447\u044C\u044F</label>
        </div>
        <div class="rules-collapsible ${hasDraw ? "is-open" : ""}" data-draw-points-field><div class="rules-collapsible__inner"><div class="grid-2">
          <label class="stack-field">\u041E\u0447\u043A\u0438 \u0437\u0430 \u043D\u0438\u0447\u044C\u044E <input ${disabled} class="search-input" data-r="drawPoints" type="number" value="${points?.drawPoints ?? ""}" /></label>
        </div></div></div>
      </div>`;
    };
    renderRules();
    const RULES_VALIDATION_MSG = "\u041F\u0440\u043E\u0432\u0435\u0440\u044C\u0442\u0435 \u043F\u0440\u0430\u0432\u0438\u043B\u044C\u043D\u043E\u0441\u0442\u044C \u0437\u0430\u043F\u043E\u043B\u043D\u0435\u043D\u0438\u044F \u043F\u043E\u043B\u0435\u0439: \u0447\u0438\u0441\u043B\u043E\u0432\u044B\u0435 \u043F\u043E\u043B\u044F \u043D\u0435 \u0434\u043E\u043B\u0436\u043D\u044B \u0431\u044B\u0442\u044C \u043F\u0443\u0441\u0442\u044B\u043C\u0438.";
    const setExpanded = (selector, expand) => {
      const target = rulesHost.querySelector(selector);
      if (!target) return;
      target.classList.toggle("is-open", expand);
    };
    rulesHost.addEventListener("change", (e) => {
      if (statusCode !== 0) return;
      const target = e.target;
      if (target instanceof HTMLInputElement && target.dataset.rb === "hasOvertime") {
        setExpanded("[data-overtime-fields]", target.checked);
        setExpanded("[data-ot-points-fields]", target.checked);
      }
      if (target instanceof HTMLInputElement && target.dataset.rb === "hasDraw") {
        setExpanded("[data-draw-points-field]", target.checked);
      }
      clearInvalidHighlightsIn(rulesHost);
      setDirty("rules", true);
    });
    rulesHost.addEventListener("input", () => {
      if (statusCode !== 0) return;
      clearInvalidHighlightsIn(rulesHost);
      setDirty("rules", true);
    });
    const collectTournamentRulesFromDom = () => {
      const invalid = [];
      const addInv = (el2) => {
        if (el2 && !invalid.includes(el2)) invalid.push(el2);
      };
      const qi = (key) => rulesHost.querySelector(`[data-r="${key}"]`);
      const qb = (key) => rulesHost.querySelector(`[data-rb="${key}"]`);
      const readBool = (key) => qb(key)?.checked ?? false;
      const readRequiredInt = (key) => {
        const el2 = qi(key);
        const raw = el2?.value.trim() ?? "";
        if (!el2 || raw === "") {
          addInv(el2);
          return null;
        }
        const n = Number(raw);
        if (Number.isNaN(n)) {
          addInv(el2);
          return null;
        }
        return n;
      };
      const hasOvertime = readBool("hasOvertime");
      const hasDraw = readBool("hasDraw");
      const periodsCount = readRequiredInt("periodsCount");
      const periodDurationSeconds = readRequiredInt("periodDurationSeconds");
      const maxPlayers = readRequiredInt("maxPlayers");
      const minPlayers = readRequiredInt("minPlayers");
      const minForwards = readRequiredInt("minForwards");
      const maxForwards = readRequiredInt("maxForwards");
      const minDefensemans = readRequiredInt("minDefensemans");
      const maxDefensemans = readRequiredInt("maxDefensemans");
      const minGoalies = readRequiredInt("minGoalies");
      const maxGoalies = readRequiredInt("maxGoalies");
      const winPoints = readRequiredInt("winPoints");
      const lossPoints = readRequiredInt("lossPoints");
      let overtimesCount = null;
      let overtimeDurationSeconds = null;
      let goalEndsOvertime = false;
      let otWinPoints = null;
      let otLossPoints = null;
      if (hasOvertime) {
        overtimesCount = readRequiredInt("overtimesCount");
        overtimeDurationSeconds = readRequiredInt("overtimeDurationSeconds");
        goalEndsOvertime = readBool("goalEndsOvertime");
        otWinPoints = readRequiredInt("otWinPoints");
        otLossPoints = readRequiredInt("otLossPoints");
      }
      let drawPoints = null;
      if (hasDraw) {
        drawPoints = readRequiredInt("drawPoints");
      }
      if (invalid.length) return { ok: false, invalid };
      return {
        ok: true,
        value: {
          matchTimeRules: {
            periodsCount,
            periodDurationSeconds,
            isDrawPossible: hasDraw,
            hasOvertime,
            hasShootout: false,
            overtimeRules: hasOvertime ? {
              overtimesCount,
              overtimeDurationSeconds,
              goalEndsOvertime
            } : null,
            shootoutRules: null
          },
          matchRosterRules: {
            maxPlayers,
            minPlayers,
            minForwards,
            maxForwards,
            minDefensemans,
            maxDefensemans,
            minGoalies,
            maxGoalies
          },
          matchPointsRules: {
            winPoints,
            lossPoints,
            otWinPoints,
            otLossPoints,
            shootoutWinPoints: null,
            shootoutLossPoints: null,
            drawPoints
          }
        }
      };
    };
    const saveRules = async () => {
      if (!id) return true;
      const built = collectTournamentRulesFromDom();
      if (!built.ok) {
        showValidationToast({ message: RULES_VALIDATION_MSG, highlightInputs: built.invalid });
        return false;
      }
      rules = built.value;
      await tournamentSetRules(id, built.value);
      showSuccessToast({ message: "\u041F\u0440\u0430\u0432\u0438\u043B\u0430 \u0442\u0443\u0440\u043D\u0438\u0440\u0430 \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u044B." });
      setDirty("rules", false);
      return true;
    };
    root.querySelectorAll("[data-save-rules]").forEach(
      (b) => b.addEventListener("click", () => void saveRules())
    );
    const listHost = root.querySelector("[data-teams-list]");
    const renderTeams = () => {
      listHost.innerHTML = currentTeams.map((x) => `<article class="list-row">
          <div class="list-row__lead">
            <span class="entity-link">${dtoImg(x.name, x.photo, x.photoMime, "mini-photo")}<span>${escapeHtml(x.name)}</span></span>
            <span class="list-row__meta muted">${escapeHtml(x.city ?? "\u2014")}</span>
          </div>
          ${statusCode === 1 ? `<button type="button" class="icon-btn" data-remove-team="${x.id}" title="\u0423\u0434\u0430\u043B\u0438\u0442\u044C \u0438\u0437 \u0437\u0430\u044F\u0432\u043A\u0438">\u{1F5D1}</button>` : ""}
        </article>`).join("");
    };
    renderTeams();
    listHost.addEventListener("click", (e) => {
      if (statusCode !== 1) return;
      const btn = e.target.closest("[data-remove-team]");
      if (!btn) return;
      const teamId = Number(btn.dataset.removeTeam);
      currentTeams = currentTeams.filter((x) => x.id !== teamId);
      renderTeams();
      setDirty("teams", true);
    });
    root.querySelector("[data-add-team]")?.addEventListener("click", async () => {
      if (statusCode !== 1) return;
      const modal = openModal("\u0420\u0435\u0433\u0438\u0441\u0442\u0440\u0430\u0446\u0438\u044F \u043A\u043E\u043C\u0430\u043D\u0434", `
      <div class="action-row"><button class="action-btn" data-apply-top>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
      <div class="panel">
        <div class="filters-row">
        <input class="search-input" data-team-search placeholder="\u041F\u043E\u0438\u0441\u043A \u043A\u043E\u043C\u0430\u043D\u0434" />
        </div>
        <div class="list-wrap" data-team-picker-list style="min-height: 220px"></div>
        <div class="action-row"><button class="action-btn action-btn--ghost" type="button" data-team-more>\u041F\u043E\u043A\u0430\u0437\u0430\u0442\u044C \u0435\u0449\u0451</button></div>
      </div>
      <div class="action-row"><button class="action-btn" data-apply>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>`, { wide: true });
      const PAGE_SIZE6 = 40;
      const selected = new Set(currentTeams.map((x) => x.id));
      const list = modal.body.querySelector("[data-team-picker-list]");
      const searchInput = modal.body.querySelector("[data-team-search]");
      const moreBtn = modal.body.querySelector("[data-team-more]");
      const applyTopBtn = modal.body.querySelector("[data-apply-top]");
      const applyBtn = modal.body.querySelector("[data-apply]");
      let page = 1;
      let search = "";
      let hasMore = true;
      const loaded = /* @__PURE__ */ new Map();
      let reqId = 0;
      const upsertRows = (rows, append) => {
        if (!append) list.innerHTML = "";
        for (const team of rows) {
          loaded.set(team.id, team);
          list.insertAdjacentHTML("beforeend", `<label class="list-row">
            <span class="entity-link">
              <input type="checkbox" data-team-check="${team.id}" ${selected.has(team.id) ? "checked" : ""} />
              ${dtoImg(team.name, team.photo, team.photoMime, "mini-photo")}
              <span>${escapeHtml(team.name)}</span>
            </span>
          </label>`);
        }
      };
      const loadPage = async (append) => {
        const currentReq = ++reqId;
        moreBtn.disabled = true;
        const hadRows = Boolean(list.querySelector("[data-team-check]"));
        if (!append) {
          if (hadRows) {
            list.style.opacity = "0.55";
            list.style.pointerEvents = "none";
          } else {
            list.innerHTML = '<p class="muted">\u0417\u0430\u0433\u0440\u0443\u0437\u043A\u0430...</p>';
          }
        }
        const rows = await teamsPaged(page, PAGE_SIZE6, search);
        if (currentReq !== reqId) return;
        if (!append && hadRows) {
          list.style.opacity = "";
          list.style.pointerEvents = "";
        }
        hasMore = rows.length === PAGE_SIZE6;
        upsertRows(rows, append);
        if (!rows.length && !append) list.innerHTML = '<p class="muted">\u041D\u0438\u0447\u0435\u0433\u043E \u043D\u0435 \u043D\u0430\u0439\u0434\u0435\u043D\u043E</p>';
        moreBtn.disabled = !hasMore;
        moreBtn.hidden = !hasMore;
      };
      const reload = async () => {
        page = 1;
        hasMore = true;
        await loadPage(false);
      };
      let searchDebounce = 0;
      searchInput.addEventListener("input", () => {
        const q = searchInput.value.trim();
        if (searchDebounce) window.clearTimeout(searchDebounce);
        searchDebounce = window.setTimeout(() => {
          searchDebounce = 0;
          search = q;
          void reload();
        }, 300);
      });
      moreBtn.addEventListener("click", () => {
        if (!hasMore) return;
        page += 1;
        void loadPage(true);
      });
      list.addEventListener("change", (e) => {
        const target = e.target;
        if (!target.matches("[data-team-check]")) return;
        const teamId = Number(target.dataset.teamCheck);
        if (target.checked) selected.add(teamId);
        else selected.delete(teamId);
      });
      const applySelection = () => {
        const persistedMap = new Map(persistedTeams.map((x) => [x.id, x]));
        const nextTeams = [];
        for (const teamId of selected) {
          const loadedTeam = loaded.get(teamId) ?? persistedMap.get(teamId);
          if (loadedTeam) nextTeams.push(loadedTeam);
        }
        currentTeams = nextTeams;
        renderTeams();
        setDirty("teams", true);
        modal.close();
      };
      applyTopBtn.addEventListener("click", applySelection);
      applyBtn.addEventListener("click", applySelection);
      void reload();
    });
    const saveTeams = async () => {
      if (!id || statusCode !== 1) return;
      await tournamentSetRegistrationTeams(id, currentTeams.map((x) => x.id));
      showSuccessToast({ message: "\u0421\u043F\u0438\u0441\u043E\u043A \u043A\u043E\u043C\u0430\u043D\u0434 \u0442\u0443\u0440\u043D\u0438\u0440\u0430 \u0441\u043E\u0445\u0440\u0430\u043D\u0451\u043D." });
      setDirty("teams", false);
    };
    root.querySelectorAll("[data-save-teams]").forEach((b) => b.addEventListener("click", () => void saveTeams()));
    root.querySelectorAll("[data-next-status]").forEach((btn) => {
      btn.addEventListener("click", async () => {
        if (!id) return;
        btn.disabled = true;
        try {
          if (statusCode === 0) {
            await tournamentRegistrationStep(id);
            showSuccessToast({ message: "\u0420\u0435\u0433\u0438\u0441\u0442\u0440\u0430\u0446\u0438\u044F \u043A\u043E\u043C\u0430\u043D\u0434 \u043D\u0430 \u0442\u0443\u0440\u043D\u0438\u0440 \u043E\u0442\u043A\u0440\u044B\u0442\u0430." });
          } else if (statusCode === 1) {
            await tournamentStart(id);
            showSuccessToast({ message: "\u0422\u0443\u0440\u043D\u0438\u0440 \u043D\u0430\u0447\u0430\u0442." });
          } else if (statusCode === 2) {
            await tournamentFinish(id);
            showSuccessToast({ message: "\u0422\u0443\u0440\u043D\u0438\u0440 \u0437\u0430\u0432\u0435\u0440\u0448\u0451\u043D." });
          }
          goSameMode(`/admin/edit/tournament/${id}`);
        } catch (e) {
          const msg = e instanceof Error ? e.message : String(e);
          showErrorToast({ message: msg });
        } finally {
          btn.disabled = false;
        }
      });
    });
    root.querySelector("[data-save-all]")?.addEventListener("click", async () => {
      if (dirty.general) await saveGeneral();
      if (dirty.rules) {
        const okRules = await saveRules();
        if (!okRules) return;
      }
      if (dirty.teams) await saveTeams();
    });
  }

  // src/views/editTeam.ts
  async function renderEditTeamPage(root, idOrNew) {
    const isNew = idOrNew === "new";
    const id = isNew ? null : Number(idOrNew);
    const team = id ? await teamById(id) : { id: 0, name: "", city: null, photo: null, photoMime: null };
    let form = { ...team };
    let dirty = false;
    const setDirty = (v) => {
      dirty = v;
      root.querySelectorAll("[data-save]").forEach((b) => {
        b.disabled = !dirty;
      });
    };
    const viewLink = id !== null ? `<a class="action-btn action-btn--ghost" href="${urlFor(`/team/${id}`, true)}" data-app-link>\u041A \u043F\u0440\u043E\u0441\u043C\u043E\u0442\u0440\u0443 \u043A\u043E\u043C\u0430\u043D\u0434\u044B</a>` : "";
    root.replaceChildren(el(`
    <section class="page">
      <div class="page-head page-head--row">
        <h1 class="page-title">${isNew ? "\u0421\u043E\u0437\u0434\u0430\u043D\u0438\u0435 \u043A\u043E\u043C\u0430\u043D\u0434\u044B" : "\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u043D\u0438\u0435 \u043A\u043E\u043C\u0430\u043D\u0434\u044B"}</h1>
        ${viewLink}
      </div>
      <div class="action-row"><button class="action-btn" data-save disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
      <div class="panel">
        <div class="stack-field"><span class="stack-field__label">\u041D\u0430\u0437\u0432\u0430\u043D\u0438\u0435</span><input class="search-input" data-name value="${escapeHtml(form.name)}" /></div>
        <div class="stack-field"><span class="stack-field__label">\u0413\u043E\u0440\u043E\u0434</span><input class="search-input" data-city value="${escapeHtml(form.city ?? "")}" /></div>
        <div data-team-photo>${photoMediaFieldHtml("\u0424\u043E\u0442\u043E", dtoImg(form.name || "\u041A\u043E\u043C\u0430\u043D\u0434\u0430", form.photo, form.photoMime))}</div>
      </div>
      <div class="action-row"><button class="action-btn" data-save disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
    </section>
  `));
    const nameInput = root.querySelector("[data-name]");
    const cityInput = root.querySelector("[data-city]");
    const photoRoot = root.querySelector("[data-team-photo]");
    [nameInput, cityInput].forEach((x) => x.addEventListener("input", () => setDirty(true)));
    mountPhotoMediaField(photoRoot, {
      getPreviewHtml: () => dtoImg(nameInput.value.trim() || "\u041A\u043E\u043C\u0430\u043D\u0434\u0430", form.photo, form.photoMime),
      onPickFile: async (f) => {
        form.photo = await fileToBase64(f);
        form.photoMime = f.type;
        setDirty(true);
      },
      onClear: () => {
        form.photo = null;
        form.photoMime = null;
        setDirty(true);
      }
    });
    const syncPreviewName = () => {
      const pv = photoRoot.querySelector("[data-photo-preview]");
      if (pv) pv.innerHTML = dtoImg(nameInput.value.trim() || "\u041A\u043E\u043C\u0430\u043D\u0434\u0430", form.photo, form.photoMime);
    };
    nameInput.addEventListener("input", () => syncPreviewName());
    const save = async () => {
      const name = nameInput.value.trim();
      if (!name) {
        showErrorToast({ message: "\u041D\u0430\u0437\u0432\u0430\u043D\u0438\u0435 \u043D\u0435 \u043C\u043E\u0436\u0435\u0442 \u0431\u044B\u0442\u044C \u043F\u0443\u0441\u0442\u044B\u043C" });
        return;
      }
      const city = cityInput.value.trim() || null;
      let teamId = id;
      if (!teamId) teamId = await createTeam(name);
      await teamChangeGeneralInfo(teamId, { name, city, photo: form.photo, photoMime: form.photoMime });
      showSuccessToast({
        message: isNew ? "\u041A\u043E\u043C\u0430\u043D\u0434\u0430 \u0441\u043E\u0437\u0434\u0430\u043D\u0430, \u0441\u0432\u0435\u0434\u0435\u043D\u0438\u044F \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u044B." : "\u0421\u0432\u0435\u0434\u0435\u043D\u0438\u044F \u043E \u043A\u043E\u043C\u0430\u043D\u0434\u0435 \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u044B."
      });
      setDirty(false);
      if (isNew) goSameMode(`/team/${teamId}`);
    };
    root.querySelectorAll("[data-save]").forEach((b) => b.addEventListener("click", () => void save()));
  }

  // src/views/editPlayer.ts
  async function renderEditPlayerPage(root, idOrNew) {
    const isNew = idOrNew === "new";
    const id = isNew ? null : Number(idOrNew);
    const p = id ? await playerById(id) : {
      id: 0,
      name: "",
      surname: "",
      position: { code: "Forward", name: "\u041D\u0430\u043F\u0430\u0434\u0430\u044E\u0449\u0438\u0439" },
      teamId: null,
      teamName: null,
      photo: null,
      photoMime: null,
      citizenship: null,
      birthday: null,
      number: null
    };
    const positionsMap = await playerPositions();
    const positions = Object.entries(positionsMap).map(([code, name2]) => ({ code, name: name2 }));
    const positionCodes = positions.map((x) => x.code);
    const normalizePositionCode = (rawCode, rawName) => {
      if (typeof rawCode === "string" && positionCodes.includes(rawCode)) return rawCode;
      if (typeof rawCode === "number" && rawCode >= 0 && rawCode < positionCodes.length) return positionCodes[rawCode];
      if (typeof rawName === "string") {
        const byName = positions.find((x) => x.name === rawName);
        if (byName) return byName.code;
      }
      return positionCodes[0] ?? "Forward";
    };
    let photo = p.photo;
    let photoMime = p.photoMime;
    let citizenshipPhoto = p.citizenship?.photo ?? null;
    let citizenshipPhotoMime = p.citizenship?.photoMime ?? null;
    let dirty = false;
    let selectedPositionCode = normalizePositionCode(
      p.position?.code,
      p.position?.name
    );
    let selectedTeamId = p.teamId ?? null;
    const setDirty = (v) => {
      dirty = v;
      root.querySelectorAll("[data-save]").forEach((b) => {
        b.disabled = !dirty;
      });
    };
    const viewLink = id !== null ? `<a class="action-btn action-btn--ghost" href="${urlFor(`/player/${id}`, true)}" data-app-link>\u041A \u043F\u0440\u043E\u0441\u043C\u043E\u0442\u0440\u0443 \u0438\u0433\u0440\u043E\u043A\u0430</a>` : "";
    root.replaceChildren(
      el(`
    <section class="page">
      <div class="page-head page-head--row">
        <h1 class="page-title">${isNew ? "\u0421\u043E\u0437\u0434\u0430\u043D\u0438\u0435 \u0438\u0433\u0440\u043E\u043A\u0430" : "\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u043D\u0438\u0435 \u0438\u0433\u0440\u043E\u043A\u0430"}</h1>
        ${viewLink}
      </div>
      <div class="action-row"><button class="action-btn" data-save disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
      <div class="panel">
        <div class="stack-field"><span class="stack-field__label">\u0418\u043C\u044F</span><input class="search-input" data-name value="${escapeHtml(p.name)}" /></div>
        <div class="stack-field"><span class="stack-field__label">\u0424\u0430\u043C\u0438\u043B\u0438\u044F</span><input class="search-input" data-surname value="${escapeHtml(p.surname)}" /></div>
        <div class="stack-field"><span class="stack-field__label">\u0410\u043C\u043F\u043B\u0443\u0430</span><div data-position-select></div></div>
        <div class="stack-field"><span class="stack-field__label">\u041D\u043E\u043C\u0435\u0440</span><input class="search-input" type="number" data-number value="${p.number ?? ""}" /></div>
        <div class="stack-field"><span class="stack-field__label">\u0414\u0430\u0442\u0430 \u0440\u043E\u0436\u0434\u0435\u043D\u0438\u044F</span><input class="search-input" type="date" data-birthday value="${p.birthday ?? ""}" /></div>
        <div class="stack-field"><span class="stack-field__label">\u041A\u043E\u043C\u0430\u043D\u0434\u0430</span><div data-team-select></div></div>

        <div data-player-photo>${photoMediaFieldHtml("\u0424\u043E\u0442\u043E", dtoImg(`${p.name} ${p.surname}`.trim() || "\u0418\u0433\u0440\u043E\u043A", p.photo, p.photoMime, "media-block__img player-cards__img--round"))}</div>

        <div class="stack-field"><span class="stack-field__label">\u0413\u0440\u0430\u0436\u0434\u0430\u043D\u0441\u0442\u0432\u043E</span><input class="search-input" data-cit-name value="${escapeHtml(p.citizenship?.name ?? "")}" /></div>
        <div data-cit-photo>${photoMediaFieldHtml("\u0424\u043E\u0442\u043E \u0433\u0440\u0430\u0436\u0434\u0430\u043D\u0441\u0442\u0432\u0430", dtoImg(p.citizenship?.name ?? "\u0413\u0440\u0430\u0436\u0434\u0430\u043D\u0441\u0442\u0432\u043E", p.citizenship?.photo ?? null, p.citizenship?.photoMime ?? null, "media-block__img"))}</div>

      </div>
      <div class="action-row"><button class="action-btn" data-save disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C</button></div>
    </section>
  `)
    );
    const name = root.querySelector("[data-name]");
    const surname = root.querySelector("[data-surname]");
    const positionHost = root.querySelector("[data-position-select]");
    const number = root.querySelector("[data-number]");
    const birthday = root.querySelector("[data-birthday]");
    const teamHost = root.querySelector("[data-team-select]");
    const citName = root.querySelector("[data-cit-name]");
    const playerPhotoRoot = root.querySelector("[data-player-photo]");
    const citPhotoRoot = root.querySelector("[data-cit-photo]");
    const positionItems = positions.map((pos, idx) => ({
      id: idx + 1,
      name: pos.name,
      sub: null
    }));
    const initialPosIdx = Math.max(0, positionCodes.indexOf(selectedPositionCode));
    const initialPositionItems = positionItems.filter((_, i) => i === initialPosIdx);
    mountEntitySelect(positionHost, {
      placeholder: "\u0410\u043C\u043F\u043B\u0443\u0430",
      multiple: false,
      showMore: false,
      searchable: false,
      initialSelected: initialPositionItems.length ? initialPositionItems : [],
      load: async () => [...positionItems],
      onChange: (sel) => {
        const one = sel[0];
        if (!one) return;
        const idx = one.id - 1;
        const code = positions[idx]?.code;
        if (code) selectedPositionCode = code;
        setDirty(true);
      }
    });
    const teamInitial = p.teamId != null ? [{ id: p.teamId, name: p.teamName ?? "", sub: null, photo: null, photoMime: null }] : [];
    mountEntitySelect(teamHost, {
      placeholder: "\u041A\u043E\u043C\u0430\u043D\u0434\u0430",
      multiple: false,
      pageSize: 40,
      initialSelected: teamInitial,
      load: async ({ page, pageSize, search }) => {
        const rows = await teamsPaged(page, pageSize, search);
        return rows.map((t) => ({
          id: t.id,
          name: t.name,
          sub: t.city,
          photo: t.photo,
          photoMime: t.photoMime
        }));
      },
      onChange: (sel) => {
        selectedTeamId = sel[0]?.id ?? null;
        setDirty(true);
      }
    });
    const refreshPlayerPhotoPreview = () => {
      const pv = playerPhotoRoot.querySelector("[data-photo-preview]");
      if (pv) pv.innerHTML = dtoImg(`${name.value.trim()} ${surname.value.trim()}`.trim() || "\u0418\u0433\u0440\u043E\u043A", photo, photoMime, "media-block__img player-cards__img--round");
    };
    const refreshCitizenshipPhotoPreview = () => {
      const pv = citPhotoRoot.querySelector("[data-photo-preview]");
      if (pv) pv.innerHTML = dtoImg(citName.value.trim() || "\u0413\u0440\u0430\u0436\u0434\u0430\u043D\u0441\u0442\u0432\u043E", citizenshipPhoto, citizenshipPhotoMime, "media-block__img");
    };
    mountPhotoMediaField(playerPhotoRoot, {
      getPreviewHtml: () => dtoImg(`${name.value.trim()} ${surname.value.trim()}`.trim() || "\u0418\u0433\u0440\u043E\u043A", photo, photoMime, "media-block__img player-cards__img--round"),
      onPickFile: async (f) => {
        photo = await fileToBase64(f);
        photoMime = f.type;
        setDirty(true);
      },
      onClear: () => {
        photo = null;
        photoMime = null;
        setDirty(true);
      }
    });
    mountPhotoMediaField(citPhotoRoot, {
      getPreviewHtml: () => dtoImg(citName.value.trim() || "\u0413\u0440\u0430\u0436\u0434\u0430\u043D\u0441\u0442\u0432\u043E", citizenshipPhoto, citizenshipPhotoMime, "media-block__img"),
      onPickFile: async (f) => {
        citizenshipPhoto = await fileToBase64(f);
        citizenshipPhotoMime = f.type;
        setDirty(true);
      },
      onClear: () => {
        citizenshipPhoto = null;
        citizenshipPhotoMime = null;
        setDirty(true);
      }
    });
    [name, surname, number, birthday, citName].forEach(
      (inp) => inp.addEventListener("input", () => {
        setDirty(true);
        if (inp === name || inp === surname) refreshPlayerPhotoPreview();
        if (inp === citName) refreshCitizenshipPhotoPreview();
      })
    );
    const save = async () => {
      if (!name.value.trim() || !surname.value.trim()) {
        showErrorToast({ message: "\u0418\u043C\u044F \u0438 \u0444\u0430\u043C\u0438\u043B\u0438\u044F \u043E\u0431\u044F\u0437\u0430\u0442\u0435\u043B\u044C\u043D\u044B \u0434\u043B\u044F \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u0438\u044F." });
        return;
      }
      const selectedPositionValue = Math.max(0, positionCodes.indexOf(selectedPositionCode));
      let playerId = id;
      if (!playerId) {
        playerId = await createPlayer({ name: name.value.trim(), surname: surname.value.trim(), position: selectedPositionValue });
      }
      const data = {
        name: name.value.trim(),
        surname: surname.value.trim(),
        position: selectedPositionValue,
        teamId: selectedTeamId,
        number: number.value ? Number(number.value) : null,
        birthday: birthday.value || null,
        citizenship: citName.value.trim() ? { name: citName.value.trim(), photo: citizenshipPhoto, photoMime: citizenshipPhotoMime } : null,
        photo,
        photoMime
      };
      await playerChangeGeneralInfo(playerId, data);
      showSuccessToast({
        message: isNew ? "\u0418\u0433\u0440\u043E\u043A \u0441\u043E\u0437\u0434\u0430\u043D, \u0441\u0432\u0435\u0434\u0435\u043D\u0438\u044F \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u044B." : "\u0421\u0432\u0435\u0434\u0435\u043D\u0438\u044F \u043E\u0431 \u0438\u0433\u0440\u043E\u043A\u0435 \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u044B."
      });
      setDirty(false);
      if (isNew) goSameMode(`/player/${playerId}`);
    };
    root.querySelectorAll("[data-save]").forEach((b) => b.addEventListener("click", () => void save()));
  }

  // src/views/editMatch.ts
  function setupTabs2(host) {
    if (!host) return;
    const btns = host.querySelectorAll(".tabs__btn");
    const panels = host.querySelectorAll(".tabs__panel");
    btns.forEach((btn) => btn.addEventListener("click", () => {
      const tab = btn.dataset.tab;
      btns.forEach((b) => b.classList.toggle("tabs__btn--active", b === btn));
      panels.forEach((p) => {
        const on = p.dataset.panel === tab;
        p.classList.toggle("tabs__panel--active", on);
        if (on) p.removeAttribute("hidden");
        else p.setAttribute("hidden", "");
      });
    }));
  }
  function pad2(n) {
    return String(n).padStart(2, "0");
  }
  function toDateInputValue(iso) {
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return "";
    return `${d.getFullYear()}-${pad2(d.getMonth() + 1)}-${pad2(d.getDate())}`;
  }
  function toTimeInputValue(iso) {
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return "";
    return `${pad2(d.getHours())}:${pad2(d.getMinutes())}`;
  }
  function combineLocalDateTime(date, time) {
    const dateT = date.trim();
    const timeT = time.trim();
    if (!dateT || !timeT) return null;
    const dt = /* @__PURE__ */ new Date(`${dateT}T${timeT}`);
    if (Number.isNaN(dt.getTime())) return null;
    return dt.toISOString();
  }
  var SCHEDULE_MSG = "\u0417\u0430\u043F\u043E\u043B\u043D\u0438\u0442\u0435 \u0434\u0430\u0442\u0443 \u0438 \u0432\u0440\u0435\u043C\u044F \u0440\u0430\u0441\u043F\u0438\u0441\u0430\u043D\u0438\u044F.";
  async function renderEditMatchPage(root, id) {
    const match = await matchById(id);
    if (match.status.code !== 0) {
      root.replaceChildren(
        el(`
        <section class="page page--edit-match">
          <h1 class="page-title">\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u043D\u0438\u0435 \u043C\u0430\u0442\u0447\u0430</h1>
          <div class="panel panel--error">
            <p>\u0418\u0437\u043C\u0435\u043D\u0435\u043D\u0438\u0435 \u043C\u0430\u0442\u0447\u0430 \u043D\u0435\u0432\u043E\u0437\u043C\u043E\u0436\u043D\u043E: \u0434\u043E\u043F\u0443\u0441\u043A\u0430\u0435\u0442\u0441\u044F \u0442\u043E\u043B\u044C\u043A\u043E \u0432 \u0441\u0442\u0430\u0442\u0443\u0441\u0435 \xAB\u0437\u0430\u044F\u0432\u043A\u0430 \u0438\u0433\u0440\u043E\u043A\u043E\u0432\xBB.</p>
            <p><a href="${urlFor(`/match/${id}`, true)}" data-app-link>\u041A \u043C\u0430\u0442\u0447\u0443</a></p>
          </div>
        </section>
      `)
      );
      return;
    }
    let scheduleDate = toDateInputValue(match.scheduleAt);
    let scheduleTime = toTimeInputValue(match.scheduleAt);
    let initialScheduleAt = match.scheduleAt;
    const isScheduleDirty = () => {
      const iso = combineLocalDateTime(scheduleDate, scheduleTime);
      if (!iso) return true;
      return new Date(iso).getTime() !== new Date(initialScheduleAt).getTime();
    };
    const syncScheduleDirty = (rootEl) => {
      const dirty = isScheduleDirty();
      rootEl.querySelectorAll("[data-save-schedule]").forEach((b) => {
        b.disabled = !dirty;
      });
    };
    root.replaceChildren(
      el(`
      <section class="page page--edit-match" data-tabs>
        <div class="page-head page-head--row">
        <h1 class="page-title">\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u043D\u0438\u0435 \u043C\u0430\u0442\u0447\u0430</h1>
        <a class="action-btn action-btn--ghost" href="${urlFor(`/match/${id}`, true)}" data-app-link>\u041A \u043F\u0440\u043E\u0441\u043C\u043E\u0442\u0440\u0443 \u043C\u0430\u0442\u0447\u0430</a>
      </div>
        <p class="muted">${escapeHtml(match.tournament.name)} \xB7 ${escapeHtml(match.homeTeam.name)} \u2014 ${escapeHtml(match.awayTeam.name)}</p>
        <div class="tabs">
          <button type="button" class="tabs__btn tabs__btn--active" data-tab="general">\u041E\u0441\u043D\u043E\u0432\u043D\u043E\u0435</button>
          <button type="button" class="tabs__btn" data-tab="rosters">\u0421\u043E\u0441\u0442\u0430\u0432\u044B</button>
        </div>
        <div class="tabs__panel tabs__panel--active" data-panel="general">
          <div class="action-row">
              <button class="action-btn" type="button" data-save-schedule disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C \u0440\u0430\u0441\u043F\u0438\u0441\u0430\u043D\u0438\u0435</button>
            </div>
          <div class="panel">
            <div class="grid-2">
              <div class="stack-field"><span class="stack-field__label">\u0414\u0430\u0442\u0430 \u043C\u0430\u0442\u0447\u0430</span><input class="search-input" data-schedule-date type="date" value="${escapeHtml(scheduleDate)}" /></div>
              <div class="stack-field"><span class="stack-field__label">\u0412\u0440\u0435\u043C\u044F</span><input class="search-input" data-schedule-time type="time" value="${escapeHtml(scheduleTime)}" /></div>
            </div>
            <div class="action-row">
              <button class="action-btn" type="button" data-save-schedule disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C \u0440\u0430\u0441\u043F\u0438\u0441\u0430\u043D\u0438\u0435</button>
            </div>
          </div>
        </div>
        <div class="tabs__panel" data-panel="rosters" hidden>
          <div class="action-row"><button class="action-btn" type="button" data-save-rosters disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C \u0441\u043E\u0441\u0442\u0430\u0432\u044B</button></div>
          <div class="panel">
            <div class="roster-grid">
              <div class="roster-col"><h3>${escapeHtml(match.homeTeam.name)}</h3><ul class="roster-list" data-roster-home></ul></div>
              <div class="roster-col"><h3>${escapeHtml(match.awayTeam.name)}</h3><ul class="roster-list" data-roster-away></ul></div>
            </div>
            <div class="action-row"><button class="action-btn" type="button" data-save-rosters disabled>\u0421\u043E\u0445\u0440\u0430\u043D\u0438\u0442\u044C \u0441\u043E\u0441\u0442\u0430\u0432\u044B</button></div>
          </div>
        </div>
      </section>
    `)
    );
    setupTabs2(root.querySelector("[data-tabs]"));
    syncScheduleDirty(root);
    const dateInput = root.querySelector("[data-schedule-date]");
    const timeInput = root.querySelector("[data-schedule-time]");
    dateInput.addEventListener("input", () => {
      scheduleDate = dateInput.value;
      clearInvalidHighlightsIn(root);
      syncScheduleDirty(root);
    });
    timeInput.addEventListener("input", () => {
      scheduleTime = timeInput.value;
      clearInvalidHighlightsIn(root);
      syncScheduleDirty(root);
    });
    const onSaveSchedule = async () => {
      const iso = combineLocalDateTime(dateInput.value, timeInput.value);
      if (!iso) {
        const bad = [];
        if (!dateInput.value.trim()) bad.push(dateInput);
        if (!timeInput.value.trim()) bad.push(timeInput);
        showValidationToast({ message: SCHEDULE_MSG, highlightInputs: bad.length ? bad : [dateInput, timeInput] });
        return;
      }
      clearInvalidHighlightsIn(root);
      await matchChangeGeneralInfo(id, { scheduleAt: iso });
      initialScheduleAt = iso;
      scheduleDate = toDateInputValue(iso);
      scheduleTime = toTimeInputValue(iso);
      dateInput.value = scheduleDate;
      timeInput.value = scheduleTime;
      showSuccessToast({ message: "\u0420\u0430\u0441\u043F\u0438\u0441\u0430\u043D\u0438\u0435 \u043C\u0430\u0442\u0447\u0430 \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u043E." });
      syncScheduleDirty(root);
    };
    root.querySelectorAll("[data-save-schedule]").forEach((b) => b.addEventListener("click", () => void onSaveSchedule()));
    const rosterHome = root.querySelector("[data-roster-home]");
    const rosterAway = root.querySelector("[data-roster-away]");
    const [homePlayers, awayPlayers] = await Promise.all([
      playersByTeam(match.homeTeam.id),
      playersByTeam(match.awayTeam.id)
    ]);
    const homeInitial = new Set(match.homeTeamRoster.map((x) => x.id));
    const awayInitial = new Set(match.awayTeamRoster.map((x) => x.id));
    const homeSelected = new Set(homeInitial);
    const awaySelected = new Set(awayInitial);
    const saveBtns = root.querySelectorAll("[data-save-rosters]");
    const isDirty = () => [...homeSelected].sort().join(",") !== [...homeInitial].sort().join(",") || [...awaySelected].sort().join(",") !== [...awayInitial].sort().join(",");
    const syncDirty = () => {
      saveBtns.forEach((b) => {
        b.disabled = !isDirty();
      });
    };
    const renderRoster = (players, selected, host, teamId) => {
      host.innerHTML = players.map((pl) => `
        <li class="roster-item">
          <label class="roster-item__link">
            ${dtoImg(`${pl.name} ${pl.surname}`, pl.photo, pl.photoMime, "mini-photo player-cards__img--round")}
            <span class="roster-item__name"><a href="${urlFor(`/player/${pl.id}`, true)}" data-app-link>${escapeHtml(`${pl.name} ${pl.surname}`)}</a> \xB7 ${escapeHtml(pl.position.name)}</span>
            <input type="checkbox" data-roster-check="${teamId}:${pl.id}" ${selected.has(pl.id) ? "checked" : ""} />
          </label>
        </li>
      `).join("");
    };
    renderRoster(homePlayers, homeSelected, rosterHome, match.homeTeam.id);
    renderRoster(awayPlayers, awaySelected, rosterAway, match.awayTeam.id);
    root.querySelectorAll("[data-roster-check]").forEach((input) => {
      input.addEventListener("change", () => {
        const [teamIdS, playerIdS] = (input.dataset.rosterCheck ?? ":").split(":");
        const teamId = Number(teamIdS);
        const playerId = Number(playerIdS);
        const target = teamId === match.homeTeam.id ? homeSelected : awaySelected;
        if (input.checked) target.add(playerId);
        else target.delete(playerId);
        syncDirty();
      });
    });
    saveBtns.forEach((saveBtn) => saveBtn.addEventListener("click", async () => {
      await addPlayersToRoster(id, match.homeTeam.id, [...homeSelected]);
      await addPlayersToRoster(id, match.awayTeam.id, [...awaySelected]);
      showSuccessToast({ message: "\u0421\u043E\u0441\u0442\u0430\u0432\u044B \u043A\u043E\u043C\u0430\u043D\u0434 \u043D\u0430 \u043C\u0430\u0442\u0447 \u0441\u043E\u0445\u0440\u0430\u043D\u0435\u043D\u044B." });
      homeInitial.clear();
      [...homeSelected].forEach((x) => homeInitial.add(x));
      awayInitial.clear();
      [...awaySelected].forEach((x) => awayInitial.add(x));
      syncDirty();
    }));
    window.addEventListener("beforeunload", (e) => {
      if (!isDirty()) return;
      e.preventDefault();
      e.returnValue = "";
    });
  }

  // src/router.ts
  function parsePath(pathname, search) {
    const admin = isAdminPath(pathname);
    const strippedPath = stripAdminPrefix(pathname);
    const q = new URLSearchParams(search);
    const parts = strippedPath.split("/").filter(Boolean);
    if (parts.length === 0) return { name: "home", query: q, admin };
    if (admin && parts[0] === "tournaments" && parts.length === 1) {
      return { name: "admin-tournaments", query: q };
    }
    if (admin && parts[0] === "teams" && parts.length === 1) {
      return { name: "admin-teams", query: q };
    }
    if (admin && parts[0] === "players" && parts.length === 1) {
      return { name: "admin-players", query: q };
    }
    if (admin && parts[0] === "edit" && parts[1] === "tournament" && parts[2]) {
      return { name: "admin-edit-tournament", idOrNew: parts[2] };
    }
    if (admin && parts[0] === "edit" && parts[1] === "team" && parts[2]) {
      return { name: "admin-edit-team", idOrNew: parts[2] };
    }
    if (admin && parts[0] === "edit" && parts[1] === "player" && parts[2]) {
      return { name: "admin-edit-player", idOrNew: parts[2] };
    }
    if (admin && parts[0] === "edit" && parts[1] === "match" && parts[2]) {
      const mid = Number(parts[2]);
      if (Number.isFinite(mid)) return { name: "admin-edit-match", id: mid };
    }
    if (parts[0] === "match" && parts[1]) {
      const id = Number(parts[1]);
      if (Number.isFinite(id)) return { name: "match", id, admin };
    }
    if (parts[0] === "tournament" && parts[1]) {
      const id = Number(parts[1]);
      if (Number.isFinite(id)) return { name: "tournament", id, admin };
    }
    if (parts[0] === "team" && parts[1]) {
      const id = Number(parts[1]);
      if (Number.isFinite(id)) return { name: "team", id, admin };
    }
    if (parts[0] === "player" && parts[1]) {
      const id = Number(parts[1]);
      if (Number.isFinite(id)) return { name: "player", id, admin };
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
          await renderHome(root, route.query, route.admin);
          break;
        case "match":
          await renderMatchPage(root, route.id, route.admin);
          break;
        case "tournament":
          await renderTournamentPage(root, route.id, route.admin);
          break;
        case "team":
          await renderTeamPage(root, route.id, route.admin);
          break;
        case "player":
          await renderPlayerPage(root, route.id, route.admin);
          break;
        case "admin-tournaments":
          await renderAdminTournamentsPage(root, route.query);
          break;
        case "admin-teams":
          await renderAdminTeamsPage(root, route.query);
          break;
        case "admin-players":
          await renderAdminPlayersPage(root, route.query);
          break;
        case "admin-edit-tournament":
          if (route.idOrNew === "new") {
            go("/tournaments?create=1", true);
            break;
          }
          await renderEditTournamentPage(root, route.idOrNew);
          break;
        case "admin-edit-team":
          await renderEditTeamPage(root, route.idOrNew);
          break;
        case "admin-edit-player":
          await renderEditPlayerPage(root, route.idOrNew);
          break;
        case "admin-edit-match":
          await renderEditMatchPage(root, route.id);
          break;
        default:
          renderNotFound(root);
      }
    } finally {
      window.scrollTo(0, 0);
    }
  }
  function initRouter() {
    const root = document.getElementById("app");
    if (!root) return;
    const run = () => {
      const route = parsePath(window.location.pathname, window.location.search);
      renderHeader(isAdminPath(window.location.pathname));
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
  function renderHeader(admin) {
    const nav = document.querySelector("[data-main-nav]");
    const logo = document.querySelector(".site-logo");
    if (!nav || !logo) return;
    logo.setAttribute("href", withMode("/", admin));
    const links = [
      { href: withMode("/", admin), text: "\u0413\u043B\u0430\u0432\u043D\u0430\u044F" },
      ...admin ? [
        { href: "/admin/tournaments", text: "\u0422\u0443\u0440\u043D\u0438\u0440\u044B" },
        { href: "/admin/teams", text: "\u041A\u043E\u043C\u0430\u043D\u0434\u044B" },
        { href: "/admin/players", text: "\u0418\u0433\u0440\u043E\u043A\u0438" }
      ] : []
    ];
    nav.innerHTML = links.map((x) => `<a href="${x.href}" data-app-link>${x.text}</a>`).join("");
  }

  // src/main.ts
  initRouter();
})();
//# sourceMappingURL=bundle.js.map
