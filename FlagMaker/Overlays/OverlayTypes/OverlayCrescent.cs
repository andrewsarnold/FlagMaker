using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	public class OverlayCrescent : Overlay
	{
		const double Ratio = 1.3;

		public OverlayCrescent(int maximum)
			: base(new List<Attribute>
			       {
				       new Attribute("X", true, 1),
				       new Attribute("Y", true, 1),
				       new Attribute("Size", true, 1),
				       new Attribute("Rotation", true, 0)
			       }, maximum)
		{
		}

		public OverlayCrescent(Color color, int x, int y, int size, int rotation, int maximum)
			:base(color, new List<Attribute>
			             {
				             new Attribute("X", true, x),
				             new Attribute("Y", true, y),
				             new Attribute("Size", true, size),
				             new Attribute("Rotation", true, rotation)
			             }, maximum)
		{
		}

		public override string Name
		{
			get { return "crescent"; }
		}

		public override void Draw(Canvas canvas)
		{
			var size = canvas.Width * (Attributes.Get("Size").Value / Maximum) / 4;
			double rotation = Attributes.Get("Rotation").Value / Maximum;

			var path = new Path
			{
				Fill = new SolidColorBrush(Color),
				Data = new CombinedGeometry(GeometryCombineMode.Exclude,
					new EllipseGeometry(new Point(0, 0), size, size),
					new EllipseGeometry(new Point(size - size / Math.Pow(Ratio, 1.5), 0), size / Ratio, size / Ratio),
					new RotateTransform(rotation * 360)),
				SnapsToDevicePixels = true
			};

			canvas.Children.Add(path);

			if (Maximum % 2 == 0)
			{
				Canvas.SetLeft(path, (canvas.Width * (Attributes.Get("X").Value / Maximum)));
				Canvas.SetTop(path, (canvas.Height * (Attributes.Get("Y").Value / Maximum)));
			}
			else
			{
				Canvas.SetLeft(path, (canvas.Width * (Attributes.Get("X").Value / (Maximum + 1))));
				Canvas.SetTop(path, (canvas.Height * (Attributes.Get("Y").Value / (Maximum + 1))));
			}
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("X").Value = values[0];
			Attributes.Get("Y").Value = values[1];
			Attributes.Get("Size").Value = values[2];
			Attributes.Get("Rotation").Value = values[3];
		}

		public override string ExportSvg(int width, int height)
		{
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

			var size = width * (Attributes.Get("Size").Value / Maximum) / 4;
			var sb = new StringBuilder();

			sb.AppendLine("<defs><mask id=\"c\">");
			sb.Append(string.Format("<circle cx=\"0\" cy=\"0\" r=\"{0}\" fill=\"#ffffff\" mask=\"url(#c)\" />", size));
			sb.Append(string.Format("<circle cx=\"{0}\" cy=\"0\" r=\"{1}\" />",
				size - size / Math.Pow(Ratio, 1.5), size / Ratio));
			sb.AppendLine("</mask></defs>");

			sb.Append(string.Format("<g transform=\"translate({0},{1}) rotate({2})\">",
				x, y, 360 * (Attributes.Get("Rotation").Value / Maximum)));

			sb.Append(string.Format("<circle cx=\"0\" cy=\"0\" r=\"{0}\" fill=\"#{1}\" mask=\"url(#c)\" />",
				size, Color.ToHexString()));
			sb.Append("</g>");
			return sb.ToString();
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new Shape[]
				       {
					       new Path
					       {
						       Data = new CombinedGeometry
						              {
							              Geometry1 = new EllipseGeometry(new Point(15, 10), 7, 7),
							              Geometry2 = new EllipseGeometry(new Point(17.5, 10), 5, 5),
							              GeometryCombineMode = GeometryCombineMode.Exclude
						              }
					       }
				       };
			}
		}
	}
}
