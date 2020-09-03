using System;
using System.Collections;
using System.Collections.Generic;
using blaseball.db;
using blaseball.runtime;
using blaseball.runtime.events;
using blaseball.ui;
using blaseball.vo;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using Zenject;

public class GameViewer : MonoBehaviour
{
	[Inject] public GameRunner gameRunner; 
	[Inject] public IUILogger Logger;
	[Inject] public IBlaseballDatabase Database;
	[Inject] public BBPlaybook Playbook;

	protected BBGame game;

	protected Queue<BBGameState> queue;

	protected bool ReadyToProcessNewPlay = false;
	protected BBGameState currentState;
	protected BBAbstractPlay currentPlay;
	public TVCameraGraphicsMasterControl cameraGraphicsMasterControl;

	[Header("Cameras")]
	public CinemachineVirtualCamera cameraOne;
	public CinemachineVirtualCamera cameraTwo;
	public Transform cameraFollower;

	[Header("Timelines!")]
	public PlayableDirector director;
	public List<TimelineAsset> technicalDifficulties;
	public List<TimelineAsset> newBatterAnimations;
	public List<TimelineAsset> strikeSwingingAnimations;
	public List<TimelineAsset> strikeLookingAnimations;
	[Header("Actors!")]
	public Animator DefaultBatter;
	public Animator DefaultCatcher;
	public Animator DefaultUmpire;
	public Animator DefaultPitcher;
	[Header("Game Objects")]
	public Transform Bat;
	public Transform Ball;
	
	void Start()
	{

		game = gameRunner.getFocusedGame();
		queue = new Queue<BBGameState>();

		if(game == null){
			SceneManager.LoadScene("Title Scene");
			return;
		}

		queue = new Queue<BBGameState>();
		game.OnUpdateReady += LogUpdate;
		ReadyToProcessNewPlay = true;

		currentState = game.GetUpdate(gameRunner.CurrentIndex);
		if(currentState != null) queue.Enqueue(currentState);
		HandleTechnicalDifficulties(null);

	}

	void Update() {
		// If ready to process a new play
		if(ReadyToProcessNewPlay) {
			if(queue.Count > 0) {
				ReadyToProcessNewPlay = false;
				ProcessPlay(queue.Dequeue());
			}
		} else {
			// Why? Probably because we're playing a movie...

			// if current movie is over, play idle animations of crowd and stuff
		}
	}

	private void LogUpdate(BBGameState gameState)
	{
		queue.Enqueue(gameState);
	}

	private void ProcessPlay(BBGameState gameState) {
		currentState = gameState;
		currentPlay = Playbook.GetPlayFromState(gameState);
		CleanScene();

		switch(currentPlay) {
			/*
			case StartGamePlay case1: 
			HandleStartGame(case1); break;
			case BBTopOfInningPlay case2:
			HandleTopOfInnings(case2); break;
			case BBBottomOfInningPlay case3:
			HandleBottomOfInnings(case3); break;
			
			case BBBatterAtPlatePlay case4:
			BatterUp(case4); break;
			*/

			case BBBatterAtPlatePlay bBatterAtPlatePlay:
			BatterUp(bBatterAtPlatePlay);
			break;

			case BBStrikePlay bBStrikePlay:
			Strike(bBStrikePlay);
			break;


			default:
			HandleTechnicalDifficulties(currentPlay);
			break;
		}
		//Logger.Log(gameState.lastUpdate);

		//StartCoroutine("ArbitraryWait");
	}

	public void OnAnimationComplete () {
		Logger.Log("Viewer Ready for next update");
		ReadyToProcessNewPlay = true;
		if(gameRunner.CurrentIndex != -1) {
			gameRunner.CurrentIndex++;
			BBGameState state = game.GetUpdate(gameRunner.CurrentIndex);
			if(state != null) {
				LogUpdate(state);
			}
		}
	}

	public void OnUIUpdateRequest () {
		switch(currentPlay) {
			case BBStrikePlay bbStrike:
				cameraGraphicsMasterControl.ShowStrike(bbStrike);
				break;

		}
	}
	
	private void HandleTechnicalDifficulties(BBAbstractPlay caseFail)
	{
		SetupAndPlay(technicalDifficulties[0]);
		if(caseFail == null) {
			cameraGraphicsMasterControl.ShowTechnicalDifficulties("Waiting for setup!");
		}
		else {
			cameraGraphicsMasterControl.ShowTechnicalDifficulties(caseFail.gameState.lastUpdate);
		}
		//technicalDifficulties[0].Stop();
		//technicalDifficulties[0].Play(technicalDifficulties[0].playableAsset);
		//Logger.Log($"UNHANDLED: {caseFail.gameState.lastUpdate} ({caseFail.ToString()})");
	}

