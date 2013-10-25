﻿using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes
{
	public class OverlayMapleLeaf : PathOverlay
	{
		private const string Path = "m 0.0840585,-210.996 -34.1131205,63.62529 c -3.87086,6.91501 -10.80627,6.27363 -17.74169,2.41138 l -24.69699,-12.78858 18.40705,97.727112 c 3.87086,17.85419 -8.54859,17.85419 -14.67765,10.13435 l -43.101048,-48.25099 -6.99738,24.503 c -0.80692,3.21777 -4.35481,6.59744 -9.67748,5.79261 l -54.50177,-11.45912 14.31524,52.04475 c 3.06451,11.58054 5.4549,16.37528 -3.09375,19.4295901 l -19.42619,9.13025 93.82127,76.2083799 c 3.713498,2.88151 5.589708,8.067 4.267678,12.7621 l -8.211358,26.947068 c 32.304048,-3.72371 61.248978,-9.32594 93.569388,-12.77619 2.8532305,-0.30459 7.6298805,4.40408 7.6102905,7.71058 l -4.28024,98.72342 15.7063895,0 -2.4723695,-98.5117 c -0.0197,-3.3065 4.3137195,-8.22689 7.1669495,-7.9223 32.32041,3.45026 61.26538,9.05248 93.569422,12.77619 L 97.31536,90.274122 c -1.322032,-4.6951 0.55417,-9.88059 4.26767,-12.7621 L 195.40428,1.3036421 175.97811,-7.826608 c -8.54867,-3.05431 -6.15828,-7.84905 -3.09377,-19.42959 l 14.31527,-52.04475 -54.5018,11.45912 c -5.32267,0.80483 -8.87056,-2.57484 -9.6775,-5.79261 l -6.99737,-24.503 -43.101032,48.25099 c -6.12908,7.71984 -18.54854,7.71984 -14.67768,-10.13435 l 18.40702,-97.727112 -24.69694,12.78858 c -6.93559,3.86225 -13.87083,4.50363 -17.7417,-2.41138";
		private static readonly Vector PathSize = new Vector(392, 422);

		public OverlayMapleLeaf(int maximumX, int maximumY)
			: base("maple leaf", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayMapleLeaf(Color color, int maximumX, int maximumY)
			: base(color, "maple leaf", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}
