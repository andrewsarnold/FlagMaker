using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Divisions
{
	internal class DivisionX : Division
	{
		public DivisionX(Color color1, Color color2)
			: base(new List<Color>
			{
				color1,
				color2
			}, new List<double>())
		{
		}

		public override string Name { get { return "bends both"; } }

		public override void Draw(Canvas canvas)
		{
			double centerX = canvas.Width / 2.0;
			double centerY = canvas.Height / 2.0;

			var top = new Path
						  {
							  Fill = new SolidColorBrush(Colors[0]),
							  Width = canvas.Width,
							  Height = canvas.Height,
							  Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M 0,0 {0},0 {1},{2} 0,0", canvas.Width, centerX, centerY)),
							  SnapsToDevicePixels = true
						  };
			canvas.Children.Add(top);
			Canvas.SetLeft(top, 0);
			Canvas.SetTop(top, 0);

			var bottom = new Path
							 {
								 Fill = new SolidColorBrush(Colors[0]),
								 Width = canvas.Width,
								 Height = canvas.Height,
								 Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M 0,{3} {0},{3} {1},{2} 0,{3}", canvas.Width, centerX, centerY, canvas.Height)),
								 SnapsToDevicePixels = true
							 };
			canvas.Children.Add(bottom);
			Canvas.SetLeft(bottom, 0);
			Canvas.SetTop(bottom, 0);

			var left = new Path
						   {
							   Fill = new SolidColorBrush(Colors[1]),
							   Width = canvas.Width,
							   Height = canvas.Height,
							   Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M 0,0 {1},{2} 0,{0} 0,0", canvas.Height, centerX, centerY)),
							   SnapsToDevicePixels = true
						   };
			canvas.Children.Add(left);
			Canvas.SetLeft(left, 0);
			Canvas.SetTop(left, 0);

			var right = new Path
							{
								Fill = new SolidColorBrush(Colors[1]),
								Width = canvas.Width,
								Height = canvas.Height,
								Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M {3},0 {1},{2} {3},{0} {3},0", canvas.Height, centerX, centerY, canvas.Width)),
								SnapsToDevicePixels = true
							};
			canvas.Children.Add(right);
			Canvas.SetLeft(right, 0);
			Canvas.SetTop(right, 0);
		}

		public override void SetColors(List<Color> colors)
		{
			Colors[0] = colors[0];
			Colors[1] = colors[1];
		}

		public override void SetValues(List<double> values)
		{
		}

		public override string ExportSvg(int width, int height)
		{
			var sb = new StringBuilder();

			int centerX = width / 2;
			int centerY = height / 2;

			// top
			sb.Append(string.Format(CultureInfo.InvariantCulture, "<polygon points=\"0,0 {0:0.###},0 {1:0.###},{2:0.###}\" {3} />",
				width,
				centerX,
				centerY,
				Colors[0].ToSvgFillWithOpacity()));

			// left
			sb.Append(string.Format(CultureInfo.InvariantCulture, "<polygon points=\"0,0 0,{0:0.###} {1:0.###},{2:0.###}\" {3} />",
				height,
				centerX,
				centerY,
				Colors[1].ToSvgFillWithOpacity()));

			// bottom
			sb.Append(string.Format(CultureInfo.InvariantCulture, "<polygon points=\"0,{0:0.###} {1:0.###},{0:0.###} {2:0.###},{3:0.###}\" {4} />",
				height,
				width,
				centerX,
				centerY,
				Colors[0].ToSvgFillWithOpacity()));

			// right
			sb.Append(string.Format(CultureInfo.InvariantCulture, "<polygon points=\"{0:0.###},0 {0:0.###},{1:0.###} {2:0.###},{3:0.###}\" {4} />",
				width,
				height,
				centerX,
				centerY,
				Colors[1].ToSvgFillWithOpacity()));

			return sb.ToString();
		}
	}
}