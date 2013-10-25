using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Divisions
{
	internal class DivisionGrid : Division
	{
		public DivisionGrid(Color color1, Color color2, int horizonalCount, int verticalCount)
			: base(new List<Color>
			{
				color1, color2
			}, new List<double>
			{
				horizonalCount, verticalCount
			})
		{
		}

		public override string Name { get { return "grid"; } }

		public override void Draw(Canvas canvas)
		{
			double height = canvas.Height / Values[1];
			double width = canvas.Width / Values[0];

			for (int x = 0; x < Values[0]; x++)
			{
				for (int y = 0; y < Values[1]; y++)
				{
					var rect = new Rectangle
								   {
									   Fill = new SolidColorBrush(Colors[(x + y) % 2]),
									   Width = width,
									   Height = height,
									   SnapsToDevicePixels = true
								   };
					canvas.Children.Add(rect);
					Canvas.SetTop(rect, y * height);
					Canvas.SetLeft(rect, x * width);
				}
			}
		}

		public override void SetColors(List<Color> colors)
		{
			Colors[0] = colors[0];
			Colors[1] = colors[1];
		}

		public override void SetValues(List<double> values)
		{
			Values[0] = values[0];
			Values[1] = values[1];
		}

		public override string ExportSvg(int width, int height)
		{
			var sb = new StringBuilder();

			double h = height / Values[1];
			double w = width / Values[0];

			for (int x = 0; x < Values[0]; x++)
			{
				for (int y = 0; y < Values[1]; y++)
				{
					sb.Append(string.Format("<rect width=\"{0}\" height=\"{1}\" fill=\"#{2}\" x=\"{3}\" y=\"{4}\"/>",
						w, h, ((x + y) % 2 == 0 ? Colors[0] : Colors[1]).ToHexString(), x * w, y * h));
				}
			}

			return sb.ToString();
		}
	}
}