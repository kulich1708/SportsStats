import {
	createTournamentMatch,
	teamByTournament,
	teamStatsByTournament,
	tournamentById,
	tournamentMatchesCalendar,
	tournamentMatchesResult,
} from '../api';
import { bindMatchRowClicks, renderMatchRow } from '../components/matchRow';
import { goSameMode, urlFor } from '../nav';
import type { TeamDTO, TeamStatsDTO } from '../types';
import { dtoImg, el, escapeHtml } from '../ui/utils';
import { openModal } from '../ui/modal';
import { mountEntitySelect } from '../ui/entitySelect';
import { showErrorToast, showSuccessToast, showValidationToast } from '../ui/errors';

type SortKey =
	| 'teamName'
	| 'games'
	| 'regularWins'
	| 'otWins'
	| 'otLosses'
	| 'regularLosses'
	| 'draws'
	| 'points';

const PAGE_SIZE = 40;

function safeTeamName(name: string | null | undefined): string {
	return (name ?? '').trim();
}

function comparePrimary(a: TeamStatsDTO, b: TeamStatsDTO, key: SortKey, asc: boolean): number {
	const s = asc ? 1 : -1;
	switch (key) {
		case 'teamName':
			return safeTeamName(a.teamName).localeCompare(safeTeamName(b.teamName), 'ru') * s;
		case 'games':
			return (a.games - b.games) * s;
		case 'regularWins':
			return (a.regularWins - b.regularWins) * s;
		case 'otWins':
			return (a.otWins - b.otWins) * s;
		case 'otLosses':
			return (a.otLosses - b.otLosses) * s;
		case 'regularLosses':
			return (a.regularLosses - b.regularLosses) * s;
		case 'draws':
			return (a.draws - b.draws) * s;
		case 'points':
			return (a.points - b.points) * s;
		default:
			return 0;
	}
}

function tieBreak(a: TeamStatsDTO, b: TeamStatsDTO): number {
	if (b.points !== a.points) return b.points - a.points;
	if (b.regularWins !== a.regularWins) return b.regularWins - a.regularWins;
	if (b.otWins !== a.otWins) return b.otWins - a.otWins;
	return safeTeamName(a.teamName).localeCompare(safeTeamName(b.teamName), 'ru');
}

function sortRows(rows: TeamStatsDTO[], key: SortKey, asc: boolean): TeamStatsDTO[] {
	return [...rows].sort((a, b) => {
		const p = comparePrimary(a, b, key, asc);
		if (p !== 0) return p;
		return tieBreak(a, b);
	});
}

function renderTableBody(rows: TeamStatsDTO[], teams: Map<number, TeamDTO>, admin: boolean): string {
	return rows
		.map((r, idx) => {
			const team = teams.get(r.teamId);
			const name = safeTeamName(r.teamName) || '-';
			return `
      <tr>
        <td>${idx + 1}</td>
        <td class="data-table__team-cell">
          <a class="entity-link" href="${urlFor(`/team/${r.teamId}`, admin)}" data-app-link>
            ${dtoImg(name, team?.photo, team?.photoMime, 'mini-photo')}
            <span>${escapeHtml(name)}</span>
          </a>
          ${admin ? `<a class="icon-btn" href="${urlFor(`/admin/edit/team/${r.teamId}`, true)}" data-app-link title="Редактировать команду">✎</a>` : ''}
        </td>
        <td>${r.games}</td>
        <td>${r.regularWins}</td>
        <td>${r.otWins}</td>
        <td>${r.otLosses}</td>
        <td>${r.regularLosses}</td>
        <td>${r.draws}</td>
        <td>—</td>
        <td>—</td>
        <td>—</td>
        <td class="col-points">${r.points}</td>
      </tr>`;
		})
		.join('');
}



function pad2(n: number): string {
	return String(n).padStart(2, '0');
}

function combineLocalDateTimeForMatch(date: string, time: string): string | null {
	const dateT = date.trim();
	const timeT = time.trim();
	if (!dateT || !timeT) return null;
	const dt = new Date(`${dateT}T${timeT}`);
	if (Number.isNaN(dt.getTime())) return null;
	return dt.toISOString();
}

