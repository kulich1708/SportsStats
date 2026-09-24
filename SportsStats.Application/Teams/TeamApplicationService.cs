using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Domain.Common;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SportsStats.Application.Teams
{
	public class TeamApplicationService(
		ITeamRepository teamRepository,
		IUnitOfWork unitOfWork)
	{
		private readonly ITeamRepository _teamRepository = teamRepository;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

		public async Task<int> CreateAsync(string name)
		{
			Team team = new(name);

			_teamRepository.Add(team);
			await _unitOfWork.SaveChangesAsync();

			return team.Id;
		}
		public async Task<TeamDTO> GetByIdAsync(int id)
		{
			Team team = await _teamRepository.GetByIdAsync(id);
			return TeamMapper.ToDTO(team);
		}
		public async Task<List<TeamDTO>> GetAllAsync(int page, int pageSize, string? search = null)
		{
			var teams = await _teamRepository.GetAllAsync(page, pageSize, search);
			return teams.Select(TeamMapper.ToDTO).ToList();
		}
		public async Task<List<TeamDTO>> GetByTournamentAsync(int tournamentId, string? search = null)
		{
			var teams = await _teamRepository.GetByTournamentAsync(tournamentId, search);
			return teams.Select(TeamMapper.ToDTO).ToList();
		}
		public async Task ChangeGeneralInfo(int id, string name, string? city, byte[]? photo, string? photoMime)
		{
			var team = await _teamRepository.GetByIdAsync(id);
			team.SetName(name);
			team.SetCity(city);
			team.SetPhoto(photo, photoMime);

			await _unitOfWork.SaveChangesAsync();
		}
	}
}
