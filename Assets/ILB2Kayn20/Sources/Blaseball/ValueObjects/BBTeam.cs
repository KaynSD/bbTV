namespace blaseball.vo {
	[System.Serializable]
	public class BBTeam {
		public string id;
		public string fullName, location, nickname;
		public string mainColor, secondaryColor;
		public string emoji;
		public string slogan;
		public int championships;
		public int shameRuns, totalShames, totalShamings, seasonShames, seasonShamings;

		public string[] lineup, rotation, bullpen, bench;
		public string[] seasonAttributes, permanentAttributes;
	}
}