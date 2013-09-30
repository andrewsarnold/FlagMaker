using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayEllipse : Overlay
	{
		public OverlayEllipse(int maximum)
			: base(new List<Attribute>
			       {
				       new Attribute("X", true, 1),
				       new Attribute("Y", true, 1),
				       new Attribute("Width", true, 1),
				       new Attribute("Height", true, 0)
			       }, maximum)
		{
		}

		public OverlayEllipse(Color color, int x, int y, int size, int rotation, int maximum)
			: base(color, new List<Attribute>
			             {
				             new Attribute("X", true, x),
				             new Attribute("Y", true, y),
				             new Attribute("Width", true, size),
				             new Attribute("Height", true, rotation)
			             }, maximum)
		{
		}

		public override string Name { get { return "ellipse"; } }

		public override void Draw(Canvas canvas)
		{
			var width = canvas.Width * (Attributes.Get("Width").Value / Maximum);
			var height = Attributes.Get("Height").Value == 0
							 ? width
							 : canvas.Height * (Attributes.Get("Height").Value / Maximum);

			var path = new Ellipse
			{
				Fill = new SolidColorBrush(Color),
				Width = width,
				Height = height,
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(path);

			if (Maximum % 2 == 0)
			{
				Canvas.SetLeft(path, (canvas.Width * (Attributes.Get("X").Value / Maximum)) - width / 2);
				Canvas.SetTop(path, (canvas.Height * (Attributes.Get("Y").Value / Maximum)) - height / 2);
			}
			else
			{
				Canvas.SetLeft(path, (canvas.Width * (Attributes.Get("X").Value / (Maximum + 1))) - width / 2);
				Canvas.SetTop(path, (canvas.Height * (Attributes.Get("Y").Value / (Maximum + 1))) - height / 2);
			}
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
			var w = width * (Attributes.Get("Width").Value / Maximum);
			var h = Attributes.Get("Height").Value == 0
							 ? w
							 : height * (Attributes.Get("Height").Value / Maximum);

			double x, y;
			if (Maximum % 2 == 0)
			{
				x = (width * (Attributes.Get("X").Value / Maximum));
				y = (height * (Attributes.Get("Y").Value / Maximum));
			}
			else
			{
				x = (width * (Attributes.Get("X").Value / (Maximum + 1)));
				y = (height * (Attributes.Get("Y").Value / (Maximum + 1)));
			}

			return string.Format("<ellipse cx=\"{0}\" cy=\"{1}\" rx=\"{2}\" ry=\"{3}\" fill=\"#{4}\" />",
				x, y, w / 2, h / 2,
				Color.ToHexString());
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
					       new Ellipse
					       {
						       Width = 20,
						       Height = 20
					       }
				       };
			}
		}
	}
}