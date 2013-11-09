using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlaySaltire : Overlay
	{
		public OverlaySaltire(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute("Ratio", true, 1, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlaySaltire(Color color, double ratio, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			             {
				             new Attribute("Ratio", true, ratio, true)
			             }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "saltire"; } }

		public override void Draw(Canvas canvas)
		{
			var widthX = (int)(canvas.Width / (Attributes.Get("Ratio").Value + 3));
			var widthY = (int)(canvas.Height / (Attributes.Get("Ratio").Value + 3));

			var path1 = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = canvas.Width,
				Height = canvas.Height,
				Data =
					Geometry.Parse(string.Format("M {0},0 0,0 0,{1} {2},{3} {4},{3} {4},{5} {0},0", widthX, widthY,
												 canvas.Width - widthX, canvas.Height, canvas.Width, canvas.Height - widthY)),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(path1);

			var path2 = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = canvas.Width,
				Height = canvas.Height,
				Data =
					Geometry.Parse(string.Format("M {0},0 {1},0 {1},{5} {2},{3} 0,{3} 0,{4} {0},0", canvas.Width - widthX,
												 canvas.Width, widthX, canvas.Height, canvas.Height - widthY, widthY)),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(path2);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("Ratio").Value = values[0];
		}

		public override string ExportSvg(int width, int height)
		{
			var wX = (int)(width / (Attributes.Get("Ratio").Value + 3));
			var wY = (int)(height / (Attributes.Get("Ratio").Value + 3));

			return string.Format("<polygon points=\"{0},0 0,0 0,{5} {1},{2} {3},{2} {3},{4} {0},0\" fill=\"#{6}\" /><polygon points=\"{1},0 {3},0 {3},{0} {0},{2} 0,{2} 0,{4} {1},0\" fill=\"#{6}\" />",
				wX, width - wX, height, width, height - wY, wY, Color.ToHexString());
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
					       new Line
					       {
						       StrokeThickness = 5,
						       X1 = 0,
						       X2 = 30,
						       Y1 = 0,
						       Y2 = 20
					       },
					       new Line
					       {
						       StrokeThickness = 5,
						       X1 = 30,
						       X2 = 0,
						       Y1 = 0,
						       Y2 = 20
					       }
				       };
			}
		}
	}
}