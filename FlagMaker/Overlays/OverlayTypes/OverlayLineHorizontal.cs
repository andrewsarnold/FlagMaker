using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayLineHorizontal : Overlay
	{
		public OverlayLineHorizontal(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute("Y", true, 1, false),
				       new Attribute("Thickness", true, 1, false)
			       }, maximumX, maximumY)
		{
		}

		public OverlayLineHorizontal(Color color, double thickness, double y, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			             {
				             new Attribute("Y", true, y, false),
				             new Attribute("Thickness", true, thickness, false)
			             }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "line horizontal"; } }

		public override void Draw(Canvas canvas)
		{
			double thick = canvas.Height * ((Attributes.Get("Thickness").Value + 1) / (MaximumY * 2));
			
			var horizontal = new Rectangle
								 {
									 Fill = new SolidColorBrush(Color),
									 Width = canvas.Width,
									 Height = thick,
									 SnapsToDevicePixels = true
								 };
			canvas.Children.Add(horizontal);

			Canvas.SetTop(horizontal, canvas.Height * (Attributes.Get("Y").Value / MaximumY) - thick / 2);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("Y").Value = values[0];
			Attributes.Get("Thickness").Value = values[1];
		}

		public override string ExportSvg(int width, int height)
		{
			double thick = height * ((Attributes.Get("Thickness").Value + 1) / (MaximumY * 2));

			double y = MaximumY % 2 == 0
				? height * (Attributes.Get("Y").Value / MaximumY) - thick / 2
				: height * (Attributes.Get("Y").Value / (MaximumY + 1)) - thick / 2;

			return string.Format(CultureInfo.InvariantCulture, "<rect width=\"{0}\" height=\"{1}\" x=\"0\" y=\"{2}\" fill=\"#{3}\" />",
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