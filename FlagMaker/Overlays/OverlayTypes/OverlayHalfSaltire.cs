using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal sealed class OverlayHalfSaltire : Overlay
	{
		public OverlayHalfSaltire(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.Thickness, true, 1, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlayHalfSaltire(Color color, double ratio, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			             {
				             new Attribute(strings.Thickness, true, ratio, true)
			             }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "half saltire"; } }

		public override void Draw(Canvas canvas)
		{
			var widthX = canvas.Width * (Attributes.Get(strings.Thickness).Value / MaximumX) / 4;
			var widthY = canvas.Height * (Attributes.Get(strings.Thickness).Value / MaximumX) / 4;

			var centerX = canvas.Width/2;
			var centerY = canvas.Height/2;

			var pathTopLeft = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = canvas.Width,
				Height = canvas.Height,
				Data =
					Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M 0,0 {0},{1} {2},{1} 0,{3}",
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
					Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M {0},{1} {0},{2} {3},0 {4},0",
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
					Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M {0},{1} {0},{2} {3},{4} 0,{4}",
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
					Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M {0},{1} {2},{1} {3},{4} {3},{5}",
					centerX, centerY, centerX + widthX, canvas.Width, canvas.Height - widthY, canvas.Height)),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(pathBottomRight);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.Thickness).Value = values[0];
		}

		public override string ExportSvg(int width, int height)
		{
			var wX = width * (Attributes.Get(strings.Thickness).Value / MaximumX) / 4;
			var wY = height * (Attributes.Get(strings.Thickness).Value / MaximumX) / 4;

			var centerX = width/2;
			var centerY = height/2;

			var c = Color.ToSvgFillWithOpacity();

			var sb = new StringBuilder();

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<polygon points=\"0,0 {0:0.###},{1:0.###} {2:0.###},{1:0.###} 0,{3:0.###}\" {4} />",
				centerX, centerY, centerX - wX, wY, c));

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<polygon points=\"{0:0.###},{1:0.###} {0:0.###},{2:0.###} {3:0.###},0 {4:0.###},0\" {5} />",
				centerX, centerY, centerY - wY, width - wX, width, c));

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<polygon points=\"{0:0.###},{1:0.###} {0:0.###},{2:0.###} {3:0.###},{4:0.###} 0,{4:0.###}\" {5} />",
				centerX, centerY, centerY + wY, wX, height, c));

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<polygon points=\"{0:0.###},{1:0.###} {2:0.###},{1:0.###} {3:0.###},{4:0.###} {3:0.###},{5:0.###}\" {6} />",
				centerX, centerY, centerX + wX, width, height - wY, height, c));

			return sb.ToString();
		}

		protected override IEnumerable<Shape> Thumbnail
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
						       Y2 = 30
					       },
					       new Line
					       {
						       StrokeThickness = 3,
						       X1 = 30,
						       X2 = 0,
						       Y1 = 0,
						       Y2 = 30
					       }
				       };
			}
		}
	}
}