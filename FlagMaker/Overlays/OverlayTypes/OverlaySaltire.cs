using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal sealed class OverlaySaltire : Overlay
	{
		public OverlaySaltire(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.Thickness, true, 1, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlaySaltire(Color color, double ratio, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			             {
				             new Attribute(strings.Thickness, true, ratio, true)
			             }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "saltire"; } }

		public override void Draw(Canvas canvas)
		{
			var widthX = canvas.Width * (Attributes.Get(strings.Thickness).Value / MaximumX) / 2;
			var widthY = canvas.Height * (Attributes.Get(strings.Thickness).Value / MaximumX) / 2;

			var path1 = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = canvas.Width,
				Height = canvas.Height,
				Data =
					Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M {0},0 0,0 0,{1} {2},{3} {4},{3} {4},{5} {0},0", widthX, widthY,
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
					Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M {0},0 {1},0 {1},{5} {2},{3} 0,{3} 0,{4} {0},0", canvas.Width - widthX,
												 canvas.Width, widthX, canvas.Height, canvas.Height - widthY, widthY)),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(path2);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.Thickness).Value = values[0];
		}

		public override string ExportSvg(int width, int height)
		{
			var wX = width * (Attributes.Get(strings.Thickness).Value / MaximumX) / 2;
			var wY = height * (Attributes.Get(strings.Thickness).Value / MaximumX) / 2;

			return string.Format(CultureInfo.InvariantCulture, "<polygon points=\"{0:0.###},0 0,0 0,{5:0.###} {1:0.###},{2:0.###} {3:0.###},{2:0.###} {3:0.###},{4:0.###} {0:0.###},0\" {6} /><polygon points=\"{1:0.###},0 {3:0.###},0 {3:0.###},{5:0.###} {0:0.###},{2:0.###} 0,{2:0.###} 0,{4:0.###} {1:0.###},0\" {6} />",
				wX, width - wX, height, width, height - wY, wY, Color.ToSvgFillWithOpacity());
		}

		protected override IEnumerable<Shape> Thumbnail
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
						       Y2 = 30
					       },
					       new Line
					       {
						       StrokeThickness = 5,
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