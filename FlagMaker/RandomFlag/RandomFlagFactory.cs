using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using FlagMaker.Divisions;
using FlagMaker.Overlays;
using FlagMaker.Overlays.OverlayTypes;
using FlagMaker.Overlays.OverlayTypes.PathTypes;
using FlagMaker.Overlays.OverlayTypes.RepeaterTypes;
using FlagMaker.Overlays.OverlayTypes.ShapeTypes;

namespace FlagMaker.RandomFlag
{
	public class RandomFlagFactory
	{
		private Ratio _ratio;
		private Ratio _gridSize;
		private DivisionTypes _divisionType;
		private Division _division;
		private List<Overlay> _overlays;
		private ColorScheme _colorScheme;
		private bool _canHaveCanton;

		private readonly List<Overlay> _emblems =
			OverlayFactory.GetOverlaysByType(typeof(OverlayPath)).Select(p => OverlayFactory.GetInstance(p))
				.Union(OverlayFactory.CustomTypes.Select(t => t.Value))
				.ToList();
		
		public Flag GenerateFlag()
		{
			_colorScheme = new ColorScheme();
			_canHaveCanton = true;
			GetRatio();
			_overlays = new List<Overlay>();
			_division = GetDivision();

			return new Flag("Random", _ratio, _gridSize, _division, _overlays);
		}

		private Flag GenerateFlag(ColorScheme colorScheme)
		{
			_colorScheme = colorScheme;
			_canHaveCanton = false;
			GetRatio();
			_overlays = new List<Overlay>();
			_division = GetDivision();

			return new Flag("Random", _ratio, _gridSize, _division, _overlays);
		}

		private void GetRatio()
		{
			_ratio = new List<Ratio>
			         {
				         new Ratio(3, 2),
				         new Ratio(5, 3),
				         new Ratio(2, 1)
			         }[Randomizer.RandomWeighted(new List<int> { 6, 1, 3 })];
			_gridSize = new Ratio(_ratio.Width * 8, _ratio.Height * 8);
		}

		private Division GetDivision()
		{
			// Roughly based on real-life usage
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
					return GetVertical();
				case DivisionTypes.Quartered:
					return GetQuartered();
				case DivisionTypes.Blank:
					return GetBlank();
				default: // Last three (Band1, Band2, MultiStripes) not implemented yet
					return GetStripes();
					//throw new Exception("No valid type selection");
			}
		}

		#region Division getters

		private DivisionGrid GetStripes()
		{
			var stripeCount = Randomizer.Clamp(Randomizer.NextNormalized(8, 3), 3, 15, true);
			
			if (Randomizer.ProbabilityOfTrue(0.14))
			{
				AddTriangle(1.0, HoistElementWidth(true), _colorScheme.Color2);
			}
			else
			{
				double width = HoistElementWidth(false);
				var stripe = (int)(stripeCount / 2.0) + 1;
				var height = _gridSize.Height * ((double)stripe / stripeCount);
				if (width < height) width = height;

				_overlays.Add(new OverlayBox(stripeCount > 5 && Randomizer.ProbabilityOfTrue(0.3) ? _colorScheme.Color1 : _colorScheme.Color2, 0, 0, width, height, _gridSize.Width, _gridSize.Height));
				AddEmblem(1.0, width / 2.0, height / 2.0, _colorScheme.Metal, true, _colorScheme.Color1);
			}

			return new DivisionGrid(_colorScheme.Color1, _colorScheme.Metal, 1, stripeCount);
		}

		private DivisionPales GetPale()
		{
			var color2 = Randomizer.ProbabilityOfTrue(0.27) ? _colorScheme.Color1 : _colorScheme.Color2;
			var isBalanced = Randomizer.ProbabilityOfTrue(0.9);

			AddEmblem(isBalanced ? 0.2 : 1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, Randomizer.ProbabilityOfTrue(0.5) ? _colorScheme.Color1 : _colorScheme.Color2, true, _colorScheme.Metal);

			return new DivisionPales(_colorScheme.Color1, _colorScheme.Metal, color2, 1, isBalanced ? 1 : 2, 1);
		}

