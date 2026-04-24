import { createTournament, tournamentChangeGeneralInfo, tournamentsPaged } from '../api';
import { goSameMode, urlFor } from '../nav';
import { openModal } from '../ui/modal';
import { clearInvalidHighlightsIn, showSuccessToast, showValidationToast } from '../ui/errors';
import { dtoImg, el, escapeHtml, fileToBase64, mountPhotoMediaField, photoMediaFieldHtml } from '../ui/utils';

const PAGE_SIZE = 40;
let timer = 0;

type CreateTournamentGeneral = {
  name: string;
  photo: string | null;
  photoMime: string | null;
};

function openCreateTournamentModal(): void {
  const state: CreateTournamentGeneral = {
    name: '',
    photo: null,
    photoMime: null,
  };

  const modal = openModal(
    'Создание турнира',
    `
      <div class="action-row"><button class="action-btn" data-create-top>Создать</button></div>
      <div class="panel">
        <div class="stack-field"><span class="stack-field__label">Название</span><input class="search-input" data-name /></div>
        <div data-tournament-photo>${photoMediaFieldHtml('Фото', dtoImg('Турнир', null, null))}</div>
      </div>
      <div class="action-row"><button class="action-btn" data-create-bottom>Создать</button></div>
    `,
  );

  const nameInput = modal.body.querySelector<HTMLInputElement>('[data-name]')!;
  const photoRoot = modal.body.querySelector<HTMLElement>('[data-tournament-photo]')!;

  const showNameError = () => {
    showValidationToast({
      message: 'Проверьте правильность заполнения полей: название турнира не должно быть пустым.',
      highlightInputs: [nameInput],
    });
  };

  nameInput.addEventListener('input', () => {
    state.name = nameInput.value.trim();
    clearInvalidHighlightsIn(modal.body);
  });

  mountPhotoMediaField(photoRoot, {
    getPreviewHtml: () => dtoImg(state.name || 'Турнир', state.photo, state.photoMime),
    onPickFile: async (file) => {
      state.photo = await fileToBase64(file);
      state.photoMime = file.type;
    },
    onClear: () => {
      state.photo = null;
      state.photoMime = null;
    },
  });

  const createNow = async () => {
    const name = nameInput.value.trim();
    if (!name) {
      showNameError();
      return;
    }

    const tournamentId = await createTournament(name);
    await tournamentChangeGeneralInfo(tournamentId, {
      name,
      photo: state.photo,
      photoMime: state.photoMime,
    });

    showSuccessToast({ message: 'Турнир создан, основная информация сохранена.' });

    modal.close();
    goSameMode(`/admin/edit/tournament/${tournamentId}`);
  };

  modal.body.querySelectorAll<HTMLButtonElement>('[data-create-top], [data-create-bottom]')
    .forEach((b) => b.addEventListener('click', () => void createNow()));
}

function tournamentRow(t: import('../types').TournamentShortDTO): string {
  return `
    <article class="list-row">
      <div class="list-row__lead">
        <a class="entity-link" href="${urlFor(`/tournament/${t.id}`, true)}" data-app-link>${dtoImg(t.name, t.photo, t.photoMime, 'mini-photo')}<span>${escapeHtml(t.name)}</span></a>
        <span class="muted list-row__meta">${escapeHtml(t.status.description)}</span>
      </div>
      <a class="icon-btn" href="${urlFor(`/admin/edit/tournament/${t.id}`, true)}" data-app-link>✎</a>
    </article>`;
}

export async function renderAdminTournamentsPage(root: HTMLElement, query: URLSearchParams): Promise<void> {
  let page = Number(query.get('page') || 1);
  let search = query.get('search') || '';
  let rows = await tournamentsPaged(page, PAGE_SIZE, search);
  const render = () => rows.map((t) => tournamentRow(t)).join('');

  root.replaceChildren(el(`
    <section class="page"><div class="page-head page-head--row"><h1 class="page-title">Турниры</h1><button type="button" class="action-btn" data-create-tournament>Создать турнир</button></div>
      <div class="filters-row">
        <input class="search-input" placeholder="Поиск турниров" value="${escapeHtml(search)}" data-search />
      </div>
      <div class="list-wrap" data-list>${render()}</div>
      <button class="action-btn action-btn--ghost" data-more>Показать больше</button>
    </section>`));

  root.querySelector<HTMLButtonElement>('[data-create-tournament]')
    ?.addEventListener('click', () => openCreateTournamentModal());

  if (query.get('create') === '1') {
    const next = new URLSearchParams(query);
    next.delete('create');
    const nextSearch = next.toString();
    window.history.replaceState({}, '', `/admin/tournaments${nextSearch ? `?${nextSearch}` : ''}`);
    openCreateTournamentModal();
  }

  const inp = root.querySelector<HTMLInputElement>('[data-search]');
  const list = root.querySelector<HTMLElement>('[data-list]');
  inp?.addEventListener('input', () => {
    if (timer) window.clearTimeout(timer);
    timer = window.setTimeout(async () => {
      search = inp.value.trim();
      page = 1;
      rows = await tournamentsPaged(page, PAGE_SIZE, search);
      if (list) list.innerHTML = render();
    }, 700);
  });
  root.querySelector<HTMLButtonElement>('[data-more]')?.addEventListener('click', async () => {
    page += 1;
    const chunk = await tournamentsPaged(page, PAGE_SIZE, search);
    if (chunk.length === 0) return;
    rows = [...rows, ...chunk];
    if (list) list.insertAdjacentHTML('beforeend', chunk.map((t) => tournamentRow(t)).join(''));
  });
}
