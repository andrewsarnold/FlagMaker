using System.Collections.Generic;
using System.Globalization;
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
				       new Attribute(strings.SkewX, true, maximumX / 2.0, true),
				       new Attribute(strings.SkewY, true, maximumY / 2.0, false),
				       new Attribute(strings.Width, true, 1, true),
				       new Attribute(strings.Height, true, 1, false),
				       new Attribute(strings.Rotation, true, 0, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlayTransformer(double skewX, double skewY, double sizeX, double sizeY, double rotation, int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.SkewX, true, skewX, true),
				       new Attribute(strings.SkewY, true, skewY, false),
				       new Attribute(strings.Width, true, sizeX, true),
				       new Attribute(strings.Height, true, sizeY, false),
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
				Height = canvas.Height,
				RenderTransform = GetTransformation((int)canvas.Width, (int)canvas.Height)
			};

			Overlay.Draw(transformCanvas);
			canvas.Children.Add(transformCanvas);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.SkewX).Value = values[0];
			Attributes.Get(strings.SkewY).Value = values[1];
			Attributes.Get(strings.Width).Value = values[2];
			Attributes.Get(strings.Height).Value = values[3];
			Attributes.Get(strings.Rotation).Value = values[4];
		}

		public override string ExportSvg(int width, int height)
		{
			if (Overlay == null) return string.Empty;

			var matrix = GetTransformation(width, height).Value;

			return string.Format(CultureInfo.InvariantCulture, "<g transform=\"matrix({0:0.###},{1:0.###},{2:0.###},{3:0.###},{4:0.###},{5:0.###})\">{6}</g>",
				matrix.M11,
				matrix.M12,
				matrix.M21,
				matrix.M22,
				matrix.OffsetX,
				matrix.OffsetY,
				Overlay.ExportSvg(width, height));
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

		private TransformGroup GetTransformation(int width, int height)
		{
			var centerX = width / 2;
			var centerY = height / 2;

			var skewX = 90 * (Attributes.Get(strings.SkewX).Value - MaximumX / 2.0) / MaximumX;
			var skewY = 90 * (Attributes.Get(strings.SkewY).Value - MaximumY / 2.0) / MaximumY;

			var scaleX = Attributes.Get(strings.Width).Value;
			var scaleY = Attributes.Get(strings.Height).Value;

			var rotation = (Attributes.Get(strings.Rotation).Value / MaximumX) * 360;

			var transformGroup = new TransformGroup();
			var skewTransform = new SkewTransform(skewX, skewY, centerX, centerY);
			var rotateTransform = new RotateTransform(rotation, centerX, centerY);
			var scaleTransform = new ScaleTransform(scaleX, scaleY, centerX, centerY);

			transformGroup.Children.Add(rotateTransform);
			transformGroup.Children.Add(scaleTransform);
			transformGroup.Children.Add(skewTransform);

			return transformGroup;
		}
	}
}