		private DivisionFesses GetFesses()
		{
			DivisionFesses fesses;
			Color color1 = _colorScheme.Color1, color2 = _colorScheme.Metal, hoistColor = _colorScheme.Color3;
			var probabilityOfHoist = 0.0;
			var probabilityOfEmblem = 0.0;
			var emblemX = 0.5;

			switch (Randomizer.RandomWeighted(new List<int> { 51, 5, 3 }))
			{
				case 0: // balanced
					Color color3 = _colorScheme.Color2;
					switch (Randomizer.RandomWeighted(new List<int> { 6, 26, 7, 4 }))
					{
						case 0:
							color3 = _colorScheme.Color1;
							hoistColor = _colorScheme.Color2;
							break;
						case 2:
							color1 = _colorScheme.Metal;
							color2 = _colorScheme.Color1;
							break;
						case 3:
							color2 = _colorScheme.Color2;
							color3 = _colorScheme.Metal;
							break;
					}

					fesses = new DivisionFesses(color1, color2, color3, 1, 1, 1);
					probabilityOfHoist = 0.2;
					probabilityOfEmblem = 0.75;
					break;

				case 1: // center bigger
					fesses = new DivisionFesses(_colorScheme.Color1, _colorScheme.Metal, Randomizer.ProbabilityOfTrue(0.8) ? _colorScheme.Color1 : _colorScheme.Color2, 1, 2, 1);
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
						color1 = _colorScheme.Metal;
						color2 = _colorScheme.Color1;
					}

					AddEmblem(0.33, _gridSize.Width * 3.0 / 4.0, _gridSize.Height / 4.0, color1 == _colorScheme.Metal ? _colorScheme.Color1 : _colorScheme.Metal, true, color1 == _colorScheme.Metal ? _colorScheme.Metal : _colorScheme.Color1);
					fesses = new DivisionFesses(color1, color2, _colorScheme.Color2, 2, 1, 1);
					break;
			}

			if (Randomizer.ProbabilityOfTrue(probabilityOfHoist))
			{
				var width = HoistElementWidth(true);
				AddTriangle(1, width, hoistColor);
				AddEmblem(0.33, width * 3.0 / 8.0, _gridSize.Height / 2.0, _colorScheme.Metal, true, _colorScheme.Color1);
			}
			else if (color2 == _colorScheme.Metal)
			{
				var useColor1 = Randomizer.ProbabilityOfTrue(0.5);
				AddEmblem(probabilityOfEmblem, _gridSize.Width * emblemX, _gridSize.Height / 2.0, useColor1 ? _colorScheme.Color1 : _colorScheme.Color2, true, useColor1 ? _colorScheme.Color2 : _colorScheme.Color1);
			}

