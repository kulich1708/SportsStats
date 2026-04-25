import { tournamentsByDate } from '../api';
import { renderMatchRow, bindMatchRowClicks } from '../components/matchRow';
import { goSameMode, urlFor } from '../nav';
import {
  addDays,
  dayMonthLabel,
  dtoImg,
  el,
  escapeHtml,
  parseDateOnlyIso,
  toDateOnlyIso,
  weekdayShort,
} from '../ui/utils';



function bindHomeAccordions(host: HTMLElement): void {
  host.querySelectorAll<HTMLElement>('[data-accordion]').forEach((block) => {
    const summary = block.querySelector<HTMLElement>('[data-accordion-summary]');
    const body = block.querySelector<HTMLElement>('[data-accordion-body]');
    if (!summary || !body) return;

    const apply = (open: boolean) => {
      block.classList.toggle('is-open', open);
      body.classList.toggle('is-open', open);
      summary.setAttribute('aria-expanded', open ? 'true' : 'false');
    };

    apply(block.classList.contains('is-open'));

    const toggle = (e?: MouseEvent) => {
      if (e) {
        const t = e.target as HTMLElement | null;
        if (t?.closest('a[data-app-link], button, input, textarea, select')) return;
      }
      apply(!block.classList.contains('is-open'));
    };

    summary.addEventListener('click', (e) => toggle(e as MouseEvent));
    summary.addEventListener('keydown', (e) => {
      if (e.key === 'Enter' || e.key === ' ') {
        e.preventDefault();
        toggle();
      }
    });
  });
}

function isSameDay(a: Date, b: Date): boolean {
  return (
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate()
  );
}

export async function renderHome(root: HTMLElement, query: URLSearchParams, admin: boolean): Promise<void> {
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
      const matchesHtml = t.matches
        .map((m) => renderMatchRow(m, { admin, showEdit: admin && m.status.code === 0 }))
        .join('');
      return `
        <section class="tournament-block tournament-block--accordion is-open" data-accordion>
          <div class="tournament-block__head" data-accordion-summary tabindex="0" role="button" aria-expanded="true">
            <span class="tournament-block__chevron" aria-hidden="true"></span>
            ${dtoImg(t.name, t.photo, t.photoMime, 'mini-photo')}
            <span class="tournament-block__title-wrap">
              <a class="tournament-block__title" href="${urlFor(`/tournament/${t.id}`, admin)}" data-app-link>${escapeHtml(t.name)}</a>
            </span>
            ${admin ? `<a class="icon-btn" href="${urlFor(`/admin/edit/tournament/${t.id}`, true)}" data-app-link title="Редактировать турнир">✎</a>` : ''}
          </div>
          <div class="tournament-block__collapsible is-open" data-accordion-body>
            <div class="tournament-block__collapsible-inner">
              <div class="tournament-block__body">
                ${matchesHtml || '<p class="muted">Нет матчей на эту дату.</p>'}
              </div>
            </div>
          </div>
        </section>
      `;
    })
    .join('');

  root.replaceChildren(
    el(`
      <section class="page page--home">
        <div class="page-head">
          <h1 class="page-title">Матчи</h1>
          <p class="page-sub">Турниры и игры на выбранный день</p>
          ${admin ? '<p class="muted">Режим администратора</p>' : ''}
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
      if (d) goSameMode(`/?date=${d}`);
    });
  });

  const any = section.querySelector<HTMLInputElement>('.date-any__input');
  any?.addEventListener('change', () => {
    if (any.value) goSameMode(`/?date=${any.value}`);
  });

  bindMatchRowClicks(section);
  bindHomeAccordions(section);
}
