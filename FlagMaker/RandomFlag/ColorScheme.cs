using System.Collections.Generic;
using System.Windows.Media;

namespace FlagMaker.RandomFlag
{
	public class ColorScheme
	{
		#region Color definitions

		private static readonly Color Yellow = Color.FromRgb(253, 200, 47);
		private static readonly Color White = Color.FromRgb(255, 255, 255);
		private static readonly Color Black = Color.FromRgb(0, 0, 0);
		private static readonly Color Red = Color.FromRgb(198, 12, 48);
		private static readonly Color Orange = Color.FromRgb(255, 99, 25);
		private static readonly Color Green = Color.FromRgb(20, 77, 41);
		private static readonly Color Blue = Color.FromRgb(0, 57, 166);

		#endregion

		private readonly Color _color1;
		private readonly Color _color2;
		private readonly Color _color3;
		private readonly Color _metal;

		public ColorScheme()
		{
			var colors = new[] { Black, Red, Orange, Green, Blue };
			var color1Index = Randomizer.RandomWeighted(new List<int> { 27, 102, 4, 45, 58 });

			var firstOrderBase = new List<List<int>>
			                     {               // B   R   O  G   B
				                     new List<int>{ 0,  38, 0, 22, 11 }, // Black
				                     new List<int>{ 38, 0,  0, 76, 69 }, // Red
				                     new List<int>{ 0,  0,  0, 8,  1  }, // Orange
				                     new List<int>{ 22, 76, 8, 0,  34 }, // Green
				                     new List<int>{ 11, 69, 1, 34, 0 }   // Blue
			                     };

			var color2Index = Randomizer.RandomWeighted(firstOrderBase[color1Index]);

			int color3Index;
			do
			{
				color3Index = Randomizer.RandomWeighted(firstOrderBase[color1Index]);
			} while (color3Index == color2Index);

			var yellowProbabilities = new List<List<double>>
			                          {                   // B     R     O     G     B
				                          new List<double> { 0.00, 0.54, 0.00, 0.25, 0.60 }, // Black
				                          new List<double> { 0.54, 0.00, 0.00, 0.59, 0.24 }, // Red
				                          new List<double> { 0.00, 0.00, 0.00, 0.00, 0.00 }, // Orange
				                          new List<double> { 0.25, 0.59, 0.00, 0.00, 0.55 }, // Green
				                          new List<double> { 0.60, 0.60, 0.00, 0.55, 0.00 }, // Blue
			                          };
			var yellowProbability = yellowProbabilities[color1Index][color2Index];

			_color1 = TweakColor(colors[color1Index]);
			_color2 = TweakColor(colors[color2Index]);
			_color3 = TweakColor(colors[color3Index]);
			_metal = Randomizer.ProbabilityOfTrue(yellowProbability) ? TweakColor(Yellow) : White;
		}

		private ColorScheme(Color color1, Color color2, Color color3, Color metal)
		{
			_color1 = color1;
			_color2 = color2;
			_color3 = color3;
			_metal = metal;
		}

		public Color Color1 { get { return _color1; } }
		public Color Color2 { get { return _color2; } }
		public Color Color3 { get { return _color3; } }
		public Color Metal { get { return _metal; } }

		public ColorScheme Swapped
		{
			get { return new ColorScheme(_color3, _color1, _color2, _metal); }
		}

		private Color TweakColor(Color color)
		{
			if (color == Colors.Black || color == Colors.White)
			{ // Don't adjust black or white, it looks bad
				return color;
			}

			return Color.FromRgb(
				(byte)Randomizer.Clamp(Randomizer.NextNormalized(color.R, 15), 0, 255),
				(byte)Randomizer.Clamp(Randomizer.NextNormalized(color.G, 15), 0, 255),
				(byte)Randomizer.Clamp(Randomizer.NextNormalized(color.B, 15), 0, 255));
		}
	}
}
