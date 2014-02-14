using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

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

		protected override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new Shape[]
				       {
					       new Rectangle
					       {
						       Width = 20,
						       Height = 15,
							   Margin = new Thickness(5, 7.5, 0, 0)
					       }
				       };
			}
		}

		public override void Draw(Canvas canvas)
		{
			var width = canvas.Width * (Attributes.Get(strings.Width).Value / MaximumX);
			var height = Attributes.Get(strings.Height).Value == 0
							 ? width
							 : canvas.Height * (Attributes.Get(strings.Height).Value / MaximumY);

			var rect = new Rectangle
						   {
							   Fill = new SolidColorBrush(Color),
							   Width = width,
							   Height = height,
							   SnapsToDevicePixels = true
						   };
			canvas.Children.Add(rect);
			Canvas.SetTop(rect, canvas.Height * (Attributes.Get(strings.Y).Value / MaximumY));
			Canvas.SetLeft(rect, canvas.Width * (Attributes.Get(strings.X).Value / MaximumX));
		}

		public override string ExportSvg(int width, int height)
		{
			var w = width * (Attributes.Get(strings.Width).Value / MaximumX);
			var h = Attributes.Get(strings.Height).Value == 0
				? w
				: height * (Attributes.Get(strings.Height).Value / MaximumY);

			return string.Format(CultureInfo.InvariantCulture, "<rect width=\"{0:0.###}\" height=\"{1:0.###}\" x=\"{2:0.###}\" y=\"{3:0.###}\" fill=\"#{4}\" />",
				w, h,
				width * (Attributes.Get(strings.X).Value / MaximumX),
				height * (Attributes.Get(strings.Y).Value / MaximumY),
				Color.ToHexString());
		}
	}
}