function openCreateMatchModal(tournamentId: number, allTeams: import('../types').TeamDTO[]): void {
	type Picker = ReturnType<typeof mountEntitySelect>;

	const toItems = (excludeId?: number) =>
		allTeams
			.filter((x) => (excludeId ? x.id !== excludeId : true))
			.map((x) => ({
				id: x.id,
				name: x.name,
				sub: x.city,
				photo: x.photo ?? null,
				photoMime: x.photoMime ?? null,
			}));

	let excludedHome: number | undefined;
	let excludedAway: number | undefined;

	const modal = openModal(
		'Создание матча',
		`
      <div class="grid-2">
        <div><span>Хозяева</span><div data-home-select></div></div>
        <div><span>Гости</span><div data-away-select></div></div>
        <div class="stack-field"><span class="stack-field__label">Дата</span><input class="search-input" data-match-date type="date" /></div>
        <div class="stack-field"><span class="stack-field__label">Время</span><input class="search-input" data-match-time type="time" /></div>
      </div>
      <div class="action-row"><button class="action-btn" type="button" data-create-match-submit>Создать</button></div>
    `,
		{ tall: true },
	);

	const homeHost = modal.body.querySelector<HTMLElement>('[data-home-select]')!;
	const awayHost = modal.body.querySelector<HTMLElement>('[data-away-select]')!;

	let homePicker!: Picker;
	let awayPicker!: Picker;

	homePicker = mountEntitySelect(homeHost, {
		placeholder: 'Команда',
		multiple: false,
		showMore: false,
		load: async ({ search }) => {
			const q = search.toLowerCase();
			return toItems(excludedAway).filter((x) => x.name.toLowerCase().includes(q));
		},
		onChange: () => {
			excludedHome = homePicker.getSelected()[0]?.id;
			void awayPicker.reload();
		},
	});

	awayPicker = mountEntitySelect(awayHost, {
		placeholder: 'Команда',
		multiple: false,
		showMore: false,
		load: async ({ search }) => {
			const q = search.toLowerCase();
			return toItems(excludedHome).filter((x) => x.name.toLowerCase().includes(q));
		},
		onChange: () => {
			excludedAway = awayPicker.getSelected()[0]?.id;
			void homePicker.reload();
		},
	});

	const dateInput = modal.body.querySelector<HTMLInputElement>('[data-match-date]')!;
	const timeInput = modal.body.querySelector<HTMLInputElement>('[data-match-time]')!;

	modal.body.querySelector<HTMLButtonElement>('[data-create-match-submit]')?.addEventListener('click', async () => {
		const homeId = homePicker.getSelected()[0]?.id;
		const awayId = awayPicker.getSelected()[0]?.id;
		const homeTrigger = homeHost.querySelector<HTMLElement>('.entity-select__trigger');
		const awayTrigger = awayHost.querySelector<HTMLElement>('.entity-select__trigger');
		const bad: HTMLElement[] = [];
		if (!homeId && homeTrigger) bad.push(homeTrigger);
		if (!awayId && awayTrigger) bad.push(awayTrigger);
		if (!dateInput.value.trim()) bad.push(dateInput);
		if (!timeInput.value.trim()) bad.push(timeInput);
		if (bad.length) {
			showValidationToast({
				message: 'Проверьте правильность заполнения полей: выберите обе команды и укажите дату и время.',
				highlightInputs: bad,
			});
			return;
		}
		if (homeId === awayId) {
			showValidationToast({
				message: 'Нельзя выбрать одну и ту же команду хозяев и гостей.',
				highlightInputs: [homeTrigger, awayTrigger].filter((x): x is HTMLElement => Boolean(x)),
			});
			return;
		}

		const scheduledAt = combineLocalDateTimeForMatch(dateInput.value, timeInput.value);
		if (!scheduledAt) {
			showValidationToast({
				message: 'Проверьте правильность заполнения полей: укажите корректную дату и время.',
				highlightInputs: [dateInput, timeInput],
			});
			return;
		}

		try {
			const newId = await createTournamentMatch(tournamentId, {
				homeTeamId: homeId,
				awayTeamId: awayId,
				scheduledAt,
			});
			showSuccessToast({ message: 'Матч турнира создан.' });
			modal.close();
			goSameMode(`/admin/edit/match/${newId}`);
		} catch (e: unknown) {
			showErrorToast({ message: e instanceof Error ? e.message : String(e) });
		}
	});
}

