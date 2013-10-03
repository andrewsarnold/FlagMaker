using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayLineHorizontal : Overlay
	{
		public OverlayLineHorizontal(int maximum)
			: base(new List<Attribute>
			       {
				       new Attribute("Y", true, 1),
				       new Attribute("Thickness", true, 1)
			       }, maximum)
		{
		}

		public OverlayLineHorizontal(Color color, int thickness, int x, int y, int maximum)
			: base(color, new List<Attribute>
			             {
				             new Attribute("Y", true, y),
				             new Attribute("Thickness", true, thickness)
			             }, maximum)
		{
		}

		public override string Name { get { return "line horizontal"; } }

		public override void Draw(Canvas canvas)
		{
			double thick = canvas.Width * ((Attributes.Get("Thickness").Value + 1) / (Maximum * 2));
			
			var horizontal = new Rectangle
								 {
									 Fill = new SolidColorBrush(Color),
									 Width = canvas.Width,
									 Height = thick,
									 SnapsToDevicePixels = true
								 };
			canvas.Children.Add(horizontal);

			if (Maximum % 2 == 0)
			{
				Canvas.SetTop(horizontal, canvas.Height * (Attributes.Get("Y").Value / Maximum) - thick / 2);
			}
			else
			{
				Canvas.SetTop(horizontal, canvas.Height * (Attributes.Get("Y").Value / (Maximum + 1)) - thick / 2);
			}
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("Y").Value = values[0];
			Attributes.Get("Thickness").Value = values[1];
		}

		public override string ExportSvg(int width, int height)
		{
			double thick = width * ((Attributes.Get("Thickness").Value + 1) / (Maximum * 2));

			double y = Maximum % 2 == 0
				? height * (Attributes.Get("Y").Value / Maximum) - thick / 2
				: height * (Attributes.Get("Y").Value / (Maximum + 1)) - thick / 2;

			return string.Format("<rect width=\"{0}\" height=\"{1}\" x=\"0\" y=\"{2}\" fill=\"#{3}\" />",
				width, thick, y, Color.ToHexString());
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new Shape[]
				       {
						   new Line
						   {
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