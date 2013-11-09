using System.Collections.Generic;
using System.Windows.Media;

namespace FlagMaker.Overlays.OverlayTypes.RepeaterTypes
{
	public abstract class OverlayRepeater : Overlay
	{
		protected Overlay Overlay;

		protected OverlayRepeater(List<Attribute> attributes, int maximumX, int maximumY)
			: base(Colors.Transparent, attributes, maximumX, maximumY)
		{
		}

		public void SetOverlay(Overlay overlay)
		{
			Overlay = overlay;
		}
	}
}