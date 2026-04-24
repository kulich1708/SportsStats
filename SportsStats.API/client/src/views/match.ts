import { addGoal, fillGoal, finishMatch, matchById, startMatch } from '../api';
import { urlFor } from '../nav';
import type { GoalDTO, MatchDTO, PlayerDTO } from '../types';
import { citizenshipBadgeHtml, dtoImg, el, escapeHtml, formatGoalTime, formatTimeOnly } from '../ui/utils';
import { openModal } from '../ui/modal';
import { showErrorToast, showSuccessToast } from '../ui/errors';
import { mountEntitySelect, type EntityItem } from '../ui/entitySelect';

function periodTitle(period: number, periodsCount: number): string {
  if (period <= periodsCount) return `${period}-й период`;
  const otIndex = period - periodsCount;
  return `${otIndex}-й овертайм`;
}

function mergeDisplayedPeriods(match: MatchDTO, periodsCount: number, goals: GoalDTO[]): number[] {
  const periods = new Set<number>(Array.from({ length: periodsCount }, (_, i) => i + 1));
  goals.forEach((g) => periods.add(g.period));

  const tr = match.rules?.matchTimeRules;
  const ot = tr?.overtimeRules;
  if (tr?.hasOvertime && ot) {
    const otGoalPeriods = goals.map((g) => g.period).filter((pp) => pp > periodsCount);
    if (otGoalPeriods.length > 0) {
      const maxFromGoals = Math.max(...otGoalPeriods);
      let hi: number;
      if (ot.goalEndsOvertime) {
        hi = maxFromGoals;
      } else {
        const cap = ot.overtimesCount != null ? periodsCount + ot.overtimesCount : maxFromGoals;
        hi = Math.max(cap, maxFromGoals);
      }
      for (let pp = periodsCount + 1; pp <= hi; pp++) periods.add(pp);
    }
  }

  return [...periods].sort((a, b) => a - b);
}

type StrengthForm = 'EvenStrength' | 'PowerPlay' | 'Shorthanded';

const STRENGTH_FROM_API: Record<string, StrengthForm> = {
  'В равных составах': 'EvenStrength',
  'В большинстве': 'PowerPlay',
  'В меньшенстве': 'Shorthanded',
  EvenStrength: 'EvenStrength',
  PowerPlay: 'PowerPlay',
  Shorthanded: 'Shorthanded',
};

function strengthFromGoalApi(text: string | undefined): StrengthForm {
  const t = (text ?? '').trim();
  if (!t) return 'EvenStrength';
  const mapped = STRENGTH_FROM_API[t];
  if (mapped) return mapped;
  const low = t.toLowerCase();
  if (low === 'evenstrength' || t === '0') return 'EvenStrength';
  if (low === 'powerplay' || t === '1') return 'PowerPlay';
  if (low === 'shorthanded' || t === '2') return 'Shorthanded';
  return 'EvenStrength';
}

function emptyNetFromGoalApi(text: string | undefined): boolean {
  const t = (text ?? '').trim();
  if (!t) return false;
  if (t === 'В пустые ворота') return true;
  const low = t.toLowerCase();
  if (low === 'emptynet' || t === '0') return true;
  return false;
}

function sortGoals(goals: GoalDTO[]): GoalDTO[] {
  return [...goals].sort((a, b) => a.period - b.period || a.time - b.time || (a.id ?? 0) - (b.id ?? 0));
}

function scoreAfterGoal(match: MatchDTO): Map<string, string> {
  const key = (g: GoalDTO) => String(g.id);
  const map = new Map<string, string>();
  let h = 0;
  let a = 0;
  for (const g of sortGoals(match.goals)) {
    if (g.scoringTeamId.id === match.homeTeam.id) h += 1;
    else a += 1;
    map.set(key(g), `${h}:${a}`);
  }
  return map;
}

function setupTabs(host: HTMLElement | null): void {
  if (!host) return;
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
}

