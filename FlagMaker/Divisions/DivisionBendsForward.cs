using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Divisions
{
	internal class DivisionBendsForward : Division
	{
		private Color _color1;
		private Color _color2;

		public DivisionBendsForward(Color color1, Color color2)
		{
			_color1 = color1;
			_color2 = color2;
		}

		public override string Name { get { return "bends forward"; } }

		public override void Draw(Canvas canvas)
		{
			var topLeft = new Path
							  {
								  Fill = new SolidColorBrush(_color1),
								  Width = canvas.Width,
								  Height = canvas.Height,
								  Data = Geometry.Parse(string.Format("M 0,0 {0},0 0,{1} 0,0", canvas.Width, canvas.Height)),
								  SnapsToDevicePixels = true
							  };
			canvas.Children.Add(topLeft);
			Canvas.SetLeft(topLeft, 0);
			Canvas.SetTop(topLeft, 0);

			var bottomRight = new Path
				                  {
					                  Fill = new SolidColorBrush(_color2),
					                  Width = canvas.Width,
					                  Height = canvas.Height,
					                  Data = Geometry.Parse(string.Format("M {0},0 {0},{1} 0,{1} {0},0", canvas.Width, canvas.Height)),
					                  SnapsToDevicePixels = true
				                  };
			canvas.Children.Add(bottomRight);
			Canvas.SetLeft(bottomRight, 0);
			Canvas.SetTop(bottomRight, 0);
		}

		public override void SetColors(List<Color> colors)
		{
			_color1 = colors[0];
			_color2 = colors[1];
		}

		public override void SetValues(List<double> values)
		{
		}

		public override string ExportSvg(int width, int height)
		{
			var sb = new StringBuilder();

			// left
			sb.Append(string.Format("<polygon points=\"0,0 {0},0 0,{1}\" style=\"fill:{2};\" />",
				width,
				height,
				_color1.ToHexString()));

			// right
			sb.Append(string.Format("<polygon points=\"{0},{1} {0},0 0,{1}\" style=\"fill:{2};\" />",
				width,
				height,
				_color2.ToHexString()));

			return sb.ToString();
		}
	}
}