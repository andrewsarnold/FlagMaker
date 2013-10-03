using System.Windows;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes
{
	public class OverlayStarSeven : PathOverlay
	{
		private const string Path = "m 0.02435,-8.5465374 1.73553,5.39612 5.30095,-2.00753 -3.13677,4.72132021 4.87464,2.89276979 -5.64703,0.49127 0.77763,5.61477 -3.90495,-4.10872 -3.90496,4.10872 0.77763,-5.61477 -5.64702,-0.49127 4.87464,-2.89276979 -3.13678,-4.72132021 5.30095,2.00753 1.73554,-5.39612 z";
		private static readonly Vector PathSize = new Vector(22, 22);

		public OverlayStarSeven(int maximum)
			: base("star seven", Path, PathSize, maximum)
		{
		}

		public OverlayStarSeven(Color color, int maximum)
			: base(color, "star seven", Path, PathSize, maximum)
		{
		}
	}
}
