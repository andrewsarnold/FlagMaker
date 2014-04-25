using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using FlagMaker.Divisions;
using FlagMaker.Overlays;
using FlagMaker.Overlays.OverlayTypes;
using FlagMaker.Overlays.OverlayTypes.ShapeTypes;

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
		private static Division _division;

		public static Flag GenerateFlag()
		{
			GetColorScheme();
			GetRatio();
			_division = GetDivision();

			return new Flag("Random", _ratio, _gridSize, _division, GetOverlays());
		}

		private static void GetColorScheme()
		{
			_metal = Randomizer.ProbabilityOfTrue(0.4) ? Yellow : White;

			var colorsUsed = Randomizer.Clamp(Randomizer.NextNormalized(2, 1), 2, 5);
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

			//_divisionType = DivisionTypes.Horizontal;
			switch (_divisionType)
			{
				case DivisionTypes.Stripes:
					return new DivisionGrid(_colors[0], _metal, 1, Randomizer.Clamp(Randomizer.NextNormalized(8, 3), 3, 15, true));
				case DivisionTypes.Pales:
				case DivisionTypes.Fesses:
					return SetUpFessesAndPales();
				case DivisionTypes.DiagonalForward:
					return new DivisionBendsForward(_colors[0], _colors[1]);
				case DivisionTypes.DiagonalBackward:
						return new DivisionBendsBackward(_colors[0], _colors[1]);
				case DivisionTypes.X:
					return new DivisionX(_colors[0], _colors[1]);
				case DivisionTypes.Horizontal:
					return Randomizer.ProbabilityOfTrue(0.6) 
						? new DivisionGrid(_colors[0], _metal, 1, 2)
						: new DivisionGrid(_metal, _colors[0], 1, 2);
				case DivisionTypes.Vertical:
					return new DivisionGrid(_colors[0], _colors[1], 2, 1);
				case DivisionTypes.Quartered:
					return new DivisionGrid(_colors[0], _colors[1], 2, 2);
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

			switch (_divisionType)
			{
				case DivisionTypes.Stripes:
					AddHoist(list, true);
					break;
				case DivisionTypes.Pales:
				case DivisionTypes.Fesses:
				case DivisionTypes.DiagonalForward:
				case DivisionTypes.DiagonalBackward:
				case DivisionTypes.X:
					//AddOverlaysX(list);
					break;
				case DivisionTypes.Horizontal:
					AddHoist(list, false);
					break;
				case DivisionTypes.Vertical:
				case DivisionTypes.Quartered:
				case DivisionTypes.Blank:
					break;
			}
			return list;
		}

		private static double HoistElementWidth()
		{
			return Math.Round(_gridSize.Width * Randomizer.NextNormalized(0.35, 0.05), 3);
		}

		private static void AddHoist(ICollection<Overlay> list, bool allowCanton)
		{
			// Made-up values
			var type = Randomizer.RandomWeighted(new List<int> { allowCanton ? 4 : 0, 3, 6 });
			var width = HoistElementWidth();

			switch (type)
			{
				case 0: // Canton box
					var height = _gridSize.Height / 2.0;
					if (_divisionType == DivisionTypes.Stripes && _division.Values[1] > 1)
					{
						var stripe = (int)(_division.Values[1] / 2) + 1;
						height = _gridSize.Height * (stripe / _division.Values[1]);
					}
					list.Add(new OverlayBox(_colors[1], 0, 0, width, height, 0, 0));
					break;
				case 1: // Full hoist box
					list.Add(new OverlayBox(_colors[1], 0, 0, width, _gridSize.Height, 0, 0));
					break;
				case 2: // Triangle
					list.Add(new OverlayTriangle(_colors[1], 0, 0, width, _gridSize.Height / 2.0, 0, _gridSize.Height, 0, 0));
					break;
			}
		}

		private static void AddOverlaysX(ICollection<Overlay> list)
		{
			list.Add(new OverlaySaltire(_metal, _gridSize.Width / 3.0, 0, 0));
			if (Randomizer.ProbabilityOfTrue(0.1))
			{
				list.Add(new OverlayHalfSaltire(_colors[0], _gridSize.Width / 2.0, 0, 0));
				list.Add(new OverlayCross(_metal, _gridSize.Width / 10.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, 0, 0));
			}
		}

		private enum DivisionTypes
		{
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
