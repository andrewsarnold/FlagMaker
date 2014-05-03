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
		private static readonly Color Orange = Color.FromRgb(255, 99, 25);
		private static readonly Color Green = Color.FromRgb(20, 77, 41);
		private static readonly Color Blue = Color.FromRgb(0, 57, 166);

		#endregion

		private static Color _color1;
		private static Color _color2;
		private static Color _color3;
		private static Color _metal;
		private static Ratio _ratio;
		private static Ratio _gridSize;
		private static DivisionTypes _divisionType;
		private static Division _division;
		private static List<Overlay> _overlays; 

		private static readonly List<Overlay> Emblems =
			OverlayFactory.GetOverlaysByType(typeof (OverlayPath)).Select(p => OverlayFactory.GetInstance(p))
				.Union(OverlayFactory.CustomTypes.Select(t => t.Value))
				.ToList();
		
		public static Flag GenerateFlag()
		{
			GetColorScheme();
			GetRatio();
			_overlays = new List<Overlay>();
			_division = GetDivision();

			return new Flag("Random", _ratio, _gridSize, _division, _overlays);
		}

		private static void GetColorScheme()
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

			_color1 = colors[color1Index];
			_color2 = colors[color2Index];
			_color3 = colors[color3Index];
			_metal = Randomizer.ProbabilityOfTrue(yellowProbability) ? Yellow : White;
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
			_divisionType = (DivisionTypes)Randomizer.RandomWeighted(new List<int> { 7, 22, 60, 2, 7, 3, 20, 8, 2, 38, 7, 2, 5 });

			switch (_divisionType)
			{
				case DivisionTypes.Stripes:
					return GetStripes();
				case DivisionTypes.Pales:
					return GetPale();
				case DivisionTypes.Fesses:
					return GetFesses();
				case DivisionTypes.DiagonalForward:
					return GetBendsForward();
				case DivisionTypes.DiagonalBackward:
					return GetBendsBackward();
				case DivisionTypes.X:
					return GetX();
				case DivisionTypes.Horizontal:
					return GetHorizontal();
				case DivisionTypes.Vertical:
					return new DivisionGrid(_color1, _color2, 2, 1);
				case DivisionTypes.Quartered:
					return new DivisionGrid(_color1, _color2, 2, 2);
				case DivisionTypes.Blank:
					return new DivisionGrid(_color1, _color1, 1, 1);
				default:
					return GetStripes();
					//throw new Exception("No valid type selection");
			}
		}

		#region Division getters

		private static DivisionGrid GetStripes()
		{
			var stripeCount = Randomizer.Clamp(Randomizer.NextNormalized(8, 3), 3, 15, true);
			
			if (Randomizer.ProbabilityOfTrue(0.14))
			{
				AddTriangle(1.0, HoistElementWidth(true), _color2);
			}
			else
			{
				double width = HoistElementWidth(false);
				var stripe = (int)(stripeCount / 2.0) + 1;
				var height = _gridSize.Height * ((double)stripe / stripeCount);
				if (width < height) width = height;

				_overlays.Add(new OverlayBox(stripeCount > 5 && Randomizer.ProbabilityOfTrue(0.3) ? _color1 : _color2, 0, 0, width, height, 0, 0));
				AddEmblem(1.0, width / 2.0, height / 2.0, _metal);
			}

			return new DivisionGrid(_color1, _metal, 1, stripeCount);
		}

		private static DivisionPales GetPale()
		{
			var color2 = Randomizer.ProbabilityOfTrue(0.27) ? _color1 : _color2;
			var isBalanced = Randomizer.ProbabilityOfTrue(0.9);

			AddEmblem(isBalanced ? 0.2 : 1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, Randomizer.ProbabilityOfTrue(0.5) ? _color1 : _color2);

			return new DivisionPales(_color1, _metal, color2, 1, isBalanced ? 1 : 2, 1);
		}

		private static DivisionFesses GetFesses()
		{
			DivisionFesses fesses;
			Color color1 = _color1, color2 = _metal, hoistColor = _color3;
			var probabilityOfHoist = 0.0;
			var probabilityOfEmblem = 0.0;
			var emblemX = 0.5;

			switch (Randomizer.RandomWeighted(new List<int> { 51, 5, 3 }))
			{
				case 0: // balanced
					Color color3 = _color2;
					switch (Randomizer.RandomWeighted(new List<int> { 6, 26, 7, 4 }))
					{
						case 0:
							color3 = _color1;
							hoistColor = _color2;
							break;
						case 2:
							color1 = _metal;
							color2 = _color1;
							break;
						case 3: 
							color2 = _color2;
							color3 = _metal;
							break;
					}

					fesses = new DivisionFesses(color1, color2, color3, 1, 1, 1);
					probabilityOfHoist = 0.2;
					probabilityOfEmblem = 0.75;
					break;

				case 1: // center bigger
					fesses = new DivisionFesses(_color1, _metal, Randomizer.ProbabilityOfTrue(0.8) ? _color1 : _color2, 1, 2, 1);
					probabilityOfHoist = 0.2;
					probabilityOfEmblem = 1.0;
					
					if (Randomizer.ProbabilityOfTrue(0.3))
					{
						emblemX = 0.33;
					}

					break;

				default: // top-heavy
					if (Randomizer.ProbabilityOfTrue(0.667))
					{
						color1 = _metal;
						color2 = _color1;	
					}

					AddEmblem(0.33, _gridSize.Width * 3.0 / 4.0, _gridSize.Height / 4.0, color1 == _metal ? _color1 : _metal);
					fesses = new DivisionFesses(color1, color2, _color2, 2, 1, 1);
					break;
			}

			if (Randomizer.ProbabilityOfTrue(probabilityOfHoist))
			{
				var width = HoistElementWidth(true);
				AddTriangle(1, width, hoistColor);
				AddEmblem(0.33, width * 3.0 / 8.0, _gridSize.Height / 2.0, _metal);
			}
			else if (color2 == _metal)
			{
				AddEmblem(probabilityOfEmblem, _gridSize.Width * emblemX, _gridSize.Height / 2.0, Randomizer.ProbabilityOfTrue(0.5) ? _color1 : _color2);
			}

			return fesses;
		}

		private static DivisionBendsForward GetBendsForward()
		{
			if (Randomizer.ProbabilityOfTrue(0.875))
			{
				var width = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 3.0, _gridSize.Width / 10.0), 1, _gridSize.Width);

				if (Randomizer.ProbabilityOfTrue(0.7))
				{
					_overlays.Add(new OverlayFimbriationForward(_metal, width + 1, 0, 0));
					_overlays.Add(new OverlayFimbriationForward(_color2, width - 1, 0, 0));
				}
				else
				{
					_overlays.Add(new OverlayFimbriationForward(_metal, width, 0, 0));
				}
			}

			AddEmblem(0.5, _gridSize.Width / 5.0, _gridSize.Height / 3.0, _metal);
			return new DivisionBendsForward(_color1, _color2);
		}

		private static DivisionBendsBackward GetBendsBackward()
		{
			if (Randomizer.ProbabilityOfTrue(0.875))
			{
				var width = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 3.0, _gridSize.Width / 10.0), 1, _gridSize.Width);

				if (Randomizer.ProbabilityOfTrue(0.7))
				{
					_overlays.Add(new OverlayFimbriationBackward(_metal, width + 1, 0, 0));
					_overlays.Add(new OverlayFimbriationBackward(_color2, width - 1, 0, 0));
				}
				else
				{
					_overlays.Add(new OverlayFimbriationBackward(_metal, width, 0, 0));
				}
			}

			AddEmblem(0.5, _gridSize.Width * 4.0 / 5.0, _gridSize.Height / 3.0, _metal);
			return new DivisionBendsBackward(_color1, _color2);
		}

		private static DivisionX GetX()
		{
			if (Randomizer.ProbabilityOfTrue(0.3))
			{
				_overlays.Add(new OverlayBorder(_color2, _gridSize.Width / 8.0, 0, 0));
				AddCircleEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _color2, _metal);
				return new DivisionX(_color1, _metal);
			}
			
			var thickness = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 7.0, 1.5), 3, _gridSize.Width / 3);
			_overlays.Add(new OverlaySaltire(_metal, thickness, 0, 0));
			AddCircleEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _metal, _color1);

			return new DivisionX(_color1, _color2);
		}

		private static DivisionGrid GetHorizontal()
		{
			Color color1 = _color1, color2 = _color2, color3 = _metal;

			switch (Randomizer.RandomWeighted(new List<int>{10, 6, 4}))
			{
				case 0: // No hoist
					if (Randomizer.ProbabilityOfTrue(0.1))
					{
						color1 = _metal;
						color2 = _color1;
					}
					else if (Randomizer.ProbabilityOfTrue(0.44))
					{
						color2 = _metal;
					}

					var x = _gridSize.Width / 2.0;
					var y = _gridSize.Height / 2.0;
					if (Randomizer.ProbabilityOfTrue(0.33))
					{
						x = _gridSize.Width / 4.0;
						y = _gridSize.Height / 4.0;
					}

					AddEmblem(0.5, x, y, color1 == _metal || color2 == _metal ? _color2 : _metal);
					break;
				case 1: // Canton
					if (Randomizer.ProbabilityOfTrue(0.75))
					{
						color1 = _metal;
						color3 = _color1;
					}

					if (Randomizer.ProbabilityOfTrue(0.25))
					{
						_overlays.Add(new OverlayBox(color3, 0, 0, _gridSize.Height / 2.0, _gridSize.Height / 2.0, 0, 0));
						AddEmblem(1.0, _gridSize.Height / 4.0, _gridSize.Height / 4.0, color1);
					}
					else
					{
						var boxWidth = HoistElementWidth(false);
						_overlays.Add(new OverlayBox(color3, 0, 0, boxWidth, _gridSize.Height, 0, 0));
						AddEmblem(0.33, boxWidth / 2.0, _gridSize.Height / 2.0, color1);
					}
					break;
				default: // Triangle
					if (Randomizer.ProbabilityOfTrue(0.16))
					{
						color1 = _metal;
						color3 = _color1;
					}
						
					var triangleWidth = HoistElementWidth(true);
					AddTriangle(1.0, triangleWidth, color3);
					AddEmblem(0.33, triangleWidth * 3.0 / 8.0, _gridSize.Height / 2.0, _color3);

					if (Randomizer.ProbabilityOfTrue(0.33))
					{
						_overlays.Add(new OverlayPall(_color3, triangleWidth, _gridSize.Width / Randomizer.NextNormalized(10.0, 1.0), 0, 0));
					}

					break;
			}

			return new DivisionGrid(color1, color2, 1, 2);
		}

		#endregion

		#region Utility functions

		private static int HoistElementWidth(bool isTriangle)
		{
			return (int)(_gridSize.Width * Randomizer.NextNormalized(isTriangle ? 0.45 : 0.35, 0.05));
		}

		private static void AddTriangle(double probability, int width, Color color)
		{
			if (!Randomizer.ProbabilityOfTrue(probability)) return;
			_overlays.Add(new OverlayTriangle(color, 0, 0, width, _gridSize.Height / 2.0, 0, _gridSize.Height, 0, 0));
		}

		private static void AddCircleEmblem(double probability, double x, double y, Color circleColor, Color emblemColor)
		{
			if (!Randomizer.ProbabilityOfTrue(probability)) return;

			_overlays.Add(new OverlayEllipse(circleColor, x, y, _gridSize.Width / 4.0, 0.0, 0, 0));

			AddEmblem(1.0, x, y, emblemColor);
		}

		private static void AddEmblem(double probability, double x, double y, Color color)
		{
			if (!Randomizer.ProbabilityOfTrue(probability)) return;

			var emblem = Emblems[Randomizer.Next(Emblems.Count)];
			emblem.SetColors(new List<Color> { color });
			emblem.SetValues(new List<double> { x, y, _gridSize.Width / 6.0, 0 });
			_overlays.Add(emblem);
		}

		#endregion

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
			Blank,
			Band1,
			Band2,
			MultiStripes
		}
	}
}
