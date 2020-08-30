namespace blaseball.vo {
	public delegate void BBGameStateDelegate(BBGameState gameState);
	[System.Serializable]
	public class BBGameState {
		public string id;
		public string lastUpdate;

		public int day, season, inning, seriesIndex, seriesLength, phase, weather;
		public bool topOfInning, finalized, gameStart, shame, gameComplete, isPostseason;

		public string awayTeam, homeTeam;
		public int awayScore, homeScore;

		public int atBatBalls, atBatStrikes, halfInningOuts, halfInningScore;



		public string awayPitcher, awayBatter;
		public string homePitcher, homeBatter;
		

		public int[] basesOccupied;
		public string[] baseRunners;

		public float awayOdds, homeOdds;

		/// <summary>
		/// Allergies, Incinerations, Feedback. If present, they're for the WHOLE game
		/// </summary>
		public string[] outcomes;
	}
}