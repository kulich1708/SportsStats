import type { CitizenshipDTO } from '../types';
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

export function dtoPhotoSrc(photo: string | null | undefined, photoMime: string | null | undefined): string {
  if (!photo || !photoMime) return '/assets/placeholder.svg';
  return `data:${photoMime};base64,${photo}`;
}

export function dtoImg(
  alt: string,
  photo?: string | null,
  photoMime?: string | null,
  className = 'media-block__img',
): string {
  return `<img class="${escapeHtml(className)}" src="${escapeHtml(dtoPhotoSrc(photo, photoMime))}" alt="${escapeHtml(alt)}" loading="lazy" />`;
}

export async function fileToBase64(file: File): Promise<string> {
  const b = await file.arrayBuffer();
  let binary = '';
  const bytes = new Uint8Array(b);
  const chunk = 0x8000;
  for (let i = 0; i < bytes.length; i += chunk) {
    binary += String.fromCharCode(...bytes.slice(i, i + chunk));
  }
  return btoa(binary);
}

export function el(html: string): HTMLElement {
  const t = document.createElement('template');
  t.innerHTML = html.trim();
  const n = t.content.firstChild;
  if (!n || !(n instanceof HTMLElement)) throw new Error('el: invalid html');
  return n;
}


/** Компактный флаг гражданства рядом с именем (высота ~ как у текста). */
export function citizenshipBadgeHtml(c: CitizenshipDTO | null | undefined): string {
  if (!c || (!c.name?.trim() && !c.photo)) return '';
  const alt = c.name.trim() || 'Гражданство';
  return `<span class="citizenship-inline" title="${escapeHtml(c.name)}">${dtoImg(alt, c.photo, c.photoMime, 'citizenship-flag-img')}</span>`;
}

export function iconSvgEdit(): string {
  return `<svg class="icon-svg" width="16" height="16" viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zm2.92 2.83H5v-.92l9.06-9.06.92.92L5.92 20.08zM20.71 7.04a1 1 0 0 0 0-1.41l-2.34-2.34a1 1 0 0 0-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/></svg>`;
}

export function iconSvgTrash(): string {
  return `<svg class="icon-svg" width="16" height="16" viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M9 3V2h6v1h5v2H4V3h5zm1 5h2v9h-2V8zm4 0h2v9h-2V8zM6 6h12l-1 14H7L6 6z"/></svg>`;
}

export function photoMediaFieldHtml(label: string, previewInnerHtml: string): string {
  return `
    <div class="media-field">
      <div class="media-field__toolbar">
        <span class="media-field__label">${escapeHtml(label)}</span>
        <input type="file" accept="image/*" class="media-field__file" data-photo-file hidden />
        <button type="button" class="icon-btn icon-btn--tight" data-photo-pick title="Выбрать изображение" aria-label="Выбрать изображение">${iconSvgEdit()}</button>
        <button type="button" class="icon-btn icon-btn--tight icon-btn--danger" data-photo-clear title="Удалить изображение" aria-label="Удалить изображение">${iconSvgTrash()}</button>
      </div>
      <div class="media-field__preview" data-photo-preview>${previewInnerHtml}</div>
    </div>`;
}

export function mountPhotoMediaField(root: HTMLElement, opts: {
  getPreviewHtml: () => string;
  onPickFile: (file: File) => Promise<void>;
  onClear: () => void;
}): void {
  const input = root.querySelector<HTMLInputElement>('[data-photo-file]')!;
  const preview = root.querySelector<HTMLElement>('[data-photo-preview]')!;
  const pick = root.querySelector<HTMLButtonElement>('[data-photo-pick]')!;
  const clear = root.querySelector<HTMLButtonElement>('[data-photo-clear]')!;
  const sync = () => {
    preview.innerHTML = opts.getPreviewHtml();
  };
  pick.addEventListener('click', () => input.click());
  input.addEventListener('change', async () => {
    const f = input.files?.[0];
    if (!f) return;
    await opts.onPickFile(f);
    sync();
    input.value = '';
  });
  clear.addEventListener('click', () => {
    opts.onClear();
    input.value = '';
    sync();
  });
}

