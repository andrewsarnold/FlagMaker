using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayFimbriationForward : Overlay
	{
		public OverlayFimbriationForward(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute("Ratio", true, 1, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlayFimbriationForward(Color color, double ratio, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			              {
				              new Attribute("Ratio", true, ratio, true)
			              }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "fimbriation forward"; } }

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
					           Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M {0},0 {1},0 {1},{5} {2},{3} 0,{3} 0,{4} {0},0", canvas.Width - widthX,
						           canvas.Width, widthX, canvas.Height, canvas.Height - widthY, widthY)),
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
			var wY = (int)(height / (Attributes.Get("Ratio").Value + 2));

			return string.Format(CultureInfo.InvariantCulture, "<polygon points=\"{0},0 {1},0 {1},{5} {2},{3} 0,{3} 0,{4} {0},0\" fill=\"#{6}\" />",
				width - wX, width, wX, height, height - wY, wY,
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