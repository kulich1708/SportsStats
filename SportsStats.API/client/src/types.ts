/** Mirrors API DTOs (camelCase JSON). */

export interface TeamDTO {
  id: number;
  name: string;
  city: string | null;
  photo: string | null;
  photoMime: string | null;
}

export interface CitizenshipDTO {
  name: string;
  photo: string | null;
  photoMime: string | null;
}

export interface PlayerPositionDTO {
  code: string;
  name: string;
}

export interface PlayerDTO {
  id: number;
  name: string;
  surname: string;
  teamId: number | null;
  teamName: string | null;
  position: PlayerPositionDTO;
  photo: string | null;
  photoMime: string | null;
  citizenship: CitizenshipDTO | null;
  birthday?: string | null;
  number?: number | null;
}

export interface TournamentShortDTO {
  id: number;
  name: string;
  photo: string | null;
  photoMime: string | null;
  status: TournamentStatusDTO;
}

export interface TournamentStatusDTO {
  code: number;
  description: string;
  nextActionDescription: string;
}

export interface MatchStatusDTO {
  code: number;
  description: string;
  nextActionDescription: string;
}

export interface MatchShortDTO {
  id: number;
  tournamentId: number;
  homeTeam: TeamDTO;
  awayTeam: TeamDTO;
  scheduleAt: string;
  startedAt: string | null;
  finishedAt: string | null;
  status: MatchStatusDTO;
  homeTeamScore: number;
  awayTeamScore: number;
  homeTeamWinType: string;
  awayTeamWinType: string;
  isOvertime: boolean;
}

export interface TournamentWithMatchesDTO {
  id: number;
  name: string;
  photo: string | null;
  photoMime: string | null;
  startedAt: string | null;
  finishedAt: string | null;
  status: TournamentStatusDTO;
  matches: MatchShortDTO[];
}

export interface GoalDTO {
  /** camelCase JSON */
  id?: number;
  /** PascalCase JSON */
  Id?: number;
  scoringTeamId: TeamDTO;
  goalScorerId: PlayerDTO;
  period: number;
  time: number;
  firstAssistId: PlayerDTO | null;
  secondAssistId: PlayerDTO | null;
  strengthType: string;
  netType: string;
}

export interface MatchOvertimeRulesDTO {
  overtimesCount: number | null;
  overtimeDurationSeconds: number | null;
  goalEndsOvertime: boolean;
}

export interface MatchShootoutRulesDTO {
}

export interface MatchTimeRulesDTO {
  periodsCount: number;
  periodDurationSeconds: number;
  isDrawPossible: boolean;
  hasOvertime: boolean;
  hasShootout: boolean;
  overtimeRules: MatchOvertimeRulesDTO | null;
  shootoutRules: MatchShootoutRulesDTO | null;
}

export interface MatchRosterRulesDTO {
  maxPlayers: number;
  minPlayers: number;
  minForwards: number;
  maxForwards: number;
  minDefensemans: number;
  maxDefensemans: number;
  minGoalies: number;
  maxGoalies: number;
}

export interface MatchPointsRulesDTO {
  winPoints: number;
  lossPoints: number;
  otWinPoints: number | null;
  otLossPoints: number | null;
  shootoutWinPoints: number | null;
  shootoutLossPoints: number | null;
  drawPoints: number | null;
}

export interface TournamentRulesDTO {
  matchTimeRules: MatchTimeRulesDTO;
  matchRosterRules: MatchRosterRulesDTO;
  matchPointsRules: MatchPointsRulesDTO;
}

export interface MatchDTO {
  id: number;
  homeTeam: TeamDTO;
  awayTeam: TeamDTO;
  homeTeamRoster: PlayerDTO[];
  awayTeamRoster: PlayerDTO[];
  scheduleAt: string;
  startedAt: string | null;
  finishedAt: string | null;
  tournament: TournamentShortDTO;
  status: MatchStatusDTO;
  homeTeamScore: number;
  awayTeamScore: number;
  homeTeamWinType: string;
  awayTeamWinType: string;
  isOvertime: boolean;
  goals: GoalDTO[];
  rules: TournamentRulesDTO;
}

export interface TeamStatsDTO {
  teamId: number;
  teamName: string;
  tournamentId: number;
  tournamentName: string;
  games: number;
  regularWins: number;
  otWins: number;
  draws: number;
  otLosses: number;
  regularLosses: number;
  points: number;
}

export interface TeamInTournamentDTO {
  id: number;
  name: string;
  photo?: string | null;
  photoMime?: string | null;
}

export interface TournamentDTO {
  id: number;
  name: string;
  photo: string | null;
  photoMime: string | null;
  startedAt: string | null;
  finishedAt: string | null;
  status: TournamentStatusDTO;
  tournamentRules: TournamentRulesDTO | null;
  teams: TeamInTournamentDTO[];
}
