using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using FlagMaker.Localization;
using FlagMaker.Overlays.OverlayTypes;
using FlagMaker.Overlays.OverlayTypes.PathTypes;
using FlagMaker.Overlays.OverlayTypes.RepeaterTypes;
using FlagMaker.Overlays.OverlayTypes.ShapeTypes;

namespace FlagMaker.Overlays
{
	public static class OverlayFactory
	{
		private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>
		                                                           {
			                                                           { "anchor", typeof (OverlayAnchor) },
			                                                           { "angola", typeof (OverlayAngola) },
			                                                           { "box", typeof (OverlayBox) },
			                                                           { "border", typeof (OverlayBorder) },
			                                                           { "branches", typeof (OverlayBranches) },
			                                                           { "cedar", typeof (OverlayCedar) },
			                                                           { "chakra", typeof (OverlayChakra) },
			                                                           { "checkerboard", typeof (OverlayCheckerboard) },
			                                                           { "coronet", typeof (OverlayCoronet) },
			                                                           { "cpusa", typeof (OverlayCpusa) },
			                                                           { "crescent", typeof (OverlayCrescent) },
			                                                           { "cross", typeof (OverlayCross) },
			                                                           { "crown", typeof (OverlayCrown) },
			                                                           { "diamond", typeof (OverlayDiamond) },
			                                                           { "eagle", typeof (OverlayEagle) },
			                                                           { "eagle american", typeof (OverlayEagleAmerican) },
			                                                           { "egypt", typeof (OverlayEgypt) },
			                                                           { "ellipse", typeof (OverlayEllipse) },
			                                                           { "ermine", typeof (OverlayErmine) },
			                                                           { "flag", typeof (OverlayFlag) },
																	   { "flash", typeof(OverlayFlash) },
			                                                           { "fleur de lis", typeof (OverlayFleurDeLis) },
			                                                           { "forth international", typeof (OverlayForthInternational) },
			                                                           { "equitorial cross", typeof (OverlayEquitorialCross) },
			                                                           { "fimbriation backward", typeof (OverlayFimbriationBackward) },
			                                                           { "fimbriation forward", typeof (OverlayFimbriationForward) },
			                                                           { "half saltire", typeof (OverlayHalfSaltire) },
			                                                           { "hammer and sickle", typeof (OverlayHammerSickle) },
			                                                           { "hand", typeof (OverlayHand) },
			                                                           { "harp", typeof (OverlayHarp) },
			                                                           { "iran", typeof (OverlayIran) },
			                                                           { "iron cross", typeof (OverlayIronCross) },
			                                                           { "laurel", typeof (OverlayLaurel) },
			                                                           { "kangaroo", typeof (OverlayKangaroo) },
			                                                           { "kiwi", typeof (OverlayKiwi) },
			                                                           { "line horizontal", typeof (OverlayLineHorizontal) },
			                                                           { "line vertical", typeof (OverlayLineVertical) },
			                                                           { "maltese cross", typeof (OverlayMalteseCross) },
			                                                           { "maple leaf", typeof (OverlayMapleLeaf) },
			                                                           { "mozambique", typeof (OverlayMozambique) },
			                                                           { "pall", typeof (OverlayPall) },
			                                                           { "papua new guinea", typeof (OverlayPapuaNewGuinea) },
			                                                           { "parteiadler", typeof (OverlayParteiadler) },
			                                                           { "pentagram", typeof (OverlayPentagram) },
																	   { "rays", typeof(OverlayRays) },
			                                                           { "reichsadler", typeof (OverlayReichsadler) },
																	   { "repeater lateral", typeof(OverlayRepeaterLateral) },
																	   { "repeater radial", typeof(OverlayRepeaterRadial) },
			                                                           { "saltire", typeof (OverlaySaltire) },
			                                                           { "shahadah", typeof (OverlayShahadah) },
			                                                           { "sikh", typeof (OverlaySikh) },
			                                                           { "springbok", typeof (OverlaySpringbok) },
			                                                           { "star", typeof (OverlayStar) },
			                                                           { "star four", typeof (OverlayStarFour) },
			                                                           { "star eight", typeof (OverlayStarEight) },
			                                                           { "star of david", typeof (OverlayStarOfDavid) },
			                                                           { "star seven", typeof (OverlayStarSeven) },
			                                                           { "star six", typeof (OverlayStarSix) },
			                                                           { "sword", typeof (OverlaySword) },
			                                                           { "sun", typeof (OverlaySun) },
			                                                           { "swastika", typeof (OverlaySwastika) },
			                                                           { "takbir", typeof (OverlayTakbir) },
			                                                           { "tree", typeof (OverlayTree) },
			                                                           { "triangle", typeof (OverlayTriangle) },
			                                                           { "trident", typeof (OverlayTrident) },
			                                                           { "triskele", typeof (OverlayTriskele) },
			                                                           { "yin", typeof (OverlayYin) }
		                                                           };

