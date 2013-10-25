using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayFimbriationBackward : Overlay
	{
		public OverlayFimbriationBackward(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute("Ratio", true, 1, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlayFimbriationBackward(Color color, double ratio, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			              {
				              new Attribute("Ratio", true, ratio, true)
			              }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "fimbriation backward"; } }

		public override void Draw(Canvas canvas)
		{
			var widthX = (int)(canvas.Width / (Attributes.Get("Ratio").Value + 2));
			var widthY = (int)(canvas.Height / (Attributes.Get("Ratio").Value + 2));

			var path = new Path
			           {
				           Fill = new SolidColorBrush(Color),
				           Width = canvas.Width,
				           Height = canvas.Height,
				           Data =
					           Geometry.Parse(string.Format("M {0},0 0,0 0,{1} {2},{3} {4},{3} {4},{5} {0},0", widthX, widthY,
						           canvas.Width - widthX, canvas.Height, canvas.Width, canvas.Height - widthY)),
				           SnapsToDevicePixels = true
			           };
			canvas.Children.Add(path);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("Ratio").Value = values[0];
		}

		public override string ExportSvg(int width, int height)
		{
			var wX = (int)(width / (Attributes.Get("Ratio").Value + 2));
			var wY = (int)(width / (Attributes.Get("Ratio").Value + 2));

			return string.Format("<polygon points=\"{0},0 0,0 0,{5} {1},{2} {3},{2} {3},{4} {0},0\" fill=\"#{6}\" />",
				wX, width - wX, height, width, height - wY, wY,
				Color.ToHexString());
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
						   }
				       };
			}
		}
	}
}