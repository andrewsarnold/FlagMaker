using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes.RepeaterTypes
{
	public class OverlayRepeaterRadial : OverlayRepeater
	{
		public OverlayRepeaterRadial(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.X, true, 1, true),
				       new Attribute(strings.Y, true, 1, false),
				       new Attribute(strings.Radius, true, 1, true),
				       new Attribute(strings.Count, true, 1, true),
				       new Attribute(strings.Rotate, true, 1, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlayRepeaterRadial(double x, double y, double radius, int count, int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.X, true, x, true),
				       new Attribute(strings.Y, true, y, false),
				       new Attribute(strings.Radius, true, radius, true),
				       new Attribute(strings.Rotate, true, count, true),
			       }, maximumX, maximumY)
		{
		}

		public override string Name
		{
			get { return "repeater radial"; }
		}

		public override void Draw(Canvas canvas)
		{
			if (Overlay == null) return;

			var locX = canvas.Width * (Attributes.Get(strings.X).Value / MaximumX);
			var locY = canvas.Height * (Attributes.Get(strings.Y).Value / MaximumY);
			var radius = canvas.Width * (Attributes.Get(strings.Radius).Value / MaximumX);
			var interval = 2 * Math.PI / Attributes.Get(strings.Count).Value;
			bool rotate = Attributes.Get(strings.Rotate).Value > MaximumX / 2.0;

			for (int i = 0; i < Attributes.Get(strings.Count).Value; i++)
			{
				var c = new Canvas
				{
					Width = radius,
					Height = radius
				};
				
				Overlay.Draw(c);
				
				if (rotate)
				{
					c.RenderTransform = new RotateTransform(i * 360 / Attributes.Get(strings.Count).Value);
				}

				canvas.Children.Add(c);

				Canvas.SetLeft(c, locX + Math.Cos(i * interval - Math.PI / 2) * radius);
				Canvas.SetTop(c, locY + Math.Sin(i * interval - Math.PI / 2) * radius);
			}
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.X).Value = values[0];
			Attributes.Get(strings.Y).Value = values[1];
			Attributes.Get(strings.Radius).Value = values[2];
			Attributes.Get(strings.Count).Value = values[3];
			Attributes.Get(strings.Rotate).Value = values[4];
		}

		public override string ExportSvg(int width, int height)
		{
			if (Overlay == null) return string.Empty;

			var locX = width * (Attributes.Get(strings.X).Value / MaximumX);
			var locY = height * (Attributes.Get(strings.Y).Value / MaximumY);
			var radius = width * (Attributes.Get(strings.Radius).Value / MaximumX);
			var interval = 2 * Math.PI / Attributes.Get(strings.Count).Value;
			bool rotate = Attributes.Get(strings.Rotate).Value > MaximumX / 2.0;

			var id = Guid.NewGuid();
			var sb = new StringBuilder();

			sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "<defs><g id=\"{0}\">{1}</g></defs>",
				id, Overlay.ExportSvg((int)radius, (int)radius)));

			for (int i = 0; i < Attributes.Get(strings.Count).Value; i++)
			{
				sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "<g transform=\"translate({0:0.###},{1:0.###}){2:0.###}\">",
					locX + Math.Cos(i * interval - Math.PI / 2) * radius,
					locY + Math.Sin(i * interval - Math.PI / 2) * radius,
					rotate ? string.Format(CultureInfo.InvariantCulture, "rotate({0:0.###})", i * 360 / Attributes.Get(strings.Count).Value) : string.Empty));
				sb.AppendLine(string.Format("<use xlink:href=\"#{0}\" />", id));
				sb.AppendLine("</g>");
			}
			
			return sb.ToString();
		}

		protected override IEnumerable<Shape> Thumbnail
		{
			get
			{
				const int count = 7;
				const int radius = 8;
				const double interval = 2 * Math.PI / count;
				var shapes = new List<Shape>();

				for (int i = 0; i < count; i++)
				{
					var left = Math.Cos(i * interval) * radius + 11.5;
					var top = Math.Sin(i * interval) * radius + 11.5;
					shapes.Add(new Ellipse
							   {
								   Width = 3,
								   Height = 3,
								   Margin = new Thickness(left, top, 0, 0)
							   });
				}

				return shapes;
			}
		}
	}
}