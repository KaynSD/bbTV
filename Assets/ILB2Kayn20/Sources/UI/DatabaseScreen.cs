using System.Collections;
using System.Collections.Generic;
using blaseball.db;
using blaseball.service;
using UnityEngine;

public class DatabaseScreen : MonoBehaviour
{
	public JTService service;
	public JTDatabase database;
	public DatabaseLeaguesPanel DatabaseLeaguesPanel;
	void Start() {
		//service = new JTService();
		//database = JTDatabase.Load("C:/Users/Keith Evans/Desktop/wafc.blbd");

		//DatabaseLeaguesPanel.ShowSubleagues(database);
	}
}