export async function renderMatchPage(root: HTMLElement, id: number, admin: boolean): Promise<void> {
  const match = await matchById(id);
  const periodsCount = match.rules?.matchTimeRules?.periodsCount ?? 3;
  const scoreMap = scoreAfterGoal(match);
  const goals = sortGoals(match.goals);
  const goalKey = (g: GoalDTO) => String(g.id);
  const periods = mergeDisplayedPeriods(match, periodsCount, goals);

  root.replaceChildren(
    el(`
      <section class="page page--match" data-tabs>
        <div class="match-hero">
          <div class="match-hero__side">
            <a href="${urlFor(`/team/${match.homeTeam.id}`, admin)}" data-app-link>${dtoImg(match.homeTeam.name, match.homeTeam.photo, match.homeTeam.photoMime)}</a>
            <a class="match-hero__team" href="${urlFor(`/team/${match.homeTeam.id}`, admin)}" data-app-link>${escapeHtml(match.homeTeam.name)}</a>
          </div>
          <div class="match-hero__center">
            <div class="match-hero__score">${match.homeTeamScore} : ${match.awayTeamScore}</div>
            ${match.isOvertime ? '<div class="badge">ОТ</div>' : ''}
            <div class="match-hero__time">${escapeHtml(formatTimeOnly(match.scheduleAt))}</div>
            <div class="match-hero__status">${escapeHtml(match.status.description)}</div>
            <a class="muted-link" href="${urlFor(`/tournament/${match.tournament.id}`, admin)}" data-app-link>${escapeHtml(match.tournament.name)}</a>
          </div>
          <div class="match-hero__side">
            <a href="${urlFor(`/team/${match.awayTeam.id}`, admin)}" data-app-link>${dtoImg(match.awayTeam.name, match.awayTeam.photo, match.awayTeam.photoMime)}</a>
            <a class="match-hero__team" href="${urlFor(`/team/${match.awayTeam.id}`, admin)}" data-app-link>${escapeHtml(match.awayTeam.name)}</a>
          </div>
        </div>
        ${admin ? `<div class="action-row">
          ${match.status.code === 0 ? `<a class="action-btn" href="${urlFor(`/admin/edit/match/${id}`, true)}" data-app-link>Редактировать матч</a>` : ''}
          ${match.status.code < 2 ? `<button type="button" class="action-btn" data-next-status>${escapeHtml(match.status.nextActionDescription)}</button>` : ''}
          ${match.status.code === 1 ? `<button type="button" class="action-btn" data-add-goal>Добавить гол</button>` : ''}
        </div>` : ''}
        <div class="tabs"><button type="button" class="tabs__btn tabs__btn--active" data-tab="overview">Обзор</button><button type="button" class="tabs__btn" data-tab="rosters">Составы</button></div>
        <div class="tabs__panel tabs__panel--active" data-panel="overview">
          <div class="periods">
            ${periods.map((p) => {
              const pg = goals.filter((g) => g.period === p);
              if (pg.length === 0) return `<section class="period-block"><h3 class="period-block__title">${periodTitle(p, periodsCount)}</h3><p class="period-block__empty">—</p></section>`;
              return `<section class="period-block"><h3 class="period-block__title">${periodTitle(p, periodsCount)}</h3>${pg.map((g) => `
                <article class="goal-line">
                  <div class="goal-line__row"><span class="goal-line__time">${formatGoalTime(g.time)}</span><span class="goal-line__score">${scoreMap.get(goalKey(g)) ?? ''}</span>
                    <span class="goal-line__scorer"><a href="${urlFor(`/player/${g.goalScorerId.id}`, admin)}" data-app-link>${escapeHtml(`${g.goalScorerId.name} ${g.goalScorerId.surname}`)}</a>${citizenshipBadgeHtml(g.goalScorerId.citizenship ?? null)}</span>
                    ${admin ? `<button type="button" class="icon-btn" data-goal-id="${g.id}" title="Редактировать гол">✎</button>` : ''}
                  </div>
                  <div class="goal-line__detail">${escapeHtml([g.strengthType, g.netType].filter(Boolean).join(' · '))}</div>
                  <div class="goal-line__assists">${g.firstAssistId || g.secondAssistId ? `Ассистенты: ${[g.firstAssistId, g.secondAssistId].filter(Boolean).map((x) => `${x!.name} ${x!.surname}`).map(escapeHtml).join(', ')}` : ''}</div>
                </article>`).join('')}</section>`;
            }).join('')}
          </div>
        </div>
        <div class="tabs__panel" data-panel="rosters" hidden>
          <div class="roster-grid">
            <div class="roster-col"><h3>${escapeHtml(match.homeTeam.name)}</h3><ul class="roster-list" data-roster-home></ul></div>
            <div class="roster-col"><h3>${escapeHtml(match.awayTeam.name)}</h3><ul class="roster-list" data-roster-away></ul></div>
          </div>
        </div>
      </section>
    `),
  );

  setupTabs(root.querySelector('[data-tabs]'));

  const rosterHome = root.querySelector<HTMLElement>('[data-roster-home]');
  const rosterAway = root.querySelector<HTMLElement>('[data-roster-away]');
  const homePlayers: PlayerDTO[] = match.homeTeamRoster;
  const awayPlayers: PlayerDTO[] = match.awayTeamRoster;

  const renderRoster = (players: PlayerDTO[], host: HTMLElement | null) => {
    if (!host) return;
    host.innerHTML = players.map((pl) => `
      <li class="roster-item">
        <label class="roster-item__link">
          ${dtoImg(`${pl.name} ${pl.surname}`, pl.photo, pl.photoMime, 'mini-photo player-cards__img--round')}
          <span class="roster-item__name"><a href="${urlFor(`/player/${pl.id}`, admin)}" data-app-link>${escapeHtml(`${pl.name} ${pl.surname}`)}</a>${citizenshipBadgeHtml(pl.citizenship ?? null)} · ${escapeHtml(pl.position.name)}</span>
        </label>
      </li>
    `).join('');
  };
  renderRoster(homePlayers, rosterHome);
  renderRoster(awayPlayers, rosterAway);
  if (admin) {
    root.querySelector<HTMLButtonElement>('[data-next-status]')?.addEventListener('click', async () => {
      try {
        if (match.status.code === 0) {
          await startMatch(id);
          showSuccessToast({ message: 'Матч начат.' });
        } else if (match.status.code === 1) {
          await finishMatch(id);
          showSuccessToast({ message: 'Матч завершён.' });
        }
        window.dispatchEvent(new PopStateEvent('popstate'));
      } catch (e) {
        showErrorToast({ message: e instanceof Error ? e.message : String(e) });
      }
    });

﻿    const openGoalDialog = async (editGoal?: GoalDTO) => {
      const ASSIST_NONE_ID = -1;
      const assistNoneItem = (): EntityItem => ({
        id: ASSIST_NONE_ID,
        name: 'Без ассистента',
        sub: null,
        photo: null,
        photoMime: null,
      });
      const playerToItem = (p: PlayerDTO): EntityItem => ({
        id: p.id,
        name: `${p.name} ${p.surname}`,
        sub: p.position.name,
        photo: p.photo ?? null,
        photoMime: p.photoMime ?? null,
        citizenship: p.citizenship ?? null,
      });

      const initialTeamId = editGoal?.scoringTeamId.id ?? match.homeTeam.id;
      let teamId = initialTeamId;
      const strength: StrengthForm = editGoal ? strengthFromGoalApi(editGoal.strengthType) : 'EvenStrength';
      const emptyNetChecked = editGoal ? emptyNetFromGoalApi(editGoal.netType) : false;

      const modal = openModal(
        editGoal ? 'Редактирование гола' : 'Добавление гола',
        `<div class="goal-dialog">
          ${editGoal ? `<div class="goal-dialog__readonly-meta muted">
            <p class="goal-dialog__readonly-line"><strong>${escapeHtml(editGoal.scoringTeamId.name)}</strong> · ${escapeHtml(periodTitle(editGoal.period, periodsCount))} · ${formatGoalTime(editGoal.time)}</p>
          </div>` : `<div class="goal-dialog__time-row">
            <div class="field-stack">
              <span class="field-stack__label">Период</span>
              <input class="field-stack__input search-input" required type="number" min="1" max="${periodsCount + 5}" value="1" data-period />
            </div>
            <div class="field-stack">
              <span class="field-stack__label">Минуты</span>
              <input class="field-stack__input search-input" required type="number" min="0" max="59" value="0" data-min />
            </div>
            <div class="field-stack">
              <span class="field-stack__label">Секунды</span>
              <input class="field-stack__input search-input" required type="number" min="0" max="59" value="0" data-sec />
            </div>
          </div>
          <div>
            <div class="goal-dialog__section-label">Команда</div>
            <div class="goal-dialog__teams" role="group" aria-label="Команда">
              <button type="button" class="goal-dialog__team-tile${initialTeamId === match.homeTeam.id ? ' goal-dialog__team-tile--active' : ''}" data-team-tile="home">
                ${dtoImg(match.homeTeam.name, match.homeTeam.photo, match.homeTeam.photoMime, 'goal-dialog__team-photo')}
                <span class="goal-dialog__team-name">${escapeHtml(match.homeTeam.name)}</span>
              </button>
              <button type="button" class="goal-dialog__team-tile${initialTeamId === match.awayTeam.id ? ' goal-dialog__team-tile--active' : ''}" data-team-tile="away">
                ${dtoImg(match.awayTeam.name, match.awayTeam.photo, match.awayTeam.photoMime, 'goal-dialog__team-photo')}
                <span class="goal-dialog__team-name">${escapeHtml(match.awayTeam.name)}</span>
              </button>
            </div>
          </div>`}
          <div class="goal-dialog__field">
            <span class="goal-dialog__label">Автор</span>
            <div data-scorer></div>
          </div>
          <div class="goal-dialog__field">
            <span class="goal-dialog__label">1-й ассистент</span>
            <div data-a1></div>
          </div>
          <div class="goal-dialog__field">
            <span class="goal-dialog__label">2-й ассистент</span>
            <div data-a2></div>
          </div>
          <div class="goal-types goal-dialog__goal-types">
            <label class="goal-types__choice"><input type="radio" name="strengthType" value="EvenStrength" ${strength === 'EvenStrength' ? 'checked' : ''} /> В равных</label>
            <label class="goal-types__choice"><input type="radio" name="strengthType" value="PowerPlay" ${strength === 'PowerPlay' ? 'checked' : ''} /> В большинстве</label>
            <label class="goal-types__choice"><input type="radio" name="strengthType" value="Shorthanded" ${strength === 'Shorthanded' ? 'checked' : ''} /> В меньшинстве</label>
            <label class="goal-types__choice"><input type="checkbox" data-empty-net ${emptyNetChecked ? 'checked' : ''} /> В пустые ворота</label>
          </div>
          <div class="action-row goal-dialog__actions"><button class="action-btn" type="button" data-save-goal>Сохранить</button></div>
        </div>`,
        { tall: true, wide: true },
      );

      const getRosterByTeam = (tid: number) => (tid === match.homeTeam.id ? match.homeTeamRoster : match.awayTeamRoster);

      const homeTile = modal.body.querySelector<HTMLButtonElement>('[data-team-tile="home"]');
      const awayTile = modal.body.querySelector<HTMLButtonElement>('[data-team-tile="away"]');

      const setTeamTiles = () => {
        if (!homeTile || !awayTile) return;
        homeTile.classList.toggle('goal-dialog__team-tile--active', teamId === match.homeTeam.id);
        awayTile.classList.toggle('goal-dialog__team-tile--active', teamId === match.awayTeam.id);
      };

      const scorerHost = modal.body.querySelector<HTMLElement>('[data-scorer]')!;
      const a1Host = modal.body.querySelector<HTMLElement>('[data-a1]')!;
      const a2Host = modal.body.querySelector<HTMLElement>('[data-a2]')!;

      type Picker = ReturnType<typeof mountEntitySelect>;
      const peer: { scorer: Picker | null; a1: Picker | null; a2: Picker | null } = { scorer: null, a1: null, a2: null };

      const reloadExcept = (k: keyof typeof peer) => {
        (['scorer', 'a1', 'a2'] as const).forEach((key) => {
          if (key === k) return;
          void peer[key]?.reload();
        });
      };

      const clearPickerHosts = () => {
        scorerHost.innerHTML = '';
        a1Host.innerHTML = '';
        a2Host.innerHTML = '';
      };

      const mountPickers = () => {
        clearPickerHosts();
        peer.scorer = peer.a1 = peer.a2 = null;

        const roster = getRosterByTeam(teamId);
        const matchesSearch = (pl: PlayerDTO, search: string) =>
          `${pl.name} ${pl.surname}`.toLowerCase().includes(search.trim().toLowerCase());


        const buildPlayers = (search: string, exclude: Set<number>) =>
          roster
            .filter((pl) => !exclude.has(pl.id))
            .filter((pl) => matchesSearch(pl, search))
            .map(playerToItem);

        const initialScorer =
          editGoal && editGoal.scoringTeamId.id === teamId ? [playerToItem(editGoal.goalScorerId)] : [];
        const initialA1 =
          editGoal && editGoal.scoringTeamId.id === teamId && editGoal.firstAssistId
            ? [playerToItem(editGoal.firstAssistId)]
            : [];
        const initialA2 =
          editGoal && editGoal.scoringTeamId.id === teamId && editGoal.secondAssistId
            ? [playerToItem(editGoal.secondAssistId)]
            : [];

        peer.scorer = mountEntitySelect(scorerHost, {
          placeholder: 'Выберите игрока',
          multiple: false,
          showMore: false,
          initialSelected: initialScorer,
          load: async ({ search }) => {
            const ex = new Set<number>();
            const a1v = peer.a1?.getSelected()[0]?.id;
            const a2v = peer.a2?.getSelected()[0]?.id;
            if (a1v && a1v !== ASSIST_NONE_ID) ex.add(a1v);
            if (a2v && a2v !== ASSIST_NONE_ID) ex.add(a2v);
            return buildPlayers(search, ex);
          },
          onChange: () => reloadExcept('scorer'),
        });

        peer.a1 = mountEntitySelect(a1Host, {
          placeholder: 'Игрок или без ассистента',
          multiple: false,
          showMore: false,
          initialSelected: initialA1,
          load: async ({ search }) => {
            const ex = new Set<number>();
            const sv = peer.scorer?.getSelected()[0]?.id;
            const a2v = peer.a2?.getSelected()[0]?.id;
            if (sv) ex.add(sv);
            if (a2v && a2v !== ASSIST_NONE_ID) ex.add(a2v);
            return [assistNoneItem(), ...buildPlayers(search, ex)];
          },
          onChange: (sel) => {
            if (sel[0]?.id === ASSIST_NONE_ID) peer.a2?.setSelected([assistNoneItem()]);
            reloadExcept('a1');
          },
        });

        peer.a2 = mountEntitySelect(a2Host, {
          placeholder: 'Игрок или без ассистента',
          multiple: false,
          showMore: false,
          initialSelected: initialA2,
          load: async ({ search }) => {
            const ex = new Set<number>();
            const sv = peer.scorer?.getSelected()[0]?.id;
            const a1v = peer.a1?.getSelected()[0]?.id;
            if (sv) ex.add(sv);
            if (a1v && a1v !== ASSIST_NONE_ID) ex.add(a1v);
            return [assistNoneItem(), ...buildPlayers(search, ex)];
          },
          onChange: () => reloadExcept('a2'),
        });

        setTeamTiles();
      };

      mountPickers();

      const bindTeamTile = (btn: HTMLButtonElement, id: number) => {
        btn.addEventListener('click', () => {
          if (teamId === id) return;
          teamId = id;
          setTeamTiles();
          mountPickers();
        });
      };
      if (homeTile && awayTile) {
        bindTeamTile(homeTile, match.homeTeam.id);
        bindTeamTile(awayTile, match.awayTeam.id);
      }

      modal.body.querySelector<HTMLButtonElement>('[data-save-goal]')?.addEventListener('click', async () => {
        const period = editGoal
          ? editGoal.period
          : Number((modal.body.querySelector('[data-period]') as HTMLInputElement).value);
        const time = editGoal
          ? editGoal.time
          : Number((modal.body.querySelector('[data-min]') as HTMLInputElement).value) * 60 +
            Number((modal.body.querySelector('[data-sec]') as HTMLInputElement).value);
        const scorerId = peer.scorer!.getSelected()[0]?.id;
        if (!scorerId) {
          showErrorToast({ message: '\u0423\u043a\u0430\u0436\u0438\u0442\u0435 \u0430\u0432\u0442\u043e\u0440\u0430 \u0433\u043e\u043b\u0430.' });
          return;
        }
        const rawA1 = peer.a1!.getSelected()[0]?.id;
        const rawA2 = peer.a2!.getSelected()[0]?.id;
        const a1 = rawA1 && rawA1 !== ASSIST_NONE_ID ? rawA1 : null;
        const a2 = rawA2 && rawA2 !== ASSIST_NONE_ID ? rawA2 : null;
        const strengthType = ((modal.body.querySelector('input[name="strengthType"]:checked') as HTMLInputElement)?.value ||
          'EvenStrength') as 'EvenStrength' | 'PowerPlay' | 'Shorthanded';
        const netType = (modal.body.querySelector('[data-empty-net]') as HTMLInputElement).checked ? ('EmptyNet' as const) : null;
        try {
          if (!editGoal) {
            const goalId = await addGoal(id, { scoringTeamId: teamId, goalScorerId: scorerId, period, time });
            await fillGoal(id, goalId, { scorerId, firstAssistId: a1, secondAssistId: a2, strengthType, netType });
            showSuccessToast({ message: 'Гол добавлен и сохранён.' });
          } else {
            const editGoalId = editGoal.id;
            if (editGoalId === undefined) {
              showErrorToast({ message: '\u0423 \u0433\u043e\u043b\u0430 \u043d\u0435\u0442 \u0438\u0434\u0435\u043d\u0442\u0438\u0444\u0438\u043a\u0430\u0442\u043e\u0440\u0430 \u2014 \u0441\u043e\u0445\u0440\u0430\u043d\u0435\u043d\u0438\u0435 \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e.' });
              return;
            }
            await fillGoal(id, editGoalId, { scorerId, firstAssistId: a1, secondAssistId: a2, strengthType, netType });
            showSuccessToast({ message: 'Сведения о голе сохранены.' });
          }
          modal.close();
          window.dispatchEvent(new PopStateEvent('popstate'));
        } catch (e: unknown) {
          showErrorToast({ message: e instanceof Error ? e.message : String(e) });
        }
      });
    };



    root.querySelector<HTMLButtonElement>('[data-add-goal]')?.addEventListener('click', () => { void openGoalDialog(); });
    root.querySelectorAll<HTMLButtonElement>('[data-goal-id]').forEach((btn) => {
      btn.addEventListener('click', () => {
        const gid = Number(btn.dataset.goalId);
        const g = goals.find((x) => x.id === gid);
        if (g) void openGoalDialog(g);
      });
    });
  }
}