		public static Dictionary<string, OverlayPath> CustomTypes;

		public static Type GetOverlayType(string name)
		{
			var result = CustomTypes.Any(t => t.Key == name)
				? CustomTypes[name].GetType()
				: TypeMap.First(t => t.Key == name).Value;

			if (result == null)
			{
				throw new Exception(string.Format(strings.OverlayLoadError, name));
			}

			return result;
		}

		public static Overlay GetInstance(string name, int maxX = 1, int maxY = 1)
		{
			return GetInstance(GetOverlayType(name), maxX, maxY, name);
		}

		public static Overlay GetFlagInstance(string path, int maxX = 1, int maxY = 1)
		{
			return new OverlayFlag(Flag.LoadFromFile(path), path, maxX, maxY);
		}

		public static Overlay GetInstance(Type type, int maxX = 1, int maxY = 1, string name = "")
		{
			if (type == typeof(OverlayPath)) // custom overlay
			{
				var overlay = CustomTypes[name];

				// Create a unique copy
				var overlayCopy = overlay.Copy();
				overlayCopy.SetMaximum(maxX, maxY);
				return overlayCopy;
			}

			return (Overlay)Activator.CreateInstance(type, maxX, maxY);
		}

		public static Overlay GetDefaultOverlay(int maxX = 1, int maxY = 1)
		{
			return GetInstance(TypeMap["box"], maxX, maxY);
		}

		public static void FillCustomOverlays()
		{
			CustomTypes = new Dictionary<string, OverlayPath>();

			var path = string.Format("{0}Custom", AppDomain.CurrentDomain.BaseDirectory);

			foreach (var file in Directory.GetFiles(path, "*.ovr"))
			{
				try
				{
					var name = string.Empty;
					double width = 0;
					double height = 0;
					var pathData = string.Empty;

					using (var sr = new StreamReader(file))
					{
						string line;
						while ((line = sr.ReadLine()) != null)
						{
							switch (line.Split('=')[0].ToLower())
							{
								case "name":
									name = line.Split('=')[1];
									break;
								case "width":
									width = int.Parse(line.Split('=')[1]);
									break;
								case "height":
									height = int.Parse(line.Split('=')[1]);
									break;
								case "path":
									pathData = line.Split('=')[1];
									break;
							}
						}
					}

					if (CustomTypes.Any(t => String.Equals(t.Key, name, StringComparison.InvariantCultureIgnoreCase)) ||
					    TypeMap.Any(t => String.Equals(t.Key, name, StringComparison.InvariantCultureIgnoreCase)))
					{
						throw new DuplicateNameException(string.Format(strings.OverlayNameExists, name));
					}

					var overlay = new OverlayPath(name, pathData, new Vector(width, height), 1, 1);
					CustomTypes.Add(name, overlay);
				}
				catch (DuplicateNameException)
				{
					throw;
				}
				catch (Exception)
				{
					throw new Exception(string.Format(strings.OverlayLoadError, Path.GetFileNameWithoutExtension(file)));
				}
			}
		}

		public static IEnumerable<Type> GetOverlaysByType(Type type)
		{
			return TypeMap.Where(o => o.Value.IsSubclassOf(type)).Select(o => o.Value);
		}

		public static IEnumerable<Type> GetOverlaysNotInTypes(IEnumerable<Type> types)
		{
			return TypeMap.Where(o => !types.Any(t => o.Value.IsSubclassOf(t))).Select(o => o.Value);
		} 
	}
}
