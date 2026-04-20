using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsStats.Application.Matches.DTOs.Requests;
using SportsStats.Application.Players;
using SportsStats.Application.Players.DTOs.Requests;
using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Application.Teams.DTOs.Responses;

namespace SportsStats.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlayersController(PlayerApplicationService playerApplicationService) : ControllerBase
	{
		PlayerApplicationService _playerApplicationService = playerApplicationService;

		[HttpGet("/by-team")]
		public async Task<ActionResult<List<PlayerDTO>>> GetByTeam([FromQuery] int teamId)
		{
			return Ok(await _playerApplicationService.GetByteamAsync(teamId));
		}
		[HttpGet]
		public async Task<ActionResult<List<PlayerDTO>>> GetAll([FromQuery] MatchPaginationDTO dto)
		{
			return Ok(await _playerApplicationService.GetAllAsync(dto.Page, dto.PageSize));
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<PlayerDTO>> Get(int id)
		{
			return Ok(await _playerApplicationService.GetAsync(id));
		}

		[HttpPost]
		public async Task<ActionResult<int>> Create([FromBody] CreatePlayerDTO dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Name))
				return BadRequest(new { error = "Имя не может быть пустым" });
			if (string.IsNullOrWhiteSpace(dto.Surname))
				return BadRequest(new { error = "Фамилия не может быть пустой" });

			return Ok(await _playerApplicationService.CreateAsync(dto.Name, dto.Surname, dto.Position));
		}
		[HttpPost("{playerId}/change-team")]
		public async Task<ActionResult> ChangeTeam(int playerId, [FromBody] ChangeTeamDTO dto)
		{
			await _playerApplicationService.ChangeTeamAsync(playerId, dto.TeamId);
			return Ok();
		}
	}
}
