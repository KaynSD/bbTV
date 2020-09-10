using System;
using System.Collections;
using System.Collections.Generic;
using blaseball.db;
using blaseball.file;
using blaseball.runtime;
using blaseball.runtime.events;
using blaseball.ui;
using blaseball.vo;
using Cinemachine;
using kaynsd.helpers.rng;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Zenject;

public class GameViewer : MonoBehaviour
{
	[Inject] public GameRunner gameRunner; 
	[Inject] public IUILogger Logger;
	[Inject] public IBlaseballDatabase Database;
	[Inject] public BBPlaybook Playbook;
	[Inject] public IBlaseballFileLoader Loader;

	[Header("Graphics Package")]
	public TVCameraGraphicsMasterControl cameraGraphicsMasterControl;

	[Header("Cameras")]
	public CinemachineVirtualCamera cameraOne;
	public CinemachineVirtualCamera cameraTwo;
	public Transform cameraFollower;

	[Header("Timelines!")]
	public PlayableDirector director;
	[Header(" - Defaults / Technical Difficulties")]
	public List<TimelineAsset> technicalDifficulties;
	[Header(" - Changeups")]
	public List<TimelineAsset> newBatterAnimations;
	[Header(" - At Bats")]
	public List<TimelineAsset> strikeSwingingAnimations;
	public List<TimelineAsset> strikeLookingAnimations;
	public List<TimelineAsset> strikeOutLookingAnimations;
	public List<TimelineAsset> strikeOutSwingingAnimations;
	public List<TimelineAsset> ballcountAnimations;
	[Header("Actors!")]
	public CharacterCutsceneControl DefaultCharacterPrefab;
	public CharacterCutsceneControl DefaultNPCPrefab;
	[Header("Game Objects")]
	public Transform Bat;
	public Transform Ball;

	[Header("Scoreboard Megatron Materials")]
	public Material Scoreboard;
	public Material AwayTeamPanel;
	public Material HomeTeamPanel;


	// The game we are viewing goes here
	protected BBGame game;
	protected int historicalPlaybackCurrentIndex;

	// The queue for plays and how to handle them and the game state goes here
	protected Queue<BBGameState> queue;

	protected bool ReadyToProcessNewPlay = false;
	protected BBGameState currentState;
	protected BBGameState previousState;
	protected BBAbstractPlay currentPlay;

	protected CharacterCutsceneControl Batter;
	protected CharacterCutsceneControl Pitcher;
	protected CharacterCutsceneControl Umpire;
	protected CharacterCutsceneControl Catcher;

	protected List<float> PregeneratedRandomValues;

	

