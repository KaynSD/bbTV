
using System.Collections.Generic;
using UnityEngine;

namespace blaseball.runtime {
	[System.Serializable]
	public class BBAnnouncements
	{
		protected Queue<string> announcements;
		int lastRandomValue = -1;
		public BBAnnouncements()
		{
			announcements = new Queue<string>();
			lastRandomValue = -1;
		}

		protected string[] GenericAnnouncements = new string[]{
			"In memoriam: so so many",
			"EVERYTHING WAS PEANUTS",
			"THE COMMISSIONER IS STILL DOING A GREAT JOB",
			"THE FEEDBACK IS LOUD",
			"We have eaten -Infinity collective peanuts.",
			"NO THOUGHTS ONLY BLASEBALL",
			"WE ARE ALL LOVE BLASEBALL",
			"YOU ARE NOW PARTICIPATING IN THE CULTURAL EVENT OF BLASEBALL"
		};

		public void AddAnnouncement(string announcement) {
			announcements.Enqueue(announcement);
		}

		public string GetAnnouncement(bool requireNonsense) {

			if(!requireNonsense && announcements.Count > 0 && Random.value > 0.1f) {
				return announcements.Dequeue();
			} else {
				int randomValue; 
				do {
					randomValue = Random.Range(0, GenericAnnouncements.Length);
				} while(randomValue == lastRandomValue);

				lastRandomValue = randomValue;
				return GenericAnnouncements[randomValue];
			}
		}
	}
}