import { teamsPaged } from '../api';
import { urlFor } from '../nav';
import { dtoImg, el, escapeHtml } from '../ui/utils';

const PAGE_SIZE = 40;
let timer = 0;

function teamRow(t: import('../types').TeamDTO, admin: boolean): string {
  return `
    <article class="list-row">
      <div class="list-row__lead">
        <a class="entity-link" href="${urlFor(`/team/${t.id}`, admin)}" data-app-link>${dtoImg(t.name, t.photo, t.photoMime, 'mini-photo')}<span>${escapeHtml(t.name)}</span></a>
        <span class="muted list-row__meta">${escapeHtml(t.city ?? '—')}</span>
      </div>
      ${admin ? `<a class="icon-btn" href="${urlFor(`/admin/edit/team/${t.id}`, true)}" data-app-link>✎</a>` : ''}
    </article>`;
}

export async function renderTeamsPage(root: HTMLElement, query: URLSearchParams, admin: boolean): Promise<void> {
  let page = Number(query.get('page') || 1);
  let search = query.get('search') || '';
  let rows = await teamsPaged(page, PAGE_SIZE, search);
  const render = () => rows.map((t) => teamRow(t, admin)).join('');

  root.replaceChildren(el(`
    <section class="page"><div class="page-head page-head--row"><h1 class="page-title">\u041a\u043e\u043c\u0430\u043d\u0434\u044b</h1>${admin ? `<a class="action-btn" href="${urlFor('/admin/edit/team/new', true)}" data-app-link>\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u043a\u043e\u043c\u0430\u043d\u0434\u0443</a>` : ''}</div>
      <div class="filters-row">
        <input class="search-input" placeholder="\u041f\u043e\u0438\u0441\u043a \u043a\u043e\u043c\u0430\u043d\u0434\u044b" value="${escapeHtml(search)}" data-search />
      </div>
      <div class="list-wrap" data-list>${render()}</div>
      <button class="action-btn action-btn--ghost" data-more>\u041f\u043e\u043a\u0430\u0437\u0430\u0442\u044c \u0431\u043e\u043b\u044c\u0448\u0435</button>
    </section>`));

  const inp = root.querySelector<HTMLInputElement>('[data-search]');
  const list = root.querySelector<HTMLElement>('[data-list]');
  inp?.addEventListener('input', () => {
    if (timer) window.clearTimeout(timer);
    timer = window.setTimeout(async () => {
      search = inp.value.trim();
      page = 1;
      rows = await teamsPaged(page, PAGE_SIZE, search);
      if (list) list.innerHTML = render();
    }, 700);
  });
  root.querySelector<HTMLButtonElement>('[data-more]')?.addEventListener('click', async () => {
    page += 1;
    const chunk = await teamsPaged(page, PAGE_SIZE, search);
    if (chunk.length === 0) return;
    rows = [...rows, ...chunk];
    if (list) list.insertAdjacentHTML('beforeend', chunk.map((t) => teamRow(t, admin)).join(''));
  });
}

export async function renderAdminTeamsPage(root: HTMLElement, query: URLSearchParams): Promise<void> {
  await renderTeamsPage(root, query, true);
}
