using SportsStats.Application.Tournaments.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.Mappers.Rules
{
	public class MatchPointsRulesMapper
	{
		public static MatchPointsRulesDTO ToDTO(MatchPointsRules rules) => new(
			rules.WinPoints,
			rules.LossPoints,
			rules.OTWinPoints,
			rules.OTLossPoints,
			rules.ShootoutWinPoints,
			rules.ShootoutLossPoints,
			rules.DrawPoints
		);

		public static MatchPointsRules ToDomain(MatchPointsRulesDTO rules) => new(
			rules.WinPoints,
			rules.LossPoints,
			rules.OTWinPoints,
			rules.OTLossPoints,
			rules.ShootoutWinPoints,
			rules.ShootoutLossPoints,
			rules.DrawPoints
		);
	}
}
