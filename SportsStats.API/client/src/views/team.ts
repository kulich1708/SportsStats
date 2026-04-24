import { getJson, playersByTeam, teamCalendar, teamResults } from '../api';
import { bindMatchRowClicks, renderMatchRow } from '../components/matchRow';
import { urlFor } from '../nav';
import type { TeamDTO, TournamentWithMatchesDTO } from '../types';
import { citizenshipBadgeHtml, dtoImg, el, escapeHtml } from '../ui/utils';

const PAGE_SIZE = 40;

async function teamMeta(id: number): Promise<TeamDTO> {
  return getJson<TeamDTO>(`/Teams/${id}`);
}

function renderTournamentGroups(blocks: TournamentWithMatchesDTO[], admin: boolean): string {
  if (blocks.length === 0) return '<p class="muted">Нет матчей.</p>';
  return blocks
    .map((t) => `
      <section class="team-tblock">
        <h3 class="team-tblock__title">
          <a class="entity-link" href="${urlFor(`/tournament/${t.id}`, admin)}" data-app-link>
            ${dtoImg(t.name, t.photo, t.photoMime, 'mini-photo')}
            <span>${escapeHtml(t.name)}</span>
          </a>
          
        </h3>
        <div class="match-stack">${t.matches.map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 })).join('')}</div>
      </section>
    `)
    .join('');
}

export async function renderTeamPage(root: HTMLElement, id: number, admin: boolean): Promise<void> {
  const team = await teamMeta(id);
  let calPage = 1;
  let resPage = 1;
  let [cal, res, roster] = await Promise.all([
    teamCalendar(id, calPage, PAGE_SIZE).catch(() => [] as TournamentWithMatchesDTO[]),
    teamResults(id, resPage, PAGE_SIZE).catch(() => [] as TournamentWithMatchesDTO[]),
    playersByTeam(id),
  ]);

  root.replaceChildren(
    el(`
      <section class="page page--team" data-team-tabs>
        <div class="entity-head">
          ${dtoImg(team.name, team.photo, team.photoMime)}
          <div>
            <h1 class="page-title">${escapeHtml(team.name)}</h1>
            ${team.city ? `<p class="page-sub">${escapeHtml(team.city)}</p>` : ''}
            ${admin ? `<div class="action-row"><a class="action-btn" href="${urlFor(`/admin/edit/team/${team.id}`, true)}" data-app-link>Изменить команду</a></div>` : ''}
          </div>
        </div>

        <div class="tabs">
          <button type="button" class="tabs__btn tabs__btn--active" data-tab="calendar">Календарь</button>
          <button type="button" class="tabs__btn" data-tab="results">Результаты</button>
          <button type="button" class="tabs__btn" data-tab="roster">Состав</button>
        </div>

        <div class="tabs__panel tabs__panel--active" data-panel="calendar">${renderTournamentGroups(cal, admin)}<button class="action-btn action-btn--ghost" data-more-cal>Показать больше</button></div>
        <div class="tabs__panel" data-panel="results" hidden>${renderTournamentGroups(res, admin)}<button class="action-btn action-btn--ghost" data-more-res>Показать больше</button></div>
        <div class="tabs__panel" data-panel="roster" hidden>
          <ul class="player-cards">
            ${roster.map((p) => `
              <li class="player-cards__item">
                <a href="${urlFor(`/player/${p.id}`, admin)}" data-app-link class="player-cards__link">
                  ${dtoImg(`${p.name} ${p.surname}`, p.photo, p.photoMime, 'player-cards__img player-cards__img--round')}
                  <span class="player-cards__name">${escapeHtml(`${p.name} ${p.surname}`)}</span>${citizenshipBadgeHtml(p.citizenship ?? null)}
                  <span class="player-cards__meta">${escapeHtml(p.position.name)}</span>
                </a>
                ${admin ? `<a class="icon-btn" href="${urlFor(`/admin/edit/player/${p.id}`, true)}" data-app-link title="Редактировать игрока">✎</a>` : ''}
              </li>`).join('')}
          </ul>
        </div>
      </section>
    `),
  );

  const host = root.querySelector<HTMLElement>('[data-team-tabs]');
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

  bindMatchRowClicks(host);

  host.querySelector<HTMLButtonElement>('[data-more-cal]')?.addEventListener('click', async () => {
    calPage += 1;
    const chunk = await teamCalendar(id, calPage, PAGE_SIZE);
    if (chunk.length === 0) return;
    cal = [...cal, ...chunk];
    const panel = host.querySelector<HTMLElement>('[data-panel="calendar"]');
    if (!panel) return;
    panel.insertAdjacentHTML('afterbegin', renderTournamentGroups(chunk, admin));
    bindMatchRowClicks(panel);
  });
  host.querySelector<HTMLButtonElement>('[data-more-res]')?.addEventListener('click', async () => {
    resPage += 1;
    const chunk = await teamResults(id, resPage, PAGE_SIZE);
    if (chunk.length === 0) return;
    res = [...res, ...chunk];
    const panel = host.querySelector<HTMLElement>('[data-panel="results"]');
    if (!panel) return;
    panel.insertAdjacentHTML('afterbegin', renderTournamentGroups(chunk, admin));
    bindMatchRowClicks(panel);
  });
}

