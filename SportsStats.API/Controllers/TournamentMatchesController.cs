using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsStats.Application.Matches;
using SportsStats.Application.Matches.DTOs.Requests;
using SportsStats.Application.Matches.DTOs.Responses;

namespace SportsStats.API.Controllers
{
	[Route("api/tournaments/{tournamentId}/[controller]")]
	[ApiController]
	public class TournamentMatchesController(
		MatchLifecycleService matchLifecycleService,
		MatchQueriesHandler matchQueriesHandler) : ControllerBase
	{
		private readonly MatchLifecycleService _matchLifecycleService = matchLifecycleService;
		private readonly MatchQueriesHandler _matchQueriesHandler = matchQueriesHandler;
		[HttpGet]
		public async Task<ActionResult<List<MatchShortDTO>>> GetAll(int tournamentId, [FromQuery] GetAllMatchesDTO dto)
		{
			List<MatchShortDTO> matches = await _matchQueriesHandler.GetAllAsync(tournamentId, dto.TeamId);
			return Ok(matches);
		}
		[HttpGet]
		public async Task<ActionResult<List<MatchShortDTO>>> GetByTournamentAndDate(int tournamentId, [FromQuery] DateOnly date)
		{
			List<MatchShortDTO> matches = await _matchQueriesHandler.GetByDateAsync(tournamentId, date);
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
