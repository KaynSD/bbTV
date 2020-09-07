using System;
using System.Collections;
using System.Collections.Generic;
using blaseball.runtime.events;
using blaseball.vo;
using UnityEngine;

public class TVCameraGraphicsMasterControl : MonoBehaviour
{
	public RewindPanel rewindPanel;
	public TechnicalDifficultiesChiron technicalDifficultiesChiron;
	public PlayerInformationChiron playerInformationChiron;
	public MajorItemsChiron majorItemsChiron;
	public void ClearAllGraphics() {
		technicalDifficultiesChiron.Hide();
		playerInformationChiron.Hide();
		majorItemsChiron.Hide();
	}
	internal void EnableRewind()
	{
		rewindPanel.Show();
	}

	internal void DisableRewind()
	{
		rewindPanel.Disable();
	}

	public void ShowTechnicalDifficulties(string lastUpdate) {
		ClearAllGraphics();

		technicalDifficultiesChiron.Show();
		technicalDifficultiesChiron.ShowText(lastUpdate);
	}
	public void ShowBatter(BBPlayer player, BBTeam team) {
		ClearAllGraphics();
		playerInformationChiron.Show();
		playerInformationChiron.ShowBatterText(player, team);
	}
	public void ShowPitcher(BBPlayer player, BBTeam team) {
		
	}
	public void ShowFielder(BBPlayer player, BBTeam team) {
		
	}

	internal void ShowOnlyMajorItems() {
		majorItemsChiron.Show();
	}

	internal void ShowStrike(BBAbstractPlay bbStrike)
	{
		bool isChangeover = bbStrike is BBStrikeOutPlay;

		majorItemsChiron.Show();
		BBGameState state = bbStrike.gameState;
		
		majorItemsChiron.SetText(state.lastUpdate);

		if(isChangeover) {
			majorItemsChiron.SetStrikes(3, true);
		} else {
			majorItemsChiron.SetStrikes(state.atBatStrikes, true);
		}

		majorItemsChiron.SetBalls(state.atBatBalls);
		majorItemsChiron.SetOuts(state.halfInningOuts, isChangeover);
		majorItemsChiron.ShowBase1(Array.IndexOf(state.basesOccupied, 0) > -1);
		majorItemsChiron.ShowBase2(Array.IndexOf(state.basesOccupied, 1) > -1);
		majorItemsChiron.ShowBase3(Array.IndexOf(state.basesOccupied, 2) > -1);
	}

	internal void ShowBallCount(BBAbstractPlay bBBallPlay)
	{
		bool isChangeover = bBBallPlay is BBDrawsAWalkPlay;

		majorItemsChiron.Show();
		BBGameState state = bBBallPlay.gameState;

		majorItemsChiron.Show();
		majorItemsChiron.SetText(state.lastUpdate);
		majorItemsChiron.SetStrikes(state.atBatStrikes);
		if(isChangeover) {
			majorItemsChiron.SetBalls(4, true);
		} else {
			majorItemsChiron.SetBalls(state.atBatBalls, true);
		}
		majorItemsChiron.SetOuts(state.halfInningOuts, isChangeover);
		majorItemsChiron.ShowBase1(Array.IndexOf(state.basesOccupied, 0) > -1);
		majorItemsChiron.ShowBase2(Array.IndexOf(state.basesOccupied, 1) > -1);
		majorItemsChiron.ShowBase3(Array.IndexOf(state.basesOccupied, 2) > -1);
	}

	internal void ResetPlate(BBAbstractPlay play)
	{
		BBGameState state = play.gameState;
		majorItemsChiron.SetText(state.lastUpdate);
		majorItemsChiron.SetStrikes(state.atBatStrikes);
		majorItemsChiron.SetBalls(state.atBatBalls);
		majorItemsChiron.SetOuts(state.halfInningOuts);
		majorItemsChiron.ShowBase1(Array.IndexOf(state.basesOccupied, 0) > -1);
		majorItemsChiron.ShowBase2(Array.IndexOf(state.basesOccupied, 1) > -1);
		majorItemsChiron.ShowBase3(Array.IndexOf(state.basesOccupied, 2) > -1);
	}
}
