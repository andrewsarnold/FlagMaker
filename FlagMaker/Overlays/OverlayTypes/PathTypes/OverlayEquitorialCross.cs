using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayEquitorialCross : OverlayPath
	{
		private const string Path = "M 1,3 1,1 3,1 3,-1 1,-1 1,-3 -1,-3 -1,-1 -3,-1 -3,1 -1,1 -1,3 Z";
		private static readonly Vector PathSize = new Vector(6, 6);

		public OverlayEquitorialCross(int maximumX, int maximumY)
			: base("equitorial cross", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayEquitorialCross(Color color, int maximumX, int maximumY)
			: base(color, "equitorial cross", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}
