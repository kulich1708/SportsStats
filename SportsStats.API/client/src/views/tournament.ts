import {
  tournamentById,
  teamStatsByTournament,
  tournamentMatchesCalendar,
  tournamentMatchesResult,
} from '../api';
import type { TeamStatsDTO } from '../types';
import { bindMatchRowClicks, renderMatchRow } from '../components/matchRow';
import { el, escapeHtml, placeholderImg } from '../ui/utils';

type SortKey =
  | 'teamName'
  | 'games'
  | 'regularWins'
  | 'otWins'
  | 'otLosses'
  | 'regularLosses'
  | 'draws'
  | 'points';

const PAGE_SIZE = 200;

function numCell(v: number | null): string {
  if (v === null) return '—';
  return String(v);
}

function comparePrimary(a: TeamStatsDTO, b: TeamStatsDTO, key: SortKey, asc: boolean): number {
  const s = asc ? 1 : -1;
  switch (key) {
    case 'teamName':
      return a.teamName.localeCompare(b.teamName, 'ru') * s;
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
  return a.teamName.localeCompare(b.teamName, 'ru');
}

function sortRows(rows: TeamStatsDTO[], key: SortKey, asc: boolean): TeamStatsDTO[] {
  return [...rows].sort((a, b) => {
    const p = comparePrimary(a, b, key, asc);
    if (p !== 0) return p;
    return tieBreak(a, b);
  });
}

function renderTableBody(rows: TeamStatsDTO[]): string {
  return rows
    .map((r, idx) => {
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
    })
    .join('');
}

export async function renderTournamentPage(root: HTMLElement, id: number): Promise<void> {
  const [t, stats, finished, schedule] = await Promise.all([
    tournamentById(id),
    teamStatsByTournament(id),
    tournamentMatchesResult(id, 1, PAGE_SIZE),
    tournamentMatchesCalendar(id, 1, PAGE_SIZE),
  ]);

  let sortKey: SortKey = 'points';
  let sortAsc = false;

  const resort = () => sortRows(stats, sortKey, sortAsc);

  const tableHtml = (rows: TeamStatsDTO[]) => `
    <div class="table-wrap">
      <table class="data-table" data-sort-table>
        <thead>
          <tr>
            <th>#</th>
            <th data-sort="teamName">Команда</th>
            <th data-sort="games">И</th>
            <th data-sort="regularWins" title="Победы в основное время">В</th>
            <th data-sort="otWins" title="Победы в овертайме">ВО</th>
            <th data-sort="otLosses" title="Поражения в овертайме">ПО</th>
            <th data-sort="regularLosses" title="Поражения в основное время">П</th>
            <th data-sort="draws">Н</th>
            <th title="Забито (пока нет в API)">З</th>
            <th title="Пропущено (пока нет в API)">Пр</th>
            <th title="Разница (пока нет в API)">Р</th>
            <th data-sort="points">О</th>
          </tr>
        </thead>
        <tbody>${renderTableBody(rows)}</tbody>
      </table>
    </div>
    <p class="table-hint muted">Колонки «З», «П», «Р» (забито / пропущено / разница) пока недоступны в API — зарезервированы под будущие данные.</p>
  `;

  const resultsHtml = finished.map((m) => renderMatchRow(m)).join('') || '<p class="muted">Нет завершённых матчей.</p>';
  const calHtml = schedule.map((m) => renderMatchRow(m)).join('') || '<p class="muted">Нет запланированных матчей.</p>';

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
          <button type="button" class="tabs__btn tabs__btn--active" data-tab="table">Таблица</button>
          <button type="button" class="tabs__btn" data-tab="results">Результаты</button>
          <button type="button" class="tabs__btn" data-tab="calendar">Календарь</button>
        </div>

        <div class="tabs__panel tabs__panel--active" data-panel="table">${tableHtml(resort())}</div>
        <div class="tabs__panel" data-panel="results" hidden><div class="match-stack" data-result-matches>${resultsHtml}</div></div>
        <div class="tabs__panel" data-panel="calendar" hidden><div class="match-stack" data-cal-matches>${calHtml}</div></div>
      </section>
    `),
  );

  const host = root.querySelector<HTMLElement>('[data-tournament-tabs]');
  if (!host) return;

  const btns = host.querySelectorAll<HTMLButtonElement>('.tabs__btn');
  const panels = host.querySelectorAll<HTMLElement>('.tabs__panel');
  btns.forEach((btn) => {
    btn.addEventListener('click', () => {
      const tab = btn.dataset.tab;
      btns.forEach((b) => b.classList.toggle('tabs__btn--active', b === btn));
      panels.forEach((p) => {
        const on = p.dataset.panel === tab;
        p.classList.toggle('tabs__panel--active', on);
        if (on) p.removeAttribute('hidden');
        else p.setAttribute('hidden', '');
      });
    });
  });

  const tbody = host.querySelector('tbody');
  host.querySelector('[data-sort-table]')?.addEventListener('click', (e) => {
    const th = (e.target as HTMLElement).closest<HTMLTableCellElement>('th[data-sort]');
    if (!th || !tbody) return;
    const k = th.dataset.sort as SortKey;
    if (sortKey === k) sortAsc = !sortAsc;
    else {
      sortKey = k;
      sortAsc = k === 'teamName';
    }
    tbody.innerHTML = renderTableBody(resort());
  });

  bindMatchRowClicks(host);
}
