using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Shared
{
	public record MatchPointsRulesDTO(
		int WinPoints,
		int OTWinPoints,
		int LossPoints,
		int OTLossPoints,
		int? DrawPoints
	);
}
