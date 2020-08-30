using UnityEngine;

[System.Serializable]
public class ApplicationConfig {
	public string BlaseballServiceSSEPath = "https://www.blaseball.com/events/streamData";
	public string RootDirectory = Application.streamingAssetsPath;
	public string DatabaseLocation {get => $"{RootDirectory}/data.db"; }

}
