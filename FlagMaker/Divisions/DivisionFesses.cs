using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Divisions
{
	internal class DivisionFesses : Division
	{
		public DivisionFesses(Color color1, Color color2, Color color3, int v1, int v2, int v3)
			: base(new List<Color>
			{
				color1, color2, color3
			}, new List<double>
			{
				v1, v2, v3
			})
		{
		}

		public override string Name { get { return "fesses"; } }

		public override void Draw(Canvas canvas)
		{
			double r1Size = canvas.Height * (Values[0] / (Values[0] + Values[1] + Values[2]));
			double r2Size = canvas.Height * (Values[1] / (Values[0] + Values[1] + Values[2]));
			double r3Size = canvas.Height * (Values[2] / (Values[0] + Values[1] + Values[2]));

			var rect1 = new Rectangle
				            {
					            Fill = new SolidColorBrush(Colors[0]),
					            Width = canvas.Width,
								Height = canvas.Height
				            };
			canvas.Children.Add(rect1);
			Canvas.SetTop(rect1, 0);
			Canvas.SetLeft(rect1, 0);

			var rect2 = new Rectangle
				            {
								Fill = new SolidColorBrush(Colors[1]),
					            Width = canvas.Width,
					            Height = r2Size + r3Size
				            };
			canvas.Children.Add(rect2);
			Canvas.SetTop(rect2, r1Size);
			Canvas.SetLeft(rect2, 0);

			var rect3 = new Rectangle
				            {
								Fill = new SolidColorBrush(Colors[2]),
					            Width = canvas.Width,
					            Height = r3Size
				            };
			canvas.Children.Add(rect3);
			Canvas.SetTop(rect3, canvas.Height - r3Size);
			Canvas.SetLeft(rect3, 0);
		}

		public override void SetColors(List<Color> colors)
		{
			for (int i = 0; i < 3; i++)
				Colors[i] = colors[i];
		}

		public override void SetValues(List<double> values)
		{
			Values[0] = values[0];
			Values[1] = values[1];
			Values[2] = values[2];
		}

		public override string ExportSvg(int width, int height)
		{
			var sb = new StringBuilder();

			double r1Size = height * (Values[0] / (Values[0] + Values[1] + Values[2]));
			double r2Size = height * (Values[1] / (Values[0] + Values[1] + Values[2]));
			double r3Size = height * (Values[2] / (Values[0] + Values[1] + Values[2]));

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<rect width=\"{0:0.###}\" height=\"{1:0.###}\" {2} x=\"0\" y=\"0\" />",
				width,
				r1Size,
				Colors[0].ToSvgFillWithOpacity()));

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<rect width=\"{0:0.###}\" height=\"{1:0.###}\" {2} x=\"0\" y=\"{3:0.###}\" />",
				width,
				r2Size,
				Colors[1].ToSvgFillWithOpacity(),
				r1Size));

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<rect width=\"{0:0.###}\" height=\"{1:0.###}\" {2} x=\"0\" y=\"{3:0.###}\" />",
				width,
				r3Size,
				Colors[2].ToSvgFillWithOpacity(),
				r1Size + r2Size));

			return sb.ToString();
		}
	}
}