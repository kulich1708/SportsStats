import { addPlayersToRoster, matchById, matchChangeGeneralInfo, playersByTeam } from '../api';
import { urlFor } from '../nav';
import { clearInvalidHighlightsIn, showSuccessToast, showValidationToast } from '../ui/errors';
import type { MatchDTO, PlayerDTO } from '../types';
import { dtoImg, el, escapeHtml } from '../ui/utils';

function setupTabs(host: HTMLElement | null): void {
	if (!host) return;
	const btns = host.querySelectorAll<HTMLButtonElement>('.tabs__btn');
	const panels = host.querySelectorAll<HTMLElement>('.tabs__panel');
	btns.forEach((btn) => btn.addEventListener('click', () => {
		const tab = btn.dataset.tab;
		btns.forEach((b) => b.classList.toggle('tabs__btn--active', b === btn));
		panels.forEach((p) => {
			const on = p.dataset.panel === tab;
			p.classList.toggle('tabs__panel--active', on);
			if (on) p.removeAttribute('hidden');
			else p.setAttribute('hidden', '');
		});
	}));
}

function pad2(n: number): string {
	return String(n).padStart(2, '0');
}

function toDateInputValue(iso: string): string {
	const d = new Date(iso);
	if (Number.isNaN(d.getTime())) return '';
	return `${d.getFullYear()}-${pad2(d.getMonth() + 1)}-${pad2(d.getDate())}`;
}

function toTimeInputValue(iso: string): string {
	const d = new Date(iso);
	if (Number.isNaN(d.getTime())) return '';
	return `${pad2(d.getHours())}:${pad2(d.getMinutes())}`;
}

function combineLocalDateTime(date: string, time: string): string | null {
	const dateT = date.trim();
	const timeT = time.trim();
	if (!dateT || !timeT) return null;
	const dt = new Date(`${dateT}T${timeT}`);
	if (Number.isNaN(dt.getTime())) return null;
	return dt.toISOString();
}

const SCHEDULE_MSG = 'Заполните дату и время расписания.';

