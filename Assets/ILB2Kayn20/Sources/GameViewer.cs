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
using UnityEngine.UI;
using Zenject;

public class GameViewer : MonoBehaviour
{
	[Inject] public GameRunner gameRunner; 
	[Inject] public IUILogger Logger;
	[Inject] public IBlaseballDatabase Database;
	[Inject] public BBPlaybook Playbook;
	public TVCameraGraphicsMasterControl cameraGraphicsMasterControl;

	// The game we are viewing goes here
	protected BBGame game;
	protected int historicalPlaybackCurrentIndex;

	// The queue for plays and how to handle them and the game state goes here
	protected Queue<BBGameState> queue;

	protected bool ReadyToProcessNewPlay = false;
	protected BBGameState currentState;
	protected BBGameState previousState;
	protected BBAbstractPlay currentPlay;

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
	public CharacterCutsceneControl DefaultCharacterPrefab;
	public CharacterCutsceneControl DefaultNPCPrefab;
	[Header("Game Objects")]
	public Transform Bat;
	public Transform Ball;

	[Header("Scoreboard Megatron Materials")]
	public Material Scoreboard;
	public Material AwayTeamPanel;
	public Material HomeTeamPanel;
	protected CharacterCutsceneControl Batter;
	protected CharacterCutsceneControl Pitcher;
	protected CharacterCutsceneControl Umpire;
	protected CharacterCutsceneControl Catcher;

	

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
		SceneManager.LoadScene("Title Scene");
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

	private void ProcessPlay(BBGameState gameState) {
		cameraGraphicsMasterControl.rewindPanel.SetLength(game.HistoryLength - 1, historicalPlaybackCurrentIndex);

		previousState = currentState;
		currentState = gameState;
		currentPlay = Playbook.GetPlayFromState(gameState);
		CleanScene();

		switch(currentPlay) {
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
				LogUpdate(state);
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
	}

	private void BatterUp(BBBatterAtPlatePlay play) {
		BBPlayer batter = Database.GetPlayer(play.Batter());
		BBTeam battingTeam = Database.GetTeam(play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		BBTeam fieldingTeam = Database.GetTeam(!play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		
		SetupBatter(play.Batter(), battingTeam.id);
		SetupCatcher(fieldingTeam.lineup[8], fieldingTeam.id);

		SetupAndPlay(newBatterAnimations[0]);
		cameraGraphicsMasterControl.ShowBatter(batter, battingTeam);
	}

	private void Strike(BBStrikePlay play) {
		BBTeam battingTeam = Database.GetTeam(play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		BBTeam fieldingTeam = Database.GetTeam(!play.gameState.topOfInning ? play.gameState.awayTeam : play.gameState.homeTeam);
		

		SetupBatter(play.Batter(), battingTeam.id);
		SetupPitcher(play.Pitcher(), fieldingTeam.id);

		Debug.Log(play.TypeOfStrike);
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

		Bat.SetParent(GetBatter().LeftHandAttachmentReference, false);

		GetBatter().SetPlayerName(player.name);
		GetBatter().SetPrimaryColor(teamColorMain);
		GetBatter().SetSecondaryColor(teamColorSecond);
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

		GetPitcher().SetPlayerName(player.name);
		GetPitcher().SetPrimaryColor(teamColorMain);
		GetPitcher().SetSecondaryColor(teamColorSecond);
	}

	/// <summary>
	/// Todo, get the batter catcher model if one is available
	/// </summary>
	/// <param name="batterID"></param>
	/// <param name="teamID"></param>
	private void SetupCatcher(string batterID, string teamID)
	{
		BBPlayer player = Database.GetPlayer(batterID);
		BBTeam team = Database.GetTeam(teamID);

		Color teamColorMain = Color.white;
		Color teamColorSecond = Color.white;

		ColorUtility.TryParseHtmlString(team.mainColor, out teamColorMain);
		ColorUtility.TryParseHtmlString(team.secondaryColor, out teamColorSecond);

		GetCatcher().SetPlayerName(player.name);
		GetCatcher().SetPrimaryColor(teamColorMain);
		GetCatcher().SetSecondaryColor(teamColorSecond);
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
				director.SetGenericBinding(binding.sourceObject, GetBatter().animator);
				break;

				case "Catcher" :
				director.SetGenericBinding(binding.sourceObject, GetCatcher().animator);
				break;

				case "Umpire" :
				director.SetGenericBinding(binding.sourceObject, GetUmpire().animator);
				break;

				case "Pitcher" :
				director.SetGenericBinding(binding.sourceObject, GetPitcher().animator);
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
	
	private CharacterCutsceneControl GetBatter()
	{
		if(Batter == null) Batter = Instantiate(DefaultCharacterPrefab).GetComponent<CharacterCutsceneControl>();
		return Batter;
	}
	private CharacterCutsceneControl GetUmpire()
	{
		if(Umpire == null) Umpire = Instantiate(DefaultNPCPrefab).GetComponent<CharacterCutsceneControl>();
		return Umpire;
	}
	private CharacterCutsceneControl GetCatcher()
	{
		if(Catcher == null) Catcher = Instantiate(DefaultCharacterPrefab).GetComponent<CharacterCutsceneControl>();
		return Catcher;
	}
	private CharacterCutsceneControl GetPitcher() 
	{
		if(Pitcher == null) Pitcher = Instantiate(DefaultCharacterPrefab).GetComponent<CharacterCutsceneControl>();
		return Pitcher;
	}
}
