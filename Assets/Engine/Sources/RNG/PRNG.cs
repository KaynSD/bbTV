using System;
using System.Collections.Generic;

namespace kaynsd.helpers.rng {

	public abstract class PRNG
	{
		public abstract float next();

		public float Range(float low, float high) {
			if(high < low) {
				float temp = high;
				low = high;
				high = temp;
			}
			float v = next();
			return low + v * (high - low);
		}
		public int Range(int low, int high) {
			if(high < low) {
				int temp = high;
				low = high;
				high = temp;
			}
			float v = next();
			return (int)Math.Floor(low + v * (high - low));
		}

		public void Shuffle<T>(T[] array) {
			int n = array.Length;
			for (int i = 0; i < (n - 1); i++) {
				int r = i + Range(0, n - i);
				T t = array[r];
				array[r] = array[i];
				array[i] = t;
			}
		}
		public void Shuffle<T>(List<T> list) {  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = Range(0, n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}
	}
}