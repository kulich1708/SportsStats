export function escapeHtml(s: string): string {
  return s
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#039;');
}

const FINISHED = 'Закончен';

export function isMatchFinished(status: string): boolean {
  return status === FINISHED;
}

/** Seconds within period → mm:ss */
export function formatGoalTime(seconds: number): string {
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
}

export function formatDateTime(iso: string): string {
  const d = new Date(iso);
  return new Intl.DateTimeFormat('ru-RU', {
    day: '2-digit',
    month: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).format(d);
}

export function formatTimeOnly(iso: string): string {
  const d = new Date(iso);
  return new Intl.DateTimeFormat('ru-RU', {
    hour: '2-digit',
    minute: '2-digit',
  }).format(d);
}

export function toDateOnlyIso(d: Date): string {
  const y = d.getFullYear();
  const m = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${y}-${m}-${day}`;
}

export function parseDateOnlyIso(s: string): Date {
  const [y, m, d] = s.split('-').map(Number);
  return new Date(y, m - 1, d);
}

export function weekdayShort(d: Date): string {
  return new Intl.DateTimeFormat('ru-RU', { weekday: 'short' }).format(d);
}

export function dayMonthLabel(d: Date): string {
  return new Intl.DateTimeFormat('ru-RU', { day: '2-digit', month: '2-digit' }).format(d);
}

export function addDays(d: Date, n: number): Date {
  const x = new Date(d);
  x.setDate(x.getDate() + n);
  return x;
}

export function placeholderImg(alt: string, className = 'media-block__img'): string {
  return `<img class="${escapeHtml(className)}" src="/assets/placeholder.svg" alt="${escapeHtml(alt)}" width="160" height="160" loading="lazy" />`;
}

export function el(html: string): HTMLElement {
  const t = document.createElement('template');
  t.innerHTML = html.trim();
  const n = t.content.firstChild;
  if (!n || !(n instanceof HTMLElement)) throw new Error('el: invalid html');
  return n;
}
