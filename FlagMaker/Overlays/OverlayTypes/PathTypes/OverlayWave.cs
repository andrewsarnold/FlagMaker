using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayWave : OverlayPath
	{
		private const string Path = "m -47.998328 -1.8672054 q 6 2.9333319 12 0 6 -2.9333332 11.99999 0 6 2.9333319 12 0 6.0000006 -2.9333332 12.00000069 0 5.99999991 2.9333319 11.99998931 0 5.999999 -2.9333332 11.999999 0 6 2.9333319 12 0 6 -2.9333332 11.999991 0 v 3.7333318 q -5.99999 -2.933333 -11.99999 0 -6 2.9333319 -12 0 -6 -2.933333 -12 0 -5.9999892 2.9333319 -11.99998931 0 -5.99999989 -2.933333 -12.00000069 0 -6 2.9333319 -12 0 -5.99999 -2.933333 -11.99999 0 -6 2.9333319 -12 0 z";
		private static readonly Vector PathSize = new Vector(90, 8);

		public OverlayWave(int maximumX, int maximumY)
			: base("wave", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayWave(Color color, int maximumX, int maximumY)
			: base(color, "wave", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}
