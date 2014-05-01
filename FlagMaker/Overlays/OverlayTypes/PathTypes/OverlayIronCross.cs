using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayIronCross : OverlayPath
	{
		private const string Path = "M -155.90625,-270 A 352.46,352.46 0 0 1 -31.84375,-31.71875 352.46,352.46 0 0 1 -270,-155.90625 l 0,311.8125 A 352.46,352.46 0 0 1 -31.71875,31.84375 352.46,352.46 0 0 1 -155.90625,270 l 311.8125,0 A 352.46,352.46 0 0 1 31.84375,31.71875 352.46,352.46 0 0 1 270,155.90625 l 0,-311.8125 A 352.46,352.46 0 0 1 31.71875,-31.84375 352.46,352.46 0 0 1 155.90625,-270 l -311.8125,0 z";
		private static readonly Vector PathSize = new Vector(540, 540);

		public OverlayIronCross(int maximumX, int maximumY)
			: base("iron cross", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayIronCross(Color color, int maximumX, int maximumY)
			: base(color, "iron cross", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}
