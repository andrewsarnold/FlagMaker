using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes
{
	public class OverlayStar : PathOverlay
	{
		private const string Path = "m0,-24 6,17h18l-14,11 5,17-15-10-15,10 5-17-14-11h18z";
		private static readonly Vector PathSize = new Vector(50, 50);

		public OverlayStar(int maximum)
			: base("star", Path, PathSize, maximum)
		{
		}

		public OverlayStar(Color color, int maximum)
			: base(color, "star", Path, PathSize, maximum)
		{
		}
	}
}
