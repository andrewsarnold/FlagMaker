using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Divisions
{
	internal class DivisionPales : Division
	{
		public DivisionPales(Color color1, Color color2, Color color3, int v1, int v2, int v3)
			: base(new List<Color>
			{
				color1, color2, color3
			}, new List<double>
			{
				v1, v2, v3
			})
		{
		}

		public override string Name { get { return "pales"; } }

		public override void Draw(Canvas canvas)
		{
			double r1Size = canvas.Width * (Values[0] / (Values[0] + Values[1] + Values[2]));
			double r2Size = canvas.Width * (Values[1] / (Values[0] + Values[1] + Values[2]));
			double r3Size = canvas.Width * (Values[2] / (Values[0] + Values[1] + Values[2]));

			var rect1 = new Rectangle
				            {
								Fill = new SolidColorBrush(Colors[0]),
					            Width = r1Size,
					            Height = canvas.Height,
								SnapsToDevicePixels = true
				            };
			canvas.Children.Add(rect1);
			Canvas.SetTop(rect1, 0);
			Canvas.SetLeft(rect1, 0);

			var rect2 = new Rectangle
				            {
								Fill = new SolidColorBrush(Colors[1]),
					            Width = r2Size,
								Height = canvas.Height,
								SnapsToDevicePixels = true
				            };
			canvas.Children.Add(rect2);
			Canvas.SetTop(rect2, 0);
			Canvas.SetLeft(rect2, r1Size);

			var rect3 = new Rectangle
				            {
								Fill = new SolidColorBrush(Colors[2]),
					            Width = r3Size,
								Height = canvas.Height,
								SnapsToDevicePixels = true
				            };
			canvas.Children.Add(rect3);
			Canvas.SetTop(rect3, 0);
			Canvas.SetLeft(rect3, canvas.Width - r3Size);
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

			double r1Size = width * (Values[0] / (Values[0] + Values[1] + Values[2]));
			double r2Size = width * (Values[1] / (Values[0] + Values[1] + Values[2]));
			double r3Size = width * (Values[2] / (Values[0] + Values[1] + Values[2]));

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<rect width=\"{0:0.###}\" height=\"{1:0.###}\" fill=\"#{2}\" x=\"0\" y=\"0\" />",
				r1Size,
				height,
				Colors[0].ToHexString()));

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<rect width=\"{0:0.###}\" height=\"{1:0.###}\" fill=\"#{2}\" x=\"{3:0.###}\" y=\"0\" />",
				r2Size,
				height,
				Colors[1].ToHexString(),
				r1Size));

			sb.Append(string.Format(CultureInfo.InvariantCulture, "<rect width=\"{0:0.###}\" height=\"{1:0.###}\" fill=\"#{2}\" x=\"{3:0.###}\" y=\"0\" />",
				r3Size,
				height,
				Colors[2].ToHexString(),
				r1Size + r2Size));

			return sb.ToString();
		}
	}
}