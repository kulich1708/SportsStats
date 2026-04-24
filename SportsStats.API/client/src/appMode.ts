export const ADMIN_PREFIX = '/admin';

export function isAdminPath(pathname: string): boolean {
  return pathname === ADMIN_PREFIX || pathname.startsWith(`${ADMIN_PREFIX}/`);
}

export function stripAdminPrefix(pathname: string): string {
  let p = pathname;
  while (p === ADMIN_PREFIX || p.startsWith(`${ADMIN_PREFIX}/`)) {
    p = p.slice(ADMIN_PREFIX.length);
    if (p.length === 0) return '/';
  }
  return p;
}

export function withMode(path: string, admin: boolean): string {
  if (!admin) return path;
  if (path === ADMIN_PREFIX || path.startsWith(`${ADMIN_PREFIX}/`)) return path;
  if (path === '/') return ADMIN_PREFIX;
  return `${ADMIN_PREFIX}${path}`;
}
