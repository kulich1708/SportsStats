using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsStats.Application.Matches;
using SportsStats.Application.Matches.DTOs.Requests;
using SportsStats.Application.Matches.DTOs.Responses;
using SportsStats.Application.Shared;

namespace SportsStats.API.Controllers
{
	[Route("api/tournaments/{tournamentId}/matches")]
	[ApiController]
	public class TournamentMatchesController(
		MatchLifecycleService matchLifecycleService,
		MatchQueriesHandler matchQueriesHandler) : ControllerBase
	{
		private readonly MatchLifecycleService _matchLifecycleService = matchLifecycleService;
		private readonly MatchQueriesHandler _matchQueriesHandler = matchQueriesHandler;
		[HttpGet("calendar")]
		public async Task<ActionResult<List<MatchShortDTO>>> GetScheduleByTournament(int tournamentId, [FromQuery] PaginationDTO dto)
		{
			List<MatchShortDTO> matches = await _matchQueriesHandler.GetScheduleByTournamentAsync(tournamentId, dto.Page, dto.PageSize);
			return Ok(matches);
		}
		[HttpGet("result")]
		public async Task<ActionResult<List<MatchShortDTO>>> GetFinishedByTournament(int tournamentId, [FromQuery] PaginationDTO dto)
		{
			List<MatchShortDTO> matches = await _matchQueriesHandler.GetFinishedByTournamentAsync(tournamentId, dto.Page, dto.PageSize);
			return Ok(matches);
		}
		[HttpGet("by-date/{date}")]
		public async Task<ActionResult<List<MatchShortDTO>>> GetByTournamentAndDate(int tournamentId, DateOnly date)
		{
			List<MatchShortDTO> matches = await _matchQueriesHandler.GetByDateAsync(date, tournamentId);
			return Ok(matches);
		}

		[HttpPost]
		public async Task<ActionResult<int>> Create(int tournamentId, [FromBody] CreateMatchDTO dto)
		{
			int id = await _matchLifecycleService.CreateAsync(tournamentId, dto.HomeTeamId, dto.AwayTeamId, dto.ScheduledAt);
			return Ok(id);
		}
	}
}
