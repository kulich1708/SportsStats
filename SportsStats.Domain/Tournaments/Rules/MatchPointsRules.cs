using SportsStats.Domain.Shared.Enums;

public record MatchPointsRules
{
	public int WinPoints { get; private set; }      // Победа в основное время
	public int OTWinPoints { get; private set; }    // Победа в овертайме
	public int LossPoints { get; private set; }     // Поражение в основное время
	public int OTLossPoints { get; private set; }   // Поражение в овертайме
	public int? DrawPoints { get; private set; }    // Ничья (если есть)

	private MatchPointsRules() { }
	public MatchPointsRules(int win, int otWin, int loss, int otLoss, int? draw = null)
	{
		WinPoints = win;
		OTWinPoints = otWin;
		LossPoints = loss;
		OTLossPoints = otLoss;
		DrawPoints = draw;
	}

	public int GetPoints(MatchWinType outcome) => outcome switch
	{
		MatchWinType.REGULATION_WIN => WinPoints,
		MatchWinType.OT_WIN => OTWinPoints,
		MatchWinType.REGULATION_LOSS => LossPoints,
		MatchWinType.OT_LOSS => OTLossPoints,
		MatchWinType.DRAW => DrawPoints ?? 0,
		_ => 0
	};
	public static MatchPointsRules CreateKHLRules()
	{
		return new MatchPointsRules(win: 2, otWin: 2, loss: 0, otLoss: 1);
	}
}