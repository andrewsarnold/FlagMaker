using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes
{
	public class OverlayTriangle : Overlay
	{
		public OverlayTriangle(int maximumX, int maximumY)
			: base(new List<Attribute>
			{
				new Attribute(strings.X1, true, 1, true),
				new Attribute(strings.Y1, true, 1, false),
				new Attribute(strings.X2, true, 1, true),
				new Attribute(strings.Y2, true, 2, false),
				new Attribute(strings.X3, true, 2, true),
				new Attribute(strings.Y3, true, 1, false)
			}, maximumX, maximumY)
		{
		}

		public OverlayTriangle(Color color, double x1, double y1, double x2,
			double y2, double x3, double y3, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			{
				new Attribute(strings.X1, true, x1, true),
				new Attribute(strings.Y1, true, y1, false),
				new Attribute(strings.X2, true, x2, true),
				new Attribute(strings.Y2, true, y2, false),
				new Attribute(strings.X3, true, x3, true),
				new Attribute(strings.Y3, true, y3, false),
			}, maximumX, maximumY)
		{
		}

		public override string Name { get { return "triangle"; } }

		public override void Draw(Canvas canvas)
		{
			var x1 = canvas.Width * (Attributes.Get(strings.X1).Value / MaximumX);
			var y1 = canvas.Height * (Attributes.Get(strings.Y1).Value / MaximumY);
			var x2 = canvas.Width * (Attributes.Get(strings.X2).Value / MaximumX);
			var y2 = canvas.Height * (Attributes.Get(strings.Y2).Value / MaximumY);
			var x3 = canvas.Width * (Attributes.Get(strings.X3).Value / MaximumX);
			var y3 = canvas.Height * (Attributes.Get(strings.Y3).Value / MaximumY);

			var path = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = canvas.Width,
				Height = canvas.Height,
				Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture,
					"M {0},{1} {2},{3} {4},{5}",
					x1, y1, x2, y2, x3, y3)),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(path);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.X1).Value = values[0];
			Attributes.Get(strings.Y1).Value = values[1];
			Attributes.Get(strings.X2).Value = values[2];
			Attributes.Get(strings.Y2).Value = values[3];
			Attributes.Get(strings.X3).Value = values[4];
			Attributes.Get(strings.Y3).Value = values[5];
		}

		public override string ExportSvg(int width, int height)
		{
			var x1 = width * (Attributes.Get(strings.X1).Value / MaximumX);
			var y1 = height * (Attributes.Get(strings.Y1).Value / MaximumY);
			var x2 = width * (Attributes.Get(strings.X2).Value / MaximumX);
			var y2 = height * (Attributes.Get(strings.Y2).Value / MaximumY);
			var x3 = width * (Attributes.Get(strings.X3).Value / MaximumX);
			var y3 = height * (Attributes.Get(strings.Y3).Value / MaximumY);

			return string.Format(CultureInfo.InvariantCulture, "<polygon points=\"{0:0.###},{1:0.###} {2:0.###},{3:0.###} {4:0.###},{5:0.###}\" {6} />",
				x1, y1, x2, y2, x3, y3,
				Color.ToSvgFillWithOpacity());
		}

		protected override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
						   new Path
						   {
							   Data = Geometry.Parse("M 0,0 15,15 0,30 0,0")
						   }
				       };
			}
		}
	}
}