export async function renderEditMatchPage(root: HTMLElement, id: number): Promise<void> {
	const match = await matchById(id);
	if (match.status.code !== 0) {
		root.replaceChildren(
			el(`
        <section class="page page--edit-match">
          <h1 class="page-title">Редактирование матча</h1>
          <div class="panel panel--error">
            <p>Изменение матча невозможно: допускается только в статусе «заявка игроков».</p>
            <p><a href="${urlFor(`/match/${id}`, true)}" data-app-link>К матчу</a></p>
          </div>
        </section>
      `),
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

	const syncScheduleDirty = (rootEl: HTMLElement) => {
		const dirty = isScheduleDirty();
		rootEl.querySelectorAll<HTMLButtonElement>('[data-save-schedule]').forEach((b) => {
			b.disabled = !dirty;
		});
	};


	root.replaceChildren(
		el(`
      <section class="page page--edit-match" data-tabs>
        <div class="page-head page-head--row">
        <h1 class="page-title">\u0420\u0435\u0434\u0430\u043a\u0442\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u0435 \u043c\u0430\u0442\u0447\u0430</h1>
        <a class="action-btn action-btn--ghost" href="${urlFor(`/match/${id}`, true)}" data-app-link>\u041a \u043f\u0440\u043e\u0441\u043c\u043e\u0442\u0440\u0443 \u043c\u0430\u0442\u0447\u0430</a>
      </div>
        <p class="muted">${escapeHtml(match.tournament.name)} · ${escapeHtml(match.homeTeam.name)} — ${escapeHtml(match.awayTeam.name)}</p>
        <div class="tabs">
          <button type="button" class="tabs__btn tabs__btn--active" data-tab="general">Основное</button>
          <button type="button" class="tabs__btn" data-tab="rosters">Составы</button>
        </div>
        <div class="tabs__panel tabs__panel--active" data-panel="general">
          <div class="action-row">
              <button class="action-btn" type="button" data-save-schedule disabled>Сохранить расписание</button>
            </div>
          <div class="panel">
            <div class="grid-2">
              <div class="stack-field"><span class="stack-field__label">Дата матча</span><input class="search-input" data-schedule-date type="date" value="${escapeHtml(scheduleDate)}" /></div>
              <div class="stack-field"><span class="stack-field__label">Время</span><input class="search-input" data-schedule-time type="time" value="${escapeHtml(scheduleTime)}" /></div>
            </div>
            <div class="action-row">
              <button class="action-btn" type="button" data-save-schedule disabled>Сохранить расписание</button>
            </div>
          </div>
        </div>
        <div class="tabs__panel" data-panel="rosters" hidden>
          <div class="action-row"><button class="action-btn" type="button" data-save-rosters disabled>Сохранить составы</button></div>
          <div class="panel">
            <div class="roster-grid">
              <div class="roster-col"><h3>${escapeHtml(match.homeTeam.name)}</h3><ul class="roster-list" data-roster-home></ul></div>
              <div class="roster-col"><h3>${escapeHtml(match.awayTeam.name)}</h3><ul class="roster-list" data-roster-away></ul></div>
            </div>
            <div class="action-row"><button class="action-btn" type="button" data-save-rosters disabled>Сохранить составы</button></div>
          </div>
        </div>
      </section>
    `),
	);

	setupTabs(root.querySelector('[data-tabs]'));

	syncScheduleDirty(root);

	const dateInput = root.querySelector<HTMLInputElement>('[data-schedule-date]')!;
	const timeInput = root.querySelector<HTMLInputElement>('[data-schedule-time]')!;

	dateInput.addEventListener('input', () => {
		scheduleDate = dateInput.value;
		clearInvalidHighlightsIn(root);
		syncScheduleDirty(root);
	});
	timeInput.addEventListener('input', () => {
		scheduleTime = timeInput.value;
		clearInvalidHighlightsIn(root);
		syncScheduleDirty(root);
	});

		const onSaveSchedule = async () => {
		const iso = combineLocalDateTime(dateInput.value, timeInput.value);
		if (!iso) {
			const bad: HTMLElement[] = [];
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
		showSuccessToast({ message: 'Расписание матча сохранено.' });
		syncScheduleDirty(root);
	};
	root.querySelectorAll<HTMLButtonElement>('[data-save-schedule]').forEach((b) => b.addEventListener('click', () => void onSaveSchedule()));

	const rosterHome = root.querySelector<HTMLElement>('[data-roster-home]')!;
	const rosterAway = root.querySelector<HTMLElement>('[data-roster-away]')!;
	const [homePlayers, awayPlayers] = await Promise.all([
		playersByTeam(match.homeTeam.id),
		playersByTeam(match.awayTeam.id),
	]);

	const homeInitial = new Set(match.homeTeamRoster.map((x) => x.id));
	const awayInitial = new Set(match.awayTeamRoster.map((x) => x.id));
	const homeSelected = new Set(homeInitial);
	const awaySelected = new Set(awayInitial);
	const saveBtns = root.querySelectorAll<HTMLButtonElement>('[data-save-rosters]');

	const isDirty = () =>
		[...homeSelected].sort().join(',') !== [...homeInitial].sort().join(',') ||
		[...awaySelected].sort().join(',') !== [...awayInitial].sort().join(',');

	const syncDirty = () => {
		saveBtns.forEach((b) => {
			b.disabled = !isDirty();
		});
	};

	const renderRoster = (players: PlayerDTO[], selected: Set<number>, host: HTMLElement, teamId: number) => {
		host.innerHTML = players
			.map((pl) => `
        <li class="roster-item">
          <label class="roster-item__link">
            ${dtoImg(`${pl.name} ${pl.surname}`, pl.photo, pl.photoMime, 'mini-photo player-cards__img--round')}
            <span class="roster-item__name"><a href="${urlFor(`/player/${pl.id}`, true)}" data-app-link>${escapeHtml(`${pl.name} ${pl.surname}`)}</a> · ${escapeHtml(pl.position.name)}</span>
            <input type="checkbox" data-roster-check="${teamId}:${pl.id}" ${selected.has(pl.id) ? 'checked' : ''} />
          </label>
        </li>
      `)
			.join('');
	};

	renderRoster(homePlayers, homeSelected, rosterHome, match.homeTeam.id);
	renderRoster(awayPlayers, awaySelected, rosterAway, match.awayTeam.id);

	root.querySelectorAll<HTMLInputElement>('[data-roster-check]').forEach((input) => {
		input.addEventListener('change', () => {
			const [teamIdS, playerIdS] = (input.dataset.rosterCheck ?? ':').split(':');
			const teamId = Number(teamIdS);
			const playerId = Number(playerIdS);
			const target = teamId === match.homeTeam.id ? homeSelected : awaySelected;
			if (input.checked) target.add(playerId);
			else target.delete(playerId);
			syncDirty();
		});
	});

	saveBtns.forEach((saveBtn) => saveBtn.addEventListener('click', async () => {
		await addPlayersToRoster(id, match.homeTeam.id, [...homeSelected]);
		await addPlayersToRoster(id, match.awayTeam.id, [...awaySelected]);
		showSuccessToast({ message: 'Составы команд на матч сохранены.' });
		homeInitial.clear();
		[...homeSelected].forEach((x) => homeInitial.add(x));
		awayInitial.clear();
		[...awaySelected].forEach((x) => awayInitial.add(x));
		syncDirty();
	}));

	window.addEventListener('beforeunload', (e) => {
		if (!isDirty()) return;
		e.preventDefault();
		e.returnValue = '';
	});
}
