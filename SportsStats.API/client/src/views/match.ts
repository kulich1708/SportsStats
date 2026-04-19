import { matchById } from '../api';
import type { GoalDTO, MatchDTO } from '../types';
import { el, escapeHtml, formatGoalTime, formatTimeOnly, placeholderImg } from '../ui/utils';

function periodTitle(period: number, periodsCount: number): string {
  if (period <= periodsCount) return `${period}-й период`;
  return 'Овертайм';
}

function sortGoals(goals: GoalDTO[]): GoalDTO[] {
  return [...goals].sort((a, b) => a.period - b.period || a.time - b.time);
}

function buildRunningScores(match: MatchDTO): Map<string, string> {
  const key = (g: GoalDTO) => `${g.period}-${g.time}-${g.goalScorerId.id}`;
  const map = new Map<string, string>();
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

export async function renderMatchPage(root: HTMLElement, id: number): Promise<void> {
  const match = await matchById(id);
  const periodsCount = match.rules?.matchDurationRules?.periodsCount ?? 3;
  const scoreMap = buildRunningScores(match);

  const goalsByPeriod = new Map<number, GoalDTO[]>();
  for (const g of match.goals) {
    const list = goalsByPeriod.get(g.period) ?? [];
    list.push(g);
    goalsByPeriod.set(g.period, list);
  }
  for (const [, list] of goalsByPeriod) {
    list.sort((a, b) => a.time - b.time);
  }

  const periodNumbers = new Set<number>();
  for (let p = 1; p <= periodsCount; p++) periodNumbers.add(p);
  for (const g of match.goals) {
    if (g.period > periodsCount) periodNumbers.add(g.period);
  }
  const sortedPeriods = [...periodNumbers].sort((a, b) => a - b);

  const goalKey = (g: GoalDTO) => `${g.period}-${g.time}-${g.goalScorerId.id}`;

  const overviewBlocks = sortedPeriods
    .map((pNum) => {
      const title = periodTitle(pNum, periodsCount);
      const goals = goalsByPeriod.get(pNum) ?? [];
      const body =
        goals.length === 0
          ? '<p class="period-block__empty">—</p>'
          : goals
              .map((g) => {
                const scoreAfter = scoreMap.get(goalKey(g)) ?? '';
                const assists: string[] = [];
                if (g.firstAssistId) assists.push(`${g.firstAssistId.name} ${g.firstAssistId.surname}`);
                if (g.secondAssistId) assists.push(`${g.secondAssistId.name} ${g.secondAssistId.surname}`);
                const detailParts = [g.strengthType, g.netType].filter(Boolean);
                const detail = detailParts.length ? detailParts.join(' · ') : '';
                const assistsHtml =
                  assists.length > 0
                    ? `<div class="goal-line__assists">Ассистенты: ${assists.map((x) => escapeHtml(x)).join(', ')}</div>`
                    : '';
                return `
                  <article class="goal-line">
                    <div class="goal-line__row">
                      <span class="goal-line__time">${formatGoalTime(g.time)}</span>
                      <span class="goal-line__score">${escapeHtml(scoreAfter)}</span>
                      <span class="goal-line__scorer">
                        <a href="/player/${g.goalScorerId.id}" data-app-link>${escapeHtml(`${g.goalScorerId.name} ${g.goalScorerId.surname}`)}</a>
                      </span>
                    </div>
                    ${detail ? `<div class="goal-line__detail">${escapeHtml(detail)}</div>` : ''}
                    ${assistsHtml}
                  </article>
                `;
              })
              .join('');
      return `
        <section class="period-block">
          <h3 class="period-block__title">${escapeHtml(title)}</h3>
          ${body}
        </section>
      `;
    })
    .join('');

  const roster = (players: typeof match.homeTeamRoster) =>
    players
      .map(
        (pl) => `
      <li class="roster-item">
        <a href="/player/${pl.id}" data-app-link class="roster-item__link">
          <span class="roster-item__num">${escapeHtml(pl.position)}</span>
          <span class="roster-item__name">${escapeHtml(`${pl.name} ${pl.surname}`)}</span>
        </a>
      </li>
    `,
      )
      .join('');

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
            ${match.isOvertime ? '<div class="badge">ОТ</div>' : ''}
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
          <button type="button" class="tabs__btn tabs__btn--active" data-tab="overview">Обзор</button>
          <button type="button" class="tabs__btn" data-tab="rosters">Составы</button>
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
    `),
  );

  setupTabs(root.querySelector('[data-tabs]'));
}

function setupTabs(host: HTMLElement | null): void {
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
}
