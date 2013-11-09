using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes.RepeaterTypes
{
	public class OverlayRepeaterLateral : OverlayRepeater
	{
		public OverlayRepeaterLateral(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute("X", true, 1, true),
				       new Attribute("Y", true, 1, false),
				       new Attribute("Width", true, 1, true),
				       new Attribute("Height", true, 1, false),
				       new Attribute("CountX", true, 1, true),
				       new Attribute("CountY", true, 1, false)
			       }, maximumX, maximumY)
		{
		}

		public OverlayRepeaterLateral(double x, double y, double width, double height, int countX, int countY, int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute("X", true, x, true),
				       new Attribute("Y", true, y, false),
				       new Attribute("Width", true, width, true),
				       new Attribute("Height", true, height, false),
				       new Attribute("CountX", true, countX, true),
				       new Attribute("CountY", true, countY, false)
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

			var locX = canvas.Width * (Attributes.Get("X").Value / MaximumX);
			var locY = canvas.Height * (Attributes.Get("Y").Value / MaximumY);

			var countX = (int)Attributes.Get("CountX").Value;
			var countY = (int)Attributes.Get("CountY").Value;
			var width = canvas.Width * (Attributes.Get("Width").Value / MaximumX);
			var height = canvas.Height * (Attributes.Get("Height").Value / MaximumY);

			double intervalX = width / countX;
			double intervalY = height / countY;

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
					canvas.Children.Add(c);

					Canvas.SetLeft(c, locX + x * intervalX);
					Canvas.SetTop(c, locY + y * intervalY);
				}
			}
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("X").Value = values[0];
			Attributes.Get("Y").Value = values[1];
			Attributes.Get("Width").Value = values[2];
			Attributes.Get("Height").Value = values[3];
			Attributes.Get("CountX").Value = values[4];
			Attributes.Get("CountY").Value = values[5];
		}

		public override string ExportSvg(int width, int height)
		{
			if (Overlay == null) return string.Empty;

			var locX = width * (Attributes.Get("X").Value / MaximumX);
			var locY = height * (Attributes.Get("Y").Value / MaximumY);

			var countX = (int)Attributes.Get("CountX").Value;
			var countY = (int)Attributes.Get("CountY").Value;
			var w = width * (Attributes.Get("Width").Value / MaximumX);
			var h = height * (Attributes.Get("Height").Value / MaximumY);

			double intervalX = w / countX;
			double intervalY = h / countY;

			var sb = new StringBuilder();

			for (int x = 0; x < countX; x++)
			{
				for (int y = 0; y < countY; y++)
				{
					sb.AppendLine(string.Format("<g transform=\"translate({0},{1})\">",
						locX + x * intervalX,
						locY + y * intervalY));
					sb.AppendLine(Overlay.ExportSvg((int)w, (int)h));
					sb.AppendLine("</g>");
				}
			}

			return sb.ToString();
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
					       new Ellipse
					       {
						       Width = 5,
						       Height = 5,
							   Margin = new Thickness(0,7.5,0,0)
					       },
					       new Ellipse
					       {
						       Width = 5,
						       Height = 5,
							   Margin = new Thickness(10,7.5,0,0)
					       },
					       new Ellipse
					       {
						       Width = 5,
						       Height = 5,
							   Margin = new Thickness(20,7.5,0,0)
					       }
				       };
			}
		}
	}
}
