using System;
using System.Collections.Generic;
using System.Linq;
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
			                                                           { "box", typeof (OverlayBox) },
			                                                           { "border", typeof (OverlayBorder) },
			                                                           { "crescent", typeof (OverlayCrescent) },
			                                                           { "cross", typeof (OverlayCross) },
			                                                           { "diamond", typeof (OverlayDiamond) },
			                                                           { "eagle", typeof (OverlayEagle) },
			                                                           { "ellipse", typeof (OverlayEllipse) },
			                                                           { "ermine", typeof (OverlayErmine) },
			                                                           { "flag", typeof (OverlayFlag) },
																	   { "flash", typeof(OverlayFlash) },
			                                                           { "fleur de lis", typeof (OverlayFleurDeLis) },
			                                                           { "equitorial cross", typeof (OverlayEquitorialCross) },
			                                                           { "fimbriation backward", typeof (OverlayFimbriationBackward) },
			                                                           { "fimbriation forward", typeof (OverlayFimbriationForward) },
			                                                           { "half saltire", typeof (OverlayHalfSaltire) },
			                                                           { "hammer and sickle", typeof (OverlayHammerSickle) },
			                                                           { "iron cross", typeof (OverlayIronCross) },
			                                                           { "line horizontal", typeof (OverlayLineHorizontal) },
			                                                           { "line vertical", typeof (OverlayLineVertical) },
			                                                           { "maltese cross", typeof (OverlayMalteseCross) },
			                                                           { "maple leaf", typeof (OverlayMapleLeaf) },
			                                                           { "pall", typeof (OverlayPall) },
			                                                           { "parteiadler", typeof (OverlayParteiadler) },
			                                                           { "pentagram", typeof (OverlayPentagram) },
																	   { "rays", typeof(OverlayRays) },
			                                                           { "reichsadler", typeof (OverlayReichsadler) },
																	   { "repeater lateral", typeof(OverlayRepeaterLateral) },
																	   { "repeater radial", typeof(OverlayRepeaterRadial) },
			                                                           { "saltire", typeof (OverlaySaltire) },
			                                                           { "star", typeof (OverlayStar) },
			                                                           { "star four", typeof (OverlayStarFour) },
			                                                           { "star eight", typeof (OverlayStarEight) },
			                                                           { "star of david", typeof (OverlayStarOfDavid) },
			                                                           { "star seven", typeof (OverlayStarSeven) },
			                                                           { "star six", typeof (OverlayStarSix) },
			                                                           { "swastika", typeof (OverlaySwastika) },
			                                                           { "sun", typeof (OverlaySun) },
			                                                           { "tree", typeof (OverlayTree) },
			                                                           { "triangle", typeof (OverlayTriangle) },
			                                                           { "yin", typeof (OverlayYin) }
		                                                           };

		public static IEnumerable<Type> GetOverlayTypes()
		{
			return TypeMap.Select(t => t.Value);
		} 

		public static Type GetOverlayType(string name)
		{
			return TypeMap.First(t => t.Key == name).Value;
		}

		public static Overlay GetInstance(string name, int maxX = 1, int maxY = 1)
		{
			return GetInstance(GetOverlayType(name), maxX, maxY);
		}

		public static Overlay GetInstance(string name, string path, int maxX = 1, int maxY = 1)
		{
			return new OverlayFlag(Flag.LoadFromFile(path), path, maxX, maxY);
		}

		public static Overlay GetInstance(Type type, int maxX = 1, int maxY = 1)
		{
			return (Overlay)Activator.CreateInstance(type, maxX, maxY);
		}
	}
}
