using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes.RepeaterTypes
{
	internal class OverlayTransformer : OverlayRepeater
	{
		public OverlayTransformer(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute("Skew X", true, maximumX / 2.0, true),
				       new Attribute("Skew Y", true, maximumY / 2.0, false),
				       new Attribute("Size X", true, 1, true),
				       new Attribute("Size Y", true, 1, false),
				       new Attribute(strings.Rotation, true, 0, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlayTransformer(double skewX, double skewY, double sizeX, double sizeY, double rotation, int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute("Skew X", true, skewX, true),
				       new Attribute("Skew Y", true, skewY, false),
				       new Attribute("Size X", true, sizeX, true),
				       new Attribute("Size Y", true, sizeY, false),
				       new Attribute(strings.Rotation, true, rotation, true)
			       }, maximumX, maximumY)
		{
		}

		public override string Name
		{
			get { return "transformer"; }
		}

		public override void Draw(Canvas canvas)
		{
			if (Overlay == null) return;

			var transformCanvas = new Canvas
			{
				Width = canvas.Width,
				Height = canvas.Height
			};

			var centerX = transformCanvas.Width / 2;
			var centerY = transformCanvas.Height / 2;

			var skewX = 90 * (Attributes.Get("Skew X").Value - MaximumX / 2.0) / MaximumX;
			var skewY = 90 * (Attributes.Get("Skew Y").Value - MaximumY / 2.0) / MaximumY;

			var scaleX = Attributes.Get("Size X").Value;
			var scaleY = Attributes.Get("Size Y").Value;

			var transformGroup = new TransformGroup();
			var skewTransform = new SkewTransform(skewX, skewY, centerX, centerY);
			var rotateTransform = new RotateTransform((Attributes.Get(strings.Rotation).Value / MaximumX) * 360, centerX, centerY);
			var scaleTransform = new ScaleTransform(scaleX, scaleY, centerX, centerY);

			transformGroup.Children.Add(rotateTransform);
			transformGroup.Children.Add(scaleTransform);
			transformGroup.Children.Add(skewTransform);

			transformCanvas.RenderTransform = transformGroup;
			Overlay.Draw(transformCanvas);
			canvas.Children.Add(transformCanvas);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("Skew X").Value = values[0];
			Attributes.Get("Skew Y").Value = values[1];
			Attributes.Get("Size X").Value = values[2];
			Attributes.Get("Size Y").Value = values[3];
			Attributes.Get(strings.Rotation).Value = values[4];
		}

		public override string ExportSvg(int width, int height)
		{
			//throw new System.NotImplementedException();
			return string.Empty;
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				{
					new Polygon
					{
						Points = new PointCollection
						{
							new Point(10, 10),
							new Point(25, 10),
							new Point(20, 20),
							new Point(5, 20)
						}
					}
				};
			}
		}
	}
}
