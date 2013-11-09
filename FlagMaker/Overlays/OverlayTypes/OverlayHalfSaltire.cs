using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayHalfSaltire : Overlay
	{
		public OverlayHalfSaltire(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute("Ratio", true, 1, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlayHalfSaltire(Color color, double ratio, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			             {
				             new Attribute("Ratio", true, ratio, true)
			             }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "half saltire"; } }

		public override void Draw(Canvas canvas)
		{
			var widthX = (int)(canvas.Width / (Attributes.Get("Ratio").Value + 3));
			var widthY = (int)(canvas.Height / (Attributes.Get("Ratio").Value + 3));

			var centerX = canvas.Width/2;
			var centerY = canvas.Height/2;

			var pathTopLeft = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = canvas.Width,
				Height = canvas.Height,
				Data =
					Geometry.Parse(string.Format("M 0,0 {0},{1} {2},{1} 0,{3}",
						centerX, centerY, centerX - widthX, widthY)),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(pathTopLeft);

			var pathTopRight = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = canvas.Width,
				Height = canvas.Height,
				Data =
					Geometry.Parse(string.Format("M {0},{1} {0},{2} {3},0 {4},0",
					centerX, centerY, centerY - widthY, canvas.Width - widthX, canvas.Width)),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(pathTopRight);

			var pathBottomLeft = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = canvas.Width,
				Height = canvas.Height,
				Data =
					Geometry.Parse(string.Format("M {0},{1} {0},{2} {3},{4} 0,{4}",
					centerX, centerY, centerY + widthY, widthX, canvas.Height)),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(pathBottomLeft);

			var pathBottomRight = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = canvas.Width,
				Height = canvas.Height,
				Data =
					Geometry.Parse(string.Format("M {0},{1} {2},{1} {3},{4} {3},{5}",
					centerX, centerY, centerX + widthX, canvas.Width, canvas.Height - widthY, canvas.Height)),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(pathBottomRight);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("Ratio").Value = values[0];
		}

		public override string ExportSvg(int width, int height)
		{
			var wX = (int)(width / (Attributes.Get("Ratio").Value + 3));
			var wY = (int)(height / (Attributes.Get("Ratio").Value + 3));

			var centerX = width/2;
			var centerY = height/2;

			var c = Color.ToHexString();

			var sb = new StringBuilder();

			sb.Append(string.Format("<polygon points=\"0,0 {0},{1} {2},{1} 0,{3}\" fill=\"#{4}\" />",
				centerX, centerY, centerX - wX, wY, c));

			sb.Append(string.Format("<polygon points=\"{0},{1} {0},{2} {3},0 {4},0\" fill=\"#{5}\" />",
				centerX, centerY, centerY - wY, width - wX, width, c));

			sb.Append(string.Format("<polygon points=\"{0},{1} {0},{2} {3},{4} 0,{4}\" fill=\"#{5}\" />",
				centerX, centerY, centerY + wY, wX, height, c));

			sb.Append(string.Format("<polygon points=\"{0},{1} {2},{1} {3},{4} {3},{5}\" fill=\"#{6}\" />",
				centerX, centerY, centerX + wX, width, height - wY, height, c));

			return sb.ToString();
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
					       new Line
					       {
						       StrokeThickness = 3,
						       X1 = 0,
						       X2 = 30,
						       Y1 = 0,
						       Y2 = 20
					       },
					       new Line
					       {
						       StrokeThickness = 3,
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