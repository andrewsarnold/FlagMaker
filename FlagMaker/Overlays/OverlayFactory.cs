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
			                                                           { "anchor", typeof (OverlayAnchor) },
			                                                           { "angola", typeof (OverlayAngola) },
			                                                           { "barbados", typeof (OverlayBarbados) },
			                                                           { "box", typeof (OverlayBox) },
			                                                           { "border", typeof (OverlayBorder) },
			                                                           { "branches", typeof (OverlayBranches) },
			                                                           { "cedar", typeof (OverlayCedar) },
			                                                           { "chakra", typeof (OverlayChakra) },
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
			                                                           { "triskele", typeof (OverlayTriskele) },
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
