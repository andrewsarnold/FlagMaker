using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayYang : OverlayPath
	{
		private const string Path = "M -83.20869,-55.472461 A 50.0022,50.0022 0 1 0 0,0 50.0022,50.0022 0 0 1 83.20869,55.472461 100.0044,100.0044 0 0 1 -83.20869,-55.472461 z";
		private static readonly Vector PathSize = new Vector(200, 200);

		public OverlayYang(int maximumX, int maximumY)
			: base("yang", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayYang(Color color, int maximumX, int maximumY)
			: base(color, "yang", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}
