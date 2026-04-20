using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStats.Application.Matches.DTOs.Requests;
using SportsStats.Application.Teams;
using SportsStats.Application.Teams.DTOs.Requests;
using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Application.Tournaments;
using SportsStats.Domain.Teams;
using SportsStats.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStats.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TeamsController(
		TeamApplicationService teamApplicationService,
		TournamentApplicationService tournamentApplicationService
		) : ControllerBase
	{
		private readonly TeamApplicationService _teamApplicationService = teamApplicationService;
		private readonly TournamentApplicationService _tournamentApplicationService = tournamentApplicationService;

		// GET: api/Teams
		[HttpGet]
		public async Task<ActionResult<List<TeamDTO>>> GetAll([FromQuery] MatchPaginationDTO dto)
		{
			return Ok(await _teamApplicationService.GetAllAsync(dto.Page, dto.PageSize));
		}
		[HttpGet("by-tournament")]
		public async Task<ActionResult<List<TeamDTO>>> GetByTournament([FromQuery] int tournamentId)
		{
			return Ok(await _teamApplicationService.GetByTournamentAsync(tournamentId));
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<TeamDTO>> Get(int id)
		{
			return Ok(await _teamApplicationService.GetAsync(id));
		}
		[HttpGet("{id}/results")]
		public async Task<ActionResult<TeamDTO>> GetFinishedMatches(int id, [FromQuery] MatchPaginationDTO dto)
		{
			return Ok(await _tournamentApplicationService.GetByTeamWithFinishedMatchesAsync(id, dto.Page, dto.PageSize));
		}
		[HttpGet("{id}/calendar")]
		public async Task<ActionResult<TeamDTO>> GetCalendar(int id, [FromQuery] MatchPaginationDTO dto)
		{
			return Ok(await _tournamentApplicationService.GetByTeamWithScheduleMatchesAsync(id, dto.Page, dto.PageSize));
		}
		[HttpPost]
		public async Task<ActionResult<int>> Create([FromBody] CreateTeamDTO dto)
		{
			string name = dto.Name;
			if (string.IsNullOrWhiteSpace(name))
				return BadRequest(new { error = "Имя команды не должно быть пустым" });

			return Ok(await _teamApplicationService.CreateAsync(name));
		}
	}
}
