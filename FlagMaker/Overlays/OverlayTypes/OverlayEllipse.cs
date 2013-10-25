using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayEllipse : Overlay
	{
		public OverlayEllipse(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute("X", true, 1, true),
				       new Attribute("Y", true, 1, false),
				       new Attribute("Width", true, 1, true),
				       new Attribute("Height", true, 0, false)
			       }, maximumX, maximumY)
		{
		}

		public OverlayEllipse(Color color, int x, int y, int width, int height, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			             {
				             new Attribute("X", true, x, true),
				             new Attribute("Y", true, y, false),
				             new Attribute("Width", true, width, true),
				             new Attribute("Height", true, height, false)
			             }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "ellipse"; } }

		public override void Draw(Canvas canvas)
		{
			var width = canvas.Width * (Attributes.Get("Width").Value / MaximumX);
			var height = Attributes.Get("Height").Value == 0
							 ? width
							 : canvas.Height * (Attributes.Get("Height").Value / MaximumY);

			var path = new Ellipse
			{
				Fill = new SolidColorBrush(Color),
				Width = width,
				Height = height,
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(path);

			Canvas.SetLeft(path, (canvas.Width * (Attributes.Get("X").Value / (MaximumX + (MaximumX % 2 == 0 ? 0 : 1)))) - width / 2);
			Canvas.SetTop(path, (canvas.Height * (Attributes.Get("Y").Value / (MaximumY + (MaximumY % 2 == 0 ? 0 : 1)))) - height / 2);
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

			double x = MaximumX % 2 == 0
				? width * (Attributes.Get("X").Value / MaximumX)
				: width * (Attributes.Get("X").Value / (MaximumX + 1));
			double y = MaximumY % 2 == 0
				? height * (Attributes.Get("Y").Value / MaximumY)
				: height * (Attributes.Get("Y").Value / (MaximumY + 1));

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