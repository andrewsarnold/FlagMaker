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
			var back = new Rectangle
						  {
							  Fill = new SolidColorBrush(Colors[0]),
							  Width = canvas.Width,
							  Height = canvas.Height,
							  SnapsToDevicePixels = true
						  };
			canvas.Children.Add(back);
			Canvas.SetLeft(back, 0);
			Canvas.SetTop(back, 0);

			var front = new Path
						   {
							   Fill = new SolidColorBrush(Colors[1]),
							   Width = canvas.Width,
							   Height = canvas.Height,
							   Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M 0,0 {0},{1} {0},0 0,{1}", canvas.Width, canvas.Height)),
							   SnapsToDevicePixels = true
						   };
			canvas.Children.Add(front);
			Canvas.SetLeft(front, 0);
			Canvas.SetTop(front, 0);
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

			// back
			sb.Append(string.Format(CultureInfo.InvariantCulture, "<rect width=\"{0:0.###}\" height=\"{1:0.###}\" x=\"0\" y=\"0\" {2} />",
				width,
				height,
				Colors[0].ToSvgFillWithOpacity()));

			// front
			sb.Append(string.Format(CultureInfo.InvariantCulture, "<polygon points=\"0,0 {0:0.###},{1:0.###} {0:0.###},0 0,{1:0.###} \" {2} />",
				width,
				height,
				Colors[1].ToSvgFillWithOpacity()));

			return sb.ToString();
		}
	}
}