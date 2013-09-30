using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Divisions
{
	internal class DivisionFesses : Division
	{
		private Color _color1;
		private Color _color2;
		private Color _color3;

		private double _size1;
		private double _size2;
		private double _size3;

		public DivisionFesses(Color color1, Color color2, Color color3, int v1, int v2, int v3)
		{
			_color1 = color1;
			_color2 = color2;
			_color3 = color3;
			_size1 = v1;
			_size2 = v2;
			_size3 = v3;
		}

		public override string Name { get { return "fesses"; } }

		public override void Draw(Canvas canvas)
		{
			double r1Size = canvas.Height * (_size1 / (_size1 + _size2 + _size3));
			double r2Size = canvas.Height * (_size2 / (_size1 + _size2 + _size3));
			double r3Size = canvas.Height * (_size3 / (_size1 + _size2 + _size3));

			var rect1 = new Rectangle
				            {
					            Fill = new SolidColorBrush(_color1),
					            Width = canvas.Width,
								Height = r1Size
				            };
			canvas.Children.Add(rect1);
			Canvas.SetTop(rect1, 0);
			Canvas.SetLeft(rect1, 0);

			var rect2 = new Rectangle
				            {
					            Fill = new SolidColorBrush(_color2),
					            Width = canvas.Width,
					            Height = r2Size
				            };
			canvas.Children.Add(rect2);
			Canvas.SetTop(rect2, r1Size);
			Canvas.SetLeft(rect2, 0);

			var rect3 = new Rectangle
				            {
					            Fill = new SolidColorBrush(_color3),
					            Width = canvas.Width,
					            Height = r3Size
				            };
			canvas.Children.Add(rect3);
			Canvas.SetTop(rect3, canvas.Height - r3Size);
			Canvas.SetLeft(rect3, 0);
		}

		public override void SetColors(List<Color> colors)
		{
			_color1 = colors[0];
			_color2 = colors[1];
			_color3 = colors[2];
		}

		public override void SetValues(List<double> values)
		{
			_size1 = values[0];
			_size2 = values[1];
			_size3 = values[2];
		}

		public override string ExportSvg(int width, int height)
		{
			var sb = new StringBuilder();

			double r1Size = height * (_size1 / (_size1 + _size2 + _size3));
			double r2Size = height * (_size2 / (_size1 + _size2 + _size3));
			double r3Size = height * (_size3 / (_size1 + _size2 + _size3));

			sb.Append(string.Format("<rect width=\"{0}\" height=\"{1}\" fill=\"#{2}\" x=\"0\" y=\"0\" />",
				width,
				r1Size,
				_color1.ToHexString()));

			sb.Append(string.Format("<rect width=\"{0}\" height=\"{1}\" fill=\"#{2}\" x=\"0\" y=\"{3}\" />",
				width,
				r2Size,
				_color2.ToHexString(),
				r1Size));

			sb.Append(string.Format("<rect width=\"{0}\" height=\"{1}\" fill=\"#{2}\" x=\"0\" y=\"{3}\" />",
				width,
				r3Size,
				_color3.ToHexString(),
				r1Size + r2Size));

			return sb.ToString();
		}
	}
}