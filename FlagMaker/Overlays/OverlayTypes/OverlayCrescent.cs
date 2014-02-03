using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes
{
	public class OverlayCrescent : Overlay
	{
		private const double Ratio = 1.3;

		public OverlayCrescent(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.X, true, 1, true),
				       new Attribute(strings.Y, true, 1, false),
				       new Attribute(strings.Size, true, 1, true),
				       new Attribute(strings.Rotation, true, 0, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlayCrescent(Color color, double x, double y, double size, double rotation, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			             {
				             new Attribute(strings.X, true, x, true),
				             new Attribute(strings.Y, true, y, false),
				             new Attribute(strings.Size, true, size, true),
				             new Attribute(strings.Rotation, true, rotation, true)
			             }, maximumX, maximumY)
		{
		}

		public override string Name
		{
			get { return "crescent"; }
		}

		public override void Draw(Canvas canvas)
		{
			var size = canvas.Width * (Attributes.Get(strings.Size).Value / MaximumX) / 4;
			double rotation = Attributes.Get(strings.Rotation).Value / MaximumX;

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

			Canvas.SetLeft(path, (canvas.Width * (Attributes.Get(strings.X).Value / MaximumX)));
			Canvas.SetTop(path, (canvas.Height * (Attributes.Get(strings.Y).Value / MaximumY)));
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.X).Value = values[0];
			Attributes.Get(strings.Y).Value = values[1];
			Attributes.Get(strings.Size).Value = values[2];
			Attributes.Get(strings.Rotation).Value = values[3];
		}

		public override string ExportSvg(int width, int height)
		{
			double x = width * (Attributes.Get(strings.X).Value / (MaximumX));
			double y = height * (Attributes.Get(strings.Y).Value / (MaximumY));

			var size = width * (Attributes.Get(strings.Size).Value / MaximumX) / 4;
			var sb = new StringBuilder();

			sb.AppendLine("<defs><mask id=\"c\">");
			sb.Append(string.Format(CultureInfo.InvariantCulture, "<circle cx=\"0\" cy=\"0\" r=\"{0}\" fill=\"#ffffff\" mask=\"url(#c)\" />", size));
			sb.Append(string.Format(CultureInfo.InvariantCulture, "<circle cx=\"{0}\" cy=\"0\" r=\"{1}\" />",
				size - size / Math.Pow(Ratio, 1.5), size / Ratio));
			sb.AppendLine("</mask></defs>");

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<g transform=\"translate({0},{1}) rotate({2})\">",
				x, y, 360 * (Attributes.Get(strings.Rotation).Value / MaximumX)));

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<circle cx=\"0\" cy=\"0\" r=\"{0}\" fill=\"#{1}\" mask=\"url(#c)\" />",
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
							              Geometry1 = new EllipseGeometry(new Point(15, 15), 7, 7),
							              Geometry2 = new EllipseGeometry(new Point(17.5, 15), 5, 5),
							              GeometryCombineMode = GeometryCombineMode.Exclude
						              }
					       }
				       };
			}
		}
	}
}
