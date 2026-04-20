using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Teams
{
	public static class TeamMapper
	{
		public static TeamDTO ToDTO(Team team) => new(team.Id, team.Name, team.City, team.Photo);
	}
}
