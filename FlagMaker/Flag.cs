using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using FlagMaker.Divisions;
using FlagMaker.Localization;
using FlagMaker.Overlays;
using FlagMaker.Overlays.OverlayTypes.RepeaterTypes;
using FlagMaker.Overlays.OverlayTypes.ShapeTypes;
using Microsoft.Win32;

namespace FlagMaker
{
	// An immutable flag object
	public class Flag
	{
		public readonly string Name;
		public readonly Ratio Ratio;
		public readonly Ratio GridSize;
		public readonly Division Division;
		public readonly List<Overlay> Overlays;

		public Flag(string name, Ratio ratio, Ratio gridSize, Division division, IEnumerable<Overlay> overlays)
		{
			Name = name;
			Ratio = ratio;
			GridSize = gridSize;
			Division = division;
			Overlays = overlays.ToList();
		}

		public static Flag LoadFromFile(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				throw new OperationCanceledException();
			}

			var name = string.Empty;
			var ratio = new Ratio(3, 2);
			var gridRatio = new Ratio(3, 2);

			string divisionType = "grid";
			Color divisionColor1 = Colors.White;
			Color divisionColor2 = Colors.White;
			Color divisionColor3 = Colors.White;
			int divisionVal1 = 1;
			int divisionVal2 = 1;
			int divisionVal3 = 1;

			var overlays = new List<TempOverlay>();

			var line = string.Empty;
			try
			{
				using (var sr = new StreamReader(filename))
				{
					bool isDivision = false;
					int overlayIndex = -1;

					while ((line = sr.ReadLine()) != null)
					{
						switch (line.Split('=')[0].ToLower())
						{
							case "name":
								name = line.Split('=')[1];
								break;
							case "ratio":
								var ratioStrings = line.Split('=')[1].Split(':');
								ratio = new Ratio(int.Parse(ratioStrings[1]), int.Parse(ratioStrings[0]));
								break;
							case "gridsize":
								var data = line.Split('=')[1].Split(':');
								gridRatio = new Ratio(int.Parse(data[1]), int.Parse(data[0]));
								break;
							case "division":
								isDivision = true;
								break;
							case "overlay":
								isDivision = false;
								overlayIndex++;
								overlays.Add(new TempOverlay());
								break;
							case "type":
								if (isDivision)
								{
									divisionType = line.Split('=')[1];
								}
								else
								{
									overlays[overlayIndex].Type = line.Split('=')[1];
								}
								break;
							case "color1":
								divisionColor1 = ParseColor(line.Split('=')[1]);
								break;
							case "color2":
								divisionColor2 = ParseColor(line.Split('=')[1]);
								break;
							case "color3":
								divisionColor3 = ParseColor(line.Split('=')[1]);
								break;
							case "color":
								overlays[overlayIndex].Color = ParseColor(line.Split('=')[1]);
								break;
							case "size1":
								if (isDivision)
								{
									divisionVal1 = int.Parse(line.Split('=')[1]);
								}
								else
								{
									overlays[overlayIndex].Values[0] = GetDoubleFromString(line.Split('=')[1]);
								}
								break;
							case "size2":
								if (isDivision)
								{
									divisionVal2 = int.Parse(line.Split('=')[1]);
								}
								else
								{
									overlays[overlayIndex].Values[1] = GetDoubleFromString(line.Split('=')[1]);
								}
								break;
							case "size3":
								if (isDivision)
								{
									divisionVal3 = int.Parse(line.Split('=')[1]);
								}
								else
								{
									overlays[overlayIndex].Values[2] = GetDoubleFromString(line.Split('=')[1]);
								}
								break;
							case "size4":
								overlays[overlayIndex].Values[3] = GetDoubleFromString(line.Split('=')[1]);
								break;
							case "size5":
								overlays[overlayIndex].Values[4] = GetDoubleFromString(line.Split('=')[1]);
								break;
							case "size6":
								overlays[overlayIndex].Values[5] = GetDoubleFromString(line.Split('=')[1]);
								break;
							case "size7":
								overlays[overlayIndex].Values[6] = GetDoubleFromString(line.Split('=')[1]);
								break;
							case "size8":
								overlays[overlayIndex].Values[7] = GetDoubleFromString(line.Split('=')[1]);
								break;
							case "path":
								overlays[overlayIndex].Path = line.Split('=')[1];
								break;
						}
					}
				}

				Division division;
				switch (divisionType)
				{
					case "fesses":
						division = new DivisionFesses(divisionColor1, divisionColor2, divisionColor3, divisionVal1, divisionVal2,
							divisionVal3);
						break;
					case "pales":
						division = new DivisionPales(divisionColor1, divisionColor2, divisionColor3, divisionVal1, divisionVal2,
							divisionVal3);
						break;
					case "bends forward":
						division = new DivisionBendsForward(divisionColor1, divisionColor2);
						break;
					case "bends backward":
						division = new DivisionBendsBackward(divisionColor1, divisionColor2);
						break;
					case "bends both":
						division = new DivisionX(divisionColor1, divisionColor2);
						break;
					default:
						division = new DivisionGrid(divisionColor1, divisionColor2, divisionVal1, divisionVal2);
						break;
				}

				return new Flag(name, ratio, gridRatio, division,
					overlays.Select(o => o.ToOverlay(gridRatio.Width, gridRatio.Height, Path.GetDirectoryName(filename))));
			}
			catch (Exception)
			{
				throw new FileLoadException(line);
			}
		}

