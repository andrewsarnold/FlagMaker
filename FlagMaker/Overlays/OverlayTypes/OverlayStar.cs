using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	public class OverlayStar : Overlay
	{
		public OverlayStar(int maximum)
			: base(new List<Attribute>
			       {
				       new Attribute("X", true, 1),
				       new Attribute("Y", true, 1),
				       new Attribute("Size", true, 1),
				       new Attribute("Rotation", true, 0)
			       }, maximum)
		{
		}

		public OverlayStar(Color color, int x, int y, int size, int rotation, int maximum)
			: base(color, new List<Attribute>
			                                 {
				                                 new Attribute("X", true, x),
				                                 new Attribute("Y", true, y),
				                                 new Attribute("Size", true, size),
				                                 new Attribute("Rotation", true, rotation)
			                                 }, maximum)
		{
		}

		public override string Name { get { return "star"; } }

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
					       new Path
					       {
						       Data = Geometry.Parse("M 15,0 12.8,7 5.5,6.9 11.5,11.1 9.1,18.1 15,13.7 20.9,18.1 18.5,11.1 24.5,6.9 17.2,7 Z")
					       }
				       };
			}
		}

		public override void Draw(Canvas canvas)
		{
			var size = canvas.Width * (Attributes.Get("Size").Value / Maximum) / 8;

			var path = new Path
						   {
							   Fill = new SolidColorBrush(Color),
							   Data = Geometry.Parse(string.Format("M {0} Z", GetPointPath(canvas.Width))),
							   SnapsToDevicePixels = true
						   };

			canvas.Children.Add(path);

			double topOffset = size / 20;
			double leftOffset = size / 20;

			if (Maximum % 2 == 0)
			{
				Canvas.SetLeft(path, (canvas.Width * (Attributes.Get("X").Value / Maximum) - leftOffset));
				Canvas.SetTop(path, (canvas.Height * (Attributes.Get("Y").Value / Maximum) + topOffset));
			}
			else
			{
				Canvas.SetLeft(path, (canvas.Width * (Attributes.Get("X").Value / (Maximum + 1)) - leftOffset));
				Canvas.SetTop(path, (canvas.Height * (Attributes.Get("Y").Value / (Maximum + 1)) + topOffset));
			}
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("X").Value = values[0];
			Attributes.Get("Y").Value = values[1];
			Attributes.Get("Size").Value = values[2];
			Attributes.Get("Rotation").Value = values[3];
		}

		public override string ExportSvg(int width, int height)
		{
			var size = width * (Attributes.Get("Size").Value / Maximum) / 8;
			double topOffset = size / 20;
			double leftOffset = size / 20;

			double x, y;
			if (Maximum % 2 == 0)
			{
				x = width * (Attributes.Get("X").Value / Maximum) - leftOffset;
				y = height * (Attributes.Get("Y").Value / Maximum) + topOffset;
			}
			else
			{
				x = width * (Attributes.Get("X").Value / (Maximum + 1)) - leftOffset;
				y = height * (Attributes.Get("Y").Value / (Maximum + 1)) + topOffset;
			}

			return string.Format("<g transform=\"translate({0},{1})\"><polygon points=\"{2}\" fill=\"#{3}\" /></g>",
				x, y, GetPointPath(width), Color.ToHexString());
		}

		private string GetPointPath(double canvasWidth)
		{
			var size = canvasWidth * (Attributes.Get("Size").Value / Maximum) / 8;
			double rotation = Attributes.Get("Rotation").Value / Maximum;

			var tips = new double[5, 2];
			var interiors = new double[5, 2];

			for (int i = 0; i < 5; i++)
			{
				var tRad = (i * 144) * (Math.PI / 180) + rotation;
				var tSin = Math.Sin(tRad);
				var tCos = Math.Cos(tRad);

				tips[i, 0] = tSin * size + Attributes.Get("X").Value;
				tips[i, 1] = tCos * size + Attributes.Get("Y").Value;

				var iRad = ((i * 144) + 180) * (Math.PI / 180) + rotation;
				var iSin = Math.Sin(iRad);
				var iCos = Math.Cos(iRad);

				interiors[i, 0] = iSin * size * 2.7 + Attributes.Get("X").Value;
				interiors[i, 1] = iCos * size * 2.7 + Attributes.Get("Y").Value;
			}

			return string.Format("{0},{1} {2},{3} {4},{5} {6},{7} {8},{9} {10},{11} {12},{13} {14},{15} {16},{17} {18},{19}",
				tips[0, 0], tips[0, 1],
				interiors[4, 0], interiors[4, 1],
				tips[3, 0], tips[3, 1],
				interiors[2, 0], interiors[2, 1],
				tips[1, 0], tips[1, 1],
				interiors[0, 0], interiors[0, 1],
				tips[4, 0], tips[4, 1],
				interiors[3, 0], interiors[3, 1],
				tips[2, 0], tips[2, 1],
				interiors[1, 0], interiors[1, 1]);
		}
	}
}
