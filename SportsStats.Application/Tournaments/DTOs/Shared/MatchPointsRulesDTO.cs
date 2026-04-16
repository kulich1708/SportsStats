using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Shared
{
	public record MatchPointsRulesDTO(
		int WinPoints,
		int OtWinPoints,
		int LossPoints,
		int OtLossPoints,
		int? DrawPoints
	);
}
