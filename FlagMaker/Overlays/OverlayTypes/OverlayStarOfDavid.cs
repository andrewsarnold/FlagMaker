using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes
{
	public class OverlayStarOfDavid : PathOverlay
	{
		private const string Path = "M 0 -150.4375 L -10.40625 -132.3125 L -43.34375 -75 L -108 -75 L -128.71875 -75 L -118.40625 -57.03125 L -86.03125 -0.6875 L -118.40625 55.6875 L -128.71875 73.65625 L -108 73.65625 L -43.34375 73.65625 L -10.40625 130.96875 L 0 149.09375 L 10.40625 130.96875 L 43.34375 73.65625 L 108 73.65625 L 128.71875 73.65625 L 118.40625 55.6875 L 86.03125 -0.6875 L 118.40625 -57.03125 L 128.71875 -75 L 108 -75 L 43.34375 -75 L 10.40625 -132.3125 L 0 -150.4375 z M 0 -102.25 L 15.65625 -75 L -15.65625 -75 L 0 -102.25 z M -87.28125 -51 L -57.125 -51 L -72.1875 -24.75 L -87.28125 -51 z M -29.4375 -51 L 29.4375 -51 L 58.375 -0.6875 L 29.4375 49.65625 L -29.4375 49.65625 L -58.375 -0.6875 L -29.4375 -51 z M 57.125 -51 L 87.28125 -51 L 72.1875 -24.75 L 57.125 -51 z M -72.1875 23.40625 L -57.125 49.65625 L -87.28125 49.65625 L -72.1875 23.40625 z M 72.1875 23.40625 L 87.28125 49.65625 L 57.125 49.65625 L 72.1875 23.40625 z M -15.65625 73.65625 L 15.65625 73.65625 L 0 100.90625 L -15.65625 73.65625 z";
		private static readonly Vector PathSize = new Vector(258, 225);

		public OverlayStarOfDavid(int maximum)
			: base("star of david", Path, PathSize, maximum)
		{
		}

		public OverlayStarOfDavid(Color color, int maximum)
			: base(color, "star of david", Path, PathSize, maximum)
		{
		}
	}
}
