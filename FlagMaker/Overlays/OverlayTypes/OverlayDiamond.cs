using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayDiamond : OverlayShapeSimple
	{
		public OverlayDiamond(int maximumX, int maximumY)
			: base(maximumX, maximumY)
		{
		}

		public OverlayDiamond(Color color, int x, int y, int width, int height, int maximumX, int maximumY)
			: base(color, x, y, width, height, maximumX, maximumY)
		{
		}

		public override string Name { get { return "diamond"; } }

		public override void Draw(Canvas canvas)
		{
			var width = canvas.Width * (Attributes.Get("Width").Value / MaximumX);
			var height = Attributes.Get("Height").Value == 0
							 ? width
							 : canvas.Height * (Attributes.Get("Height").Value / MaximumY);

			var path = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = width,
				Height = height,
				Data =
					Geometry.Parse(string.Format("M 0,{0} {1},0 {2},{0} {1},{3} 0,{0}", height / 2, width / 2, width, height)),
				SnapsToDevicePixels = true
			};

			canvas.Children.Add(path);

			Canvas.SetLeft(path, (canvas.Width * (Attributes.Get("X").Value / MaximumX)) - width / 2);
			Canvas.SetTop(path, (canvas.Height * (Attributes.Get("Y").Value / MaximumY)) - height / 2);
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new Shape[]
				       {
					       new Path
					       {
						       Data = Geometry.Parse("M 0,10 15,0 30,10 15,20 0,10")
					       }
				       };
			}
		}

		public override string ExportSvg(int width, int height)
		{
			var w = width * (Attributes.Get("Width").Value / MaximumX);
			var h = Attributes.Get("Height").Value == 0
							 ? w
							 : height * (Attributes.Get("Height").Value / MaximumY);

			double x = width * (Attributes.Get("X").Value / MaximumX);
			double y = height * (Attributes.Get("Y").Value / MaximumY);

			return string.Format("<polygon points=\"{0},{1} {2},{3} {0},{4} {5},{3} \" fill=\"#{6}\" />",
				x, y - h / 2, x + w / 2, y, y + h / 2, x - w / 2,
				Color.ToHexString());
		}
	}
}