using System;
using System.Security.Cryptography;

namespace kaynsd.helpers.rng {
	public class ParkMiller : PRNG
	{
		private int originalseed;
		private uint state;

		// A is a Prime
		protected uint a;
		// M is a Mersenne Prime
		protected uint m;

		public int Seed { get => originalseed;}
		public uint State { get => state;}

		public ParkMiller(string seed) {
			Construct(seed.GetHashCode());
		}

		public ParkMiller(int seed)
		{
			Construct(seed);
		}
		protected void Construct(int seed) {
			m = 2147483647; // Mersenne Prime; 2^31 - 1
			a = 48271; // a must be a high multiplicative order
			originalseed = seed;

			long currentState = seed % m;
       	if (currentState <= 0) currentState += (m - 1);

			state = (uint)currentState;
		}

		/// <summary>
		/// Returns a number between 1 and 2^32 - 1
		/// </summary>
		/// <returns>1 to 2^32-1</returns>
		protected uint _next() {
			// Lehmer / Park-Miller Generation = 
			// X(k+1) = a x X(k) % m;
			state = (state * a) % m;
			if(state <= 0) state += m;
			return state;
		}

		/// <summary>
		/// Returns a number between 0 (inclusive) and 1 (exclusive)
		/// </summary>
		/// <returns></returns>
		public override float next() {
			_next();
			return (float)(state - 1) / (float)(m - 1); 
		}
	}
}

//48271,-1964877855,-856141137,-613502015,-556260145,902075297,1698214639,773027713,144866575,647683937
//48271,182605794,1291390782,1716587427,735130638,471227346,468419150,205459859,673367325,1949132596