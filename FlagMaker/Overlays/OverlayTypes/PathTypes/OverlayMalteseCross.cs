using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayMalteseCross : OverlayPath
	{
		private const string Path = "M -280,120 -10.000001,9.999998 -120,280 -1.3e-6,200 120,280 9.9999989,9.999998 280,120 200,-1.2999999e-6 280,-120 9.9999989,-10.000001 120,-280 -1.3e-6,-200 -120,-280 -10.000001,-10.000001 -280,-120 -200,-1.2999999e-6 z";
		private static readonly Vector PathSize = new Vector(540, 540);

		public OverlayMalteseCross(int maximumX, int maximumY)
			: base("maltese cross", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayMalteseCross(Color color, int maximumX, int maximumY)
			: base(color, "maltese cross", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}