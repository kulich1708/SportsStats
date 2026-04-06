using SportsStats.Domain.Players;
using SportsStats.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Players
{
	public class PlayerApplicationService
	{
		private IPlayerRepository _playerRepository;
		private ITeamRepository _teamRepository;

		public Player Create(string name, string surname)
		{
			Player player = new Player(name, surname);
			return _playerRepository.Save(player);
		}
		public void ChangeTeam(int playerId, int teamId)
		{
			Player player = GetPlayerOrThrow(playerId);
			Team team = GetTeamOrThrow(teamId);

			player.ChangeTeam(teamId);

			_playerRepository.Save(player);
		}

		public void SetPosition(int playerId, PositionType position)
		{
			Player player = GetPlayerOrThrow(playerId);

			player.SetPosition(position);

			_playerRepository.Save(player);
		}

		private Player GetPlayerOrThrow(int playerId)
		{
			return _playerRepository.FindById(playerId)
				?? throw new ArgumentException("Игрок с таким Id не найден");
		}
		private Team GetTeamOrThrow(int teamId)
		{
			return _teamRepository.FindById(teamId)
				?? throw new ArgumentException("Команда с таким id не найдена");
		}
	}
}
