using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsStats.Application.Matches;
using SportsStats.Application.Matches.DTOs.Requests;
using SportsStats.Application.Matches.DTOs.Responses;

namespace SportsStats.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MatchController(MatchApplicationService matchApplicationService,
		MatchQueriesHandler matchQueriesHandler) : ControllerBase
	{
		MatchApplicationService _matchApplicationService = matchApplicationService;
		MatchQueriesHandler _matchQueriesHandler = matchQueriesHandler;
		[HttpGet]
		public async Task<ActionResult<List<MatchShortDTO>>> GetAll([FromQuery] GetAllMatchesDTO dto)
		{
			List<MatchShortDTO> matches = await _matchQueriesHandler.GetAllAsync(dto.TournamentId, dto.TeamId);
			return Ok(matches);
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<MatchDTO>> Get(int id)
		{
			MatchDTO? match = await _matchQueriesHandler.GetAsync(id);

			if (match == null)
				return NotFound();

			return Ok(match);
		}

		[HttpPost]
		public async Task<ActionResult<int>> Create([FromBody] CreateMatchDTO dto)
		{
			int id = await _matchApplicationService.CreateAsync(dto.TournamentId, dto.HomeTeamId, dto.AwayTeamId);
			return Ok(id);
		}
		[HttpPost("{id}/goals")]
		public async Task<ActionResult<int>> AddGoal(int id, [FromBody] AddGoalDTO dto)
		{
			int goalId = await _matchApplicationService.AddGoalAsync(id, dto.ScoringTeamId, dto.GoalScorerId, dto.Period, dto.Time);
			return Ok(goalId);
		}
		[HttpPost("{id}/roster")]
		public async Task<ActionResult> AddPlayerToRoster(int id, [FromBody] AddPlayerToRosterDTO dto)
		{
			await _matchApplicationService.AddPlayerToRosterAsync(id, dto.PlayerId, dto.TeamId);
			return Ok();
		}
		[HttpPost("{id}/goals/{goalId}")]
		public async Task<ActionResult> FillGoalDetaild(int id, int goalId, [FromBody] FillGoalDetailDTO dto)
		{
			await _matchApplicationService.FillGoalDetailsAsync(id, goalId, dto.FirstAssistId, dto.SecondAssistId, dto.StrengthType, dto.NetType);
			return Ok();
		}
		[HttpPost("{id}/start")]
		public async Task<ActionResult> Start(int id)
		{
			await _matchApplicationService.StartAsync(id);
			return Ok();
		}
		[HttpPost("{id}/finish")]
		public async Task<ActionResult> Finish(int id)
		{
			await _matchApplicationService.FinishAsync(id);
			return Ok();
		}
	}
}
