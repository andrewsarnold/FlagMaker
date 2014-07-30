using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal sealed class OverlayLine : Overlay
	{
		public OverlayLine(int maximumX, int maximumY)
			: base(new List<Attribute>
			{
				new Attribute(strings.X1, true, 1, true),
				new Attribute(strings.Y1, true, 1, false),
				new Attribute(strings.X2, true, 2, true),
				new Attribute(strings.Y2, true, 2, false),
				new Attribute(strings.Thickness, false, 0.5, true)
			}, maximumX, maximumY)
		{
		}

		public OverlayLine(Color color, double x1, double y1, double x2, double y2, double thickness, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			{
				new Attribute(strings.X1, true, x1, true),
				new Attribute(strings.Y1, true, y1, false),
				new Attribute(strings.X2, true, x2, true),
				new Attribute(strings.Y2, true, y2, false),
				new Attribute(strings.Thickness, true, thickness, true)
			}, maximumX, maximumY)
		{
		}

		public override string Name
		{
			get { return "line"; }
		}

		public override void Draw(Canvas canvas)
		{
			var line = new Line
			{
				StrokeThickness = canvas.Width * (Attributes.Get(strings.Thickness).Value / MaximumX),
				X1 = canvas.Width * Attributes.Get(strings.X1).Value / MaximumX,
				Y1 = canvas.Height * Attributes.Get(strings.Y1).Value / MaximumY,
				X2 = canvas.Width * Attributes.Get(strings.X2).Value / MaximumX,
				Y2 = canvas.Height * Attributes.Get(strings.Y2).Value / MaximumY,
				Stroke = new SolidColorBrush(Color),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(line);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.X1).Value = values[0];
			Attributes.Get(strings.Y1).Value = values[1];
			Attributes.Get(strings.X2).Value = values[2];
			Attributes.Get(strings.Y2).Value = values[3];
			Attributes.Get(strings.Thickness).Value = values[4];
		}

		public override string ExportSvg(int width, int height)
		{
			return string.Format("<line x1=\"{0}\" y1=\"{1}\" x2=\"{2}\" y2=\"{3}\" stroke=\"#{4}\" stroke-width=\"{5}\" />",
				width * Attributes.Get(strings.X1).Value / MaximumX,
				height * Attributes.Get(strings.Y1).Value / MaximumY,
				width * Attributes.Get(strings.X2).Value / MaximumX,
				height * Attributes.Get(strings.Y2).Value / MaximumY,
				Color.ToHexString(false),
				width * (Attributes.Get(strings.Thickness).Value / MaximumX));
		}

		protected override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new Shape[]
				       {
						   new Line
						   {
							   StrokeThickness = 5,
							   X1 = 10,
							   X2 = 20,
							   Y1 = 0,
							   Y2 = 30
						   }
				       };
			}
		}
	}
}
