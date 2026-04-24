const API_BASE = '/api';

function tryParseJson(raw: string): unknown | null {
	try {
		return JSON.parse(raw) as unknown;
	} catch {
		return null;
	}
}

function extractServerError(raw: string): string {
	if (!raw) return '';
	try {
		const parsed = JSON.parse(raw) as { error?: unknown; message?: unknown; details?: unknown };
		if (typeof parsed.error === 'string' && parsed.error.trim()) return parsed.error;
		if (typeof parsed.message === 'string' && parsed.message.trim()) return parsed.message;
		if (typeof parsed.details === 'string' && parsed.details.trim()) return parsed.details;
	} catch {
		/* raw text */
	}
	return raw;
}

function logApiError(r: Response, raw: string, message: string): void {
	console.error('[API ERROR]', {
		url: r.url,
		status: r.status,
		statusText: r.statusText,
		message,
		raw,
	});
}

async function parseJson<T>(r: Response): Promise<T> {
	if (!r.ok) {
		const raw = await r.text();
		const message = extractServerError(raw) || r.statusText || String(r.status);
		logApiError(r, raw, message);
		throw new Error(`HTTP ${r.status}: ${message}`);
	}
	return r.json() as Promise<T>;
}

export async function getJson<T>(path: string): Promise<T> {
	const r = await fetch(`${API_BASE}${path}`, {
		headers: { Accept: 'application/json' },
	});
	return parseJson<T>(r);
}

export async function postJson<TResponse = void, TBody = unknown>(
	path: string,
	body?: TBody,
): Promise<TResponse> {
	const r = await fetch(`${API_BASE}${path}`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
		body: body === undefined ? undefined : JSON.stringify(body),
	});
	if (r.status === 204) return undefined as TResponse;
	return parseJson<TResponse>(r);
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
export function teamById(id: number) {
	return getJson<import('./types').TeamDTO>(`/Teams/${id}`);
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
	return getJson<import('./types').PlayerDTO[]>(`/Players/by-team?teamId=${teamId}`);
}

export function playerById(id: number) {
	return getJson<import('./types').PlayerDTO>(`/Players/${id}`);
}

export function playerPositions() {
	return getJson<Record<string, string>>('/Players/positions');
}

export function tournamentsPaged(page: number, pageSize: number, search = '') {
	const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
	if (search) q.set('search', search);
	return getJson<import('./types').TournamentShortDTO[]>(`/Tournaments?${q}`);
}

export function teamsPaged(page: number, pageSize: number, search = '') {
	const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
	if (search) q.set('search', search);
	return getJson<import('./types').TeamDTO[]>(`/Teams?${q}`);
}

export function playersPaged(page: number, pageSize: number, search = '') {
	const q = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
	if (search) q.set('search', search);
	return getJson<import('./types').PlayerDTO[]>(`/Players?${q}`);
}

export function teamByTournament(tournamentId: number) {
	return getJson<import('./types').TeamDTO[]>(`/Teams/by-tournament?tournamentId=${tournamentId}`);
}

export function addPlayersToRoster(matchId: number, teamId: number, playerIds: number[]) {
	return postJson<void, { playerIds: number[]; teamId: number }>(`/Matches/${matchId}/roster`, {
		playerIds,
		teamId,
	});
}

export function startMatch(matchId: number) {
	return postJson<void, string | null>(`/Matches/${matchId}/start`, null);
}

export function finishMatch(matchId: number) {
	return postJson<void, string | null>(`/Matches/${matchId}/finish`, null);
}

export function addGoal(
	matchId: number,
	dto: { scoringTeamId: number; goalScorerId: number; period: number; time: number },
) {
	return postJson<number, typeof dto>(`/Matches/${matchId}/goals`, dto);
}

/** Wire shape for API: ASP.NET default JSON uses numeric enums (no JsonStringEnumConverter). */
type FillGoalDetailWire = {
	scorerId: number;
	firstAssistId: number | null;
	secondAssistId: number | null;
	strengthType: 0 | 1 | 2;
	netType: 0 | null;
};

const strengthTypeWire: Record<'EvenStrength' | 'PowerPlay' | 'Shorthanded', 0 | 1 | 2> = {
	EvenStrength: 0,
	PowerPlay: 1,
	Shorthanded: 2,
};

export function fillGoal(
	matchId: number,
	goalId: number,
	dto: {
		scorerId: number;
		firstAssistId: number | null;
		secondAssistId: number | null;
		strengthType: 'EvenStrength' | 'PowerPlay' | 'Shorthanded';
		netType: 'EmptyNet' | null;
	},
) {
	const body: FillGoalDetailWire = {
		scorerId: dto.scorerId,
		firstAssistId: dto.firstAssistId,
		secondAssistId: dto.secondAssistId,
		strengthType: strengthTypeWire[dto.strengthType],
		netType: dto.netType === 'EmptyNet' ? 0 : null,
	};
	return postJson<void, FillGoalDetailWire>(`/Matches/${matchId}/goals/${goalId}`, body);
}

export function tournamentChangeGeneralInfo(
	id: number,
	dto: { name: string; photo: string | null; photoMime: string | null },
) {
	return postJson<void, typeof dto>(`/Tournaments/${id}/general/change`, dto);
}

export function tournamentSetRules(id: number, rules: import('./types').TournamentRulesDTO) {
	return postJson<void, import('./types').TournamentRulesDTO>(`/Tournaments/${id}/rules/set`, rules);
}

export function tournamentRegistrationStep(id: number) {
	return postJson<void, null>(`/Tournaments/${id}/registration`, null);
}

export function tournamentStart(id: number) {
	return postJson<void, null>(`/Tournaments/${id}/start`, null);
}

export function tournamentFinish(id: number) {
	return postJson<void, null>(`/Tournaments/${id}/finish`, null);
}

export function tournamentSetRegistrationTeams(tournamentId: number, teamIds: number[]) {
	return postJson<void, number[]>(`/Tournaments/${tournamentId}/teams`, teamIds);
}

export function teamChangeGeneralInfo(
	id: number,
	dto: { name: string; city: string | null; photo: string | null; photoMime: string | null },
) {
	return postJson<void, typeof dto>(`/Teams/${id}/general/change`, dto);
}

export function playerChangeGeneralInfo(
	id: number,
	dto: {
		name: string;
		surname: string;
		position: string | number;
		teamId: number | null;
		number: number | null;
		birthday: string | null;
		citizenship: { name: string; photo: string | null; photoMime: string | null } | null;
		photo: string | null;
		photoMime: string | null;
	},
) {
	return postJson<void, typeof dto>(`/Players/${id}/general/change`, dto);
}

export function createTournament(name: string) {
	return postJson<number, { name: string }>(`/Tournaments`, { name });
}

export function createTeam(name: string) {
	return postJson<number, { name: string }>(`/Teams`, { name });
}

export function createPlayer(dto: { name: string; surname: string; position: string | number }) {
	return postJson<number, typeof dto>(`/Players`, dto);
}

export function createTournamentMatch(
	tournamentId: number,
	dto: { homeTeamId: number; awayTeamId: number; scheduledAt: string },
) {
	return postJson<number, typeof dto>(`/tournaments/${tournamentId}/matches`, dto);
}

export function matchChangeGeneralInfo(id: number, dto: { scheduleAt: string }) {
	return postJson<void, typeof dto>(`/Matches/${id}/general`, dto);
}
