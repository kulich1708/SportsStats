import { playerById } from '../api';
import { el, escapeHtml, placeholderImg } from '../ui/utils';

export async function renderPlayerPage(root: HTMLElement, id: number): Promise<void> {
  const p = await playerById(id);

  root.replaceChildren(
    el(`
      <section class="page page--player">
        <div class="entity-head entity-head--player">
          ${placeholderImg(`${p.name} ${p.surname}`, 'entity-head__photo')}
          <div>
            <h1 class="page-title">${escapeHtml(`${p.name} ${p.surname}`)}</h1>
            <p class="page-sub">${escapeHtml(p.position)}</p>
            <p class="page-sub">
              Команда:
              <a href="/team/${p.teamId}" data-app-link>${escapeHtml(p.teamName)}</a>
            </p>
          </div>
        </div>
      </section>
    `),
  );
}