	void Start()
	{

		game = gameRunner.getFocusedGame();
		if(game == null){
			SceneManager.LoadScene(Constants.SCENE_TITLE);
			return;
		}


		PRNG prng = new ParkMiller(game.GameID);
		PregeneratedRandomValues = new List<float>();
		for(int i = 0; i < 1000; i++) {
			PregeneratedRandomValues.Add(prng.next());
		}


		queue = new Queue<BBGameState>();
		queue = new Queue<BBGameState>();
		game.OnUpdateReady += LogUpdate;
		ReadyToProcessNewPlay = true;

		historicalPlaybackCurrentIndex = game.isRunning ? -1 : 0;
		currentState = game.GetUpdate(historicalPlaybackCurrentIndex);
		if(currentState != null) queue.Enqueue(currentState);
		HandleTechnicalDifficulties(null);

		if(game.isRunning) {
			Debug.Log("Rewind Off");
			cameraGraphicsMasterControl.DisableRewind();
		} else {
			Debug.Log("Rewind On");
			cameraGraphicsMasterControl.EnableRewind();
			cameraGraphicsMasterControl.rewindPanel.OnChanged += RewindToPoint;
		}
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

	public void Quit() {
		SceneManager.LoadScene(Constants.SCENE_TITLE);
	}

	private void RewindToPoint(int value) {
		historicalPlaybackCurrentIndex = value;
		if(historicalPlaybackCurrentIndex != game.HistoryLength) {
			BBGameState state = game.GetUpdate(value);
			if(state != null) {
				LogUpdate(state);
				ReadyToProcessNewPlay = true;
			}
		}
	}

	private void LogUpdate(BBGameState gameState)
	{
		queue.Enqueue(gameState);
	}

	private void ProcessPlay(BBGameState gameState, int index = -1) {
		cameraGraphicsMasterControl.rewindPanel.SetLength(game.HistoryLength - 1, historicalPlaybackCurrentIndex);

		if(historicalPlaybackCurrentIndex == -1) {
			index = game.HistoryLength;
		}

		previousState = currentState;
		currentState = gameState;
		currentPlay = Playbook.GetPlayFromState(gameState, index);
		CleanScene();

		switch(currentPlay) {
			case BBBatterAtPlatePlay bBatterAtPlatePlay:
			BatterUp(bBatterAtPlatePlay);
			break;

			case BBStrikePlay bBStrikePlay:
			Strike(bBStrikePlay);
			break;

			case BBBallPlay bBallPlay:
			BallCount(bBallPlay);
			break;

			case BBStrikeOutPlay bBStrikeOutPlay:
			StrikeOut(bBStrikeOutPlay);
			break;


			default:
			HandleTechnicalDifficulties(currentPlay);
			break;
		}
	}

	/// Triggered by a Signal when an animation is complete; or complete as far as the game
	/// renderer is concerned. There may be additional animation elements for idles and waiting
	/// for connection, but the timeline triggers this when the information is shown to the player
	/// and ready for the next part
	public void OnAnimationComplete () {

		Logger.Log("Viewer Ready for next update");
		// If we're in history mode, we get the next value immediately from the history
		if(historicalPlaybackCurrentIndex != -1 && historicalPlaybackCurrentIndex != game.HistoryLength - 1) {
			historicalPlaybackCurrentIndex++;
			BBGameState state = game.GetUpdate(historicalPlaybackCurrentIndex);
			if(state != null) {
				ProcessPlay(state, historicalPlaybackCurrentIndex);
			}
		}

		// Otherwise we wait for the server and follow the queue

		// Either way, ready to process the next play
		ReadyToProcessNewPlay = true;
	}

	/// <summary>
	/// Triggered by a signal, this is a signal to pass "up to date" information to the UI;
	/// showing a strike updated value or new runs or whatnot  
	/// </summary>
	public void OnUIUpdateRequest () {
		switch(currentPlay) {
			case BBStrikePlay bbStrike:
			case BBStrikeOutPlay bbStrikeOut:
				cameraGraphicsMasterControl.ShowStrike(currentPlay);
				break;
			case BBBallPlay bBBallPlay:
				cameraGraphicsMasterControl.ShowBallCount(bBBallPlay);
				break;

		}
	}

	private TimelineAsset GetRandomAnimation(List<TimelineAsset> animations, int playIndex) {
		Debug.Log($"Play Index: {playIndex}");
		while(playIndex < 0) playIndex += 1000;
		float value = PregeneratedRandomValues[playIndex % 1000];
		int index = Mathf.FloorToInt(animations.Count * value);
		return animations[index];
	}
	
	private void HandleTechnicalDifficulties(BBAbstractPlay caseFail)
	{
		if(caseFail == null) {
			SetupAndPlay(GetRandomAnimation(technicalDifficulties, 0));
			cameraGraphicsMasterControl.ShowTechnicalDifficulties("Waiting for setup!");
		}
		else {
			SetupAndPlay(GetRandomAnimation(technicalDifficulties, caseFail.playIndex));
			cameraGraphicsMasterControl.ResetPlate(caseFail);
			cameraGraphicsMasterControl.ShowTechnicalDifficulties(caseFail.gameState.lastUpdate);
			cameraGraphicsMasterControl.UpdateScores(caseFail.gameState);
		}
	}

	private void BatterUp(BBBatterAtPlatePlay play) {
		cameraGraphicsMasterControl.ResetPlate(play);

		BBPlayer batter = play.Batter();
		BBTeam battingTeam = Database.GetTeam(play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		BBTeam fieldingTeam = Database.GetTeam(!play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		
		SetupBatter(play.Batter(), battingTeam?.id ?? "-1");
		SetupCatcher(Database.GetPlayer(fieldingTeam?.lineup[8] ?? "-1"), fieldingTeam?.id ?? "-1");

		SetupAndPlay(GetRandomAnimation(newBatterAnimations, play.playIndex));
		cameraGraphicsMasterControl.ShowBatter(batter, battingTeam);
		cameraGraphicsMasterControl.UpdateScores(play.gameState);
	}

	private void Strike(BBStrikePlay play) {
		BBTeam battingTeam = Database.GetTeam(play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		BBTeam fieldingTeam = Database.GetTeam(!play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		
		SetupBatter(play.Batter(), battingTeam?.id ?? "-1" );
		SetupPitcher(play.Pitcher(), fieldingTeam?.id ?? "-1");

		switch(play.TypeOfStrike){
			case BBStrikePlay.Strike.LOOKING :
			SetupAndPlay(GetRandomAnimation(strikeLookingAnimations, play.playIndex));
			break;
			case BBStrikePlay.Strike.SWINGING :
			SetupAndPlay(GetRandomAnimation(strikeSwingingAnimations, play.playIndex));
			break;
			default:
			HandleTechnicalDifficulties(play);
			break;
		}

		cameraGraphicsMasterControl.UpdateScores(play.gameState);
	}
	private void StrikeOut(BBStrikeOutPlay play) {
		BBTeam battingTeam = Database.GetTeam(play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		BBTeam fieldingTeam = Database.GetTeam(!play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		
		BBPlayer batter = play.Batter();
		BBPlayer pitcher = play.Pitcher();
		

		SetupBatter(batter, battingTeam.id);
		SetupPitcher(pitcher, fieldingTeam.id);

		switch(play.TypeOfStrikeOut){
			case BBStrikeOutPlay.StrikeOut.LOOKING :
			SetupAndPlay(GetRandomAnimation(strikeOutLookingAnimations, play.playIndex));
			break;
			case BBStrikeOutPlay.StrikeOut.SWINGING :
			SetupAndPlay(GetRandomAnimation(strikeOutSwingingAnimations, play.playIndex));
			break;
			default:
			HandleTechnicalDifficulties(play);
			break;
		}

		cameraGraphicsMasterControl.UpdateScores(play.gameState);
	}

	private void BallCount(BBBallPlay play) {
		BBTeam battingTeam = Database.GetTeam(play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		BBTeam fieldingTeam = Database.GetTeam(!play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		
		SetupBatter(play.Batter(), battingTeam?.id ?? "Unknown");
		SetupPitcher(play.Pitcher(), fieldingTeam?.id ?? "Unknown");

		SetupAndPlay(GetRandomAnimation(ballcountAnimations, play.playIndex));
		cameraGraphicsMasterControl.UpdateScores(play.gameState);
	}

	  
	/// <summary>
	/// Todo, get the batter custom model if one is available
	/// </summary>
	/// <param name="batterID"></param>
	/// <param name="teamID"></param>
	private void SetupBatter(BBPlayer player, string teamID)
	{
		BBTeam team = Database.GetTeam(teamID);

		Color teamColorMain = Color.white;
		Color teamColorSecond = Color.white;

		ColorUtility.TryParseHtmlString(team?.mainColor ?? "#999999", out teamColorMain);
		ColorUtility.TryParseHtmlString(team?.secondaryColor ?? "#dddddd", out teamColorSecond);

		Debug.Log($"Setting Up Batter: {player}");
		Bat.gameObject.SetActive(true);

		Bat.SetParent(GetBatter(player).LeftHandAttachmentReference, false);

		GetBatter(player).SetPlayerName(player?.name ?? "Unknown Player");
		GetBatter(player).SetPrimaryColor(teamColorMain);
		GetBatter(player).SetSecondaryColor(teamColorSecond);
	}
	
	/// <summary>
	/// Todo, get the batter pitcher model if one is available
	/// </summary>
	/// <param name="batterID"></param>
	/// <param name="teamID"></param>
	private void SetupPitcher(BBPlayer player, string teamID)
	{
		BBTeam team = Database.GetTeam(teamID);

		Color teamColorMain = Color.white;
		Color teamColorSecond = Color.white;

		ColorUtility.TryParseHtmlString(team?.mainColor ?? "#999999", out teamColorMain);
		ColorUtility.TryParseHtmlString(team?.secondaryColor ?? "#dddddd", out teamColorSecond);

		GetPitcher(player).SetPlayerName(player?.name ?? "Unknown Player");
		GetPitcher(player).SetPrimaryColor(teamColorMain);
		GetPitcher(player).SetSecondaryColor(teamColorSecond);
	}

	/// <summary>
	/// Todo, get the batter catcher model if one is available
	/// </summary>
	/// <param name="batterID"></param>
	/// <param name="teamID"></param>
	private void SetupCatcher(BBPlayer player, string teamID)
	{
		BBTeam team = Database.GetTeam(teamID);

		Color teamColorMain = Color.white;
		Color teamColorSecond = Color.white;

		ColorUtility.TryParseHtmlString(team?.mainColor ?? "#999999", out teamColorMain);
		ColorUtility.TryParseHtmlString(team?.secondaryColor ?? "#dddddd", out teamColorSecond);

		GetCatcher(player).SetPlayerName(player?.name ?? "Unknown Player");
		GetCatcher(player).SetPrimaryColor(teamColorMain);
		GetCatcher(player).SetSecondaryColor(teamColorSecond);
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
				director.SetGenericBinding(binding.sourceObject, Batter.animator);
				break;

				case "Catcher" :
				director.SetGenericBinding(binding.sourceObject, Catcher.animator);
				break;

				case "Umpire" :
				director.SetGenericBinding(binding.sourceObject, GetUmpire().animator);
				break;

				case "Pitcher" :
				director.SetGenericBinding(binding.sourceObject, Pitcher.animator);
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
		Bat.SetParent(transform, false);

		cameraGraphicsMasterControl.ShowOnlyMajorItems();

		if(Batter != null) Destroy(Batter.gameObject); Batter = null;
		if(Catcher != null) Destroy(Catcher.gameObject); Catcher = null;
		if(Pitcher != null) Destroy(Pitcher.gameObject); Pitcher = null;
	}
	
	private CharacterCutsceneControl GetBatter(BBPlayer player)
	{
		if(Batter != null) return Batter;

		if(player != null) Batter = LoadCharacter(player);
		if(Batter == null) Batter = Instantiate(DefaultCharacterPrefab).GetComponent<CharacterCutsceneControl>();
		return Batter;
	}

	private CharacterCutsceneControl GetCatcher(BBPlayer player)
	{
		if(Catcher != null) return Catcher;

		if(player != null) Catcher = LoadCharacter(player);
		if(Catcher == null) Catcher = Instantiate(DefaultCharacterPrefab).GetComponent<CharacterCutsceneControl>();
		return Catcher;
	}
	private CharacterCutsceneControl GetPitcher(BBPlayer player) 
	{
		if(Pitcher != null) return Pitcher;

		if(player != null) Pitcher = LoadCharacter(player);
		if(Pitcher == null) Pitcher = Instantiate(DefaultCharacterPrefab).GetComponent<CharacterCutsceneControl>();
		return Pitcher;
	}


	private CharacterCutsceneControl LoadCharacter(BBPlayer player)
	{
		try {
			var myLoadedAssetBundle = AssetBundle.LoadFromFile(Loader.GetPlayerCustomModelPath(player.id));
			if (myLoadedAssetBundle == null)
			{
				Debug.Log("Failed to load AssetBundle!");
				return null;
			}

			var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("Body");
			GameObject playerModel = (GameObject)Instantiate(prefab);

			myLoadedAssetBundle.Unload(false);
			
			return playerModel.GetComponent<CharacterCutsceneControl>();
		} catch (Exception e) {
			return null;
		}
	}
	private CharacterCutsceneControl GetUmpire()
	{
		if(Umpire == null) Umpire = Instantiate(DefaultNPCPrefab).GetComponent<CharacterCutsceneControl>();
		return Umpire;
	}
}
