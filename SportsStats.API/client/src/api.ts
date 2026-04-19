const API_BASE = '/api';

async function parseJson<T>(r: Response): Promise<T> {
  if (!r.ok) {
    const text = await r.text();
    throw new Error(text || r.statusText || String(r.status));
  }
  return r.json() as Promise<T>;
}

export async function getJson<T>(path: string): Promise<T> {
  const r = await fetch(`${API_BASE}${path}`, {
    headers: { Accept: 'application/json' },
  });
  return parseJson<T>(r);
}

export function tournamentsByDate(dateIso: string) {
  return getJson<import('./types').TournamentWithMatchesDTO[]>(
    `/Tournaments/by-date/${dateIso}/matches`,
  );
}

export function matchById(id: number) {
  return getJson<import('./types').MatchDTO>(`/Matches/${id}`);
}

export function tournamentById(id: number) {
  return getJson<import('./types').TournamentDTO>(`/Tournaments/${id}`);
}

export function teamStatsByTournament(tournamentId: number) {
  return getJson<import('./types').TeamStatsDTO[]>(
    `/TeamStats/tournaments/${tournamentId}`,
  );
}

export function tournamentMatchesResult(tournamentId: number, page: number, pageSize: number) {
  const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
  return getJson<import('./types').MatchShortDTO[]>(
    `/tournaments/${tournamentId}/matches/result?${q}`,
  );
}

export function tournamentMatchesCalendar(tournamentId: number, page: number, pageSize: number) {
  const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
  return getJson<import('./types').MatchShortDTO[]>(
    `/tournaments/${tournamentId}/matches/calendar?${q}`,
  );
}

export function teamCalendar(teamId: number, page: number, pageSize: number) {
  const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
  return getJson<import('./types').TournamentWithMatchesDTO[]>(
    `/Teams/${teamId}/calendar?${q}`,
  );
}

export function teamResults(teamId: number, page: number, pageSize: number) {
  const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
  return getJson<import('./types').TournamentWithMatchesDTO[]>(
    `/Teams/${teamId}/results?${q}`,
  );
}

export function playersByTeam(teamId: number) {
  return getJson<import('./types').PlayerDTO[]>(`/Players?teamId=${teamId}`);
}

export function playerById(id: number) {
  return getJson<import('./types').PlayerDTO>(`/Players/${id}`);
}
