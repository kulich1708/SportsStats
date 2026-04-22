using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Shared
{
	public record MatchPointsRulesDTO(
		int WinPoints,
		int LossPoints,
		int? OTWinPoints,
		int? OTLossPoints,
		int? ShootoutWinPoints,
		int? ShootoutLossPoints,
		int? DrawPoints
	);
}
