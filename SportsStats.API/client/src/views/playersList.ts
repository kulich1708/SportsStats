import { playersByTeam, playersPaged, teamsPaged } from '../api';
import { urlFor } from '../nav';
import { mountEntitySelect } from '../ui/entitySelect';
import { citizenshipBadgeHtml, dtoImg, el, escapeHtml } from '../ui/utils';

const PAGE_SIZE = 40;

export async function renderPlayersPage(root: HTMLElement, query: URLSearchParams, admin: boolean): Promise<void> {
  let page = Number(query.get('page') || 1);
  let search = query.get('search') || '';
  let selectedTeamIds: number[] = [];
  let rows = await playersPaged(page, PAGE_SIZE, search);

  const renderRows = (items: typeof rows) =>
    items
      .map((p) => `
      <article class="list-row">
        <a class="entity-link" href="${urlFor(`/player/${p.id}`, admin)}" data-app-link>
          ${dtoImg(`${p.name} ${p.surname}`, p.photo, p.photoMime, 'mini-photo player-cards__img--round')}
          <span>${escapeHtml(`${p.name} ${p.surname}`)}</span>${citizenshipBadgeHtml(p.citizenship ?? null)}
        </a>
        <span class="muted">${escapeHtml(`${p.teamName ?? 'Без команды'} · ${p.position.name}`)}</span>
        ${admin ? `<a class="icon-btn" href="${urlFor(`/admin/edit/player/${p.id}`, true)}" data-app-link>✎</a>` : ''}
      </article>`)
      .join('');

  root.replaceChildren(
    el(`
      <section class="page">
        <div class="page-head page-head--row">
          <h1 class="page-title">Игроки</h1>
          ${admin ? `<a class="action-btn" href="${urlFor('/admin/edit/player/new', true)}" data-app-link>Добавить игрока</a>` : ''}
        </div>
        <div class="filters-row">
          <input class="search-input" placeholder="Поиск игрока" value="${escapeHtml(search)}" data-search />
          <div class="teams-filter-host" data-team-filter></div>
        </div>
        <div class="list-wrap" data-list>${renderRows(rows)}</div>
        <button class="action-btn action-btn--ghost" data-more>Показать больше</button>
      </section>
    `),
  );

  const list = root.querySelector<HTMLElement>('[data-list]')!;
  const searchInput = root.querySelector<HTMLInputElement>('[data-search]')!;
  const teamFilterHost = root.querySelector<HTMLElement>('[data-team-filter]')!;
  const moreBtn = root.querySelector<HTMLButtonElement>('[data-more]')!;

  mountEntitySelect(teamFilterHost, {
    placeholder: 'Фильтр по командам',
    multiple: true,
    load: async ({ page: p, pageSize, search: s }) => {
      const items = await teamsPaged(p, pageSize, s);
      return items.map((x) => ({ id: x.id, name: x.name, sub: x.city, photo: x.photo, photoMime: x.photoMime }));
    },
    onChange: async (selected) => {
      selectedTeamIds = selected.map((x) => x.id);
      page = 1;
      if (selectedTeamIds.length === 0) {
        rows = await playersPaged(page, PAGE_SIZE, search);
      } else {
        const chunks = await Promise.all(selectedTeamIds.map((teamId) => playersByTeam(teamId)));
        const map = new Map<number, (typeof rows)[number]>();
        chunks.flat().forEach((pl) => map.set(pl.id, pl));
        rows = [...map.values()];
      }
      list.innerHTML = renderRows(rows);
    },
  });

  let timer = 0;
  searchInput.addEventListener('input', () => {
    if (timer) window.clearTimeout(timer);
    timer = window.setTimeout(async () => {
      search = searchInput.value.trim();
      page = 1;
      if (selectedTeamIds.length > 0) {
        const chunks = await Promise.all(selectedTeamIds.map((teamId) => playersByTeam(teamId)));
        const all = chunks.flat();
        const q = search.toLowerCase();
        rows = all.filter((p) => (`${p.name} ${p.surname} ${p.teamName ?? ''}`.toLowerCase().includes(q)));
      } else {
        rows = await playersPaged(page, PAGE_SIZE, search);
      }
      list.innerHTML = renderRows(rows);
    }, 700);
  });

  moreBtn.addEventListener('click', async () => {
    if (selectedTeamIds.length > 0) return;
    page += 1;
    const chunk = await playersPaged(page, PAGE_SIZE, search);
    if (chunk.length === 0) return;
    rows = [...rows, ...chunk];
    list.insertAdjacentHTML('beforeend', renderRows(chunk));
  });
}

export async function renderAdminPlayersPage(root: HTMLElement, query: URLSearchParams): Promise<void> {
  await renderPlayersPage(root, query, true);
}