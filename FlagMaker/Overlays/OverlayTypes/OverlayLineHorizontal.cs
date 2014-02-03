using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayLineHorizontal : Overlay
	{
		public OverlayLineHorizontal(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.Y, true, 1, false),
				       new Attribute(strings.Thickness, true, 1, false)
			       }, maximumX, maximumY)
		{
		}

		public OverlayLineHorizontal(Color color, double thickness, double y, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			             {
				             new Attribute(strings.Y, true, y, false),
				             new Attribute(strings.Thickness, true, thickness, false)
			             }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "line horizontal"; } }

		public override void Draw(Canvas canvas)
		{
			double thick = canvas.Height * (Attributes.Get(strings.Thickness).Value / MaximumY);
			
			var horizontal = new Rectangle
								 {
									 Fill = new SolidColorBrush(Color),
									 Width = canvas.Width,
									 Height = thick,
									 SnapsToDevicePixels = true
								 };
			canvas.Children.Add(horizontal);

			Canvas.SetTop(horizontal, canvas.Height * (Attributes.Get(strings.Y).Value / MaximumY) - thick / 2);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.Y).Value = values[0];
			Attributes.Get(strings.Thickness).Value = values[1];
		}

		public override string ExportSvg(int width, int height)
		{
			double thick = height * (Attributes.Get(strings.Thickness).Value / MaximumY);

			return string.Format(CultureInfo.InvariantCulture, "<rect width=\"{0}\" height=\"{1}\" x=\"0\" y=\"{2}\" fill=\"#{3}\" />",
				width, thick, height * (Attributes.Get(strings.Y).Value / MaximumY) - thick / 2, Color.ToHexString());
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
							   Y1 = 15,
							   Y2 = 15
						   }
				       };
			}
		}
	}
}