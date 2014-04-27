using System;
using System.Collections.Generic;
using System.Linq;

namespace FlagMaker.RandomFlag
{
	public static class Randomizer
	{
		private static readonly Random R = new Random();

		/// <summary>
		/// Returns a nonnegative number less than the specified maxiumum.
		/// </summary>
		/// <param name="max">The exclusive upper bound of the random number returned.</param>
		/// <returns></returns>
		public static int Next(int max)
		{
			return R.Next(max);
		}

		/// <summary>
		/// Given an indexed list of weights, return an index
		/// from the list selected by weighted randomness.
		/// </summary>
		/// <param name="weights">A list of all the weights.</param>
		/// <returns>The index of the selected weight.</returns>
		public static int RandomWeighted(List<int> weights)
		{
			// Algorithm courtesy of this guy:
			// http://peterkellyonline.blogspot.com/2012/02/weighted-random-selection-in-php.html

			var sum = weights.Sum();
			var target = R.Next(1, sum + 1);

			for (int i = 0; i < weights.Count; i++)
			{
				target -= weights[i];
				if (target <= 0)
				{
					return i;
				}
			}

			throw new Exception("Overshot when getting the weighted result.");
		}

		/// <summary>
		/// Generates a random number with probability based on a bell curve.
		/// </summary>
		/// <param name="mean"></param>
		/// <param name="standardDeviation"></param>
		/// <returns></returns>
		public static double NextNormalized(double mean, double standardDeviation)
		{
			// Box–Muller transform
			// http://stackoverflow.com/a/2751988/436282
			double u1 = R.NextDouble();
			double u2 = R.NextDouble();
			double normal = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
			return (normal * standardDeviation) + mean;
		}

		/// <summary>
		/// Returns "true" with the probability specified.
		/// </summary>
		/// <param name="probability">The probability the function will return true.</param>
		/// <returns></returns>
		public static bool ProbabilityOfTrue(double probability)
		{
			return R.NextDouble() < probability;
		}

		/// <summary>
		/// Returns a random number between 0.0 and 1.0.
		/// </summary>
		/// <returns>A double-precision floating point number greater than or equal to 0.0, and less than 1.0.</returns>
		public static double NextDouble()
		{
			return R.NextDouble();
		}

		/// <summary>
		/// Casts a value to an int bounded by two range values.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="forceOdd"></param>
		/// <returns></returns>
		public static int Clamp(double value, int min, int max, bool forceOdd = false)
		{
			var val = (int)value;
			if (value < min) val = min;
			else if (value > max) val = max;

			if (forceOdd && val % 2 == 0)
			{
				val--;
			}

			return val;
		}
	}
}
