using Microsoft.AspNetCore.Mvc;
using SportsStats.Application.Tournaments;
using SportsStats.Application.Tournaments.DTOs.Requests;
using SportsStats.Application.Tournaments.DTOs.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SportsStats.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TournamentsController : ControllerBase
	{
		private readonly TournamentApplicationService _tournamentApplicationService;
		public TournamentsController(TournamentApplicationService tournamentApplicationService)
		{
			_tournamentApplicationService = tournamentApplicationService;
		}
		// GET: api/<Tournaments?onlyStarted=true>
		[HttpGet]
		public async Task<ActionResult<List<TournamentDTO>>> GetTournaments([FromQuery] bool onlyStarted = false)
		{
			var tournaments = await _tournamentApplicationService.GetTournamentsAsync(onlyStarted);
			return Ok(tournaments);
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<TournamentDTO>> GetTournament(int id)
		{
			var tournament = await _tournamentApplicationService.GetTournament(id);
			return Ok(tournament);
		}

		[HttpPost]
		public async Task<ActionResult<int>> Create([FromBody] CreateTournamentDTO createTournamentDTO)
		{
			return Ok(await _tournamentApplicationService.Create(createTournamentDTO.Name));
		}
		[HttpPost("{id}/start")]
		public async Task<ActionResult> Start(int id)
		{
			await _tournamentApplicationService.Start(id);
			return Ok();
		}
		[HttpPost("{id}/registration")]
		public async Task<ActionResult> Registration(int id)
		{
			await _tournamentApplicationService.Registration(id);
			return Ok();
		}
		[HttpPost("{id}/finish")]
		public async Task<ActionResult> Finish(int id)
		{
			await _tournamentApplicationService.Finish(id);
			return Ok();
		}

		[HttpPost("{tournamentId}/teams/{teamId}")]
		public async Task<ActionResult> RegistrateTeam(int tournamentId, int teamId)
		{
			await _tournamentApplicationService.RegistrateTeam(tournamentId, teamId);
			return Ok();
		}

		[HttpPost("{id}/rules")]
		public async Task<ActionResult> SetRules(int id)
		{
			await _tournamentApplicationService.SetRules(id);
			return Ok();
		}
	}
}
