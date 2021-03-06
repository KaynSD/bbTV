﻿using System;
using System.Collections;
using System.Collections.Generic;
using blaseball.db;
using blaseball.file;
using blaseball.runtime;
using blaseball.service;
using blaseball.ui;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MainController : MonoInstaller
{
	ApplicationConfig configuration;
	public override void InstallBindings() {
		configuration = new ApplicationConfig();

		configuration.VersionName = "Jessica Telephone";
		Container.Bind<ApplicationConfig>().FromInstance(configuration).AsSingle();

		Container.Bind<IUILogger>().FromComponentInNewPrefabResource("Logging Panel").AsSingle().NonLazy();

		// Instantiate our control objects
		Container.Bind<IBlaseballFileLoader>().To<JTFileLoader>().FromNew().AsSingle().NonLazy();
		Container.Bind<IBlaseballDatabase>().To<JTDatabase>().FromNew().AsSingle().NonLazy();
		Container.Bind<IBlaseballResultsService>().To<JTService>().FromNew().AsSingle().NonLazy();
		Container.Bind<GameRunner>().FromNew().AsSingle().Lazy();
		Container.Bind<BBPlaybook>().FromNew().AsSingle().Lazy();
		Container.Bind<BBAnnouncements>().FromNew().AsSingle().Lazy();
	}

	public void OnResolved() {
		SceneManager.LoadScene(Constants.SCENE_TITLE);
	}


}
