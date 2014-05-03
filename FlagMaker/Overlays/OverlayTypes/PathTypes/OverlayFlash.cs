using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayFlash : OverlayPath
	{
		private const string Path = "m 62.353503,-160.6321 -191.463733,190.475 114.298053,0 L -138,156.57082 -127.44397,160.7 104.12739,-5.7821001 l -110.816867,0 L 137.77859,-129.2821 z";
		private static readonly Vector PathSize = new Vector(276, 322);

		public OverlayFlash(int maximumX, int maximumY)
			: base("flash", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayFlash(Color color, int maximumX, int maximumY)
			: base(color, "flash", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}
