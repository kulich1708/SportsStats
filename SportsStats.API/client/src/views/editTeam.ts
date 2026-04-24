import { createTeam, teamById, teamChangeGeneralInfo } from '../api';
import { goSameMode, urlFor } from '../nav';
import { showErrorToast, showSuccessToast } from '../ui/errors';
import { dtoImg, el, escapeHtml, fileToBase64, mountPhotoMediaField, photoMediaFieldHtml } from '../ui/utils';

export async function renderEditTeamPage(root: HTMLElement, idOrNew: string): Promise<void> {
  const isNew = idOrNew === 'new';
  const id = isNew ? null : Number(idOrNew);
  const team = id ? await teamById(id) : { id: 0, name: '', city: null, photo: null, photoMime: null };
  let form = { ...team };
  let dirty = false;
  const setDirty = (v: boolean) => {
    dirty = v;
    root.querySelectorAll<HTMLButtonElement>('[data-save]').forEach((b) => { b.disabled = !dirty; });
  };

  const viewLink =
    id !== null
      ? `<a class="action-btn action-btn--ghost" href="${urlFor(`/team/${id}`, true)}" data-app-link>\u041a \u043f\u0440\u043e\u0441\u043c\u043e\u0442\u0440\u0443 \u043a\u043e\u043c\u0430\u043d\u0434\u044b</a>`
      : '';

  root.replaceChildren(el(`
    <section class="page">
      <div class="page-head page-head--row">
        <h1 class="page-title">${isNew ? '\u0421\u043e\u0437\u0434\u0430\u043d\u0438\u0435 \u043a\u043e\u043c\u0430\u043d\u0434\u044b' : '\u0420\u0435\u0434\u0430\u043a\u0442\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u0435 \u043a\u043e\u043c\u0430\u043d\u0434\u044b'}</h1>
        ${viewLink}
      </div>
      <div class="action-row"><button class="action-btn" data-save disabled>\u0421\u043e\u0445\u0440\u0430\u043d\u0438\u0442\u044c</button></div>
      <div class="panel">
        <div class="stack-field"><span class="stack-field__label">\u041d\u0430\u0437\u0432\u0430\u043d\u0438\u0435</span><input class="search-input" data-name value="${escapeHtml(form.name)}" /></div>
        <div class="stack-field"><span class="stack-field__label">\u0413\u043e\u0440\u043e\u0434</span><input class="search-input" data-city value="${escapeHtml(form.city ?? '')}" /></div>
        <div data-team-photo>${photoMediaFieldHtml('\u0424\u043e\u0442\u043e', dtoImg(form.name || '\u041a\u043e\u043c\u0430\u043d\u0434\u0430', form.photo, form.photoMime))}</div>
      </div>
      <div class="action-row"><button class="action-btn" data-save disabled>\u0421\u043e\u0445\u0440\u0430\u043d\u0438\u0442\u044c</button></div>
    </section>
  `));

  const nameInput = root.querySelector<HTMLInputElement>('[data-name]')!;
  const cityInput = root.querySelector<HTMLInputElement>('[data-city]')!;
  const photoRoot = root.querySelector<HTMLElement>('[data-team-photo]')!;

  [nameInput, cityInput].forEach((x) => x.addEventListener('input', () => setDirty(true)));

  mountPhotoMediaField(photoRoot, {
    getPreviewHtml: () => dtoImg(nameInput.value.trim() || '\u041a\u043e\u043c\u0430\u043d\u0434\u0430', form.photo, form.photoMime),
    onPickFile: async (f) => {
      form.photo = await fileToBase64(f);
      form.photoMime = f.type;
      setDirty(true);
    },
    onClear: () => {
      form.photo = null;
      form.photoMime = null;
      setDirty(true);
    },
  });

  const syncPreviewName = () => {
    const pv = photoRoot.querySelector<HTMLElement>('[data-photo-preview]');
    if (pv) pv.innerHTML = dtoImg(nameInput.value.trim() || '\u041a\u043e\u043c\u0430\u043d\u0434\u0430', form.photo, form.photoMime);
  };
  nameInput.addEventListener('input', () => syncPreviewName());

  const save = async () => {
    const name = nameInput.value.trim();
    if (!name) {
      showErrorToast({ message: '\u041d\u0430\u0437\u0432\u0430\u043d\u0438\u0435 \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c' });
      return;
    }
    const city = cityInput.value.trim() || null;
    let teamId = id;
    if (!teamId) teamId = await createTeam(name);
    await teamChangeGeneralInfo(teamId!, { name, city, photo: form.photo, photoMime: form.photoMime });
    showSuccessToast({
      message: isNew ? '\u041a\u043e\u043c\u0430\u043d\u0434\u0430 \u0441\u043e\u0437\u0434\u0430\u043d\u0430, \u0441\u0432\u0435\u0434\u0435\u043d\u0438\u044f \u0441\u043e\u0445\u0440\u0430\u043d\u0435\u043d\u044b.' : '\u0421\u0432\u0435\u0434\u0435\u043d\u0438\u044f \u043e \u043a\u043e\u043c\u0430\u043d\u0434\u0435 \u0441\u043e\u0445\u0440\u0430\u043d\u0435\u043d\u044b.',
    });
    setDirty(false);
    if (isNew) goSameMode(`/team/${teamId}`);
  };
  root.querySelectorAll<HTMLButtonElement>('[data-save]').forEach((b) => b.addEventListener('click', () => void save()));
}
