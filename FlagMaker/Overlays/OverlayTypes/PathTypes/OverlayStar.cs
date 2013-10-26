using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	public class OverlayStar : OverlayPath
	{
		private const string Path = "m0,-24 6,17h18l-14,11 5,17-15-10-15,10 5-17-14-11h18z";
		private static readonly Vector PathSize = new Vector(50, 50);

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
