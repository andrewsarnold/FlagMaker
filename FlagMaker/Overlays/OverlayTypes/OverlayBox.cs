using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayBox : Overlay
	{
		public OverlayBox(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
					   new Attribute("X", true, 1, true),
					   new Attribute("Y", true, 1, false),
				       new Attribute("Width", true, 1, true),
				       new Attribute("Height", true, 1, false)
			       }, maximumX, maximumY)
		{
		}

		public OverlayBox(Color color, int x, int y, int width, int height, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			              {
							  new Attribute("X", true, x, true),
							  new Attribute("Y", true, y, false),
				              new Attribute("Width", true, width, true),
				              new Attribute("Height", true, height, false)
			              }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "box"; } }

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new Shape[]
				       {
					       new Rectangle
					       {
						       Width = 15,
						       Height = 10
					       }
				       };
			}
		}

		public override void Draw(Canvas canvas)
		{
			var width = canvas.Width * (Attributes.Get("Width").Value / MaximumX);
			var height = Attributes.Get("Height").Value == 0
							 ? width
							 : canvas.Height * (Attributes.Get("Height").Value / MaximumY);

			var rect = new Rectangle
						   {
							   Fill = new SolidColorBrush(Color),
							   Width = width,
							   Height = height,
							   SnapsToDevicePixels = true
						   };
			canvas.Children.Add(rect);
			Canvas.SetTop(rect, canvas.Height * (Attributes.Get("Y").Value / MaximumY));
			Canvas.SetLeft(rect, canvas.Width * (Attributes.Get("X").Value / MaximumX));
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("X").Value = values[0];
			Attributes.Get("Y").Value = values[1];
			Attributes.Get("Width").Value = values[2];
			Attributes.Get("Height").Value = values[3];
		}

		public override string ExportSvg(int width, int height)
		{
			var w = width * (Attributes.Get("Width").Value / MaximumX);
			var h = Attributes.Get("Height").Value == 0
				? w
				: height * (Attributes.Get("Height").Value / MaximumY);

			return string.Format("<rect width=\"{0}\" height=\"{1}\" x=\"{2}\" y=\"{3}\" fill=\"#{4}\" />",
				w, h,
				width * (Attributes.Get("X").Value / MaximumX),
				height * (Attributes.Get("Y").Value / MaximumY),
				Color.ToHexString());
		}
	}
}