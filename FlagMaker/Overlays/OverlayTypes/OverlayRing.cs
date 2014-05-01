using System.Collections.Generic;
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
			var width = canvas.Width * (Attributes.Get(strings.Width).Value / MaximumX);
			var height = Attributes.Get(strings.Height).Value == 0
							 ? width
							 : canvas.Height * (Attributes.Get(strings.Height).Value / MaximumY);
			var proportion = Attributes.Get(strings.Size).Value / MaximumX;
			var sizeX = width * proportion;
			var sizeY = height * proportion;

			var locX = (canvas.Width * (Attributes.Get(strings.X).Value / MaximumX));
			var locY = (canvas.Height * (Attributes.Get(strings.Y).Value / MaximumY));

			var outer = new EllipseGeometry
			            {
							Center = new Point(locX, locY),
				            RadiusX = width / 2,
				            RadiusY = height / 2
			            };

			var inner = new EllipseGeometry
			            {
				            Center = new Point(locX, locY),
				            RadiusX = sizeX / 2,
				            RadiusY = sizeY / 2
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
			throw new System.NotImplementedException();
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
