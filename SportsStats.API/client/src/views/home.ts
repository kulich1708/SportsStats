import { tournamentsByDate } from '../api';
import { renderMatchRow, bindMatchRowClicks } from '../components/matchRow';
import { go } from '../nav';
import {
  addDays,
  dayMonthLabel,
  el,
  escapeHtml,
  parseDateOnlyIso,
  toDateOnlyIso,
  weekdayShort,
} from '../ui/utils';

function isSameDay(a: Date, b: Date): boolean {
  return (
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate()
  );
}

export async function renderHome(root: HTMLElement, query: URLSearchParams): Promise<void> {
  const today = new Date();
  const dateStr = query.get('date');
  const center = dateStr ? parseDateOnlyIso(dateStr) : today;
  const iso = toDateOnlyIso(center);

  const data = await tournamentsByDate(iso);

  const stripDays: Date[] = [];
  for (let i = -4; i <= 4; i++) stripDays.push(addDays(center, i));

  const stripHtml = stripDays
    .map((d) => {
      const active = isSameDay(d, center) ? ' date-strip__btn--active' : '';
      const label = isSameDay(d, today)
        ? `Сегодня ${dayMonthLabel(d)}`
        : `${weekdayShort(d)} ${dayMonthLabel(d)}`;
      const di = toDateOnlyIso(d);
      return `<button type="button" class="date-strip__btn${active}" data-date="${di}">${escapeHtml(label)}</button>`;
    })
    .join('');

  const blocks = data
    .map((t) => {
      const matchesHtml = t.matches.map((m) => renderMatchRow(m)).join('');
      return `
        <details class="tournament-block" open>
          <summary class="tournament-block__summary">
            <span class="tournament-block__chevron" aria-hidden="true"></span>
            <a class="tournament-block__title" href="/tournament/${t.id}" data-app-link>${escapeHtml(t.name)}</a>
          </summary>
          <div class="tournament-block__body">
            ${matchesHtml || '<p class="muted">Нет матчей на эту дату.</p>'}
          </div>
        </details>
      `;
    })
    .join('');

  root.replaceChildren(
    el(`
      <section class="page page--home">
        <div class="page-head">
          <h1 class="page-title">Матчи</h1>
          <p class="page-sub">Турниры и игры на выбранный день</p>
        </div>
        <div class="date-toolbar">
          <div class="date-strip">${stripHtml}</div>
          <label class="date-any">
            <span class="date-any__label">Другая дата</span>
            <input class="date-any__input" type="date" value="${escapeHtml(iso)}" max="2099-12-31" min="1990-01-01" />
          </label>
        </div>
        <div class="tournament-list">
          ${blocks || '<p class="muted panel">На эту дату нет запланированных матчей.</p>'}
        </div>
      </section>
    `),
  );

  const section = root.querySelector<HTMLElement>('.page--home');
  if (!section) return;

  section.querySelectorAll<HTMLButtonElement>('.date-strip__btn').forEach((btn) => {
    btn.addEventListener('click', () => {
      const d = btn.dataset.date;
      if (d) go(`/?date=${d}`);
    });
  });

  const any = section.querySelector<HTMLInputElement>('.date-any__input');
  any?.addEventListener('change', () => {
    if (any.value) go(`/?date=${any.value}`);
  });

  bindMatchRowClicks(section);
}
