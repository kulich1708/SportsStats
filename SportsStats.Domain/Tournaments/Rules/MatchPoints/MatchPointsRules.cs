using SportsStats.Domain.Shared;
using SportsStats.Domain.Shared.Enums;

namespace SportsStats.Domain.Tournaments.Rules.MatchPoints
{
	public record MatchPointsRules
	{
		public int WinPoints { get; private set; }      // Победа в основное время
		public int LossPoints { get; private set; }     // Поражение в основное время
		public int? OTWinPoints { get; private set; }    // Победа в овертайме
		public int? OTLossPoints { get; private set; }   // Поражение в овертайме
		public int? ShootoutWinPoints { get; private set; }
		public int? ShootoutLossPoints { get; private set; }
		public int? DrawPoints { get; private set; }    // Ничья (если есть)

		private MatchPointsRules() { }
		public MatchPointsRules(
			int winPoints, int lossPoints,
			int? otWinPoints = null, int? otLossPoints = null,
			int? shootoutWinPoints = null, int? shootoutLossPoints = null,
			int? drawPoints = null)
		{
			WinPoints = winPoints;
			LossPoints = lossPoints;
			OTWinPoints = otWinPoints;
			OTLossPoints = otLossPoints;
			ShootoutWinPoints = shootoutWinPoints;
			ShootoutLossPoints = shootoutLossPoints;
			DrawPoints = drawPoints;
		}

		public int? GetPoints(MatchWinType outcome) => outcome switch
		{
			MatchWinType.REGULATION_WIN => WinPoints,
			MatchWinType.REGULATION_LOSS => LossPoints,
			MatchWinType.OT_WIN => OTWinPoints,
			MatchWinType.OT_LOSS => OTLossPoints,
			MatchWinType.SHOOTOUT_WIN => ShootoutWinPoints,
			MatchWinType.SHOOTOUT_LOSS => ShootoutLossPoints,
			MatchWinType.DRAW => DrawPoints,
			_ => null
		};
		public void ValidateRules(bool hasOvertime, bool hasShootout, bool IsDrawPossible)
		{
			if (hasOvertime && (!OTWinPoints.HasValue || !OTLossPoints.HasValue))
				throw new DomainException(MatchPointsRulesError.OvertimePointsRequired);
			if (hasShootout && (!ShootoutWinPoints.HasValue || !ShootoutLossPoints.HasValue))
				throw new DomainException(MatchPointsRulesError.ShootoutPointsRequired);
			if (IsDrawPossible && !DrawPoints.HasValue)
				throw new DomainException(MatchPointsRulesError.DrawPointsRequired);

			if (!hasOvertime)
			{
				OTWinPoints = null;
				OTLossPoints = null;
			}
			if (!hasShootout)
			{
				ShootoutWinPoints = null;
				ShootoutLossPoints = null;
			}
			if (!IsDrawPossible)
				DrawPoints = null;
		}

		public static MatchPointsRules CreateKHLPointsRules()
		{
			return new(
				winPoints: 2,
				lossPoints: 0,
				otWinPoints: 1,
				otLossPoints: 0,
				shootoutWinPoints: 1,
				shootoutLossPoints: 0);
		}
	}
}