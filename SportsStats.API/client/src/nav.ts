import { withMode } from './appMode';

/** Client-side navigation without full page reload (pairs with `popstate` listener). */
export function go(path: string, admin = false): void {
  const target = withMode(path, admin);
  window.history.pushState({}, '', target);
  window.dispatchEvent(new PopStateEvent('popstate'));
}

export function currentAdminMode(): boolean {
  return window.location.pathname === '/admin' || window.location.pathname.startsWith('/admin/');
}

export function goSameMode(path: string): void {
  go(path, currentAdminMode());
}

export function urlFor(path: string, admin: boolean): string {
  return withMode(path, admin);
}