			return fesses;
		}

		private DivisionBendsForward GetBendsForward()
		{
			if (Randomizer.ProbabilityOfTrue(0.875))
			{
				var width = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 3.0, _gridSize.Width / 10.0), 1, _gridSize.Width);

				if (Randomizer.ProbabilityOfTrue(0.7))
				{
					_overlays.Add(new OverlayFimbriationForward(_colorScheme.Metal, width + 1, _gridSize.Width, _gridSize.Height));
					_overlays.Add(new OverlayFimbriationForward(_colorScheme.Color2, width - 1, _gridSize.Width, _gridSize.Height));
				}
				else
				{
					_overlays.Add(new OverlayFimbriationForward(_colorScheme.Metal, width, _gridSize.Width, _gridSize.Height));
				}
			}

			AddEmblem(0.5, _gridSize.Width / 5.0, _gridSize.Height / 4.0, _colorScheme.Metal, true, _colorScheme.Color1);
			return new DivisionBendsForward(_colorScheme.Color1, _colorScheme.Color2);
		}

		private DivisionBendsBackward GetBendsBackward()
		{
			if (Randomizer.ProbabilityOfTrue(0.875))
			{
				var width = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 3.0, _gridSize.Width / 10.0), 1, _gridSize.Width);

				if (Randomizer.ProbabilityOfTrue(0.7))
				{
					_overlays.Add(new OverlayFimbriationBackward(_colorScheme.Metal, width + 1, _gridSize.Width, _gridSize.Height));
					_overlays.Add(new OverlayFimbriationBackward(_colorScheme.Color2, width - 1, _gridSize.Width, _gridSize.Height));
				}
				else
				{
					_overlays.Add(new OverlayFimbriationBackward(_colorScheme.Metal, width, _gridSize.Width, _gridSize.Height));
				}
			}

			AddEmblem(0.5, _gridSize.Width * 4.0 / 5.0, _gridSize.Height / 4.0, _colorScheme.Metal, true, _colorScheme.Color1);
			return new DivisionBendsBackward(_colorScheme.Color1, _colorScheme.Color2);
		}

		private DivisionX GetX()
		{
			if (Randomizer.ProbabilityOfTrue(0.3))
			{
				_overlays.Add(new OverlayBorder(_colorScheme.Color2, _gridSize.Width / 8.0, _gridSize.Width, _gridSize.Height));
				AddCircleEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _colorScheme.Color2, _colorScheme.Metal, _colorScheme.Color2);
				return new DivisionX(_colorScheme.Color1, _colorScheme.Metal);
			}

			var thickness = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 7.0, 1.5), 3, _gridSize.Width / 3);
			_overlays.Add(new OverlaySaltire(_colorScheme.Metal, thickness, _gridSize.Width, _gridSize.Height));
			AddCircleEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _colorScheme.Metal, _colorScheme.Color1, _colorScheme.Metal);

			return new DivisionX(_colorScheme.Color1, _colorScheme.Color2);
		}

		private DivisionGrid GetHorizontal()
		{
			Color color1 = _colorScheme.Color1, color2 = _colorScheme.Color2, color3 = _colorScheme.Metal;

			switch (Randomizer.RandomWeighted(new List<int> { 10, 6, 4 }))
			{
				case 0: // No hoist
					if (Randomizer.ProbabilityOfTrue(0.1))
					{
						color1 = _colorScheme.Metal;
						color2 = _colorScheme.Color1;
					}
					else if (Randomizer.ProbabilityOfTrue(0.44))
					{
						color2 = _colorScheme.Metal;
					}

					var x = _gridSize.Width / 2.0;
					var y = _gridSize.Height / 2.0;
					if (Randomizer.ProbabilityOfTrue(0.33))
					{
						x = _gridSize.Width / 4.0;
						y = _gridSize.Height / 4.0;
					}

					var useColor2 = color1 == _colorScheme.Metal || color2 == _colorScheme.Metal;
					AddEmblem(0.5, x, y, useColor2 ? _colorScheme.Color2 : _colorScheme.Metal, true, useColor2 ? _colorScheme.Metal : _colorScheme.Color2);
					break;
				case 1: // Canton
					if (Randomizer.ProbabilityOfTrue(0.75))
					{
						color1 = _colorScheme.Metal;
						color3 = _colorScheme.Color1;
					}

					if (Randomizer.ProbabilityOfTrue(0.25))
					{
						_overlays.Add(new OverlayBox(color3, 0, 0, _gridSize.Height / 2.0, _gridSize.Height / 2.0, _gridSize.Width, _gridSize.Height));
						AddEmblem(1.0, _gridSize.Height / 4.0, _gridSize.Height / 4.0, color1, true, color3);
					}
					else
					{
						var boxWidth = HoistElementWidth(false);
						_overlays.Add(new OverlayBox(color3, 0, 0, boxWidth, _gridSize.Height, _gridSize.Width, _gridSize.Height));
						AddEmblem(0.33, boxWidth / 2.0, _gridSize.Height / 2.0, color1, true, color3);
					}
					break;
				default: // Triangle
					if (Randomizer.ProbabilityOfTrue(0.16))
					{
						color1 = _colorScheme.Metal;
						color3 = _colorScheme.Color1;
					}
						
					var triangleWidth = HoistElementWidth(true);
					AddTriangle(1.0, triangleWidth, color3);
					AddEmblem(0.33, triangleWidth * 3.0 / 8.0, _gridSize.Height / 2.0, _colorScheme.Color3, true, _colorScheme.Metal);

					if (Randomizer.ProbabilityOfTrue(0.33))
					{
						_overlays.Add(new OverlayPall(_colorScheme.Color3, triangleWidth, _gridSize.Width / Randomizer.NextNormalized(10.0, 1.0), _gridSize.Width, _gridSize.Height));
					}

					break;
			}

			return new DivisionGrid(color1, color2, 1, 2);
		}

		private DivisionGrid GetVertical()
		{
			Color color1 = _colorScheme.Metal, color2 = _colorScheme.Color1, color3 = _colorScheme.Color2;

			if (Randomizer.ProbabilityOfTrue(0.33))
			{
				color1 = _colorScheme.Color1;

				if (Randomizer.ProbabilityOfTrue(0.5))
				{
					color2 = _colorScheme.Color2;
					color3 = _colorScheme.Metal;
				}
				else
				{
					color2 = _colorScheme.Metal;
				}
			}

			AddEmblem(0.5, _gridSize.Width / 2.0, _gridSize.Height / 2.0, color3, true, color1);
			return new DivisionGrid(color1, color2, 2, 1);
		}

		private DivisionGrid GetQuartered()
		{
			if (Randomizer.ProbabilityOfTrue(0.5))
			{
				// Dominican Republic-style
				_overlays.Add(new OverlayCross(_colorScheme.Metal, _gridSize.Width / Randomizer.NextNormalized(10.0, 1.0), _gridSize.Width / 2.0, _gridSize.Height / 2.0, _gridSize.Width, _gridSize.Height));
				return new DivisionGrid(_colorScheme.Color1, _colorScheme.Color2, 2, 2);
			}
			
			// Panama-style
			_overlays.Add(new OverlayBox(_colorScheme.Color2, 0, _gridSize.Height / 2.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _gridSize.Width, _gridSize.Height));
			AddEmblem(1.0, _gridSize.Width / 4.0, _gridSize.Height / 4.0, _colorScheme.Color2, false, _colorScheme.Metal);
			AddEmblem(1.0, _gridSize.Width * 3.0 / 4.0, _gridSize.Height * 3.0 / 4.0, _colorScheme.Color1, false, _colorScheme.Metal);
			return new DivisionGrid(_colorScheme.Metal, _colorScheme.Color1, 2, 2);
		}

		private DivisionGrid GetBlank()
		{
			switch (Randomizer.RandomWeighted(new List<int> { 3, _canHaveCanton ? 2 : 0, 2, 1, 1, 1 }))
			{
				case 0: // Emblem
					if (Randomizer.ProbabilityOfTrue(0.5))
					{
						AddRepeater(_gridSize.Width / 2.0, _gridSize.Height / 2.0, _gridSize.Height, 0, _colorScheme.Metal, true);
					}
					else
					{
						var useColor2 = Randomizer.ProbabilityOfTrue(.11);
						AddEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, useColor2 ? _colorScheme.Color2 : _colorScheme.Metal,
							true, useColor2 ? _colorScheme.Metal : _colorScheme.Color2);
					}
					break;
				case 1: // Canton
					if (Randomizer.ProbabilityOfTrue(0.5))
					{
						AddFlag(new RandomFlagFactory().GenerateFlag(_colorScheme.Swapped));
						AddEmblem(1.0, 3 * _gridSize.Width / 4.0, _gridSize.Height / 2.0, _colorScheme.Metal, true, _colorScheme.Color2);
					}
					else
					{
						var cantonColor = Randomizer.ProbabilityOfTrue(0.5) ? _colorScheme.Color2 : _colorScheme.Metal;
						_overlays.Add(new OverlayBox(cantonColor, 0, 0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _gridSize.Width, _gridSize.Height));


						if (Randomizer.ProbabilityOfTrue(0.5))
						{
							AddRepeater(_gridSize.Width / 4.0, _gridSize.Height / 4.0, _gridSize.Width / 3.0, _gridSize.Height / 3.0, cantonColor == _colorScheme.Metal ? _colorScheme.Color1 : _colorScheme.Metal, false);
						}
						else
						{
							AddEmblem(1.0, _gridSize.Width / 4.0, _gridSize.Height / 4.0, cantonColor == _colorScheme.Metal ? _colorScheme.Color1 : _colorScheme.Metal, true, cantonColor == _colorScheme.Metal ? _colorScheme.Metal : _colorScheme.Color1);	
						}
					}
					
					break;
				case 2: // Cross
					var left = Randomizer.ProbabilityOfTrue(0.375) ? _gridSize.Width / 2.0 : _gridSize.Width / 3.0;
					var crossWidth = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 8.0, _gridSize.Width / 20.0), 2, _gridSize.Width / 3);
					if (Randomizer.ProbabilityOfTrue(0.5))
					{
						_overlays.Add(new OverlayCross(_colorScheme.Metal, crossWidth + 1, left, _gridSize.Height / 2.0, _gridSize.Width, _gridSize.Height));
						_overlays.Add(new OverlayCross(_colorScheme.Color2, crossWidth > 1 ? crossWidth - 1 : 1, left, _gridSize.Height / 2.0, _gridSize.Width, _gridSize.Height));
					}
					else
					{
						_overlays.Add(new OverlayCross(_colorScheme.Metal, crossWidth, left, _gridSize.Height / 2.0, _gridSize.Width, _gridSize.Height));
					}
					break;
				case 3: // Rays
					_overlays.Add(new OverlayRays(_colorScheme.Metal, _gridSize.Width / 2.0, _gridSize.Height / 2.0,
							Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width * 3 / 4.0, _gridSize.Width / 10.0), 4, 20), _gridSize.Width, _gridSize.Height));
					AddCircleEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _colorScheme.Metal, _colorScheme.Color1, _colorScheme.Metal);
					break;
				case 4: // Triangles
					double width1 = _gridSize.Width / 2.0, width2 = _gridSize.Width / 4.0;
					if (Randomizer.ProbabilityOfTrue(0.4))
					{
						width2 = width1;
						width1 = _gridSize.Width;
					}

					AddTriangle(1.0, (int)width1, _colorScheme.Metal);
					AddTriangle(1.0, (int)width2, _colorScheme.Color2);
					AddEmblem(0.5, _gridSize.Width / 8.0, _gridSize.Height / 2.0, _colorScheme.Metal, false, _colorScheme.Color2);
					break;
				default: // Saltire
					var saltireWidth = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 4.0, _gridSize.Width / 10.0), 2, _gridSize.Width / 3);
					if (Randomizer.ProbabilityOfTrue(0.5))
					{
						_overlays.Add(new OverlaySaltire(_colorScheme.Metal, saltireWidth + 1, _gridSize.Width, _gridSize.Height));
						_overlays.Add(new OverlaySaltire(_colorScheme.Color2, saltireWidth > 1 ? saltireWidth - 1 : 1, _gridSize.Width, _gridSize.Height));
					}
					else
					{
						_overlays.Add(new OverlaySaltire(_colorScheme.Metal, saltireWidth, _gridSize.Width, _gridSize.Height));
					}
					break;
			}

			return new DivisionGrid(_colorScheme.Color1, _colorScheme.Color1, 1, 1);
		}

		#endregion

		#region Utility functions

		private int HoistElementWidth(bool isTriangle)
		{
			return (int)(_gridSize.Width * Randomizer.NextNormalized(isTriangle ? 0.45 : 0.35, 0.05));
		}

		private void AddTriangle(double probability, int width, Color color)
		{
			if (!Randomizer.ProbabilityOfTrue(probability)) return;
			_overlays.Add(new OverlayTriangle(color, 0, 0, width, _gridSize.Height / 2.0, 0, _gridSize.Height, _gridSize.Width, _gridSize.Height));
		}

		private void AddRepeater(double x, double y, double width, double height, Color color, bool forceRadial)
		{
			var big = forceRadial;
			if (!forceRadial && Randomizer.ProbabilityOfTrue(0.5))
			{
				_overlays.Add(new OverlayRepeaterLateral(x, y, width, height,
					Randomizer.Clamp(Randomizer.NextNormalized(5, 2), 2, 8),
					Randomizer.Clamp(Randomizer.NextNormalized(4, 2), 2, 8), _gridSize.Width, _gridSize.Height));
			}
			else
			{
				big = true;
				_overlays.Add(new OverlayRepeaterRadial(x, y, width / 3.0,
					Randomizer.Clamp(Randomizer.NextNormalized(12, 4), 4, 25),
					Randomizer.ProbabilityOfTrue(0.5) ? 0 : _gridSize.Width, _gridSize.Width, _gridSize.Height));
			}

			AddEmblem(1, 0, 0, color, false, color, big);
		}

		private void AddCircleEmblem(double probability, double x, double y, Color circleColor, Color emblemColor, Color colorIfStroke)
		{
			if (!Randomizer.ProbabilityOfTrue(probability)) return;

			_overlays.Add(new OverlayEllipse(circleColor, x, y, _gridSize.Width / 4.0, 0.0, _gridSize.Width, _gridSize.Height));

			AddEmblem(1.0, x, y, emblemColor, true, colorIfStroke);
		}

		private void AddEmblem(double probability, double x, double y, Color color, bool canStroke, Color colorIfStroked, bool isBig = false)
		{
			if (probability < 1 && !Randomizer.ProbabilityOfTrue(probability)) return;
			
			var emblem = (OverlayPath)_emblems[Randomizer.Next(_emblems.Count)];
			emblem.SetMaximum(_gridSize.Width, _gridSize.Height);

			if (canStroke && Randomizer.ProbabilityOfTrue(0.1))
			{
				emblem.SetColor(colorIfStroked);
				emblem.StrokeColor = color;
				emblem.SetValues(new List<double> { x, y, _gridSize.Width / (isBig ? 3.0 : 6.0), 0, 2, _gridSize.Width });
			}
			else
			{
				emblem.SetColor(color);
				emblem.SetValues(new List<double> { x, y, _gridSize.Width / (isBig ? 3.0 : 6.0), 0, 0, 0 });
			}

			_overlays.Add(emblem);
		}

		private void AddFlag(Flag flag)
		{
			_overlays.Add(new OverlayFlag(flag, string.Empty, 0, 0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _gridSize.Width, _gridSize.Height));
		}

		#endregion
	}
}
