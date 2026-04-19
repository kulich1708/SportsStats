import { renderHome } from './views/home';
import { renderMatchPage } from './views/match';
import { renderTournamentPage } from './views/tournament';
import { renderTeamPage } from './views/team';
import { renderPlayerPage } from './views/player';
import { el } from './ui/utils';
import { go } from './nav';

export type Route =
  | { name: 'home'; query: URLSearchParams }
  | { name: 'match'; id: number }
  | { name: 'tournament'; id: number }
  | { name: 'team'; id: number }
  | { name: 'player'; id: number }
  | { name: 'notfound' };

export function parsePath(pathname: string, search: string): Route {
  const q = new URLSearchParams(search);
  const parts = pathname.split('/').filter(Boolean);

  if (parts.length === 0) return { name: 'home', query: q };

  if (parts[0] === 'match' && parts[1]) {
    const id = Number(parts[1]);
    if (Number.isFinite(id)) return { name: 'match', id };
  }
  if (parts[0] === 'tournament' && parts[1]) {
    const id = Number(parts[1]);
    if (Number.isFinite(id)) return { name: 'tournament', id };
  }
  if (parts[0] === 'team' && parts[1]) {
    const id = Number(parts[1]);
    if (Number.isFinite(id)) return { name: 'team', id };
  }
  if (parts[0] === 'player' && parts[1]) {
    const id = Number(parts[1]);
    if (Number.isFinite(id)) return { name: 'player', id };
  }

  return { name: 'notfound' };
}

function renderNotFound(root: HTMLElement): void {
  root.replaceChildren(
    el(`<section class="panel"><h1>Страница не найдена</h1><p><a href="/" data-app-link>На главную</a></p></section>`),
  );
}

export async function renderRoute(route: Route, root: HTMLElement): Promise<void> {
  root.innerHTML = '<div class="loading">Загрузка…</div>';

  try {
    switch (route.name) {
      case 'home':
        await renderHome(root, route.query);
        break;
      case 'match':
        await renderMatchPage(root, route.id);
        break;
      case 'tournament':
        await renderTournamentPage(root, route.id);
        break;
      case 'team':
        await renderTeamPage(root, route.id);
        break;
      case 'player':
        await renderPlayerPage(root, route.id);
        break;
      default:
        renderNotFound(root);
    }
  } catch (e) {
    const msg = e instanceof Error ? e.message : String(e);
    root.innerHTML = '';
    root.appendChild(
      el(`<section class="panel panel--error"><h1>Ошибка</h1><p>${escapeAttr(msg)}</p><p><a href="/" data-app-link>На главную</a></p></section>`),
    );
  } finally {
    window.scrollTo(0, 0);
  }
}

function escapeAttr(s: string): string {
  return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/"/g, '&quot;');
}

export function initRouter(): void {
  const root = document.getElementById('app');
  if (!root) return;

  const run = () => {
    const route = parsePath(window.location.pathname, window.location.search);
    void renderRoute(route, root);
  };

  window.addEventListener('popstate', run);

  document.addEventListener('click', (e) => {
    const target = e.target as HTMLElement | null;
    const a = target?.closest<HTMLAnchorElement>('a[data-app-link]');
    if (!a || !a.href) return;
    const url = new URL(a.href);
    if (url.origin !== window.location.origin) return;
    if (e.ctrlKey || e.metaKey || e.shiftKey || e.altKey) return;
    e.preventDefault();
    e.stopPropagation();
    go(url.pathname + url.search);
  });

  run();
}
