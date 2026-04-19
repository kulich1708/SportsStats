import { getJson, teamCalendar, teamResults, playersByTeam } from '../api';
import { bindMatchRowClicks, renderMatchRow } from '../components/matchRow';
import { el, escapeHtml, placeholderImg } from '../ui/utils';
import type { TeamDTO } from '../types';

const PAGE_SIZE = 200;

async function teamMeta(id: number): Promise<TeamDTO> {
  return getJson<TeamDTO>(`/Teams/${id}`);
}

function renderTournamentGroups(
  blocks: import('../types').TournamentWithMatchesDTO[],
): string {
  if (blocks.length === 0) return '<p class="muted">Нет матчей.</p>';
  return blocks
    .map((t) => {
      const rows = t.matches.map((m) => renderMatchRow(m)).join('');
      return `
        <section class="team-tblock">
          <h3 class="team-tblock__title"><a href="/tournament/${t.id}" data-app-link>${escapeHtml(t.name)}</a></h3>
          <div class="match-stack">${rows}</div>
        </section>
      `;
    })
    .join('');
}

export async function renderTeamPage(root: HTMLElement, id: number): Promise<void> {
  const team = await teamMeta(id);

  const [cal, res, roster] = await Promise.all([
    teamCalendar(id, 1, PAGE_SIZE).catch(() => [] as import('../types').TournamentWithMatchesDTO[]),
    teamResults(id, 1, PAGE_SIZE).catch(() => [] as import('../types').TournamentWithMatchesDTO[]),
    playersByTeam(id),
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
          <button type="button" class="tabs__btn tabs__btn--active" data-tab="calendar">Календарь</button>
          <button type="button" class="tabs__btn" data-tab="results">Результаты</button>
          <button type="button" class="tabs__btn" data-tab="roster">Состав</button>
        </div>

        <div class="tabs__panel tabs__panel--active" data-panel="calendar">
          ${renderTournamentGroups(cal)}
        </div>
        <div class="tabs__panel" data-panel="results" hidden>
          ${renderTournamentGroups(res)}
        </div>
        <div class="tabs__panel" data-panel="roster" hidden>
          <ul class="player-cards">
            ${roster
              .map(
                (p) => `
              <li class="player-cards__item">
                <a href="/player/${p.id}" data-app-link class="player-cards__link">
                  ${placeholderImg(`${p.name} ${p.surname}`, 'player-cards__img')}
                  <span class="player-cards__name">${escapeHtml(`${p.name} ${p.surname}`)}</span>
                  <span class="player-cards__meta">${escapeHtml(p.position)}</span>
                </a>
              </li>`,
              )
              .join('')}
          </ul>
          ${roster.length === 0 ? '<p class="muted">Игроки не найдены.</p>' : ''}
        </div>
      </section>
    `),
  );

  const host = root.querySelector<HTMLElement>('[data-team-tabs]');
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

  bindMatchRowClicks(host);
}
