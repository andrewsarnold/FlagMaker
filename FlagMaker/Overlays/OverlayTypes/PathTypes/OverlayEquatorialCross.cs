using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayEquatorialCross : OverlayPath
	{
		private const string Path = "M 3,10 3,3 10,3 10,-3 3,-3 3,-10 -3,-10 -3,-3 -10,-3 -10,3 -3,3 -3,10 Z";
		private static readonly Vector PathSize = new Vector(20, 20);

		public OverlayEquatorialCross(int maximumX, int maximumY)
			: base("equatorial cross", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayEquatorialCross(Color color, int maximumX, int maximumY)
			: base(color, "equatorial cross", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}
