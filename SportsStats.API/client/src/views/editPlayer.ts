import { createPlayer, playerById, playerChangeGeneralInfo, playerPositions, teamsPaged } from '../api';
import { goSameMode, urlFor } from '../nav';
import { showErrorToast, showSuccessToast } from '../ui/errors';
import { mountEntitySelect } from '../ui/entitySelect';
import type { EntityItem } from '../ui/entitySelect';
import { fileToBase64, dtoImg, el, escapeHtml, mountPhotoMediaField, photoMediaFieldHtml } from '../ui/utils';

interface PositionItem {
	code: string;
	name: string;
}

export async function renderEditPlayerPage(root: HTMLElement, idOrNew: string): Promise<void> {
	const isNew = idOrNew === 'new';
	const id = isNew ? null : Number(idOrNew);
	const p = id
		? await playerById(id)
		: {
			id: 0,
			name: '',
			surname: '',
			position: { code: 'Forward', name: '\u041d\u0430\u043f\u0430\u0434\u0430\u044e\u0449\u0438\u0439' },
			teamId: null,
			teamName: null,
			photo: null,
			photoMime: null,
			citizenship: null,
			birthday: null,
			number: null,
		};

	const positionsMap = await playerPositions();
	const positions: PositionItem[] = Object.entries(positionsMap).map(([code, name]) => ({ code, name }));
	const positionCodes = positions.map((x) => x.code);
	const normalizePositionCode = (rawCode: unknown, rawName: unknown): string => {
		if (typeof rawCode === 'string' && positionCodes.includes(rawCode)) return rawCode;
		if (typeof rawCode === 'number' && rawCode >= 0 && rawCode < positionCodes.length) return positionCodes[rawCode];
		if (typeof rawName === 'string') {
			const byName = positions.find((x) => x.name === rawName);
			if (byName) return byName.code;
		}
		return positionCodes[0] ?? 'Forward';
	};

	let photo = p.photo;
	let photoMime = p.photoMime;
	let citizenshipPhoto = p.citizenship?.photo ?? null;
	let citizenshipPhotoMime = p.citizenship?.photoMime ?? null;
	let dirty = false;
	let selectedPositionCode = normalizePositionCode(
		(p as { position?: { code?: unknown; name?: unknown } }).position?.code,
		(p as { position?: { code?: unknown; name?: unknown } }).position?.name,
	);
	let selectedTeamId: number | null = p.teamId ?? null;

	const setDirty = (v: boolean) => {
		dirty = v;
		root.querySelectorAll<HTMLButtonElement>('[data-save]').forEach((b) => {
			b.disabled = !dirty;
		});
	};

	const viewLink =
		id !== null
			? `<a class="action-btn action-btn--ghost" href="${urlFor(`/player/${id}`, true)}" data-app-link>\u041a \u043f\u0440\u043e\u0441\u043c\u043e\u0442\u0440\u0443 \u0438\u0433\u0440\u043e\u043a\u0430</a>`
			: '';

	root.replaceChildren(
		el(`
    <section class="page">
      <div class="page-head page-head--row">
        <h1 class="page-title">${isNew ? '\u0421\u043e\u0437\u0434\u0430\u043d\u0438\u0435 \u0438\u0433\u0440\u043e\u043a\u0430' : '\u0420\u0435\u0434\u0430\u043a\u0442\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u0435 \u0438\u0433\u0440\u043e\u043a\u0430'}</h1>
        ${viewLink}
      </div>
      <div class="action-row"><button class="action-btn" data-save disabled>\u0421\u043e\u0445\u0440\u0430\u043d\u0438\u0442\u044c</button></div>
      <div class="panel">
        <div class="stack-field"><span class="stack-field__label">\u0418\u043c\u044f</span><input class="search-input" data-name value="${escapeHtml(p.name)}" /></div>
        <div class="stack-field"><span class="stack-field__label">\u0424\u0430\u043c\u0438\u043b\u0438\u044f</span><input class="search-input" data-surname value="${escapeHtml(p.surname)}" /></div>
        <div class="stack-field"><span class="stack-field__label">\u0410\u043c\u043f\u043b\u0443\u0430</span><div data-position-select></div></div>
        <div class="stack-field"><span class="stack-field__label">\u041d\u043e\u043c\u0435\u0440</span><input class="search-input" type="number" data-number value="${p.number ?? ''}" /></div>
        <div class="stack-field"><span class="stack-field__label">\u0414\u0430\u0442\u0430 \u0440\u043e\u0436\u0434\u0435\u043d\u0438\u044f</span><input class="search-input" type="date" data-birthday value="${p.birthday ?? ''}" /></div>
        <div class="stack-field"><span class="stack-field__label">\u041a\u043e\u043c\u0430\u043d\u0434\u0430</span><div data-team-select></div></div>

        <div data-player-photo>${photoMediaFieldHtml('\u0424\u043e\u0442\u043e', dtoImg(`${p.name} ${p.surname}`.trim() || '\u0418\u0433\u0440\u043e\u043a', p.photo, p.photoMime, 'media-block__img player-cards__img--round'))}</div>

        <div class="stack-field"><span class="stack-field__label">\u0413\u0440\u0430\u0436\u0434\u0430\u043d\u0441\u0442\u0432\u043e</span><input class="search-input" data-cit-name value="${escapeHtml(p.citizenship?.name ?? '')}" /></div>
        <div data-cit-photo>${photoMediaFieldHtml('\u0424\u043e\u0442\u043e \u0433\u0440\u0430\u0436\u0434\u0430\u043d\u0441\u0442\u0432\u0430', dtoImg(p.citizenship?.name ?? '\u0413\u0440\u0430\u0436\u0434\u0430\u043d\u0441\u0442\u0432\u043e', p.citizenship?.photo ?? null, p.citizenship?.photoMime ?? null, 'media-block__img'))}</div>

      </div>
      <div class="action-row"><button class="action-btn" data-save disabled>\u0421\u043e\u0445\u0440\u0430\u043d\u0438\u0442\u044c</button></div>
    </section>
  `),
	);

	const name = root.querySelector<HTMLInputElement>('[data-name]')!;
	const surname = root.querySelector<HTMLInputElement>('[data-surname]')!;
	const positionHost = root.querySelector<HTMLElement>('[data-position-select]')!;
	const number = root.querySelector<HTMLInputElement>('[data-number]')!;
	const birthday = root.querySelector<HTMLInputElement>('[data-birthday]')!;
	const teamHost = root.querySelector<HTMLElement>('[data-team-select]')!;
	const citName = root.querySelector<HTMLInputElement>('[data-cit-name]')!;
	const playerPhotoRoot = root.querySelector<HTMLElement>('[data-player-photo]')!;
	const citPhotoRoot = root.querySelector<HTMLElement>('[data-cit-photo]')!;

	const positionItems: EntityItem[] = positions.map((pos, idx) => ({
		id: idx + 1,
		name: pos.name,
		sub: null,
	}));

	const initialPosIdx = Math.max(0, positionCodes.indexOf(selectedPositionCode));
	const initialPositionItems: EntityItem[] = positionItems.filter((_, i) => i === initialPosIdx);

	mountEntitySelect(positionHost, {
		placeholder: '\u0410\u043c\u043f\u043b\u0443\u0430',
		multiple: false,
		showMore: false,
		searchable: false,
		initialSelected: initialPositionItems.length ? initialPositionItems : [],
		load: async () => [...positionItems],
		onChange: (sel) => {
			const one = sel[0];
			if (!one) return;
			const idx = one.id - 1;
			const code = positions[idx]?.code;
			if (code) selectedPositionCode = code;
			setDirty(true);
		},
	});

	const teamInitial: EntityItem[] =
		p.teamId != null
			? [{ id: p.teamId, name: p.teamName ?? '', sub: null, photo: null, photoMime: null }]
			: [];

	mountEntitySelect(teamHost, {
		placeholder: '\u041a\u043e\u043c\u0430\u043d\u0434\u0430',
		multiple: false,
		pageSize: 40,
		initialSelected: teamInitial,
		load: async ({ page, pageSize, search }) => {
			const rows = await teamsPaged(page, pageSize, search);
			return rows.map((t) => ({
				id: t.id,
				name: t.name,
				sub: t.city,
				photo: t.photo,
				photoMime: t.photoMime,
			}));
		},
		onChange: (sel) => {
			selectedTeamId = sel[0]?.id ?? null;
			setDirty(true);
		},
	});

	const refreshPlayerPhotoPreview = () => {
		const pv = playerPhotoRoot.querySelector<HTMLElement>('[data-photo-preview]');
		if (pv) pv.innerHTML = dtoImg(`${name.value.trim()} ${surname.value.trim()}`.trim() || '\u0418\u0433\u0440\u043e\u043a', photo, photoMime, 'media-block__img player-cards__img--round');
	};
	const refreshCitizenshipPhotoPreview = () => {
		const pv = citPhotoRoot.querySelector<HTMLElement>('[data-photo-preview]');
		if (pv) pv.innerHTML = dtoImg(citName.value.trim() || '\u0413\u0440\u0430\u0436\u0434\u0430\u043d\u0441\u0442\u0432\u043e', citizenshipPhoto, citizenshipPhotoMime, 'media-block__img');
	};

	mountPhotoMediaField(playerPhotoRoot, {
		getPreviewHtml: () => dtoImg(`${name.value.trim()} ${surname.value.trim()}`.trim() || '\u0418\u0433\u0440\u043e\u043a', photo, photoMime, 'media-block__img player-cards__img--round'),
		onPickFile: async (f) => {
			photo = await fileToBase64(f);
			photoMime = f.type;
			setDirty(true);
		},
		onClear: () => {
			photo = null;
			photoMime = null;
			setDirty(true);
		},
	});

	mountPhotoMediaField(citPhotoRoot, {
		getPreviewHtml: () => dtoImg(citName.value.trim() || '\u0413\u0440\u0430\u0436\u0434\u0430\u043d\u0441\u0442\u0432\u043e', citizenshipPhoto, citizenshipPhotoMime, 'media-block__img'),
		onPickFile: async (f) => {
			citizenshipPhoto = await fileToBase64(f);
			citizenshipPhotoMime = f.type;
			setDirty(true);
		},
		onClear: () => {
			citizenshipPhoto = null;
			citizenshipPhotoMime = null;
			setDirty(true);
		},
	});

	[name, surname, number, birthday, citName].forEach((inp) =>
		inp.addEventListener('input', () => {
			setDirty(true);
			if (inp === name || inp === surname) refreshPlayerPhotoPreview();
			if (inp === citName) refreshCitizenshipPhotoPreview();
		}),
	);

	const save = async () => {
		if (!name.value.trim() || !surname.value.trim()) {
			showErrorToast({ message: '\u0418\u043c\u044f \u0438 \u0444\u0430\u043c\u0438\u043b\u0438\u044f \u043e\u0431\u044f\u0437\u0430\u0442\u0435\u043b\u044c\u043d\u044b \u0434\u043b\u044f \u0441\u043e\u0445\u0440\u0430\u043d\u0435\u043d\u0438\u044f.' });
			return;
		}

		const selectedPositionValue = Math.max(0, positionCodes.indexOf(selectedPositionCode));

		let playerId = id;
		if (!playerId) {
			playerId = await createPlayer({ name: name.value.trim(), surname: surname.value.trim(), position: selectedPositionValue });
		}

		const data = {
			name: name.value.trim(),
			surname: surname.value.trim(),
			position: selectedPositionValue,
			teamId: selectedTeamId,
			number: number.value ? Number(number.value) : null,
			birthday: birthday.value || null,
			citizenship: citName.value.trim()
				? { name: citName.value.trim(), photo: citizenshipPhoto, photoMime: citizenshipPhotoMime }
				: null,
			photo,
			photoMime,
		};
		await playerChangeGeneralInfo(playerId!, data);
		showSuccessToast({
			message: isNew ? '\u0418\u0433\u0440\u043e\u043a \u0441\u043e\u0437\u0434\u0430\u043d, \u0441\u0432\u0435\u0434\u0435\u043d\u0438\u044f \u0441\u043e\u0445\u0440\u0430\u043d\u0435\u043d\u044b.' : '\u0421\u0432\u0435\u0434\u0435\u043d\u0438\u044f \u043e\u0431 \u0438\u0433\u0440\u043e\u043a\u0435 \u0441\u043e\u0445\u0440\u0430\u043d\u0435\u043d\u044b.',
		});

		setDirty(false);
		if (isNew) goSameMode(`/player/${playerId}`);
	};

	root.querySelectorAll<HTMLButtonElement>('[data-save]').forEach((b) => b.addEventListener('click', () => void save()));
}
