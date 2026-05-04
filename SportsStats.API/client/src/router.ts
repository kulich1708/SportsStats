import { renderHome } from './views/home';
import { renderMatchPage } from './views/match';
import { renderTournamentPage } from './views/tournament';
import { renderTeamPage } from './views/team';
import { renderPlayerPage } from './views/player';
import { renderAdminTournamentsPage } from './views/tournamentsList';
import { renderAdminTeamsPage } from './views/teamsList';
import { renderAdminPlayersPage } from './views/playersList';
import { renderTournamentsPage } from './views/tournamentsList';
import { renderTeamsPage } from './views/teamsList';
import { renderPlayersPage } from './views/playersList';
import { renderEditTournamentPage } from './views/editTournament';
import { renderEditTeamPage } from './views/editTeam';
import { renderEditPlayerPage } from './views/editPlayer';
import { renderEditMatchPage } from './views/editMatch';
import { el } from './ui/utils';
import { go } from './nav';
import { isAdminPath, stripAdminPrefix, withMode } from './appMode';

export type Route =
    | { name: 'home'; query: URLSearchParams; admin: boolean }
    | { name: 'match'; id: number; admin: boolean }
    | { name: 'tournament'; id: number; admin: boolean }
    | { name: 'team'; id: number; admin: boolean }
    | { name: 'player'; id: number; admin: boolean }
    | { name: 'tournaments'; query: URLSearchParams; admin: boolean }
    | { name: 'teams'; query: URLSearchParams; admin: boolean }
    | { name: 'players'; query: URLSearchParams; admin: boolean }
    | { name: 'admin-tournaments'; query: URLSearchParams }
    | { name: 'admin-teams'; query: URLSearchParams }
    | { name: 'admin-players'; query: URLSearchParams }
    | { name: 'admin-edit-tournament'; idOrNew: string }
    | { name: 'admin-edit-team'; idOrNew: string }
    | { name: 'admin-edit-player'; idOrNew: string }
    | { name: 'admin-edit-match'; id: number }
    | { name: 'notfound' };

export function parsePath(pathname: string, search: string): Route {
    const admin = isAdminPath(pathname);
    const strippedPath = stripAdminPrefix(pathname);
    const q = new URLSearchParams(search);
    const parts = strippedPath.split('/').filter(Boolean);

    if (parts.length === 0) return { name: 'home', query: q, admin };

    if (parts[0] === 'tournaments' && parts.length === 1) {
        return admin ? { name: 'admin-tournaments', query: q } : { name: 'tournaments', query: q, admin };
    }
    if (parts[0] === 'teams' && parts.length === 1) {
        return admin ? { name: 'admin-teams', query: q } : { name: 'teams', query: q, admin };
    }
    if (parts[0] === 'players' && parts.length === 1) {
        return admin ? { name: 'admin-players', query: q } : { name: 'players', query: q, admin };
    }

    if (admin && parts[0] === 'edit' && parts[1] === 'tournament' && parts[2]) {
        return { name: 'admin-edit-tournament', idOrNew: parts[2] };
    }
    if (admin && parts[0] === 'edit' && parts[1] === 'team' && parts[2]) {
        return { name: 'admin-edit-team', idOrNew: parts[2] };
    }
    if (admin && parts[0] === 'edit' && parts[1] === 'player' && parts[2]) {
        return { name: 'admin-edit-player', idOrNew: parts[2] };
    }
    if (admin && parts[0] === 'edit' && parts[1] === 'match' && parts[2]) {
        const mid = Number(parts[2]);
        if (Number.isFinite(mid)) return { name: 'admin-edit-match', id: mid };
    }

    if (parts[0] === 'match' && parts[1]) {
        const id = Number(parts[1]);
        if (Number.isFinite(id)) return { name: 'match', id, admin };
    }
    if (parts[0] === 'tournament' && parts[1]) {
        const id = Number(parts[1]);
        if (Number.isFinite(id)) return { name: 'tournament', id, admin };
    }
    if (parts[0] === 'team' && parts[1]) {
        const id = Number(parts[1]);
        if (Number.isFinite(id)) return { name: 'team', id, admin };
    }
    if (parts[0] === 'player' && parts[1]) {
        const id = Number(parts[1]);
        if (Number.isFinite(id)) return { name: 'player', id, admin };
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
                await renderHome(root, route.query, route.admin);
                break;
            case 'match':
                await renderMatchPage(root, route.id, route.admin);
                break;
            case 'tournament':
                await renderTournamentPage(root, route.id, route.admin);
                break;
            case 'team':
                await renderTeamPage(root, route.id, route.admin);
                break;
            case 'player':
                await renderPlayerPage(root, route.id, route.admin);
                break;
            case 'tournaments':
                await renderTournamentsPage(root, route.query, route.admin);
                break;
            case 'teams':
                await renderTeamsPage(root, route.query, route.admin);
                break;
            case 'players':
                await renderPlayersPage(root, route.query, route.admin);
                break;
            case 'admin-tournaments':
                await renderAdminTournamentsPage(root, route.query);
                break;
            case 'admin-teams':
                await renderAdminTeamsPage(root, route.query);
                break;
            case 'admin-players':
                await renderAdminPlayersPage(root, route.query);
                break;
            case 'admin-edit-tournament':
                if (route.idOrNew === 'new') {
                    go('/tournaments?create=1', true);
                    break;
                }
                await renderEditTournamentPage(root, route.idOrNew);
                break;
            case 'admin-edit-team':
                await renderEditTeamPage(root, route.idOrNew);
                break;
            case 'admin-edit-player':
                await renderEditPlayerPage(root, route.idOrNew);
                break;
            case 'admin-edit-match':
                await renderEditMatchPage(root, route.id);
                break;
            default:
                renderNotFound(root);
        }
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
        renderHeader(isAdminPath(window.location.pathname));
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

function renderHeader(admin: boolean): void {
    const nav = document.querySelector<HTMLElement>('[data-main-nav]');
    const logo = document.querySelector<HTMLAnchorElement>('.site-logo');
    if (!nav || !logo) return;
    logo.setAttribute('href', withMode('/', admin));

    const links = [
        { href: withMode('/', !admin), text: admin ? 'Клиент' : 'Админка' },
        { href: withMode('/', admin), text: 'Главная' },
        { href: withMode('/tournaments', admin), text: 'Турниры' },
        { href: withMode('/teams', admin), text: 'Команды' },
        { href: withMode('/players', admin), text: 'Игроки' },
    ];
    nav.innerHTML = links.map((x) => `<a href="${x.href}" data-app-link>${x.text}</a>`).join('');
}