export async function renderTournamentPage(root: HTMLElement, id: number, admin: boolean): Promise<void> {
	const [t, stats, teams] = await Promise.all([tournamentById(id), teamStatsByTournament(id), teamByTournament(id)]);
	const teamMap = new Map(teams.map((x) => [x.id, x]));
	let resultPage = 1;
	let calendarPage = 1;
	let finished = await tournamentMatchesResult(id, resultPage, PAGE_SIZE);
	let schedule = await tournamentMatchesCalendar(id, calendarPage, PAGE_SIZE);
	let sortKey: SortKey = 'points';
	let sortAsc = false;
	const resort = () => sortRows(stats, sortKey, sortAsc);

	const tableHtml = (rows: TeamStatsDTO[]) => `
    <div class="table-wrap">
      <table class="data-table" data-sort-table>
        <thead>
          <tr><th>#</th><th data-sort="teamName">Команда</th><th data-sort="games">И</th><th data-sort="regularWins">В</th><th data-sort="otWins">ВО</th><th data-sort="otLosses">ПО</th><th data-sort="regularLosses">П</th><th data-sort="draws">Н</th><th>З</th><th>Пр</th><th>Р</th><th data-sort="points">О</th></tr>
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
            ${admin ? `<div class="action-row"><a class="action-btn" href="${urlFor(`/admin/edit/tournament/${t.id}`, true)}" data-app-link>Редактировать турнир</a>${t.status.code === 2 ? `<button type="button" class="action-btn" data-create-match>Создать матч</button>` : ''}</div>` : ''}
          </div>
        </div>
        <div class="tabs"><button type="button" class="tabs__btn tabs__btn--active" data-tab="table">Таблица</button><button type="button" class="tabs__btn" data-tab="results">Результаты</button><button type="button" class="tabs__btn" data-tab="calendar">Календарь</button></div>
        <div class="tabs__panel tabs__panel--active" data-panel="table">${tableHtml(resort())}</div>
        <div class="tabs__panel" data-panel="results" hidden><div class="match-stack" data-result-matches>${finished.map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 })).join('')}</div><button class="action-btn action-btn--ghost" data-more-results>Показать больше</button></div>
        <div class="tabs__panel" data-panel="calendar" hidden><div class="match-stack" data-cal-matches>${schedule.map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 })).join('')}</div><button class="action-btn action-btn--ghost" data-more-calendar>Показать больше</button></div>
      </section>
    `),
	);

	const host = root.querySelector<HTMLElement>('[data-tournament-tabs]');
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

	const tbody = host.querySelector('tbody');
	host.querySelector('[data-sort-table]')?.addEventListener('click', (e) => {
		const th = (e.target as HTMLElement).closest<HTMLTableCellElement>('th[data-sort]');
		if (!th || !tbody) return;
		const k = th.dataset.sort as SortKey;
		if (sortKey === k) sortAsc = !sortAsc;
		else { sortKey = k; sortAsc = k === 'teamName'; }
		tbody.innerHTML = renderTableBody(resort(), teamMap, admin);
	});

	bindMatchRowClicks(host);

	root.querySelector<HTMLButtonElement>('[data-create-match]')?.addEventListener('click', () => {
		openCreateMatchModal(id, teams);
	});

	host.querySelector<HTMLButtonElement>('[data-more-results]')?.addEventListener('click', async () => {
		resultPage += 1;
		const chunk = await tournamentMatchesResult(id, resultPage, PAGE_SIZE);
		const wrap = host.querySelector<HTMLElement>('[data-result-matches]');
		if (!wrap || chunk.length === 0) return;
		wrap.insertAdjacentHTML('beforeend', chunk.map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 })).join(''));
		bindMatchRowClicks(wrap);
		finished = [...finished, ...chunk];
	});
	host.querySelector<HTMLButtonElement>('[data-more-calendar]')?.addEventListener('click', async () => {
		calendarPage += 1;
		const chunk = await tournamentMatchesCalendar(id, calendarPage, PAGE_SIZE);
		const wrap = host.querySelector<HTMLElement>('[data-cal-matches]');
		if (!wrap || chunk.length === 0) return;
		wrap.insertAdjacentHTML('beforeend', chunk.map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 })).join(''));
		bindMatchRowClicks(wrap);
		schedule = [...schedule, ...chunk];
	});
}
