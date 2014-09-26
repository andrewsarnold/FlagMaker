using System;
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
						 new Ratio(7, 4),
				         new Ratio(2, 1)
			         }[Randomizer.RandomWeighted(new List<int> { 6, 2, 3, 4 })];
			_gridSize = new Ratio(_ratio.Width * 8, _ratio.Height * 8);
		}

		private Division GetDivision()
		{
			// Roughly based on real-life usage
			// 206 flags surveyed
			_divisionType = (DivisionTypes)Randomizer.RandomWeighted(new List<int>
			                                                          {
				                                                          9, // stripes
				                                                          22, // pales
				                                                          66, // fesses
				                                                          38, // blank
																		  21, // horizontal halves
																		  6, // vertical halves
																		  10, // diagonal
																		  12, // stripe
																		  9, // cross
																		  3, // x
																		  0 // 11 // other
			                                                          });
			//_divisionType = DivisionTypes.Diagonal;
			switch (_divisionType)
			{
				case DivisionTypes.Stripes:
					return GetStripes();
				case DivisionTypes.Pales:
					return GetPales();
				case DivisionTypes.Fesses:
					return GetFesses();
				case DivisionTypes.Blank:
					return GetBlank();
				case DivisionTypes.Horizontal:
					return GetHorizontal();
				case DivisionTypes.Vertical:
					return GetVertical();
				case DivisionTypes.Diagonal:
					return GetDiagonal();
				case DivisionTypes.Stripe:
					return GetStripe(); // to implement
				case DivisionTypes.Cross:
					return GetCross();
				case DivisionTypes.X:
					return GetX();
				default:
					throw new Exception("No valid type selection");
			}
		}

		#region Division getters

		private DivisionGrid GetStripes()
		{
			var stripeCount = Randomizer.Clamp(Randomizer.NextNormalized(10, 3), 5, 15, true);

			var stripeOuterColor = _colorScheme.Color1;
			var stripeInnerColor = _colorScheme.Metal;
			var cantonColor = _colorScheme.Color2;

			if (Randomizer.ProbabilityOfTrue(0.125))
			{
				var width = HoistElementWidth(true);
				AddTriangle(1.0, 1.0, width, cantonColor, stripeInnerColor);
			}
			else
			{
				var isMainColorMetal = Randomizer.ProbabilityOfTrue(0.142857);
				if (isMainColorMetal)
				{
					stripeOuterColor = _colorScheme.Metal;
					stripeInnerColor = _colorScheme.Color1;
					cantonColor = _colorScheme.Metal;
				}
				else if (Randomizer.ProbabilityOfTrue(0.16667))
				{
					cantonColor = stripeOuterColor;
				}

				double width = HoistElementWidth(false);
				var cantonHeight = _gridSize.Height * ((double)((int)(stripeCount / 2.0) + 1) / stripeCount);
				if (width < cantonHeight) width = cantonHeight;

				_overlays.Add(new OverlayBox(cantonColor, 0, 0, width, cantonHeight, _gridSize.Width, _gridSize.Height));

				if (Randomizer.ProbabilityOfTrue(0.142857))
				{
					AddRepeater(width / 2, cantonHeight / 2, width * 3 / 4.0, cantonHeight * 3 / 4.0, stripeInnerColor, false);
				}
				else
				{
					AddEmblem(1.0, width / 2, cantonHeight / 2, stripeInnerColor, false, Colors.White);
				}
			}

			return new DivisionGrid(stripeOuterColor, stripeInnerColor, 1, stripeCount);
		}

		private DivisionPales GetPales()
		{
			Color c1 = _colorScheme.Color1;
			Color c2;
			Color c3;
			Color emblemColor;
			var isBalanced = true;
			var emblemInCenter = true;
			double probabilityOfEmblem;

			if (Randomizer.ProbabilityOfTrue(0.13636))
			{
				c2 = _colorScheme.Color2;

				if (Randomizer.ProbabilityOfTrue(0.333))
				{
					c3 = _colorScheme.Color3;
					emblemColor = _colorScheme.Metal;
					probabilityOfEmblem = 1.0;
				}
				else if (Randomizer.ProbabilityOfTrue(0.5))
				{
					c3 = _colorScheme.Metal;
					emblemColor = _colorScheme.Metal;
					probabilityOfEmblem = 1.0;
				}
				else
				{
					c3 = _colorScheme.Color1;
					emblemInCenter = false;
					emblemColor = _colorScheme.Metal;
					probabilityOfEmblem = 1.0;
				}
			}
			else
			{
				c2 = _colorScheme.Metal;
				emblemColor = Randomizer.ProbabilityOfTrue(0.5) ? _colorScheme.Color1 : _colorScheme.Color2;

				if (Randomizer.ProbabilityOfTrue(0.2632))
				{
					c3 = _colorScheme.Color2;
					probabilityOfEmblem = 0.357;
				}
				else
				{
					c3 = c1;
					probabilityOfEmblem = 0.6;
				}

				if (Randomizer.ProbabilityOfTrue(0.1052))
				{
					isBalanced = false;
					probabilityOfEmblem = 1.0;
				}
			}

			AddEmblem(probabilityOfEmblem, emblemInCenter ? _gridSize.Width / 2.0 : _gridSize.Width / 6.0, _gridSize.Height / 2.0, emblemColor, false, Colors.White);
			return new DivisionPales(c1, c2, c3, 1, isBalanced ? 1 : 2, 1);
		}

		private DivisionFesses GetFesses()
		{
			Color c1;
			Color c2;
			Color c3;
			Color emblemColor;
			Color hoistColor;
			var isSpanish = false;
			var isLatvian = false;
			var isColombian = false;
			double probabilityOfEmblem;
			double probabilityOfHoist;

			if (Randomizer.ProbabilityOfTrue(0.166667))
			{
				c1 = _colorScheme.Metal;
				c2 = _colorScheme.Color1;
				c3 = _colorScheme.Color2;
				hoistColor = c2;
				probabilityOfHoist = 0.0909;
				probabilityOfEmblem = 0.5454;
				isColombian = Randomizer.ProbabilityOfTrue(0.1818182);
				emblemColor = isColombian ? c3 : c1;
			}
			else
			{
				c1 = _colorScheme.Color1;

				if (Randomizer.ProbabilityOfTrue(0.29))
				{
					c2 = _colorScheme.Color2;

					if (Randomizer.ProbabilityOfTrue(0.25))
					{
						c3 = _colorScheme.Color1;
						isLatvian = Randomizer.ProbabilityOfTrue(0.5);
						isSpanish = !isLatvian;
						probabilityOfEmblem = isSpanish ? 1.0 : 0.0;
						probabilityOfHoist = 0;
						hoistColor = Colors.Transparent;
						emblemColor = _colorScheme.Metal;
					}
					else if (Randomizer.ProbabilityOfTrue(0.5))
					{
						c3 = _colorScheme.Color3;
						probabilityOfHoist = 0.0;
						probabilityOfEmblem = 0.833333;
						var hasFimbriations = Randomizer.ProbabilityOfTrue(0.5);
						isSpanish = !hasFimbriations && Randomizer.ProbabilityOfTrue(0.2);
						hoistColor = Colors.Transparent;
						emblemColor = _colorScheme.Metal;

						if (hasFimbriations)
						{
							_overlays.Add(new OverlayLineHorizontal(_colorScheme.Metal, _gridSize.Width / 20.0, _gridSize.Width / 3.0, _gridSize.Width, _gridSize.Width));
							_overlays.Add(new OverlayLineHorizontal(_colorScheme.Metal, _gridSize.Width / 20.0, 2 * _gridSize.Width / 3.0, _gridSize.Width, _gridSize.Width));
						}
					}
					else
					{
						c3 = _colorScheme.Metal;
						hoistColor = _colorScheme.Color3;
						emblemColor = _colorScheme.Metal;
						probabilityOfHoist = 0.166667;
						probabilityOfEmblem = 0.166667;
					}
				}
				else
				{
					c2 = _colorScheme.Metal;

					if (Randomizer.ProbabilityOfTrue(0.2564))
					{
						c3 = _colorScheme.Color1;
						isSpanish = Randomizer.ProbabilityOfTrue(0.3);
						isLatvian = !isSpanish && Randomizer.ProbabilityOfTrue(0.1429);
						hoistColor = _colorScheme.Color2;
						emblemColor = _colorScheme.Color2;
						probabilityOfHoist = 0.2;
						probabilityOfEmblem = 0.7;
					}
					else
					{
						c3 = _colorScheme.Color2;
						hoistColor = _colorScheme.Color3;
						emblemColor = Randomizer.ProbabilityOfTrue(0.5) ? c1 : c3;
						isColombian = Randomizer.ProbabilityOfTrue(0.0345);
						probabilityOfHoist = 0.2414;
						probabilityOfEmblem = 0.6552;
					}
				}
			}

			if (isSpanish) { probabilityOfEmblem = 1.0; }
			else if (isLatvian) { probabilityOfEmblem = 0.0; }
			else if (isColombian) { probabilityOfHoist = 0.0; }

			if (Randomizer.ProbabilityOfTrue(probabilityOfHoist))
			{
				emblemColor = _colorScheme.Metal;
				AddTriangle(1.0, probabilityOfEmblem, HoistElementWidth(true), hoistColor, emblemColor);
			}
			else
			{
				AddEmblem(probabilityOfEmblem, _gridSize.Width / 2.0, _gridSize.Height / 2.0, emblemColor, false, Colors.Transparent);
			}

			return new DivisionFesses(c1, c2, c3, isLatvian || isColombian ? 2 : 1, isSpanish ? 2 : 1, isLatvian ? 2 : 1);
		}

		private DivisionGrid GetCross()
		{
			var backgroundIsMetal = Randomizer.ProbabilityOfTrue(0.25);
			var fimbriate = !backgroundIsMetal && Randomizer.ProbabilityOfTrue(0.4286);

			var background = backgroundIsMetal ? _colorScheme.Metal : _colorScheme.Color1;
			var mainColor = backgroundIsMetal ? _colorScheme.Color1 : fimbriate ? _colorScheme.Color2 : _colorScheme.Metal;
			var fimbriation = _colorScheme.Metal;
			var center = _gridSize.Height / 2.0;

			var crossWidth = Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width / 8.0, _gridSize.Width / 20.0), 2, _gridSize.Width / 4) - (fimbriate ? 1 : 0);
			var fimbriationWidth = crossWidth + 2;

			var canSaltire = false;

			var intersection = _gridSize.Width / 2.0;
			if (Randomizer.ProbabilityOfTrue(0.555556))
			{
				intersection = _gridSize.Width / 3.0;
			}
			else
			{
				if (Randomizer.ProbabilityOfTrue(0.25))
				{
					_overlays.Add(new OverlayCross(_colorScheme.Metal, crossWidth, intersection, center, _gridSize.Width, _gridSize.Height));
					return new DivisionGrid(_colorScheme.Color1, _colorScheme.Color2, 2, 2);
				}

				canSaltire = !backgroundIsMetal;
			}

			if (canSaltire && Randomizer.ProbabilityOfTrue(0.5))
			{
				if (fimbriate)
				{
					_overlays.Add(new OverlaySaltire(fimbriation, fimbriationWidth, _gridSize.Width, _gridSize.Height));
					_overlays.Add(new OverlayHalfSaltire(mainColor, crossWidth + 2, _gridSize.Width, _gridSize.Height));
				}
				else
				{
					_overlays.Add(new OverlaySaltire(_colorScheme.Color2, crossWidth, _gridSize.Width, _gridSize.Height));
				}
			}

			if (fimbriate)
			{
				_overlays.Add(new OverlayCross(fimbriation, fimbriationWidth, intersection, center, _gridSize.Width, _gridSize.Height));
			}

			_overlays.Add(new OverlayCross(mainColor, crossWidth, intersection, center, _gridSize.Width, _gridSize.Height));

			return new DivisionGrid(background, background, 1, 1);
		}

		private DivisionGrid GetBlank()
		{
			var color = _colorScheme.Color1;

			switch (new List<int>
			        {
				        1,
				        2,
				        3,
						4
			        }[Randomizer.RandomWeighted(new List<int> { _canHaveCanton ? 10 : 0, 26, 2, 1 })])
			{
				case 1:
					// Canton
					if (Randomizer.ProbabilityOfTrue(0.6))
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
				case 2:
					// Center emblem
					color = GetCenterEmblemForBlank();
					break;
				case 3:
					// Triangle
					var width = HoistElementWidth(true);
					if (Randomizer.ProbabilityOfTrue(0.5))
					{
						AddTriangle(1.0, 0.0, width <= _gridSize.Width / 2.0 ? width * 2 : _gridSize.Width, _colorScheme.Metal, _colorScheme.Metal);
					}
					else
					{
						AddTriangle(1.0, 0.0, width + 2, _colorScheme.Metal, _colorScheme.Metal);
					}
					AddTriangle(1.0, 1.0, width, _colorScheme.Color2, _colorScheme.Metal);
					break;
				case 4:
					// Rays
					_overlays.Add(new OverlayRays(_colorScheme.Metal, _gridSize.Width / 2.0, _gridSize.Height / 2.0,
							Randomizer.Clamp(Randomizer.NextNormalized(_gridSize.Width * 3 / 4.0, _gridSize.Width / 10.0), 4, 20), _gridSize.Width, _gridSize.Height));
					AddCircleEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _colorScheme.Metal, _colorScheme.Color1, _colorScheme.Metal);
					break;
			}

			return new DivisionGrid(color, _colorScheme.Color2, 1, 1);
		}

		private Color GetCenterEmblemForBlank()
		{
			switch (new List<int>
			        {
				        1,
				        2,
				        3,
						4,
						5
			        }[Randomizer.RandomWeighted(new List<int> { 20, 3, 1, _canHaveCanton ? 3 : 0, 2 })])
			{
				case 1:
					// Plain
					var isInverted = Randomizer.ProbabilityOfTrue(0.1);
					var useColor2 = Randomizer.ProbabilityOfTrue(.11);
					AddEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0,
						useColor2 ? _colorScheme.Color2 : (isInverted ? _colorScheme.Color1 : _colorScheme.Metal),
						!isInverted, useColor2 ? _colorScheme.Metal : _colorScheme.Color2);
					return isInverted ? _colorScheme.Metal : _colorScheme.Color1;
				case 2:
					// Circled
					AddCircleEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _colorScheme.Metal, _colorScheme.Color1, _colorScheme.Metal);
					return _colorScheme.Color1;
				case 3:
					// Repeater
					AddRepeater(_gridSize.Width / 2.0, _gridSize.Height / 2.0, _gridSize.Height, 0, _colorScheme.Metal, true);
					return _colorScheme.Color1;
				case 4:
					// Border
					_overlays.Add(new OverlayBorder(_colorScheme.Color2, _gridSize.Width / 8.0, _gridSize.Width, _gridSize.Height));
					AddEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _colorScheme.Metal, true, _colorScheme.Color2);
					return _colorScheme.Color1;
				default:
					// Stripes
					_overlays.Add(new OverlayLineHorizontal(_colorScheme.Color1, _gridSize.Height / 8.0, _gridSize.Height * (1 / 6.0), _gridSize.Width, _gridSize.Height));
					_overlays.Add(new OverlayLineHorizontal(_colorScheme.Color1, _gridSize.Height / 8.0, _gridSize.Height * (5 / 6.0), _gridSize.Width, _gridSize.Height));
					AddEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _colorScheme.Color1, false, _colorScheme.Color2);
					return _colorScheme.Metal;
			}
		}

		private DivisionGrid GetVertical()
		{
			switch (new List<int>
			        {
				        1,
				        2,
				        3
			        }[Randomizer.RandomWeighted(new List<int> { 2, 1, 3 })])
			{
				case 1:
					// Color 1 / Metal
					AddEmblem(1.0, Randomizer.ProbabilityOfTrue(0.5) ? _gridSize.Width / 2.0 : 3 * _gridSize.Width / 4.0, _gridSize.Height / 2.0, _colorScheme.Color2, false, _colorScheme.Metal);
					return new DivisionGrid(_colorScheme.Color1, _colorScheme.Metal, 2, 1);
				case 2:
					// Color 1 / Color 2
					AddEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _colorScheme.Metal, false, _colorScheme.Metal);
					return new DivisionGrid(_colorScheme.Color1, _colorScheme.Color2, 2, 1);
				default:
					// Metal / Color 1
					AddEmblem(0.5, _gridSize.Width / 4.0, _gridSize.Height / 4.0, _colorScheme.Color2, false, _colorScheme.Metal);
					return new DivisionGrid(_colorScheme.Metal, _colorScheme.Color1, 2, 1);
			}
		}

		private DivisionGrid GetHorizontal()
		{
			switch (new List<int>
			        {
				        1,
				        2,
				        3
			        }[Randomizer.RandomWeighted(new List<int> { 4, 11, 6 })])
			{
				case 1:
					// Color 1 / Metal
					AddEmblem(0.25, _gridSize.Width / 4.0, _gridSize.Height / 4.0, _colorScheme.Metal, false, _colorScheme.Metal);
					return new DivisionGrid(_colorScheme.Color1, _colorScheme.Metal, 1, 2);
				case 2:
					// Color 1 / Color 2

					if (Randomizer.ProbabilityOfTrue(0.181818))
					{
						var width = _gridSize.Width / 3.0;
						AddTriangle(1.0, 0.5, (int)width, Colors.Black, _colorScheme.Metal);
						_overlays.Add(new OverlayPall(_colorScheme.Metal, width, _gridSize.Width / 8.0, _gridSize.Width, _gridSize.Height));
						_overlays.Add(new OverlayPall(_colorScheme.Color3, width, _gridSize.Width / 5.0, _gridSize.Width, _gridSize.Height));
					}
					else
					{
						if (Randomizer.ProbabilityOfTrue(0.1111))
						{
							AddHoistForHorizontal(false, true);
						}
						else if (Randomizer.ProbabilityOfTrue(0.375))
						{
							var isTriangleMetal = Randomizer.ProbabilityOfTrue(0.6667);
							AddTriangle(1.0, 1.0, HoistElementWidth(true), isTriangleMetal ? _colorScheme.Metal : _colorScheme.Color3,
								isTriangleMetal ? _colorScheme.Color1 : _colorScheme.Metal);
						}
						else
						{
							// Plain with emblem
							var offset = Randomizer.ProbabilityOfTrue(0.25) ? 4.0 : 2.0;
							AddEmblem(1.0, _gridSize.Width / offset, _gridSize.Height / offset, _colorScheme.Metal, false, _colorScheme.Metal);
						}
					}

					return new DivisionGrid(_colorScheme.Color1, _colorScheme.Color2, 1, 2);
				default:
					// Metal / Color 1
					if (Randomizer.ProbabilityOfTrue(0.333))
					{
						AddHoistForHorizontal(Randomizer.ProbabilityOfTrue(0.5), false);
					}
					return new DivisionGrid(_colorScheme.Metal, _colorScheme.Color1, 1, 2);
			}
		}

		private void AddHoistForHorizontal(bool isHalfSize, bool isMetal)
		{
			var width = HoistElementWidth(false);
			_overlays.Add(new OverlayBox(isMetal ? _colorScheme.Metal : _colorScheme.Color2, 0, 0, width,
				isHalfSize ? _gridSize.Height / 2.0 : _gridSize.Height, _gridSize.Width, _gridSize.Height));
			AddEmblem(1.0, width / 2.0, isHalfSize ? _gridSize.Height / 4.0 : _gridSize.Height / 2.0, isMetal ? _colorScheme.Color1 : _colorScheme.Metal, false, _colorScheme.Metal);
		}

		private Division GetDiagonal()
		{
			bool isForward = Randomizer.ProbabilityOfTrue(0.7);

			var schemes = new List<List<Color>>
			              {
							  new List<Color> { _colorScheme.Metal, _colorScheme.Color1, Colors.Transparent, Colors.Transparent, _colorScheme.Color2 },
							  new List<Color> { _colorScheme.Color1, _colorScheme.Color2, Colors.Transparent, Colors.Transparent, _colorScheme.Metal },
							  new List<Color> { _colorScheme.Color1, _colorScheme.Color1, _colorScheme.Color2, _colorScheme.Metal, _colorScheme.Metal },
							  new List<Color> { _colorScheme.Color1, _colorScheme.Color2, _colorScheme.Color3, _colorScheme.Metal, _colorScheme.Metal },
							  new List<Color> { _colorScheme.Metal, _colorScheme.Metal, _colorScheme.Color1, Colors.Transparent, _colorScheme.Color2 },
							  new List<Color> { _colorScheme.Color1, _colorScheme.Color2, _colorScheme.Metal, Colors.Transparent , _colorScheme.Metal }
			              };
			var scheme = schemes[Randomizer.RandomWeighted(new List<int> { 1, 1, 2, 3, 1, 2 })];

			Color topColor = scheme[0];
			Color bottomColor = scheme[1];
			Color stripeColor = scheme[2];
			Color fimbriationColor = scheme[3];
			Color emblemColor = scheme[4];

			bool hasStripe = stripeColor != Colors.Transparent;
			bool hasFimbriation = fimbriationColor != Colors.Transparent;

			if (hasStripe)
			{
				if (hasFimbriation)
				{
					if (isForward)
					{
						_overlays.Add(new OverlayFimbriationForward(fimbriationColor, _gridSize.Width / 3.0, _gridSize.Width, _gridSize.Height));
					}
					else
					{
						_overlays.Add(new OverlayFimbriationBackward(fimbriationColor, _gridSize.Width / 3.0, _gridSize.Width, _gridSize.Height));
					}
				}
				if (isForward)
				{
					_overlays.Add(new OverlayFimbriationForward(stripeColor, _gridSize.Width / 4.0, _gridSize.Width, _gridSize.Height));
				}
				else
				{
					_overlays.Add(new OverlayFimbriationBackward(stripeColor, _gridSize.Width / 4.0, _gridSize.Width, _gridSize.Height));
				}
			}

			var emblemLeft = hasStripe
				? (isForward ? 1.0 : 5.0) * _gridSize.Width / 6.0
				: _gridSize.Width / 2.0;
			var emblemTop = hasStripe
				? _gridSize.Height / 4.0
				: _gridSize.Height / 2.0;
			AddEmblem(1.0, emblemLeft, emblemTop, emblemColor, false, emblemColor);

			if (isForward)
			{
				return new DivisionBendsForward(topColor, bottomColor);
			}

			return new DivisionBendsBackward(topColor, bottomColor);
		}

		private DivisionGrid GetStripe()
		{
			return new DivisionGrid(_colorScheme.Color1, _colorScheme.Color2, 1, 1);
		}

		private Division GetX()
		{
			switch (new List<int>
			        {
				        1,
				        2,
				        3
			        }[Randomizer.RandomWeighted(new List<int> { 2, 1, 1 })])
			{
				case 1:
					// Two-color, fimbriation
					_overlays.Add(new OverlaySaltire(_colorScheme.Metal, _gridSize.Width / 6.0, _gridSize.Width, _gridSize.Height));
					AddCircleEmblem(0.5, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _colorScheme.Metal, _colorScheme.Color1, _colorScheme.Color1);
					return new DivisionX(_colorScheme.Color1, _colorScheme.Color2);
				case 2:
					// Two-color, no fimbriation
					AddCircleEmblem(1.0, _gridSize.Width / 2.0, _gridSize.Height / 2.0, _colorScheme.Color2, _colorScheme.Metal, _colorScheme.Metal);
					if (Randomizer.ProbabilityOfTrue(0.5))
					{
						_overlays.Add(new OverlayBorder(_colorScheme.Color2, _gridSize.Width / 8.0, _gridSize.Width, _gridSize.Height));
					}
					return new DivisionX(_colorScheme.Color1, _colorScheme.Metal);
				default:
					// One-color
					_overlays.Add(new OverlaySaltire(_colorScheme.Metal, _gridSize.Width / 6.0, _gridSize.Width, _gridSize.Height));
					return new DivisionGrid(_colorScheme.Color1, _colorScheme.Color1, 1, 1);
			}
		}

		#endregion

		#region Utility functions

		private int HoistElementWidth(bool isTriangle)
		{
			return (int)(_gridSize.Width * Randomizer.NextNormalized(isTriangle ? 0.45 : 0.35, 0.05));
		}

		private void AddTriangle(double probability, double probabilityOfEmblem, int width, Color color, Color emblemColor)
		{
			if (!Randomizer.ProbabilityOfTrue(probability)) return;
			_overlays.Add(new OverlayTriangle(color, 0, 0, width, _gridSize.Height / 2.0, 0, _gridSize.Height, _gridSize.Width, _gridSize.Height));

			if (Randomizer.ProbabilityOfTrue(probabilityOfEmblem))
			{
				AddEmblem(1.0, width / 3.0, _gridSize.Height / 2.0, emblemColor, false, Colors.White);
			}
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
