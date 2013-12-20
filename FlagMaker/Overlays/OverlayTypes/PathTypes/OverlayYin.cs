using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	public class OverlayYin : OverlayPath
	{
		private const string Path = "M -118.53125 -116.59375 A 150.00017 150 0 0 0 131.09375 49.8125 A 75.00009 75.000005 0 1 0 6.28125 -33.375 A 75.000085 75 0 1 1 -118.53125 -116.59375 z";
		private static readonly Vector PathSize = new Vector(314, 255);

		public OverlayYin(int maximumX, int maximumY)
			: base("yin", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayYin(Color color, int maximumX, int maximumY)
			: base(color, "yin", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}