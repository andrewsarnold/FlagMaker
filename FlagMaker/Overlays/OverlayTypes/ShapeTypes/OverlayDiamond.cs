using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes.ShapeTypes
{
	internal class OverlayDiamond : OverlayShape
	{
		public OverlayDiamond(int maximumX, int maximumY)
			: base(maximumX, maximumY)
		{
		}

		public OverlayDiamond(Color color, double x, double y, double width, double height, int maximumX, int maximumY)
			: base(color, x, y, width, height, maximumX, maximumY)
		{
		}

		public override string Name { get { return "diamond"; } }

		public override void Draw(Canvas canvas)
		{
			var width = canvas.Width * (Attributes.Get(strings.Width).Value / MaximumX);
			var height = Attributes.Get(strings.Height).Value == 0
							 ? width
							 : canvas.Height * (Attributes.Get(strings.Height).Value / MaximumY);

			var path = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = width,
				Height = height,
				Data =
					Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M 0,{0} {1},0 {2},{0} {1},{3} 0,{0}", height / 2, width / 2, width, height)),
				SnapsToDevicePixels = true
			};

			canvas.Children.Add(path);

			Canvas.SetLeft(path, (canvas.Width * (Attributes.Get(strings.X).Value / MaximumX)) - width / 2);
			Canvas.SetTop(path, (canvas.Height * (Attributes.Get(strings.Y).Value / MaximumY)) - height / 2);
		}

		protected override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new Shape[]
				       {
					       new Path
					       {
						       Data = Geometry.Parse("M 0,15 15,0 30,15 15,30 Z")
					       }
				       };
			}
		}

		public override string ExportSvg(int width, int height)
		{
			var w = width * (Attributes.Get(strings.Width).Value / MaximumX);
			var h = Attributes.Get(strings.Height).Value == 0
							 ? w
							 : height * (Attributes.Get(strings.Height).Value / MaximumY);

			double x = width * (Attributes.Get(strings.X).Value / MaximumX);
			double y = height * (Attributes.Get(strings.Y).Value / MaximumY);

			return string.Format(CultureInfo.InvariantCulture, "<polygon points=\"{0:0.###},{1:0.###} {2:0.###},{3:0.###} {0:0.###},{4:0.###} {5:0.###},{3:0.###} \" fill=\"#{6}\" />",
				x, y - h / 2, x + w / 2, y, y + h / 2, x - w / 2,
				Color.ToHexString());
		}
	}
}