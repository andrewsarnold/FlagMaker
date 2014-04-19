using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	public class OverlayBolnisiCross : OverlayPath
	{
		private const string Path = "M -20.4375 -58.25 A 312 312 0 0 1 -15.71875 -15.6875 A 312 312 0 0 1 -58.375 -20.4375 A 165 165 0 0 1 -58.375 20.4375 A 312 312 0 0 1 -15.71875 15.71875 A 312 312 0 0 1 -20.4375 58.25 A 165 165 0 0 1 20.4375 58.25 A 312 312 0 0 1 15.71875 15.71875 A 312 312 0 0 1 58.125 20.4375 A 165 165 0 0 1 58.125 -20.4375 A 312 312 0 0 1 15.71875 -15.71875 A 312 312 0 0 1 20.4375 -58.25 A 165 165 0 0 1 -20.4375 -58.25 z";
		private static readonly Vector PathSize = new Vector(118, 118);

		public OverlayBolnisiCross(int maximumX, int maximumY)
			: base("bolnisi cross", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayBolnisiCross(Color color, int maximumX, int maximumY)
			: base(color, "bolnisi cross", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}
