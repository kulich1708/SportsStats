/** Client-side navigation without full page reload (pairs with `popstate` listener). */
export function go(path: string): void {
  window.history.pushState({}, '', path);
  window.dispatchEvent(new PopStateEvent('popstate'));
}
