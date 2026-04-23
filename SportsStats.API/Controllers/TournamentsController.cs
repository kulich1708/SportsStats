using Microsoft.AspNetCore.Mvc;
using SportsStats.Application.Shared;
using SportsStats.Application.Tournaments;
using SportsStats.Application.Tournaments.DTOs.Requests;
using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Application.Tournaments.DTOs.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SportsStats.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TournamentsController(TournamentApplicationService tournamentApplicationService) : ControllerBase
	{
		private readonly TournamentApplicationService _tournamentApplicationService = tournamentApplicationService;

		[HttpGet]
		public async Task<ActionResult<List<TournamentShortDTO>>> GetTournaments([FromQuery] PaginationDTO dto, string? search = null)
		{
			var tournaments = await _tournamentApplicationService.GetAllAsync(dto.Page, dto.PageSize, search);
			return Ok(tournaments);
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<TournamentDTO>> GetTournament(int id)
		{
			var tournament = await _tournamentApplicationService.GetAsync(id);
			return Ok(tournament);
		}
		//[HttpGet("by-date/{date}")]
		//public async Task<ActionResult<List<TournamentShortDTO>>> GetTournamentsByDate(DateOnly date)
		//{
		//	var tournaments = await _tournamentApplicationService.GetActiveByDateAsync(date);
		//	return Ok(tournaments);
		//}

		[HttpGet("by-date/{date}/matches")]
		public async Task<ActionResult<List<TournamentWithMatchesDTO>>> GetTournamentsByDateWithMatches(DateOnly date)
		{
			var tournaments = await _tournamentApplicationService.GetActiveByDateWithMatchesAsync(date);
			return Ok(tournaments);
		}

		[HttpPost]
		public async Task<ActionResult<int>> Create([FromBody] CreateTournamentDTO dto)
		{
			return Ok(await _tournamentApplicationService.CreateAsync(dto.Name));
		}
		[HttpPost("{id}/start")]
		public async Task<ActionResult> Start(int id, [FromBody] DateTime? startedAt)
		{
			await _tournamentApplicationService.StartAsync(id, startedAt);
			return NoContent();
		}
		[HttpPost("{id}/registration")]
		public async Task<ActionResult> Registration(int id)
		{
			await _tournamentApplicationService.RegistrationAsync(id);
			return NoContent();
		}
		[HttpPost("{id}/finish")]
		public async Task<ActionResult> Finish(int id, [FromBody] DateTime? finishedAt)
		{
			await _tournamentApplicationService.FinishAsync(id, finishedAt);
			return NoContent();
		}

		[HttpPost("{tournamentId}/teams")]
		public async Task<ActionResult> SetRegistrationTeams(int tournamentId, [FromBody] List<int> teamIds)
		{
			await _tournamentApplicationService.SetRegistrationTeamsAsync(tournamentId, teamIds);
			return NoContent();
		}

		[HttpPost("{id}/rules/set")]
		public async Task<ActionResult> SetRules(int id, [FromBody] TournamentRulesDTO rules)
		{
			Console.WriteLine(rules);
			await _tournamentApplicationService.SetRulesAsync(id, rules);
			return NoContent();
		}
		[HttpPost("{id}/general/change")]
		public async Task<ActionResult> ChangeGeneralInfo(int id, [FromBody] TournamentGeneralInfoDTO dto)
		{
			await _tournamentApplicationService.ChangeGeneralInfoAsync(id, dto.Name, dto.Photo, dto.PhotoMime);
			return NoContent();
		}
	}
}
