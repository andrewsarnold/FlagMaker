using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Divisions;

namespace FlagMaker.Overlays.OverlayTypes.ShapeTypes
{
	public class OverlayFlag : OverlayShape
	{
		private readonly Flag _flag;

		public string Path { get; private set; }

		public OverlayFlag(int maximumX, int maximumY)
			: base(maximumX, maximumY)
		{
			_flag = new Flag("flag", new Ratio(2, 3), new Ratio(2, 3), new DivisionGrid(Colors.White, Colors.Black, 2, 2), new List<Overlay>());
		}

		public OverlayFlag(Flag flag, string path, int maximumX, int maximumY)
			: base(maximumX, maximumY)
		{
			_flag = flag;
			Path = path;
		}

		public OverlayFlag(Flag flag, string path, double x, double y, double width, double height, int maximumX, int maximumY)
			: base(Colors.White, x, y, width, height, maximumX, maximumY)
		{
			_flag = flag;
			Path = path;
		}

		public override string Name { get { return "flag"; } }

		public override void Draw(Canvas canvas)
		{
			var canvasWidth = canvas.Width * Attributes.Get("Width").Value / MaximumX;
			var canvasHeight = canvas.Height * Attributes.Get("Height").Value / MaximumY;

			var c = new Canvas
					{
						Width = canvasWidth,
						Height = canvasHeight
					};

			_flag.Draw(c);
			canvas.Children.Add(c);

			Canvas.SetLeft(c, (canvas.Width * (Attributes.Get("X").Value / MaximumX)));
			Canvas.SetTop(c, (canvas.Height * (Attributes.Get("Y").Value / MaximumY)));
		}

		public override string ExportSvg(int width, int height)
		{
			var sb = new StringBuilder();

			sb.Append(string.Format("<g transform=\"translate({0},{1}) scale({2} {3})\">",
				width * (Attributes.Get("X").Value / MaximumX),
				height * (Attributes.Get("Y").Value / MaximumY),
				Attributes.Get("Width").Value / MaximumX,
				Attributes.Get("Height").Value / MaximumY));

			sb.Append(_flag.Division.ExportSvg(width, height));

			foreach (var overlay in _flag.Overlays)
			{
				sb.Append(overlay.ExportSvg(width, height));
			}

			sb.Append("</g>");

			return sb.ToString();
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
						   new Rectangle
						   {
						       Width = 30,
						       Height = 20,
							   Stroke = Brushes.Black,
							   StrokeThickness = 3
						   },
						   new Line
						   {
							   Stroke = Brushes.White,
							   StrokeThickness = 5,
							   X1 = 10,
							   X2 = 10,
							   Y1 = 0,
							   Y2 = 20
						   },
						   new Line
						   {
							   Stroke = Brushes.White,
							   StrokeThickness = 5,
							   X1 = 0,
							   X2 = 30,
							   Y1 = 10,
							   Y2 = 10
						   }
				       };
			}
		}
	}
}