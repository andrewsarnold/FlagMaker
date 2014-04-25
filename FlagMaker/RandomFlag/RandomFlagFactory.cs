using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using FlagMaker.Divisions;
using FlagMaker.Overlays;
using FlagMaker.Overlays.OverlayTypes;
using FlagMaker.Overlays.OverlayTypes.PathTypes;
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
			_divisionType = (DivisionTypes)Randomizer.RandomWeighted(new List<int> { 3, 4, 7, 1, 2, 1, 4, 1, 1, 8 });
			_divisionType = DivisionTypes.Blank;
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
					break;
				case DivisionTypes.Fesses:
					break;
				case DivisionTypes.DiagonalForward:
					AddFimbriationForward(list);
					break;
				case DivisionTypes.DiagonalBackward:
					AddFimbriationBackward(list);
					break;
				case DivisionTypes.X:
					AddOverlaysX(list, false);
					break;
				case DivisionTypes.Horizontal:
					AddHoist(list, false);
					break;
				case DivisionTypes.Vertical:
					break;
				case DivisionTypes.Quartered:
					AddCross(list, _gridSize.Width / 2.0);
					break;
				case DivisionTypes.Blank:
					AddAnyOverlays(list);
					break;
			}
			return list;
		}

		private static void AddAnyOverlays(ICollection<Overlay> list)
		{
			var type = Randomizer.RandomWeighted(new List<int> { 1, 1, 1, 1, 10000, 0, 0, 1 });
			switch (type)
			{
				case 0: // Saltire
					AddOverlaysX(list, true);
					break;
				case 1:
					AddFimbriationBackward(list);
					break;
				case 2:
					AddFimbriationForward(list);
					break;
				case 3: // Nordic cross
					AddCross(list, _gridSize.Width / 3.0);
					break;
				case 4: // Centered cross
					AddCross(list, _gridSize.Width / 2.0);
					break;
				case 5: // Diamond
					break;
				case 6: // Pall
					break;
				case 7: // Rays
					var left = _gridSize.Width / (Randomizer.ProbabilityOfTrue(0.3) ? 3.0 : 2.0);
					list.Add(new OverlayRays(_metal, left, _gridSize.Height / 2.0,
						Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width * 3 / 4.0, _gridSize.Width / 10.0), 4, 20), 0, 0));
					AddCircle(list, _metal, left, _gridSize.Height / 2.0);
					AddEmblem(list, _colors[0], left, _gridSize.Height / 2.0);
					break;
			}
		}

		private static void AddFimbriationBackward(ICollection<Overlay> list)
		{
			var width = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 3.0, _gridSize.Width / 10.0), 1, _gridSize.Width);
			if (Randomizer.ProbabilityOfTrue(0.4))
			{
				list.Add(new OverlayFimbriationBackward(_metal, width + 1, 0, 0));
				list.Add(new OverlayFimbriationBackward(_colors[1], width - 1, 0, 0));
			}
			else
			{
				list.Add(new OverlayFimbriationBackward(_metal, width, 0, 0));
			}
		}

		private static void AddFimbriationForward(ICollection<Overlay> list)
		{
			var width = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 3.0, _gridSize.Width / 10.0), 1, _gridSize.Width);
			if (Randomizer.ProbabilityOfTrue(0.4))
			{
				list.Add(new OverlayFimbriationForward(_metal, width + 1, 0, 0));
				list.Add(new OverlayFimbriationForward(_colors[1], width - 1, 0, 0));
			}
			else
			{
				list.Add(new OverlayFimbriationForward(_metal, width, 0, 0));
			}
		}

		private static void AddCross(ICollection<Overlay> list, double left)
		{
			var width = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 10.0, _gridSize.Width / 20.0), 1, _gridSize.Width / 3);
			if (Randomizer.ProbabilityOfTrue(0.4))
			{
				list.Add(new OverlayCross(_metal, width + 1, left, _gridSize.Height / 2.0, 0, 0));
				list.Add(new OverlayCross(_colors[1], width - 1, left, _gridSize.Height / 2.0, 0, 0));
			}
			else
			{
				list.Add(new OverlayCross(_metal, width, left, _gridSize.Height / 2.0, 0, 0));
			}
		} 

		private static int HoistElementWidth()
		{
			return (int)(_gridSize.Width * Randomizer.NextNormalized(0.35, 0.05));
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
					AddEmblem(list, _metal, new Rect { Top = 0, Left = 0, Bottom = height, Right = width });
					break;
				case 1: // Full hoist box
					list.Add(new OverlayBox(_colors[1], 0, 0, width, _gridSize.Height, 0, 0));
					AddEmblem(list, _metal, new Rect { Top = 0, Left = 0, Bottom = _gridSize.Height, Right = width });
					break;
				case 2: // Triangle
					list.Add(new OverlayTriangle(_colors[1], 0, 0, width, _gridSize.Height / 2.0, 0, _gridSize.Height, 0, 0));
					AddEmblem(list, _metal, new Rect { Top = 0, Left = 0, Bottom = _gridSize.Height, Right = width * 3 / 4.0 });
					break;
			}
		}

		private static void AddOverlaysX(ICollection<Overlay> list, bool allowExtra)
		{
			list.Add(new OverlaySaltire(_metal, _gridSize.Width / 3.0, 0, 0));
			if (allowExtra && Randomizer.ProbabilityOfTrue(0.1))
			{
				list.Add(new OverlayHalfSaltire(_colors[0], _gridSize.Width / 2.0, 0, 0));
				list.Add(new OverlayCross(_metal, _gridSize.Width / 10.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, 0, 0));
			}
		}

		private static void AddCircle(ICollection<Overlay> list, Color color, double x, double y)
		{
			list.Add(new OverlayEllipse(color, x, y, _gridSize.Width * 0.3, 0, 0, 0));
		}

		private static void AddEmblem(ICollection<Overlay> list, Color color, double x, double y)
		{
			const double size = 0.25;
			AddEmblem(list, color, new Rect
			                       {
				                       Bottom = _gridSize.Height / size + y,
				                       Top = _gridSize.Height / size - y,
				                       Left = _gridSize.Width / size - x,
				                       Right = _gridSize.Width / size + x,
			                       });
		}

		private static void AddEmblem(ICollection<Overlay> list, Color color, Rect rect)
		{
			var types = OverlayFactory.GetOverlaysByType(typeof(OverlayPath)).ToList();
			var type = types[Randomizer.Next(types.Count)];
			var emblem = (Overlay)Activator.CreateInstance(type, 0, 0);
			emblem.SetColors(new List<Color> { color });
			emblem.SetValues(new List<double> { (rect.Right - rect.Left) / 2, (rect.Bottom - rect.Top) / 2, (rect.Bottom - rect.Top) / 3, 0 });
			list.Add(emblem);
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

		private struct Rect
		{
			public double Top;
			public double Bottom;
			public double Left;
			public double Right;
		}
	}
}
