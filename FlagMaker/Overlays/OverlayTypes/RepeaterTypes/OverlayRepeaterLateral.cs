using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes.RepeaterTypes
{
	public class OverlayRepeaterLateral : OverlayRepeater
	{
		public OverlayRepeaterLateral(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.X, true, 1, true),
				       new Attribute(strings.Y, true, 1, false),
				       new Attribute(strings.Width, true, 1, true),
				       new Attribute(strings.Height, true, 1, false),
				       new Attribute(strings.CountX, true, 1, true),
				       new Attribute(strings.CountY, true, 1, false)
			       }, maximumX, maximumY)
		{
		}

		public OverlayRepeaterLateral(double x, double y, double width, double height, int countX, int countY, int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.X, true, x, true),
				       new Attribute(strings.Y, true, y, false),
				       new Attribute(strings.Width, true, width, true),
				       new Attribute(strings.Height, true, height, false),
				       new Attribute(strings.CountX, true, countX, true),
				       new Attribute(strings.CountY, true, countY, false)
			       }, maximumX, maximumY)
		{
		}

		public override string Name
		{
			get { return "repeater lateral"; }
		}

		public override void Draw(Canvas canvas)
		{
			if (Overlay == null) return;
			if (!Overlay.IsEnabled) return;

			var countX = (int)Attributes.Get(strings.CountX).Value;
			var countY = (int)Attributes.Get(strings.CountY).Value;
			var width = canvas.Width * (Attributes.Get(strings.Width).Value / MaximumX);
			var height = canvas.Height * (Attributes.Get(strings.Height).Value / MaximumY);

			var locX = canvas.Width * (Attributes.Get(strings.X).Value / MaximumX) - width / 2;
			var locY = canvas.Height * (Attributes.Get(strings.Y).Value / MaximumY) - height / 2;

			double intervalX = width / (countX > 1 ? countX - 1 : countX);
			double intervalY = height / (countY > 1 ? countY - 1 : countY);

			var repeaterCanvas = new Canvas();

			for (int x = 0; x < countX; x++)
			{
				for (int y = 0; y < countY; y++)
				{
					var c = new Canvas
							{
								Width = width,
								Height = height
							};
					Overlay.Draw(c);
					repeaterCanvas.Children.Add(c);

					Canvas.SetLeft(c, x * intervalX);
					Canvas.SetTop(c, y * intervalY);
				}
			}

			Canvas.SetLeft(repeaterCanvas, locX);
			Canvas.SetTop(repeaterCanvas, locY);
			canvas.Children.Add(repeaterCanvas);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.X).Value = values[0];
			Attributes.Get(strings.Y).Value = values[1];
			Attributes.Get(strings.Width).Value = values[2];
			Attributes.Get(strings.Height).Value = values[3];
			Attributes.Get(strings.CountX).Value = values[4];
			Attributes.Get(strings.CountY).Value = values[5];
		}

		public override string ExportSvg(int width, int height)
		{
			if (Overlay == null) return string.Empty;
			if (!Overlay.IsEnabled) return string.Empty;

			var countX = (int)Attributes.Get(strings.CountX).Value;
			var countY = (int)Attributes.Get(strings.CountY).Value;
			var w = width * (Attributes.Get(strings.Width).Value / MaximumX);
			var h = height * (Attributes.Get(strings.Height).Value / MaximumY);

			var locX = width * (Attributes.Get(strings.X).Value / MaximumX) - w / 2;
			var locY = height * (Attributes.Get(strings.Y).Value / MaximumY) - h / 2;

			double intervalX = w / (countX > 1 ? countX - 1 : countX);
			double intervalY = h / (countY > 1 ? countY - 1 : countY);
			
			var id = Guid.NewGuid();
			var sb = new StringBuilder();

			sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "<defs><g id=\"{0}\">{1}</g></defs>",
				id, Overlay.ExportSvg((int)w, (int)h)));

			for (int x = 0; x < countX; x++)
			{
				for (int y = 0; y < countY; y++)
				{
					sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "<g transform=\"translate({0:0.###},{1:0.###})\">",
						locX + x * intervalX,
						locY + y * intervalY));
					sb.AppendLine(string.Format("<use xlink:href=\"#{0}\" />", id));
					sb.AppendLine("</g>");
				}
			}

			return sb.ToString();
		}

		protected override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
					       new Ellipse
					       {
						       Width = 5,
						       Height = 5,
							   Margin = new Thickness(0,12.5,0,0)
					       },
					       new Ellipse
					       {
						       Width = 5,
						       Height = 5,
							   Margin = new Thickness(10,12.5,0,0)
					       },
					       new Ellipse
					       {
						       Width = 5,
						       Height = 5,
							   Margin = new Thickness(20,12.5,0,0)
					       }
				       };
			}
		}
	}
}
