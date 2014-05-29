using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayShield : OverlayPath
	{
		private const string Path = "m -99.913278,-130.6551 201.180888,-0.54666 c 0,0 0.41114,60.801139 -1.06377,127.2427789 C 98.280472,82.686289 23.081262,132.47878 0.6771725,131.19807 -25.446228,129.72547 -97.373478,77.864309 -100.42739,-2.9874312 -102.67845,-62.584321 -99.913278,-130.6551 -99.913278,-130.6551 z";
		private static readonly Vector PathSize = new Vector(207, 267);

		public OverlayShield(int maximumX, int maximumY)
			: base("shield", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayShield(Color color, int maximumX, int maximumY)
			: base(color, "shield", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}
