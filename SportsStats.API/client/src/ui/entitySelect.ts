import { citizenshipBadgeHtml, dtoImg, el, escapeHtml } from './utils';

let entitySelectRadioSeq = 0;

export interface EntityItem {
	id: number;
	name: string;
	sub?: string | null;
	photo?: string | null;
	photoMime?: string | null;
	citizenship?: import('../types').CitizenshipDTO | null;
}

type LoadFn = (args: { page: number; pageSize: number; search: string }) => Promise<EntityItem[]>;

export function mountEntitySelect(
	host: HTMLElement,
	options: {
		placeholder: string;
		multiple: boolean;
		pageSize?: number;
		load: LoadFn;
		initialSelected?: EntityItem[];
		onChange?: (selected: EntityItem[]) => void;
		/** По умолчанию true. Если false — кнопка «показать ещё» скрыта (для локальных коротких списков). */
		showMore?: boolean;
		/** When false, the trigger is readonly and the list is not filtered by typing. */
		searchable?: boolean;
	},
): { getSelected: () => EntityItem[]; setSelected: (items: EntityItem[]) => void; reload: () => Promise<void> } {
	const pageSize = options.pageSize ?? 40;
	const radioGroupId = ++entitySelectRadioSeq;
	const showMore = options.showMore !== false;
	const searchable = options.searchable !== false;
	let open = false;
	let page = 1;
	let search = '';
	let loaded: EntityItem[] = [];
	let selected = options.initialSelected ? [...options.initialSelected] : [];

	host.replaceChildren(el(`
    <div class="entity-select" data-es>
      <div class="entity-select__trigger-wrap">
        <span class="entity-select__trigger-thumb" data-es-trigger-thumb aria-hidden="true"></span>
        <input class="entity-select__trigger" data-es-trigger ${searchable ? '' : 'readonly'} />
      </div>
      <div class="entity-select__menu" data-es-menu>
        <div class="entity-select__list" data-es-list></div>
        ${showMore ? '<button type="button" class="action-btn action-btn--ghost" data-es-more>Показать ещё</button>' : ''}
      </div>
    </div>
  `));

	const root = host.querySelector<HTMLElement>('[data-es]')!;
	const trigger = host.querySelector<HTMLInputElement>('[data-es-trigger]')!;
	const thumb = host.querySelector<HTMLElement>('[data-es-trigger-thumb]');
	const list = host.querySelector<HTMLElement>('[data-es-list]')!;
	const more = host.querySelector<HTMLButtonElement>('[data-es-more]');

	trigger.placeholder = options.placeholder;

	const renderTrigger = () => {
		const one = selected.length === 1 ? selected[0] : null;
		if (thumb) {
			if (open || !one?.photo) thumb.innerHTML = '';
			else thumb.innerHTML = dtoImg(one.name, one.photo, one.photoMime, 'entity-select__thumb-img');
		}
		if (selected.length === 0) {
			if (!open) trigger.value = '';
			return;
		}
		if (!open) trigger.value = selected.map((x) => x.name).join(', ');
	};

	const isSelected = (id: number) => selected.some((x) => x.id === id);

	const renderList = () => {
		list.innerHTML = loaded
			.map((x) => `
        <label class="entity-select__item">
          <input type="${options.multiple ? 'checkbox' : 'radio'}" name="entity-select-${radioGroupId}" value="${x.id}" ${isSelected(x.id) ? 'checked' : ''} />
          ${dtoImg(x.name, x.photo, x.photoMime, 'entity-select__photo')}
          <span>
            <strong>${escapeHtml(x.name)}</strong>${citizenshipBadgeHtml(x.citizenship ?? null)}
            ${x.sub ? `<small class="muted">${escapeHtml(x.sub)}</small>` : ''}
          </span>
        </label>
      `)
			.join('');
	};

	const loadPage = async (reset: boolean) => {
		if (reset) {
			page = 1;
			loaded = [];
		}

		const effectiveSearch = searchable ? search : '';
		const chunk = await options.load({ page, pageSize, search: effectiveSearch });
		loaded = reset ? chunk : [...loaded, ...chunk];
		renderList();
	};

	const openMenu = () => {
		if (open) return;
		open = true;
		root.classList.add('is-open');
		if (searchable) {
			trigger.value = search;
			trigger.focus();
		} else {
			trigger.focus();
		}
		renderTrigger();
	};

	const closeMenu = () => {
		if (!open) return;
		open = false;
		root.classList.remove('is-open');
		search = '';
		renderTrigger();
	};

	const syncSelectionFromInputs = () => {
		const checked = list.querySelectorAll<HTMLInputElement>('input:checked');

		if (options.multiple) {
			selected = loaded.filter((x) => Array.from(checked).some((c) => Number(c.value) === x.id));
		} else {
			const one = checked[0];
			selected = one ? loaded.filter((x) => x.id === Number(one.value)) : [];
			closeMenu();
		}

		renderTrigger();
		options.onChange?.(selected);
	};

	trigger.addEventListener('click', () => {
		openMenu();
	});

	let timer = 0;
	trigger.addEventListener('input', () => {
		if (!open || !searchable) return;

		if (timer) window.clearTimeout(timer);
		timer = window.setTimeout(async () => {
			search = trigger.value.trim();
			await loadPage(true);
		}, 300);
	});

	list.addEventListener('change', syncSelectionFromInputs);

	more?.addEventListener('click', async () => {
		page += 1;
		await loadPage(false);
	});

	document.addEventListener('click', (e) => {
		if (!open) return;

		const target = e.target as Node | null;
		if (target && root.contains(target)) return;

		closeMenu();
	});

	void loadPage(true);
	renderTrigger();

	return {
		getSelected: () => [...selected],
		setSelected: (items) => {
			selected = [...items];
			renderTrigger();
			renderList();
			options.onChange?.(selected);
		},
		reload: () => loadPage(true),
	};
}
