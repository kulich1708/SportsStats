using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStats.Application.Teams;
using SportsStats.Application.Teams.DTOs.Requests;
using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Domain.Teams;
using SportsStats.Infrastructure.Persistence.DbContexts;

namespace SportsStats.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TeamsController(TeamApplicationService teamApplicationService) : ControllerBase
	{
		private readonly TeamApplicationService _teamApplicationService = teamApplicationService;

		// GET: api/Teams
		[HttpGet]
		public async Task<ActionResult<List<TeamDTO>>> GetAll([FromQuery] int? tournamentId = null)
		{
			return Ok(await _teamApplicationService.GetAllAsync(tournamentId));
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<TeamDTO>> Get(int id)
		{
			return Ok(await _teamApplicationService.GetAsync(id));
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
