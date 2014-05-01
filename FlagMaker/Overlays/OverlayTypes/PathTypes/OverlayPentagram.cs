using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayPentagram : OverlayPath
	{
		private const string Path = "M -0.0312497,-100.1 -22.46875,-30.81875 -95.25,-30.75625 -36.40625,12.05625 -58.84375,81.4 C -43.67922,70.27811 -16.21833,50.34542 3e-7,38.5875 l 58.9062497,42.78125 -22.5,-69.28125 58.843754,-42.875 -72.718754,0 L -0.0312497,-100.1 z m 0,47.4375 7.1562497,21.84375 -14.2499997,0.03125 7.09375,-21.875 z M -27.25,-16.1625 -31.625,-2.5687501 -50.21875,-16.1 -27.25,-16.1625 z m 15.375,0 23.75,0 7.34375,22.625 -19.2187497,14 -19.2187503,-14 7.34375,-22.625 z m 39.125,0 22.9375,0 L 31.65625,-2.5687501 27.25,-16.1625 z M -23.96875,21.15 -12.40625,29.49375 -31,43.025 -23.96875,21.15 z m 48,0 L 31.0625,43.025 12.5,29.525 12.5,29.49375 24.03125,21.15 z";
		private static readonly Vector PathSize = new Vector(192, 183);

		public OverlayPentagram(int maximumX, int maximumY)
			: base("pentagram", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayPentagram(Color color, int maximumX, int maximumY)
			: base(color, "pentagram", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}
