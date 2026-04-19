using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsStats.Application.Statistics;
using SportsStats.Application.Statistics.DTOs.Responses;

namespace SportsStats.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TeamStatsController(TeamStatsApplicationService teamStatsApplicationService) : ControllerBase
	{
		TeamStatsApplicationService _teamStatsApplicationService = teamStatsApplicationService;
		[HttpGet("tournaments/{tournamentId}")]
		public async Task<ActionResult<List<TeamStatsDTO>>> GetByTournament(int tournamentId)
		{
			List<TeamStatsDTO> teamStats = await _teamStatsApplicationService.GetByTournamentAsync(tournamentId);
			return Ok(teamStats);
		}
		[HttpGet("{teamId}")]
		public async Task<ActionResult<List<TeamStatsDTO>>> GetByTeam(int teamId)
		{
			List<TeamStatsDTO> teamStats = await _teamStatsApplicationService.GetByTeamAsync(teamId);
			return Ok(teamStats);
		}

	}
}
