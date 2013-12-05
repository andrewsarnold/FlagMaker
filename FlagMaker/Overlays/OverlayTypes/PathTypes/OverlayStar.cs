using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	public class OverlayStar : OverlayPath
	{
		private const string Path = "m 0,-100 24,68 H 96 L 40,12 60,80 0,40 -60,80 -40,12 -96,-32 h 72 z";
		private static readonly Vector PathSize = new Vector(192, 180);

		public OverlayStar(int maximumX, int maximumY)
			: base("star", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayStar(Color color, int maximumX, int maximumY)
			: base(color, "star", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}
