using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Divisions
{
	internal class DivisionGrid : Division
	{
		private Color _color1;
		private Color _color2;
		private double _horizonalCount;
		private double _verticalCount;

		public DivisionGrid(Color color1, Color color2, int horizonalCount, int vertialCount)
		{
			_color1 = color1;
			_color2 = color2;
			_horizonalCount = horizonalCount;
			_verticalCount = vertialCount;
		}

		public override string Name { get { return "grid"; } }

		public override void Draw(Canvas canvas)
		{
			double height = canvas.Height /_verticalCount;
			double width = canvas.Width / _horizonalCount;

			for (int x = 0; x < _horizonalCount; x++)
			{
				for (int y = 0; y < _verticalCount; y++)
				{
					var rect = new Rectangle
								   {
									   Fill = new SolidColorBrush((x + y) % 2 == 0 ? _color1 : _color2),
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
			_color1 = colors[0];
			_color2 = colors[1];
		}

		public override void SetValues(List<double> values)
		{
			_horizonalCount = values[0];
			_verticalCount = values[1];
		}

		public override string ExportSvg(int width, int height)
		{
			var sb = new StringBuilder();

			double h = height / _verticalCount;
			double w = width / _horizonalCount;

			for (int x = 0; x < _horizonalCount; x++)
			{
				for (int y = 0; y < _verticalCount; y++)
				{
					sb.Append(string.Format("<rect width=\"{0}\" height=\"{1}\" fill=\"#{2}\" x=\"{3}\" y=\"{4}\"/>",
						w, h, ((x + y) % 2 == 0 ? _color1 : _color2).ToHexString(), x * w, y * h));
				}
			}

			return sb.ToString();
		}
	}
}