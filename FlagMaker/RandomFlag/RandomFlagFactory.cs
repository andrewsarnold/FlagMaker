using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using FlagMaker.Divisions;
using FlagMaker.Overlays;
using FlagMaker.Overlays.OverlayTypes;

namespace FlagMaker.RandomFlag
{
	public static class RandomFlagFactory
	{
		#region Color definitions

		private static readonly Color Yellow = Color.FromRgb(253, 200, 47);
		private static readonly Color White = Color.FromRgb(255, 255, 255);
		private static readonly Color Black = Color.FromRgb(0, 0, 0);
		private static readonly Color Red = Color.FromRgb(198, 12, 48);
		private static readonly Color Green = Color.FromRgb(20, 77, 41);
		private static readonly Color LightBlue = Color.FromRgb(0, 101, 189);
		private static readonly Color DarkBlue = Color.FromRgb(0, 57, 166);

		#endregion

		private static List<Color> _colors;
		private static Color _metal;
		private static Ratio _ratio;
		private static Ratio _gridSize;
		private static DivisionTypes _divisionType;

		public static Flag GenerateFlag()
		{
			GetColorScheme();
			GetRatio();

			return new Flag("Random", _ratio, _gridSize, GetDivision(), GetOverlays());
		}

		private static void GetColorScheme()
		{
			_metal = Randomizer.ProbabilityOfTrue(0.4) ? Yellow : White;

			var colorsUsed = Randomizer.Clamp(Randomizer.NextNormalized(2, 1), 1, 5);
			_colors = new List<Color>
			          {
				          Black, Red, Green, LightBlue, DarkBlue
			          }.OrderBy(c => Randomizer.NextDouble()).Take(colorsUsed).ToList();
		}

		private static void GetRatio()
		{
			_ratio = new List<Ratio>
			         {
				         new Ratio(3, 2),
				         new Ratio(5, 3),
				         new Ratio(2, 1)
			         }[Randomizer.RandomWeighted(new List<int> { 6, 1, 3 })];
			_gridSize = new Ratio(_ratio.Width * 8, _ratio.Height * 8);
		}

		private static Division GetDivision()
		{
			// Roughly based on what's used in the presets
			_divisionType = (DivisionTypes) Randomizer.RandomWeighted(new List<int> { 9, 16, 50, 2, 4, 2, 18, 2, 1, 62 });

			switch (_divisionType)
			{
				case DivisionTypes.Stripes:
					return new DivisionGrid(_colors[0], _metal, 1, Randomizer.Clamp(Randomizer.NextNormalized(10, 3), 3, 15, true));
				case DivisionTypes.Pales:
				case DivisionTypes.Fesses:
					return SetUpFessesAndPales();
				case DivisionTypes.DiagonalForward:
					return new DivisionBendsForward(_colors[0], _colors.Count > 1 ? _colors[1] : _metal);
				case DivisionTypes.DiagonalBackward:
						return new DivisionBendsBackward(_colors[0], _colors.Count > 1 ? _colors[1] : _metal);
				case DivisionTypes.X:
					return new DivisionX(_colors[0], _colors.Count > 1 ? _colors[1] : _metal);
				case DivisionTypes.Horizontal:
					return new DivisionGrid(_colors[0], _colors.Count > 1 ? _colors[1] : _metal, 1, 2);
				case DivisionTypes.Vertical:
					return new DivisionGrid(_colors[0], _colors.Count > 1 ? _colors[1] : _metal, 2, 1);
				case DivisionTypes.Quartered:
					return new DivisionGrid(_colors[0], _colors.Count > 1 ? _colors[1] : _metal, 2, 2);
				case DivisionTypes.Blank:
					return new DivisionGrid(_colors[0], _colors[0], 1, 1);
				default:
					throw new Exception("No valid type selection");
			}
		}

		private static Division SetUpFessesAndPales()
		{
			var color3 = _colors.Count > 1
				? _colors[1]
				: _colors[0];

			var isBalanced = Randomizer.ProbabilityOfTrue(0.4); // Middle is larger than outsides
			var isOffset = !isBalanced && Randomizer.ProbabilityOfTrue(0.2); // One large section, two small

			if (_divisionType == DivisionTypes.Fesses)
			{
				return new DivisionFesses(_colors[0], _metal, color3,
					isOffset ? 2 : 1,
					isBalanced ? 2 : 1,
					1);
			}

			return new DivisionPales(_colors[0], _metal, color3,
					isOffset ? 2 : 1,
					isBalanced ? 2 : 1,
					1);
		}

		private static IEnumerable<Overlay> GetOverlays()
		{
			var list = new List<Overlay>();
			return list;
		}

		private enum DivisionTypes
		{
			// 0 - 9
			Stripes,
			Pales,
			Fesses,
			DiagonalBackward,
			DiagonalForward,
			X,
			Horizontal,
			Vertical,
			Quartered,
			Blank
		}
	}
}
