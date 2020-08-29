using UnityEngine;

namespace blaseball.vo {
	[System.Serializable]
	public class BBPlayer {
		public string id;
		public string name;

		public float anticapitalism, baseThirst, buoyancy, chasiness,
		coldness, continuation, divinity, groundFriction, indulgence, laserlikeness,
		martyrdom, moxie, musclitude, omniscience, overpowerment, patheticism,
		ruthlessness, shakespearianism, suppression, tenaciousness,
		thwackability, tragicness, unthwackability, watchfulness, pressurization;

		public int totalFingers, soul, fate;
		public bool deceased, peanutAllergy;

		public float cinnamon;

		/// <summary>
		/// Calculate a player's batter rating.
		/// A combination of inverse tragicness, inverse patheticism, thwackability
		/// divinity, moxie, musclitude and martyrdom
		/// 
		/// Maths by Baron Bliss of SIBR
		/// </summary>
		/// <param name="player">Player</param>
		/// <returns>the raw batting rating, unmodified by items</returns>
		public static float BatterRating(BBPlayer player) {
			return Mathf.Pow(1 - player.tragicness, 0.01f) * 
			Mathf.Pow(1 - player.patheticism, 0.05f) * 
			Mathf.Pow(player.thwackability * player.divinity, 0.35f) *
			Mathf.Pow(player.moxie * player.musclitude, 0.075f) * 
			Mathf.Pow(player.martyrdom, 0.02f);
		}

		/// <summary>
		/// Calculate a player's pitcher rating.
		/// 
		/// A combination of unthwackability, ruthlessness, overpowerment,
		/// shakespearianism and coldness
		/// 
		/// Maths by Baron Bliss of SIBR
		/// </summary>
		/// <param name="player">Player</param>
		/// <returns>the raw pitching rating, unmodified by items</returns>
		public static float PitcherRating(BBPlayer player) {
			return Mathf.Pow(player.unthwackability, 0.5f) *
			Mathf.Pow(player.ruthlessness, 0.4f) *
			Mathf.Pow(player.overpowerment, 0.15f) *
			Mathf.Pow(player.shakespearianism, 0.1f) *
			Mathf.Pow(player.coldness, 0.025f);
		}

		/// <summary>
		/// Calculate a player's defense rating.
		/// 
		/// A combination of omniecence, tenaciousness, watchfullness,
		/// anticapitalism and chasiness
		/// 
		/// Maths by Baron Bliss of SIBR
		/// </summary>
		/// <param name="player">Player</param>
		/// <returns>the raw defense rating, unmodified by items</returns>
		public static float DefenseRating(BBPlayer player) {
			return Mathf.Pow(player.omniscience * player.tenaciousness, 0.2f) *
			Mathf.Pow(player.watchfulness * player.anticapitalism * player.chasiness, 0.1f);
		}

		/// <summary>
		/// Calculate a player's baserunning rating.
		/// 
		/// A combination of laserlikeness, baseThirst, continuation,
		/// groundFriction, and indulgence
		/// 
		/// Maths by Baron Bliss of SIBR
		/// </summary>
		/// <param name="player">Player</param>
		/// <returns>the raw baserunning rating, unmodified by items</returns>
		public static float BaserunningRating(BBPlayer player) {
			return Mathf.Pow(player.laserlikeness, 0.5f) *
			Mathf.Pow(player.baseThirst * player.continuation * player.groundFriction * player.indulgence, 0.1f);
		}

		/// <summary>
		/// Calculate a player's vibes
		/// 
		/// A combination of pressurization, cinnamon, the phase of the moon and buoyancy
		/// </summary>
		/// <param name="player">Player</param>
		/// <param name="day">The Day; use 1 indexed (database stores 0 index!)</param>
		/// <returns>The current vibes for the player</returns>
		/// <todo>Check that the cos function is correct; it'll be in radians here, check it's not degrees</todo>
		public static float Vibes(BBPlayer player, int day) {
			return 0.5f * Mathf.Round(
				(player.pressurization + player.cinnamon) * 
				Mathf.Cos((Mathf.PI * day) / (5 * player.buoyancy + 3f)) -
				player.pressurization + player.cinnamon);
		}
	}

}