	private void BatterUp(BBBatterAtPlatePlay play) {
		BBPlayer batter = Database.GetPlayer(play.Batter());
		BBTeam battingTeam = Database.GetTeam(play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		BBTeam fieldingTeam = Database.GetTeam(!play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		
		SetupBatter(play.Batter(), battingTeam.id);
		SetupCatcher(fieldingTeam.id);

		
		SetupAndPlay(newBatterAnimations[0]);
		cameraGraphicsMasterControl.ShowBatter(batter, battingTeam);
		
		//Logger.Log($"Batting now for the {team.nickname} is {batter.name}, a {Math.Round(BBPlayer.BatterRating(batter) * 10) / 2} star batter");
	}

	private void Strike(BBStrikePlay play) {
		BBTeam battingTeam = Database.GetTeam(play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		BBTeam fieldingTeam = Database.GetTeam(!play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		

		SetupBatter(play.Batter(), battingTeam.id);
		SetupPitcher(play.Pitcher(), fieldingTeam.id);

		Debug.Log($"STRIKE!: {play.TypeOfStrike}");

		switch(play.TypeOfStrike){
			case BBStrikePlay.Strike.LOOKING :
			SetupAndPlay(strikeLookingAnimations[0]);
			break;
			case BBStrikePlay.Strike.SWINGING :
			SetupAndPlay(strikeSwingingAnimations[0]);
			break;
			default:
			HandleTechnicalDifficulties(play);
			break;
		}

		//cameraGraphicsMasterControl.ShowBatter(batter, battingTeam);
	
	}

	  
	/// <summary>
	/// Todo, get the batter custom model if one is available
	/// </summary>
	/// <param name="batterID"></param>
	/// <param name="teamID"></param>
	private void SetupBatter(string batterID, string teamID)
	{
		BBPlayer player = Database.GetPlayer(batterID);
		BBTeam team = Database.GetTeam(teamID);

		Color teamColorMain = Color.white;
		Color teamColorSecond = Color.white;

		ColorUtility.TryParseHtmlString(team.mainColor, out teamColorMain);
		ColorUtility.TryParseHtmlString(team.secondaryColor, out teamColorSecond);

		Debug.Log("Setting Up Batter");
		Bat.gameObject.SetActive(true);
		Animator batter = GetBatter();

		Transform t = batter.gameObject.transform.FindDeepChild("mixamorig:LeftHand");
		Bat.SetParent(t, false);

		SkinnedMeshRenderer[] meshes = batter.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		
		meshes[1].material = new Material(meshes[0].material.shader);
		meshes[1].material.color = teamColorMain;
		meshes[0].material = new Material(meshes[1].material.shader);
		meshes[0].material.color = teamColorSecond;
	}
	
	/// <summary>
	/// Todo, get the batter pitcher model if one is available
	/// </summary>
	/// <param name="batterID"></param>
	/// <param name="teamID"></param>
	private void SetupPitcher(string batterID, string teamID)
	{
		BBPlayer player = Database.GetPlayer(batterID);
		BBTeam team = Database.GetTeam(teamID);

		Color teamColorMain = Color.white;
		Color teamColorSecond = Color.white;

		ColorUtility.TryParseHtmlString(team.mainColor, out teamColorMain);
		ColorUtility.TryParseHtmlString(team.secondaryColor, out teamColorSecond);

		SkinnedMeshRenderer[] meshes = DefaultPitcher.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		
		meshes[1].material = new Material(meshes[0].material.shader);
		meshes[1].material.color = teamColorMain;
		meshes[0].material = new Material(meshes[1].material.shader);
		meshes[0].material.color = teamColorSecond;
	}

	/// <summary>
	/// Todo, get the batter catcher model if one is available
	/// </summary>
	/// <param name="batterID"></param>
	/// <param name="teamID"></param>
	private void SetupCatcher(string teamID)
	{
		BBTeam team = Database.GetTeam(teamID);
		Color teamColorMain = Color.white;
		Color teamColorSecond = Color.white;

		ColorUtility.TryParseHtmlString(team.mainColor, out teamColorMain);
		ColorUtility.TryParseHtmlString(team.secondaryColor, out teamColorSecond);

		SkinnedMeshRenderer[] meshes = DefaultCatcher.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		
		meshes[1].material = new Material(meshes[0].material.shader);
		meshes[1].material.color = teamColorMain;
		meshes[0].material = new Material(meshes[1].material.shader);
		meshes[0].material.color = teamColorSecond;
	}

	private void SetupAndPlay(TimelineAsset playableAsset)
	{

		var bindings = playableAsset.outputs;
		var enumerator = bindings.GetEnumerator();

		while(enumerator.MoveNext()) {
			PlayableBinding binding = enumerator.Current;
			switch(binding.streamName) {
				case "Signal" : 
				director.SetGenericBinding(binding.sourceObject, GetComponent<SignalReceiver>()); break;
				
				case "Camera1" :  
				case "Camera1_Active" :  
				cameraOne.gameObject.SetActive(true);
				director.SetGenericBinding(binding.sourceObject, cameraOne.GetComponent<Animator>()); break;
				
				case "Camera2" :  
				case "Camera2_Active" :  
				cameraTwo.gameObject.SetActive(true);
				director.SetGenericBinding(binding.sourceObject, cameraTwo.GetComponent<Animator>()); break;

				case "Target" : 
				director.SetGenericBinding(binding.sourceObject, cameraFollower.GetComponent<Animator>()); break;

				case "Batter" :
				director.SetGenericBinding(binding.sourceObject, GetBatter());
				break;

				case "Catcher" :
				director.SetGenericBinding(binding.sourceObject, GetCatcher());
				break;

				case "Umpire" :
				director.SetGenericBinding(binding.sourceObject, GetUmpire());
				break;

				case "Ball" :
				director.SetGenericBinding(binding.sourceObject, Ball.GetComponent<Animator>());
				break;

			}
		}

		director.Play(playableAsset, DirectorWrapMode.Loop);
	}


	private void CleanScene()
	{
		cameraGraphicsMasterControl.ClearAllGraphics();
		cameraTwo.gameObject.SetActive(false);
		cameraOne.gameObject.SetActive(false);
		Bat.gameObject.SetActive(false);
		cameraGraphicsMasterControl.ShowOnlyMajorItems();
	}
	
	private Animator GetBatter()
	{
		return DefaultBatter;
	}
	private Animator GetUmpire()
	{
		return DefaultUmpire;
	}
	private Animator GetCatcher()
	{
		return DefaultCatcher;
	}
	private Animator GetPitcher() 
	{
		return DefaultPitcher;
	}
}
