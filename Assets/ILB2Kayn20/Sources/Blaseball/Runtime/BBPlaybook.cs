using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using blaseball.runtime.events;
using blaseball.vo;

namespace blaseball.runtime {
	public class BBPlaybook {

		List<Regex> regexes;
		List<Type> types;
		public BBPlaybook()
		{
			// This list is roughly sorted in how common each one is?
			types = new List<Type>(){
				typeof(BBBallPlay), typeof(BBFoulBallPlay), typeof(BBStrikePlay),
				typeof(BBFlyoutPlay), typeof(BBGroundOutPlay), typeof(BBStrikeOutPlay),
				typeof(BBDrawsAWalkPlay), typeof(BBRunsPlay),
				
				typeof(BBBatterAtPlatePlay), typeof(BBBottomOfInningPlay), typeof(BBTopOfInningPlay),
				typeof(StartGamePlay), typeof(GameOverPlay),

				typeof(BBFieldersChoice), typeof(BBCaughtStealingPlay), typeof(BBStealsBasePlay),
				typeof(BBHomeRunPlay), typeof(BBSacrificePlay)
			};
			BuildRegex();
		}

		private void BuildRegex()
		{
			regexes = new List<Regex>();

			for(int i = 0; i < types.Count; i++) {
				BBAbstractPlay sample = (BBAbstractPlay)Activator.CreateInstance(types[i]);
				regexes.Add(sample.lastUpdateMatches);
			}
		}

		public BBAbstractPlay GetPlayFromState(BBGameState gameState) {
			BBAbstractPlay play = new UnknownPlay();

			for(int i = 0; i < regexes.Count; i++) {
				Regex regex = regexes[i];
				if(regex.IsMatch(gameState.lastUpdate)) {
					play = (BBAbstractPlay)Activator.CreateInstance(types[i]);
					break;
				}
			}

			play.Parse(gameState);
			return play;
		}
	}
}