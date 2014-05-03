using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayStarSix : OverlayPath
	{
		private const string Path = "M 2.9e-7,-57.5 -16.66667,-28.758199 -50,-28.758199 -33.33333,-5.0999996e-6 -50,28.758199 l 33.33333,0 L 2.9e-7,57.5 16.66667,28.758199 50,28.758199 33.33333,-5.0999996e-6 50,-28.758199 l -33.33333,0 L 2.9e-7,-57.5 z";
		private static readonly Vector PathSize = new Vector(100, 115);

		public OverlayStarSix(int maximumX, int maximumY)
			: base("star six", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayStarSix(Color color, int maximumX, int maximumY)
			: base(color, "star six", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}