		public static string GetFlagPath()
		{
			var dlg = new OpenFileDialog
			{
				DefaultExt = ".flag",
				Filter = string.Format("{0} (*.flag)|*.flag|{1} (*.*)|*.*", strings.Flag, strings.AllFiles),
				Multiselect = false
			};

			bool? result = dlg.ShowDialog();
			if (!((bool)result)) return string.Empty;
			return dlg.FileName;
		}

		public void Draw(Canvas canvas)
		{
			canvas.Children.Clear();
			Division.Draw(canvas);

			SetRepeaterOverlays();

			for (int i = 0; i < Overlays.Count; i++)
			{
				// Skip overlays used in repeaters
				if (i > 0 && Overlays[i - 1] is OverlayRepeater) continue;

				Overlays[i].Draw(canvas);
			}
		}

		public void ExportToSvg(string path)
		{
			const int width = 600;
			var height = (int)(((double)Ratio.Height / Ratio.Width) * width);

			using (var sw = new StreamWriter(path))
			{
				sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
				sw.WriteLine("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");
				sw.WriteLine("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"{0}\" height=\"{1}\">", width, height);

				sw.WriteLine(Division.ExportSvg(width, height));

				SetRepeaterOverlays();

				for (int i = 0; i < Overlays.Count; i++)
				{
					if (i > 0 && Overlays[i - 1] is OverlayRepeater) continue;

					var overlay = Overlays[i];

					try
					{
						sw.WriteLine(overlay.ExportSvg(width, height));
					}
					catch (NotImplementedException)
					{
						// Ignore overlays without SVG implementation
					}
				}

				sw.WriteLine("</svg>");
			}
		}

		public List<Color> ColorsUsed()
		{
			var colors = new List<Color>();

			if (Division is DivisionGrid && Division.Values[0] == 1 && Division.Values[1] == 1)
			{
				colors.Add(Division.Colors[0]);
			}
			else
			{
				colors.AddRange(Division.Colors);
			}

			foreach (var overlay in Overlays)
			{
				var flag = overlay as OverlayFlag;
				if (flag != null)
				{
					colors.AddRange(flag.Flag.ColorsUsed());
				}
				else if (!(overlay is OverlayRepeater))
				{
					colors.Add(overlay.Color);
				}
			}

			return colors.Distinct().OrderBy(c => c.Hue()).ToList();
		}

		private void SetRepeaterOverlays()
		{
			// Clear last repeater in list
			if (Overlays.Count > 0 && Overlays[Overlays.Count - 1] is OverlayRepeater)
			{
				((OverlayRepeater)Overlays[Overlays.Count - 1]).SetOverlay(null);
			}

			// Set overlays for others
			for (int i = Overlays.Count - 1; i > 0; i--)
			{
				var repeater = Overlays[i - 1] as OverlayRepeater;
				if (repeater != null)
				{
					repeater.SetOverlay(Overlays[i]);
				}
			}
		}

		private static Color ParseColor(string str)
		{
			Byte a = 0xff, r, b, g;

			if (str.Length == 8)
			{
				a = byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber);
				r = byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber);
				g = byte.Parse(str.Substring(4, 2), NumberStyles.HexNumber);
				b = byte.Parse(str.Substring(6, 2), NumberStyles.HexNumber);
			}
			else
			{
				r = byte.Parse(str.Substring(0, 2), NumberStyles.HexNumber);
				g = byte.Parse(str.Substring(2, 2), NumberStyles.HexNumber);
				b = byte.Parse(str.Substring(4, 2), NumberStyles.HexNumber);
			}

			return Color.FromArgb(a, r, g, b);
		}

		private static double GetDoubleFromString(string data)
		{
			// Doubles in files can be written as "123.45" or "123,45".
			// (Ignore thousands separators - not really applicable for FlagMaker.)
			// If the user saved a file with commas, replace and parse with invariant culture.
			return double.Parse(data.Replace(',', '.'), CultureInfo.InvariantCulture);
		}

		private class TempOverlay
		{
			public string Type;
			public readonly List<double> Values;
			public Color Color;
			public string Path;

			public TempOverlay()
			{
				Values = new List<double>
				{
					1,
					1,
					1,
					1,
					1,
					1,
					1,
					1
				};
			}

			public Overlay ToOverlay(int maxX, int maxY, string directory)
			{
				Overlay overlay;

				if (!string.IsNullOrWhiteSpace(Path))
				{
					overlay = Type == "flag"
						? OverlayFactory.GetFlagInstance(Path, maxX, maxY)
						: OverlayFactory.GetImageInstance(Path, directory, maxX, maxY);
				}
				else
				{
					overlay = OverlayFactory.GetInstance(Type, maxX, maxY);
				}

				if (overlay == null) return null;

				overlay.SetColors(new List<Color> { Color });
				overlay.SetValues(Values);
				return overlay;
			}
		}
	}
}
