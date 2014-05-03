using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	internal sealed class OverlayYin : OverlayPath
	{
		private const string Path = "M -21.643592,-14.393365 A 12.993263,12.993263 0 1 0 -0.02149594,0.02136686 12.993263,12.993263 0 0 1 21.600599,14.436096 25.986525,25.986525 0 0 1 -21.643592,-14.393365 z";
		private static readonly Vector PathSize = new Vector(56, 45);

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