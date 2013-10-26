using System.Collections.Generic;
using System.Windows.Media;

namespace FlagMaker.Overlays
{
	public abstract class OverlayShapeSimple : OverlayShape
	{
		public OverlayShapeSimple(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute("X", true, 1, true),
				       new Attribute("Y", true, 1, false),
				       new Attribute("Width", true, 1, true),
				       new Attribute("Height", true, 0, false)
			       }, maximumX, maximumY)
		{
		}

		public OverlayShapeSimple(Color color, int x, int y, int width, int height, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			              {
				              new Attribute("X", true, x, true),
				              new Attribute("Y", true, y, false),
				              new Attribute("Width", true, width, true),
				              new Attribute("Height", true, height, false)
			              }, maximumX, maximumY)
		{
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("X").Value = values[0];
			Attributes.Get("Y").Value = values[1];
			Attributes.Get("Width").Value = values[2];
			Attributes.Get("Height").Value = values[3];
		}
	}
}