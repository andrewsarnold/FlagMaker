using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayStarEight : OverlayPath
	{
		private const string Path = "M 35.38014,35.33055 9.5832897,23.09028 0.03507971,50 -9.55088,23.1037 -35.33053,35.38014 -23.09027,9.58329 -50,0.03508036 -23.10369,-9.5508796 -35.38014,-35.33053 -9.58328,-23.09027 -0.03507029,-50 9.5508897,-23.1037 35.33054,-35.38014 23.09028,-9.5832796 50.00001,-0.03507964 23.10371,9.55088 z";
		private static readonly Vector PathSize = new Vector(100, 100);

		public OverlayStarEight(int maximumX, int maximumY)
			: base("star eight", Path, PathSize, maximumX, maximumY)
		{
		}

		public OverlayStarEight(Color color, int maximumX, int maximumY)
			: base(color, "star eight", Path, PathSize, maximumX, maximumY)
		{
		}
	}
}