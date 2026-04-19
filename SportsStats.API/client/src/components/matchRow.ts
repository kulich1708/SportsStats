import type { MatchShortDTO } from '../types';
import { go } from '../nav';
import { escapeHtml, isMatchFinished } from '../ui/utils';

export function renderMatchRow(m: MatchShortDTO, opts?: { showOtForFinished?: boolean }): string {
  const finished = isMatchFinished(m.status);
  const rowClass = finished ? 'match-row match-row--done' : 'match-row match-row--live';
  const ot =
    finished && m.isOvertime && opts?.showOtForFinished !== false
      ? '<span class="match-row__ot" title="Овертайм">ОТ</span>'
      : '';

  const score = `${m.homeTeamScore} : ${m.awayTeamScore}`;

  return `
    <div class="${rowClass}" data-nav-match="${m.id}">
      <div class="match-row__teams">
        <a class="match-row__team" href="/team/${m.homeTeam.id}" data-app-link>${escapeHtml(m.homeTeam.name)}</a>
        <span class="match-row__score">${escapeHtml(score)}${ot}</span>
        <a class="match-row__team match-row__team--away" href="/team/${m.awayTeam.id}" data-app-link>${escapeHtml(m.awayTeam.name)}</a>
      </div>
      <div class="match-row__meta">
        <span class="match-row__time">${escapeHtml(new Date(m.scheduleAt).toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' }))}</span>
        <span class="match-row__status">${escapeHtml(m.status)}</span>
      </div>
    </div>
  `;
}

export function bindMatchRowClicks(container: HTMLElement): void {
  container.querySelectorAll<HTMLElement>('[data-nav-match]').forEach((row) => {
    row.addEventListener('click', (e) => {
      const t = e.target as HTMLElement | null;
      if (t?.closest('a[data-app-link]')) return;
      const id = row.dataset.navMatch;
      if (id) {
        e.preventDefault();
        go(`/match/${id}`);
      }
    });
  });
}
