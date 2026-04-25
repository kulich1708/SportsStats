import type { MatchShortDTO } from '../types';
import { goSameMode, urlFor } from '../nav';
import { dtoImg, escapeHtml, isMatchFinished } from '../ui/utils';

export function renderMatchRow(
  m: MatchShortDTO,
  opts?: { showOtForFinished?: boolean; admin?: boolean; showEdit?: boolean },
): string {
  const finished = isMatchFinished(m.status.description);
  const rowClass = finished ? 'match-row match-row--done' : 'match-row match-row--live';
  const ot =
    finished && m.isOvertime && opts?.showOtForFinished !== false
      ? '<span class="match-row__ot" title="\u041e\u0432\u0435\u0440\u0442\u0430\u0439\u043c">\u041e\u0422</span>'
      : '';

  const score = `${m.homeTeamScore} : ${m.awayTeamScore}`;

  return `
    <div class="${rowClass}" data-nav-match="${m.id}">
      <div class="match-row__teams">
        <div class="match-row__team-col match-row__team-col--home">
          <a class="match-row__team-link entity-link" href="${urlFor(`/team/${m.homeTeam.id}`, !!opts?.admin)}" data-app-link>
            ${dtoImg(m.homeTeam.name, m.homeTeam.photo, m.homeTeam.photoMime, 'mini-photo')}
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
            ${dtoImg(m.awayTeam.name, m.awayTeam.photo, m.awayTeam.photoMime, 'mini-photo')}
            <span class="match-row__team-text">${escapeHtml(m.awayTeam.name)}</span>
          </a>
        </div>
      </div>
      <div class="match-row__meta">
        <span class="match-row__time">${escapeHtml(new Date(m.scheduleAt).toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' }))}</span>
        <span class="match-row__status">${escapeHtml(m.status.description)}</span>
        ${opts?.showEdit ? `<a class="icon-btn" href="${urlFor(`/admin/edit/match/${m.id}`, true)}" data-app-link title="\u0420\u0435\u0434\u0430\u043a\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u043c\u0430\u0442\u0447">\u270e</a>` : ''}
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
        goSameMode(`/match/${id}`);
      }
    });
  });
}
