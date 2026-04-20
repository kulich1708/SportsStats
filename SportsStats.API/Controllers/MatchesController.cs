using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsStats.Application.Matches;
using SportsStats.Application.Matches.DTOs.Requests;
using SportsStats.Application.Matches.DTOs.Responses;

namespace SportsStats.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MatchesController(
		MatchGoalService matchGoalService,
		MatchLifecycleService matchLifecycleService,
		MatchFinishService matchFinishService,
		MatchRosterService matchRosterService,
		MatchQueriesHandler matchQueriesHandler) : ControllerBase
	{
		private readonly MatchGoalService _matchGoalService = matchGoalService;
		private readonly MatchLifecycleService _matchLifecycleService = matchLifecycleService;
		private readonly MatchFinishService _matchFinishService = matchFinishService;
		private readonly MatchRosterService _matchRosterService = matchRosterService;
		private readonly MatchQueriesHandler _matchQueriesHandler = matchQueriesHandler;

		[HttpGet("{id}")]
		public async Task<ActionResult<MatchDTO>> Get(int id)
		{
			MatchDTO? match = await _matchQueriesHandler.GetAsync(id);

			if (match == null)
				return NotFound();

			return Ok(match);
		}


		[HttpPost("{id}/goals")]
		public async Task<ActionResult<int>> AddGoal(int id, [FromBody] AddGoalDTO dto)
		{
			int goalId = await _matchGoalService.AddGoalAsync(id, dto.ScoringTeamId, dto.GoalScorerId, dto.Period, dto.Time);
			return Ok(goalId);
		}
		[HttpPost("{id}/roster")]
		public async Task<ActionResult> SetPlayersToRoster(int id, [FromBody] AddPlayersToRosterDTO dto)
		{
			await _matchRosterService.SetPlayersToRosterAsync(id, dto.PlayerIds, dto.TeamId);
			return NoContent();
		}
		[HttpPost("{id}/goals/{goalId}")]
		public async Task<ActionResult> FillGoalDetaild(int id, int goalId, [FromBody] FillGoalDetailDTO dto)
		{
			await _matchGoalService.FillGoalDetailsAsync(id, goalId, dto.FirstAssistId, dto.SecondAssistId, dto.StrengthType, dto.NetType);
			return NoContent();
		}
		[HttpPost("{id}/start")]
		public async Task<ActionResult> Start(int id, [FromBody] DateTime? startedAt)
		{
			await _matchLifecycleService.StartAsync(id, startedAt);
			return NoContent();
		}
		[HttpPost("{id}/finish")]
		public async Task<ActionResult> Finish(int id, [FromBody] DateTime? finishedAt)
		{
			await _matchFinishService.FinishAsync(id, finishedAt);
			return NoContent();
		}
	}
}
