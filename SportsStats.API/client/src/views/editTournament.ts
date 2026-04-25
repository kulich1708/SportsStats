import {
	createTournament,
	teamByTournament,
	teamsPaged,
	tournamentFinish,
	tournamentById,
	tournamentChangeGeneralInfo,
	tournamentRegistrationStep,
	tournamentSetRegistrationTeams,
	tournamentSetRules,
	tournamentStart,
} from '../api';
import { goSameMode, urlFor } from '../nav';
import { openModal } from '../ui/modal';
import { clearInvalidHighlightsIn, showErrorToast, showSuccessToast, showValidationToast } from '../ui/errors';
import type { TeamDTO, TournamentRulesDTO } from '../types';
import { dtoImg, el, escapeHtml, fileToBase64, mountPhotoMediaField, photoMediaFieldHtml } from '../ui/utils';

export async function renderEditTournamentPage(root: HTMLElement, idOrNew: string): Promise<void> {
	const isNew = idOrNew === 'new';
	const id = isNew ? null : Number(idOrNew);
	const t = id
		? await tournamentById(id)
		: {
			id: 0,
			name: '',
			photo: null,
			photoMime: null,
			startedAt: null,
			finishedAt: null,
			status: { code: 0, description: 'Начат', nextActionDescription: 'Откыть регистрацию' },
			tournamentRules: null,
			teams: [],
		};

	let general = { name: t.name, photo: t.photo, photoMime: t.photoMime };
	let rules: TournamentRulesDTO | null = t.tournamentRules ? structuredClone(t.tournamentRules) : null;
	const statusCode = t.status.code;
	const persistedTeams = id ? await teamByTournament(id) : [];
	let currentTeams: TeamDTO[] = [...persistedTeams];
	const dirty = { general: false, rules: false, teams: false };
	const setDirty = (k: keyof typeof dirty, v: boolean) => {
		dirty[k] = v;
		root.querySelectorAll<HTMLButtonElement>(`[data-save-${k}]`).forEach((b) => { b.disabled = !dirty[k]; });
		root.querySelectorAll<HTMLButtonElement>('[data-save-all]').forEach((b) => { b.disabled = !(dirty.general || dirty.rules || dirty.teams); });
	};
	const statusControlsVisible = Boolean(id) && statusCode < 3;
	const statusControlsHtml = statusControlsVisible
		? `
      <div class="panel">
        <p class="muted">${escapeHtml(t.status.description)}</p>
        <button type="button" class="action-btn" data-next-status>${escapeHtml(t.status.nextActionDescription)}</button>
      </div>`
		: '';

	const viewLink =
		id !== null
			? `<a class="action-btn action-btn--ghost" href="${urlFor(`/tournament/${id}`, true)}" data-app-link>\u041a \u043f\u0440\u043e\u0441\u043c\u043e\u0442\u0440\u0443 \u0442\u0443\u0440\u043d\u0438\u0440\u0430</a>`
			: '';

	root.replaceChildren(el(`
    <section class="page page--edit-tournament" data-tabs>
      <div class="page-head page-head--row">
        <h1 class="page-title">${isNew ? '\u0421\u043e\u0437\u0434\u0430\u043d\u0438\u0435 \u0442\u0443\u0440\u043d\u0438\u0440\u0430' : '\u0420\u0435\u0434\u0430\u043a\u0442\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u0435 \u0442\u0443\u0440\u043d\u0438\u0440\u0430'}</h1>
        ${viewLink}
      </div>
      ${statusControlsHtml}
      <div class="action-row"><button class="action-btn" data-save-all disabled>Сохранить всё</button></div>
      <div class="tabs"><button type="button" class="tabs__btn tabs__btn--active" data-tab="general">Общая информация</button><button type="button" class="tabs__btn" data-tab="rules">Правила</button><button type="button" class="tabs__btn" data-tab="teams">Заявка команд</button></div>
      <div class="tabs__panel tabs__panel--active" data-panel="general">
        <div class="action-row"><button class="action-btn" data-save-general disabled>Сохранить</button></div>
        <div class="panel">
          <div class="stack-field"><span class="stack-field__label">Название</span><input class="search-input" data-name value="${escapeHtml(general.name)}" /></div>
          <div data-tournament-photo>${photoMediaFieldHtml('Фото', dtoImg(general.name || 'Турнир', general.photo, general.photoMime))}</div>
        </div>
        <div class="action-row"><button class="action-btn" data-save-general disabled>Сохранить</button></div>
      </div>
      <div class="tabs__panel" data-panel="rules" hidden>
        <div class="action-row"><button class="action-btn" data-save-rules ${statusCode !== 0 ? 'disabled' : ''}>Сохранить</button></div>
        <div class="panel" data-rules-host></div>
        <div class="action-row"><button class="action-btn" data-save-rules ${statusCode !== 0 ? 'disabled' : ''}>Сохранить</button></div>
      </div>
      <div class="tabs__panel" data-panel="teams" hidden>
        <div class="action-row"><button class="action-btn" data-save-teams disabled>Сохранить</button></div>
        <div class="panel">
          ${statusCode === 0 ? '<p class="muted">Сначала надо открыть заявку команд</p>' : ''}
          ${statusCode === 1 ? '<button class="action-btn" data-add-team>Добавить команду</button>' : ''}
          <div class="list-wrap" data-teams-list></div>
        </div>
        <div class="action-row"><button class="action-btn" data-save-teams disabled>Сохранить</button></div>
      </div>
    </section>
  `));

	const setupTabs = () => {
		const host = root.querySelector<HTMLElement>('[data-tabs]')!;
		const btns = host.querySelectorAll<HTMLButtonElement>('.tabs__btn');
		const panels = host.querySelectorAll<HTMLElement>('.tabs__panel');
		btns.forEach((btn) => btn.addEventListener('click', () => {
			const tab = btn.dataset.tab;
			btns.forEach((b) => b.classList.toggle('tabs__btn--active', b === btn));
			panels.forEach((p) => {
				const on = p.dataset.panel === tab;
				p.classList.toggle('tabs__panel--active', on);
				if (on) p.removeAttribute('hidden');
				else p.setAttribute('hidden', '');
			});
		}));
	};
	setupTabs();

	const nameInput = root.querySelector<HTMLInputElement>('[data-name]')!;
	nameInput.addEventListener('input', () => {
		setDirty('general', true);
		const pv = photoRoot.querySelector<HTMLElement>('[data-photo-preview]');
		if (pv) pv.innerHTML = dtoImg(general.name || 'Турнир', general.photo, general.photoMime);
	});
	const photoRoot = root.querySelector<HTMLElement>('[data-tournament-photo]')!;
	mountPhotoMediaField(photoRoot, {
		getPreviewHtml: () => dtoImg(general.name || 'Турнир', general.photo, general.photoMime),
		onPickFile: async (file) => {
			general.photo = await fileToBase64(file);
			general.photoMime = file.type;
			setDirty('general', true);
		},
		onClear: () => {
			general.photo = null;
			general.photoMime = null;
			setDirty('general', true);
		},
	});
	const saveGeneral = async () => {
		const name = nameInput.value.trim();
		if (!name) {
			showErrorToast({ message: '\u041d\u0430\u0437\u0432\u0430\u043d\u0438\u0435 \u0442\u0443\u0440\u043d\u0438\u0440\u0430 \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c' });
			return;
		}
		let tournamentId = id;
		const createdNow = !tournamentId;
		if (!tournamentId) tournamentId = await createTournament(name);
		await tournamentChangeGeneralInfo(tournamentId!, { name, photo: general.photo, photoMime: general.photoMime });
		showSuccessToast({
			message: createdNow ? 'Турнир создан, основная информация сохранена.' : 'Основная информация турнира сохранена.',
		});
		setDirty('general', false);
		if (isNew) goSameMode(`/admin/edit/tournament/${tournamentId}`);
	};
	root.querySelectorAll<HTMLButtonElement>('[data-save-general]').forEach((b) => b.addEventListener('click', () => void saveGeneral()));

	const rulesHost = root.querySelector<HTMLElement>('[data-rules-host]')!;
	const renderRules = () => {
		const disabled = statusCode !== 0 ? 'disabled' : '';
		const time = rules?.matchTimeRules;
		const roster = rules?.matchRosterRules;
		const points = rules?.matchPointsRules;
		const hasOvertime = time?.hasOvertime ?? false;
		const hasDraw = time?.isDrawPossible ?? false;

		rulesHost.innerHTML = `
      <div class="rules-block">
        <h3>Длительность матча</h3>
        <div class="grid-2">
          <label class="stack-field">Периодов <input ${disabled} class="search-input" data-r="periodsCount" type="number" value="${time?.periodsCount ?? ''}" /></label>
          <label class="stack-field">Длительность периода (сек) <input ${disabled} class="search-input" data-r="periodDurationSeconds" type="number" value="${time?.periodDurationSeconds ?? ''}" /></label>
        </div>
        <div class="rules-toggle-row">
          <label class="rules-checkbox"><input ${disabled} data-rb="hasOvertime" type="checkbox" ${hasOvertime ? 'checked' : ''} /> Есть овертайм</label>
        </div>
        <div class="rules-collapsible ${hasOvertime ? 'is-open' : ''}" data-overtime-fields><div class="rules-collapsible__inner"><div class="grid-2">
          <label class="stack-field">Количество овертаймов <input ${disabled} class="search-input" data-r="overtimesCount" type="number" value="${time?.overtimeRules?.overtimesCount ?? ''}" /></label>
          <label class="stack-field">Длительность овертайма (сек) <input ${disabled} class="search-input" data-r="overtimeDurationSeconds" type="number" value="${time?.overtimeRules?.overtimeDurationSeconds ?? ''}" /></label>
          <label class="rules-checkbox"><input ${disabled} data-rb="goalEndsOvertime" type="checkbox" ${time?.overtimeRules?.goalEndsOvertime ? 'checked' : ''} /> Гол завершает овертайм</label>
        </div></div></div>
      </div>
      <div class="rules-block">
        <h3>Правила заявки на матч</h3>
        <div class="grid-2">
          <label class="stack-field">Минимум игроков <input ${disabled} class="search-input" data-r="minPlayers" type="number" value="${roster?.minPlayers ?? ''}" /></label>
          <label class="stack-field">Максимум игроков <input ${disabled} class="search-input" data-r="maxPlayers" type="number" value="${roster?.maxPlayers ?? ''}" /></label>
          <label class="stack-field">Минимум нападающих <input ${disabled} class="search-input" data-r="minForwards" type="number" value="${roster?.minForwards ?? ''}" /></label>
          <label class="stack-field">Максимум нападающих <input ${disabled} class="search-input" data-r="maxForwards" type="number" value="${roster?.maxForwards ?? ''}" /></label>
          <label class="stack-field">Минимум защитников <input ${disabled} class="search-input" data-r="minDefensemans" type="number" value="${roster?.minDefensemans ?? ''}" /></label>
          <label class="stack-field">Максимум защитников <input ${disabled} class="search-input" data-r="maxDefensemans" type="number" value="${roster?.maxDefensemans ?? ''}" /></label>
          <label class="stack-field">Минимум вратарей <input ${disabled} class="search-input" data-r="minGoalies" type="number" value="${roster?.minGoalies ?? ''}" /></label>
          <label class="stack-field">Максимум вратарей <input ${disabled} class="search-input" data-r="maxGoalies" type="number" value="${roster?.maxGoalies ?? ''}" /></label>
        </div>
      </div>
      <div class="rules-block">
        <h3>Правила начисления очков</h3>
        <div class="grid-2">
          <label class="stack-field">Очки за победу <input ${disabled} class="search-input" data-r="winPoints" type="number" value="${points?.winPoints ?? ''}" /></label>
          <label class="stack-field">Очки за поражение <input ${disabled} class="search-input" data-r="lossPoints" type="number" value="${points?.lossPoints ?? ''}" /></label>
        </div>
        <div class="rules-collapsible ${hasOvertime ? 'is-open' : ''}" data-ot-points-fields><div class="rules-collapsible__inner"><div class="grid-2">
          <label class="stack-field">Очки за победу в овертайме <input ${disabled} class="search-input" data-r="otWinPoints" type="number" value="${points?.otWinPoints ?? ''}" /></label>
          <label class="stack-field">Очки за поражение в овертайме <input ${disabled} class="search-input" data-r="otLossPoints" type="number" value="${points?.otLossPoints ?? ''}" /></label>
        </div></div></div>
        <div class="rules-toggle-row">
          <label class="rules-checkbox"><input ${disabled} data-rb="hasDraw" type="checkbox" ${hasDraw ? 'checked' : ''} /> Есть ничья</label>
        </div>
        <div class="rules-collapsible ${hasDraw ? 'is-open' : ''}" data-draw-points-field><div class="rules-collapsible__inner"><div class="grid-2">
          <label class="stack-field">Очки за ничью <input ${disabled} class="search-input" data-r="drawPoints" type="number" value="${points?.drawPoints ?? ''}" /></label>
        </div></div></div>
      </div>`;
	};
	renderRules();

	const RULES_VALIDATION_MSG =
		'Проверьте правильность заполнения полей: числовые поля не должны быть пустыми.';

	const setExpanded = (selector: string, expand: boolean) => {
		const target = rulesHost.querySelector<HTMLElement>(selector);
		if (!target) return;
		target.classList.toggle('is-open', expand);
	};

	rulesHost.addEventListener('change', (e) => {
		if (statusCode !== 0) return;
		const target = e.target as HTMLElement | null;
		if (target instanceof HTMLInputElement && target.dataset.rb === 'hasOvertime') {
			setExpanded('[data-overtime-fields]', target.checked);
			setExpanded('[data-ot-points-fields]', target.checked);
		}
		if (target instanceof HTMLInputElement && target.dataset.rb === 'hasDraw') {
			setExpanded('[data-draw-points-field]', target.checked);
		}
		clearInvalidHighlightsIn(rulesHost);
		setDirty('rules', true);
	});

	rulesHost.addEventListener('input', () => {
		if (statusCode !== 0) return;
		clearInvalidHighlightsIn(rulesHost);
		setDirty('rules', true);
	});

	const collectTournamentRulesFromDom = ():
		| { ok: true; value: TournamentRulesDTO }
		| { ok: false; invalid: HTMLElement[] } => {
		const invalid: HTMLElement[] = [];
		const addInv = (el: HTMLElement | null | undefined) => {
			if (el && !invalid.includes(el)) invalid.push(el);
		};

		const qi = (key: string) => rulesHost.querySelector<HTMLInputElement>(`[data-r="${key}"]`);
		const qb = (key: string) => rulesHost.querySelector<HTMLInputElement>(`[data-rb="${key}"]`);

		const readBool = (key: string): boolean => qb(key)?.checked ?? false;

		const readRequiredInt = (key: string): number | null => {
			const el = qi(key);
			const raw = el?.value.trim() ?? '';
			if (!el || raw === '') {
				addInv(el);
				return null;
			}
			const n = Number(raw);
			if (Number.isNaN(n)) {
				addInv(el);
				return null;
			}
			return n;
		};

		const hasOvertime = readBool('hasOvertime');
		const hasDraw = readBool('hasDraw');

		const periodsCount = readRequiredInt('periodsCount');
		const periodDurationSeconds = readRequiredInt('periodDurationSeconds');

		const maxPlayers = readRequiredInt('maxPlayers');
		const minPlayers = readRequiredInt('minPlayers');
		const minForwards = readRequiredInt('minForwards');
		const maxForwards = readRequiredInt('maxForwards');
		const minDefensemans = readRequiredInt('minDefensemans');
		const maxDefensemans = readRequiredInt('maxDefensemans');
		const minGoalies = readRequiredInt('minGoalies');
		const maxGoalies = readRequiredInt('maxGoalies');

		const winPoints = readRequiredInt('winPoints');
		const lossPoints = readRequiredInt('lossPoints');

		let overtimesCount: number | null = null;
		let overtimeDurationSeconds: number | null = null;
		let goalEndsOvertime = false;
		let otWinPoints: number | null = null;
		let otLossPoints: number | null = null;

		if (hasOvertime) {
			overtimesCount = readRequiredInt('overtimesCount');
			overtimeDurationSeconds = readRequiredInt('overtimeDurationSeconds');
			goalEndsOvertime = readBool('goalEndsOvertime');
			otWinPoints = readRequiredInt('otWinPoints');
			otLossPoints = readRequiredInt('otLossPoints');
		}

		let drawPoints: number | null = null;
		if (hasDraw) {
			drawPoints = readRequiredInt('drawPoints');
		}

		if (invalid.length) return { ok: false, invalid };

		return {
			ok: true,
			value: {
				matchTimeRules: {
					periodsCount: periodsCount!,
					periodDurationSeconds: periodDurationSeconds!,
					isDrawPossible: hasDraw,
					hasOvertime,
					hasShootout: false,
					overtimeRules: hasOvertime
						? {
							overtimesCount,
							overtimeDurationSeconds,
							goalEndsOvertime,
						}
						: null,
					shootoutRules: null,
				},
				matchRosterRules: {
					maxPlayers: maxPlayers!,
					minPlayers: minPlayers!,
					minForwards: minForwards!,
					maxForwards: maxForwards!,
					minDefensemans: minDefensemans!,
					maxDefensemans: maxDefensemans!,
					minGoalies: minGoalies!,
					maxGoalies: maxGoalies!,
				},
				matchPointsRules: {
					winPoints: winPoints!,
					lossPoints: lossPoints!,
					otWinPoints,
					otLossPoints,
					shootoutWinPoints: null,
					shootoutLossPoints: null,
					drawPoints,
				},
			},
		};
	};

	const saveRules = async (): Promise<boolean> => {
		if (!id) return true;
		const built = collectTournamentRulesFromDom();
		if (!built.ok) {
			showValidationToast({ message: RULES_VALIDATION_MSG, highlightInputs: built.invalid });
			return false;
		}
		rules = built.value;
		await tournamentSetRules(id, built.value);
		showSuccessToast({ message: 'Правила турнира сохранены.' });
		setDirty('rules', false);
		return true;
	};
	root.querySelectorAll<HTMLButtonElement>('[data-save-rules]').forEach((b) =>
		b.addEventListener('click', () => void saveRules()),
	);

	const listHost = root.querySelector<HTMLElement>('[data-teams-list]')!;
	const renderTeams = () => {
		listHost.innerHTML = currentTeams
			.map((x) => `<article class="list-row">
          <div class="list-row__lead">
            <span class="entity-link">${dtoImg(x.name, x.photo, x.photoMime, 'mini-photo')}<span>${escapeHtml(x.name)}</span></span>
            <span class="list-row__meta muted">${escapeHtml(x.city ?? '—')}</span>
          </div>
          ${statusCode === 1 ? `<button type="button" class="icon-btn" data-remove-team="${x.id}" title="Удалить из заявки">🗑</button>` : ''}
        </article>`)
			.join('');
	};
	renderTeams();

	listHost.addEventListener('click', (e) => {
		if (statusCode !== 1) return;
		const btn = (e.target as HTMLElement).closest<HTMLButtonElement>('[data-remove-team]');
		if (!btn) return;
		const teamId = Number(btn.dataset.removeTeam);
		currentTeams = currentTeams.filter((x) => x.id !== teamId);
		renderTeams();
		setDirty('teams', true);
	});


	root.querySelector<HTMLButtonElement>('[data-add-team]')?.addEventListener('click', async () => {
		if (statusCode !== 1) return;
		const modal = openModal('Регистрация команд', `
      <div class="action-row"><button class="action-btn" data-apply-top>Сохранить</button></div>
      <div class="panel">
        <div class="filters-row">
        <input class="search-input" data-team-search placeholder="Поиск команд" />
        </div>
        <div class="list-wrap" data-team-picker-list style="min-height: 220px"></div>
        <div class="action-row"><button class="action-btn action-btn--ghost" type="button" data-team-more>Показать ещё</button></div>
      </div>
      <div class="action-row"><button class="action-btn" data-apply>Сохранить</button></div>`, { wide: true });

		const PAGE_SIZE = 40;
		const selected = new Set(currentTeams.map((x) => x.id));
		const list = modal.body.querySelector<HTMLElement>('[data-team-picker-list]')!;
		const searchInput = modal.body.querySelector<HTMLInputElement>('[data-team-search]')!;
		const moreBtn = modal.body.querySelector<HTMLButtonElement>('[data-team-more]')!;
		const applyTopBtn = modal.body.querySelector<HTMLButtonElement>('[data-apply-top]')!;
		const applyBtn = modal.body.querySelector<HTMLButtonElement>('[data-apply]')!;
		let page = 1;
		let search = '';
		let hasMore = true;
		const loaded = new Map<number, TeamDTO>();
		let reqId = 0;

		const upsertRows = (rows: TeamDTO[], append: boolean) => {
			if (!append) list.innerHTML = '';
			for (const team of rows) {
				loaded.set(team.id, team);
				list.insertAdjacentHTML('beforeend', `<label class="list-row">
            <span class="entity-link">
              <input type="checkbox" data-team-check="${team.id}" ${selected.has(team.id) ? 'checked' : ''} />
              ${dtoImg(team.name, team.photo, team.photoMime, 'mini-photo')}
              <span>${escapeHtml(team.name)}</span>
            </span>
          </label>`);
			}
		};

		const loadPage = async (append: boolean) => {
			const currentReq = ++reqId;
			moreBtn.disabled = true;
			const hadRows = Boolean(list.querySelector('[data-team-check]'));
			if (!append) {
				if (hadRows) {
					list.style.opacity = '0.55';
					list.style.pointerEvents = 'none';
				} else {
					list.innerHTML = '<p class="muted">Загрузка...</p>';
				}
			}
			const rows = await teamsPaged(page, PAGE_SIZE, search);
			if (currentReq !== reqId) return;
			if (!append && hadRows) {
				list.style.opacity = '';
				list.style.pointerEvents = '';
			}
			hasMore = rows.length === PAGE_SIZE;
			upsertRows(rows, append);
			if (!rows.length && !append) list.innerHTML = '<p class="muted">Ничего не найдено</p>';
			moreBtn.disabled = !hasMore;
			moreBtn.hidden = !hasMore;
		};

		const reload = async () => {
			page = 1;
			hasMore = true;
			await loadPage(false);
		};

		let searchDebounce = 0;
		searchInput.addEventListener('input', () => {
			const q = searchInput.value.trim();
			if (searchDebounce) window.clearTimeout(searchDebounce);
			searchDebounce = window.setTimeout(() => {
				searchDebounce = 0;
				search = q;
				void reload();
			}, 300);
		});

		moreBtn.addEventListener('click', () => {
			if (!hasMore) return;
			page += 1;
			void loadPage(true);
		});

		list.addEventListener('change', (e) => {
			const target = e.target as HTMLInputElement;
			if (!target.matches('[data-team-check]')) return;
			const teamId = Number(target.dataset.teamCheck);
			if (target.checked) selected.add(teamId);
			else selected.delete(teamId);
		});

		const applySelection = () => {
			const persistedMap = new Map(persistedTeams.map((x) => [x.id, x] as const));
			const nextTeams: TeamDTO[] = [];
			for (const teamId of selected) {
				const loadedTeam = loaded.get(teamId) ?? persistedMap.get(teamId);
				if (loadedTeam) nextTeams.push(loadedTeam);
			}
			currentTeams = nextTeams;
			renderTeams();
			setDirty('teams', true);
			modal.close();
		};

		applyTopBtn.addEventListener('click', applySelection);
		applyBtn.addEventListener('click', applySelection);

		void reload();
	});

	const saveTeams = async () => {
		if (!id || statusCode !== 1) return;
		await tournamentSetRegistrationTeams(id, currentTeams.map((x) => x.id));
		showSuccessToast({ message: 'Список команд турнира сохранён.' });
		setDirty('teams', false);
	};
	root.querySelectorAll<HTMLButtonElement>('[data-save-teams]').forEach((b) => b.addEventListener('click', () => void saveTeams()));

	root.querySelectorAll<HTMLButtonElement>('[data-next-status]').forEach((btn) => {
		btn.addEventListener('click', async () => {
			if (!id) return;
			btn.disabled = true;
			try {
				if (statusCode === 0) {
					await tournamentRegistrationStep(id);
					showSuccessToast({ message: 'Регистрация команд на турнир открыта.' });
				} else if (statusCode === 1) {
					await tournamentStart(id);
					showSuccessToast({ message: 'Турнир начат.' });
				} else if (statusCode === 2) {
					await tournamentFinish(id);
					showSuccessToast({ message: 'Турнир завершён.' });
				}
				goSameMode(`/admin/edit/tournament/${id}`);
			} catch (e: unknown) {
				const msg = e instanceof Error ? e.message : String(e);
				showErrorToast({ message: msg });
			} finally {
				btn.disabled = false;
			}
		});
	});

	root.querySelector<HTMLButtonElement>('[data-save-all]')?.addEventListener('click', async () => {
		if (dirty.general) await saveGeneral();
		if (dirty.rules) {
			const okRules = await saveRules();
			if (!okRules) return;
		}
		if (dirty.teams) await saveTeams();
	});
}
