using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsStats.Application.Players;
using SportsStats.Application.Players.DTOs.Requests;
using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Application.Shared;
using SportsStats.Application.Teams.DTOs.Responses;

namespace SportsStats.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlayersController(PlayerApplicationService playerApplicationService) : ControllerBase
	{
		PlayerApplicationService _playerApplicationService = playerApplicationService;

		[HttpGet("by-team")]
		public async Task<ActionResult<List<PlayerDTO>>> GetByTeam([FromQuery] int teamId)
		{
			return Ok(await _playerApplicationService.GetByteamAsync(teamId));
		}
		[HttpGet]
		public async Task<ActionResult<List<PlayerDTO>>> GetAll([FromQuery] PaginationDTO dto, [FromQuery] string? search)
		{
			return Ok(await _playerApplicationService.GetAllAsync(dto.Page, dto.PageSize, search));
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<PlayerDTO>> Get(int id)
		{
			return Ok(await _playerApplicationService.GetAsync(id));
		}
		[HttpGet("positions")]
		public ActionResult GetAllPositions() => Ok(_playerApplicationService.GetAllPlayerPositions());

		[HttpPost]
		public async Task<ActionResult<int>> Create([FromBody] CreatePlayerDTO dto)
		{
			return Ok(await _playerApplicationService.CreateAsync(dto.Name, dto.Surname, dto.Position));
		}
		[HttpPost("{id}/general/change")]
		public async Task<ActionResult> ChangeGeneralInfo(int id, [FromBody] PlayerGeneralInfoDTO dto)
		{
			await _playerApplicationService.ChangeGeneralInfoAsync(id, dto);
			return NoContent();
		}
	}
}
