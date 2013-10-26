using System.Collections.Generic;
using System.Windows.Media;

namespace FlagMaker.Overlays
{
	public abstract class OverlayShape : Overlay
	{
		public OverlayShape(Color color, List<Attribute> attributes, int maximumX, int maximumY)
			: base(color, attributes, maximumX, maximumY)
		{
		}

		public OverlayShape(List<Attribute> attributes, int maximumX, int maximumY)
			: base(attributes, maximumX, maximumY)
		{
		}
	}
}