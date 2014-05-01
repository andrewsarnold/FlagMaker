using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayRing : Overlay
	{
		public OverlayRing(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
					   new Attribute(strings.X, true, 1, true),
				       new Attribute(strings.Y, true, 1, false),
				       new Attribute(strings.Width, true, 1, true),
				       new Attribute(strings.Height, true, 1, false),
					   new Attribute(strings.Size, true, 1, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlayRing(Color color, double x, double y, double width, double height, double size, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			       {
					   new Attribute(strings.X, true, x, true),
				       new Attribute(strings.Y, true, y, false),
				       new Attribute(strings.Width, true, width, true),
				       new Attribute(strings.Height, true, height, false),
					   new Attribute(strings.Size, true, size, true)
			       }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "ring"; } }

		public override void Draw(Canvas canvas)
		{
			var outerDiamX = canvas.Width * (Attributes.Get(strings.Width).Value / MaximumX);
			var outerDiamY = Attributes.Get(strings.Height).Value == 0
				? outerDiamX
				: canvas.Height * (Attributes.Get(strings.Height).Value / MaximumY);

			var proportion = Attributes.Get(strings.Size).Value / MaximumX;
			var innerDiamX = outerDiamX * proportion;
			var innerDiamY = outerDiamY * proportion;

			var locX = (canvas.Width * (Attributes.Get(strings.X).Value / MaximumX));
			var locY = (canvas.Height * (Attributes.Get(strings.Y).Value / MaximumY));

			var outer = new EllipseGeometry
						{
							Center = new Point(locX, locY),
							RadiusX = outerDiamX / 2,
							RadiusY = outerDiamY / 2
						};

			var inner = new EllipseGeometry
						{
							Center = new Point(locX, locY),
							RadiusX = innerDiamX / 2,
							RadiusY = innerDiamY / 2
						};

			var ring = new GeometryGroup
					   {
						   FillRule = FillRule.EvenOdd
					   };
			ring.Children.Add(outer);
			ring.Children.Add(inner);

			var path = new Path
					   {
						   Fill = new SolidColorBrush(Color),
						   SnapsToDevicePixels = true,
						   Data = ring
					   };

			canvas.Children.Add(path);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.X).Value = values[0];
			Attributes.Get(strings.Y).Value = values[1];
			Attributes.Get(strings.Width).Value = values[2];
			Attributes.Get(strings.Height).Value = values[3];
			Attributes.Get(strings.Size).Value = values[4];
		}

		public override string ExportSvg(int width, int height)
		{
			double x = width * Attributes.Get(strings.X).Value / MaximumX;
			double y = height * Attributes.Get(strings.Y).Value / MaximumY;

			var outerRadX = width * Attributes.Get(strings.Width).Value / MaximumX / 2;
			var outerRadY = Attributes.Get(strings.Height).Value == 0
				? outerRadX
				: height * Attributes.Get(strings.Height).Value / MaximumY / 2;

			var proportion = Attributes.Get(strings.Size).Value / MaximumX;
			var innerRadX = outerRadX * proportion;
			var innerRadY = outerRadY * proportion;

			return string.Format(CultureInfo.InvariantCulture,
				"<path d=\"" +
				"M {0:0.###},{1:0.###} m -{2:0.###},0 a {2:0.###},{3:0.###} 0 1,0 {4:0.###},0 a {2:0.###},{3:0.###} 0 1,0 -{4:0.###},0 z" +
				"M {0:0.###},{1:0.###} m {5:0.###},0 a {5:0.###},{6:0.###} 0 1,1 -{7:0.###},0 a {5:0.###},{6:0.###} 0 1,1 {7:0.###},0 z" +
				"\" {8} />",
				x, y, outerRadX, outerRadY, 2 * outerRadX,
				innerRadX, innerRadY, 2 * innerRadX,
				Color.ToSvgFillWithOpacity());
		}

		protected override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
					       new Path
					       {
							   Data = new GeometryGroup
							          {
								          FillRule = FillRule.EvenOdd,
										  Children =
										  {
											  new EllipseGeometry
											  {
												  Center = new Point(15, 15),
												  RadiusX = 15,
												  RadiusY = 15
											  },
											  new EllipseGeometry
											  {
												  Center = new Point(15, 15),
												  RadiusX = 7,
												  RadiusY = 7
											  }
										  }
							          }
					       }
				       };
			}
		}
	}
}
