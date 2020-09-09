using UnityEngine;

[System.Serializable]
public class ApplicationConfig {
	public string BlaseballServiceSSEPath = "https://www.blaseball.com/events/streamData";
	public string RootDirectory = Application.streamingAssetsPath;
	public string DatabaseLocation {get => $"{RootDirectory}/data.db"; }

	public string VersionNumber {get => Application.version;}
	public string VersionName;

	public TitleScreenSettings titleScreenSettings = new TitleScreenSettings();

}
