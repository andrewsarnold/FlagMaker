using System.Collections.Generic;
using System.Windows.Media;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes.ShapeTypes
{
	public abstract class OverlayShape : Overlay
	{
		protected OverlayShape(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.X, true, 1, true),
				       new Attribute(strings.Y, true, 1, false),
				       new Attribute(strings.Width, true, 1, true),
				       new Attribute(strings.Height, true, 1, false)
			       }, maximumX, maximumY)
		{
		}

		protected OverlayShape(Color color, double x, double y, double width, double height, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			              {
				              new Attribute(strings.X, true, x, true),
				              new Attribute(strings.Y, true, y, false),
				              new Attribute(strings.Width, true, width, true),
				              new Attribute(strings.Height, true, height, false)
			              }, maximumX, maximumY)
		{
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.X).Value = values[0];
			Attributes.Get(strings.Y).Value = values[1];
			Attributes.Get(strings.Width).Value = values[2];
			Attributes.Get(strings.Height).Value = values[3];
		}
	}
}