using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayHalfEllipse : Overlay
	{
		public OverlayHalfEllipse(int maximumX, int maximumY)
			: base(new List<Attribute>
			{
				new Attribute(strings.X, true, 1, true),
				new Attribute(strings.Y, true, 1, false),
				new Attribute(strings.Width, true, 1, true),
				new Attribute(strings.Height, true, 1, false),
				new Attribute(strings.Rotation, true, 0, true)
			}, maximumX, maximumY)
		{
		}

		public OverlayHalfEllipse(Color color, double x, double y, double width, double height, double rotation, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			{
				new Attribute(strings.X, true, x, true),
				new Attribute(strings.Y, true, y, false),
				new Attribute(strings.Width, true, width, true),
				new Attribute(strings.Height, true, height, false),
				new Attribute(strings.Rotation, true, rotation, true)
			}, maximumX, maximumY)
		{
		}

		public override string Name
		{
			get { return "half ellipse"; }
		}

		public override void Draw(Canvas canvas)
		{
			var path = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = canvas.Width,
				Height = canvas.Height,
				Data = Geometry.Parse(GetPath(canvas.Width, canvas.Height)),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(path);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.X).Value = values[0];
			Attributes.Get(strings.Y).Value = values[1];
			Attributes.Get(strings.Width).Value = values[2];
			Attributes.Get(strings.Height).Value = values[3];
			Attributes.Get(strings.Rotation).Value = values[4];
		}

		public override string ExportSvg(int width, int height)
		{
			return string.Format(CultureInfo.InvariantCulture, "<path d=\"{0}\" fill=\"#{1}\" />",
				GetPath(width, height), Color.ToHexString());
		}

		protected override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
						   new Path
						   {
							   Data = Geometry.Parse("M 0,20 A 15,15 0 0,1 30,20 z")
						   }
				       };
			}
		}

		private string GetPath(double width, double height)
		{
			var x = width * Attributes.Get(strings.X).Value / MaximumX;
			var y = height * Attributes.Get(strings.Y).Value / MaximumY;
			var radX = width * (Attributes.Get(strings.Width).Value / MaximumX) / 2;
			var radY = height * Attributes.Get(strings.Height).Value / MaximumY;

			var angle = 2 * Math.PI * Attributes.Get(strings.Rotation).Value / MaximumX;
			var xOffset = radX - radX * Math.Cos(angle);
			var yOffset = radX * Math.Sin(angle);

			var x1 = x - radX + xOffset;
			var x2 = x + radX - xOffset;
			var y1 = y - yOffset;
			var y2 = y + yOffset;

			return string.Format(CultureInfo.InvariantCulture,
				"M {0:0.###},{1:0.###} A {4:0.###},{5:0.###} {6:0.###} 1,1 {2:0.###},{3:0.###} z",
				x1, y1, x2, y2, radX, radY, angle*180/Math.PI);
		}
	}
}
