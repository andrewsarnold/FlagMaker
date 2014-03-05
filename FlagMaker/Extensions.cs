using System;
using System.Windows.Media;

namespace FlagMaker
{
	public static class Extensions
	{
		public static string ToHexString(this Color c)
		{
			return string.Format("{0}{1}{2}{3}",
				c.A < 0xff ? c.A.ToString("x2") : string.Empty,
				c.R.ToString("x2"), c.G.ToString("x2"), c.B.ToString("x2"));
		}

		public static double Hue(this Color c)
		{
			double r = c.R / 255.0;
			double g = c.G / 255.0;
			double b = c.B / 255.0;

			var max = Math.Max(Math.Max(r, g), b);
			var min = Math.Min(Math.Min(r, g), b);

			double h;

			if (max == min)
			{
				h = 360; // achromatic
			}
			else
			{
				var d = max - min;

				if (max == r)
				{
					h = (g - b) / d + (g < b ? 6 : 0);
				}
				else if (max == g)
				{
					h = (b - r) / d + 2;
				}
				else
				{
					h = (r - g) / d + 4;
				}

				h /= 6;
			}

			return h;
		}
	}
}
