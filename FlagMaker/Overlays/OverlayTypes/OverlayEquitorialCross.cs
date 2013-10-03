using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes
{
	public class OverlayEquitorialCross : PathOverlay
	{
		private const string Path = "M 1,3 1,1 3,1 3,-1 1,-1 1,-3 -1,-3 -1,-1 -3,-1 -3,1 -1,1 -1,3 Z";
		private static readonly Vector PathSize = new Vector(6, 6);

		public OverlayEquitorialCross(int maximum)
			: base("equitorial cross", Path, PathSize, maximum)
		{
		}

		public OverlayEquitorialCross(Color color, int maximum)
			: base(color, "equitorial cross", Path, PathSize, maximum)
		{
		}
	}
}
