using System.Collections;
using System.Collections.Generic;
using System.IO;
using blaseball.db;
using blaseball.file;
using UnityEngine;
using Zenject;

public class TestLoad : MonoBehaviour
{
	[Inject] IBlaseballDatabase database;
	[Inject] IBlaseballFileLoader loader;
	void Start() {
		
		database.Load();

		var myLoadedAssetBundle = AssetBundle.LoadFromFile(loader.GetTeam3DLogoPath("ca3f1c8c-c025-4d8e-8eef-5be6accbeb16"));
		if (myLoadedAssetBundle == null)
		{
			Debug.Log("Failed to load AssetBundle!");
			return;
		}

		var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("Logo");
		Instantiate(prefab);

		myLoadedAssetBundle.Unload(false);
	}
}
