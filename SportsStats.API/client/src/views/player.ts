import { playerById } from '../api';
import { urlFor } from '../nav';
import { citizenshipBadgeHtml, dtoImg, el, escapeHtml } from '../ui/utils';

export async function renderPlayerPage(root: HTMLElement, id: number, admin: boolean): Promise<void> {
	const p = await playerById(id);

	root.replaceChildren(
		el(`
      <section class="page page--player">
        <div class="entity-head entity-head--player">
          ${dtoImg(`${p.name} ${p.surname}`, p.photo, p.photoMime, 'entity-head__photo player-cards__img--round')}
          <div class="entity-head__body">
            <h1 class="page-title">${escapeHtml(`${p.name} ${p.surname}`)}</h1>
            <p class="page-sub">${escapeHtml(p.position.name)}${p.number ? ` \u2014 #${p.number}` : ''}</p>
            ${p.birthday ? `<p class="page-sub">\u0414\u0430\u0442\u0430 \u0440\u043e\u0436\u0434\u0435\u043d\u0438\u044f: ${escapeHtml(new Date(p.birthday).toLocaleDateString('ru-RU'))}</p>` : ''}
            <p class="page-sub">
              \u041a\u043e\u043c\u0430\u043d\u0434\u0430:
              ${p.teamId ? `<a class="entity-link" href="${urlFor(`/team/${p.teamId}`, admin)}" data-app-link>${escapeHtml(p.teamName ?? '-')}</a>` : '<span class="muted">\u0411\u0435\u0437 \u043a\u043e\u043c\u0430\u043d\u0434\u044b</span>'}
            </p>
            ${p.citizenship ? `<p class="page-sub page-sub--citizenship"><span class="page-sub__citizenship-label">\u0413\u0440\u0430\u0436\u0434\u0430\u043d\u0441\u0442\u0432\u043e:</span><span class="page-sub__citizenship-value">${citizenshipBadgeHtml(p.citizenship)}<span class="page-sub__citizenship-name">${escapeHtml(p.citizenship.name)}</span></span></p>` : ''}
            ${admin ? `<div class="action-row"><a class="action-btn" href="${urlFor(`/admin/edit/player/${p.id}`, true)}" data-app-link>\u0420\u0435\u0434\u0430\u043a\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0438\u0433\u0440\u043e\u043a\u0430</a></div>` : ''}
          </div>
        </div>
      </section>
    `),
	);
}
