using System.Collections;
using System.Collections.Generic;
using blaseball;
using blaseball.db;
using blaseball.service;
using TMPro;
using UnityEngine;
using Zenject;

public class TitleScreen : MonoBehaviour
{
	[Inject] public IBlaseballDatabase database;
	[Inject]	public IBlaseballResultsService service;


	public TextMeshProUGUI lastDatabaseUpdateText;
	void Start() {
		database.Load();
		service.Connect();
		lastDatabaseUpdateText.SetText($"Last Cleaned: {Helper.GetLastUpdatedText(database.lastUpdated)}");
	}

	

	public void DownloadDatabase() {
		service.BuildDatabase("", DatabaseConfigurationOptions.COMPLETE);
	}

	public void StartService() {
		service.Connect();
	}
}