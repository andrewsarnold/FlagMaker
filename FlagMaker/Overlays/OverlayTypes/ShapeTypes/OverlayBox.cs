using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes.ShapeTypes
{
	internal class OverlayBox : OverlayShape
	{
		public OverlayBox(int maximumX, int maximumY)
			: base(maximumX, maximumY)
		{
		}

		public OverlayBox(Color color, double x, double y, double width, double height, int maximumX, int maximumY)
			: base(color, x, y, width, height, maximumX, maximumY)
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

		public override string ExportSvg(int width, int height)
		{
			var w = width * (Attributes.Get("Width").Value / MaximumX);
			var h = Attributes.Get("Height").Value == 0
				? w
				: height * (Attributes.Get("Height").Value / MaximumY);

			return string.Format(CultureInfo.InvariantCulture, "<rect width=\"{0}\" height=\"{1}\" x=\"{2}\" y=\"{3}\" fill=\"#{4}\" />",
				w, h,
				width * (Attributes.Get("X").Value / MaximumX),
				height * (Attributes.Get("Y").Value / MaximumY),
				Color.ToHexString());
		}
	}
}