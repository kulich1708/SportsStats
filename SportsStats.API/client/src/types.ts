/** Mirrors API DTOs (camelCase JSON). */

export interface TeamDTO {
  id: number;
  name: string;
}

export interface PlayerDTO {
  id: number;
  name: string;
  surname: string;
  teamId: number;
  teamName: string;
  position: string;
}

export interface TournamentShortDTO {
  id: number;
  name: string;
}

export interface MatchShortDTO {
  id: number;
  tournamentId: number;
  homeTeam: TeamDTO;
  awayTeam: TeamDTO;
  scheduleAt: string;
  startedAt: string | null;
  finishedAt: string | null;
  status: string;
  homeTeamScore: number;
  awayTeamScore: number;
  homeTeamWinType: string;
  awayTeamWinType: string;
  isOvertime: boolean;
}

export interface TournamentWithMatchesDTO {
  id: number;
  name: string;
  startedAt: string | null;
  finishedAt: string | null;
  status: string;
  matches: MatchShortDTO[];
}

export interface GoalDTO {
  scoringTeamId: TeamDTO;
  goalScorerId: PlayerDTO;
  period: number;
  time: number;
  firstAssistId: PlayerDTO | null;
  secondAssistId: PlayerDTO | null;
  strengthType: string;
  netType: string;
}

export interface MatchDurationRulesDTO {
  periodsCount: number;
  periodDurationSeconds: number;
  hasOvertime: boolean;
  overtimeDurationSeconds: number | null;
  overtimesCount: number | null;
  suddenDeathOvertime: boolean;
  isDrawPossible: boolean;
  shootoutsCount: number;
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
  otWinPoints: number;
  lossPoints: number;
  otLossPoints: number;
  drawPoints: number | null;
}

export interface TournamentRulesDTO {
  matchDurationRules: MatchDurationRulesDTO;
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
  status: string;
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
}

export interface TournamentDTO {
  id: number;
  name: string;
  startedAt: string | null;
  finishedAt: string | null;
  status: string;
  tournamentRules: TournamentRulesDTO;
  teams: TeamInTournamentDTO[];
}
