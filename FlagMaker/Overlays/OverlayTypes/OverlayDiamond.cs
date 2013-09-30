using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayDiamond : Overlay
	{
		public OverlayDiamond(int maximum)
			: base(new List<Attribute>
			       {
				       new Attribute("X", true, 1),
				       new Attribute("Y", true, 1),
				       new Attribute("Width", true, 1),
				       new Attribute("Height", true, 0)
			       }, maximum)
		{
		}

		public OverlayDiamond(Color color, int x, int y, int width, int height, int maximum)
			: base(color, new List<Attribute>
			              {
				              new Attribute("X", true, x),
				              new Attribute("Y", true, y),
				              new Attribute("Width", true, width),
				              new Attribute("Height", true, height)
			              }, maximum)
		{
		}

		public override string Name { get { return "diamond"; } }

		public override void Draw(Canvas canvas)
		{
			var width = canvas.Width * (Attributes.Get("Width").Value / Maximum);
			var height = Attributes.Get("Height").Value == 0
							 ? width
							 : canvas.Height * (Attributes.Get("Height").Value / Maximum);

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
				x = width * (Attributes.Get("X").Value / Maximum);
				y = height * (Attributes.Get("Y").Value / Maximum);
			}
			else
			{
				x = width * (Attributes.Get("X").Value / (Maximum + 1));
				y = height * (Attributes.Get("Y").Value / (Maximum + 1));
			}

			return string.Format("<polygon points=\"{0},{1} {2},{3} {0},{4} {5},{3} \" fill=\"#{6}\" />",
				x, y - h / 2, x + w / 2, y, y + h / 2, x - w / 2,
				Color.ToHexString());
		}
	}
}