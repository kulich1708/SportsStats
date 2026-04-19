using SportsStats.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches
{
	public class MatchUseCaseBase(IMatchRepository matchRepository)
	{
		private readonly IMatchRepository _matchRepository = matchRepository;

		protected async Task<Match> GetMatchOrThrowAsync(int matchId)
		{
			return await _matchRepository.GetAsync(matchId)
				?? throw new ArgumentException($"Матч {matchId} не существует");
		}
	}
}
