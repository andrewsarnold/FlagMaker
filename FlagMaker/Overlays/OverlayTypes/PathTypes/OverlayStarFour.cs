using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayStarFour : OverlayPath
	{
		private const string Path = "M 30,30 145,0 30,-30 0,-145 -30,-30 -145,0 -30,30 0,145 z";
		private static readonly Vector PathSize = new Vector(290, 290);

		public OverlayStarFour(int maximumX, int maximumY)
			: base("star four", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayStarFour(Color color, int maximumX, int maximumY)
			: base(color, "star four", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}