using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes.PathTypes
{
	public class OverlayPath : Overlay
	{
		private readonly string _name;
		private readonly Vector _pathSize;
		private readonly string _path;

		public Color StrokeColor { get; set; }

		public OverlayPath(string name, string path, Vector pathSize, int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.X, true, 1, true),
				       new Attribute(strings.Y, true, 1, false),
				       new Attribute(strings.Size, true, 1, true),
				       new Attribute(strings.Rotation, true, 0, true),
					   new Attribute(strings.Stroke, true, 0, true),
					   new Attribute(strings.StrokeCurved, true, 0, true)
			       }, maximumX, maximumY)
		{
			_name = name;
			_path = path;
			_pathSize = pathSize;
			StrokeColor = Colors.White;
		}

		protected OverlayPath(Color color, string name, string path, Vector pathSize, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			       {
				       new Attribute(strings.X, true, 1, true),
				       new Attribute(strings.Y, true, 1, false),
				       new Attribute(strings.Size, true, 1, true),
				       new Attribute(strings.Rotation, true, 0, true),
					   new Attribute(strings.Stroke, true, 0, true),
					   new Attribute(strings.StrokeCurved, true, 0, true)
			       }, maximumX, maximumY)
		{
			_name = name;
			_path = path;
			_pathSize = pathSize;
			StrokeColor = Colors.White;
		}

		public override string Name
		{
			get { return _name; }
		}

		public override void Draw(Canvas canvas)
		{
			try
			{
				double xGridSize = canvas.Width / MaximumX;
				double yGridSize = canvas.Height / MaximumY;

				double x = Attributes.Get(strings.X).Value;
				double y = Attributes.Get(strings.Y).Value;

				var finalCenterPoint = new Point(x * xGridSize, y * yGridSize);
				var transformGroup = new TransformGroup();
				var rotateTransform = new RotateTransform((Attributes.Get(strings.Rotation).Value / MaximumX) * 360);
				transformGroup.Children.Add(rotateTransform);

				var scaleFactor = ScaleFactor(canvas.Width, canvas.Height);
				var scaleTransform = new ScaleTransform(scaleFactor, scaleFactor);
				transformGroup.Children.Add(scaleTransform);
				
				var path = new Path
				           {
					           Fill = new SolidColorBrush(Color),
					           RenderTransform = transformGroup,
					           Data = Geometry.Parse(_path),
					           SnapsToDevicePixels = true,
					           Stroke = new SolidColorBrush(StrokeColor),
					           StrokeThickness = StrokeThickness(canvas.Width, canvas.Height),
							   StrokeLineJoin = Attributes.Get(strings.StrokeCurved).Value > MaximumX / 2.0
						           ? PenLineJoin.Round
						           : PenLineJoin.Miter
				           };

				canvas.Children.Add(path);

				Canvas.SetLeft(path, finalCenterPoint.X);
				Canvas.SetTop(path, finalCenterPoint.Y);
			}
			catch (Exception)
			{
				MessageBox.Show(string.Format("Couldn't draw custom overlay \"{0}\".", Name));
			}
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.X).Value = values[0];
			Attributes.Get(strings.Y).Value = values[1];
			Attributes.Get(strings.Size).Value = values[2];
			Attributes.Get(strings.Rotation).Value = values[3];
			Attributes.Get(strings.Stroke).Value = values[4];
			Attributes.Get(strings.StrokeCurved).Value = values[5];
		}

		public override string ExportSvg(int width, int height)
		{
			try
			{
				double xGridSize = (double)width / MaximumX;
				double yGridSize = (double)height / MaximumY;

				double x = Attributes.Get(strings.X).Value;
				double y = Attributes.Get(strings.Y).Value;

				var finalCenterPoint = new Point(x * xGridSize, y * yGridSize);

				var idealPixelSize = Attributes.Get(strings.Size).Value / MaximumX * Math.Max(width, height);
				var scaleFactor = idealPixelSize / Math.Max(_pathSize.X, _pathSize.Y);
				var rotate = (Attributes.Get(strings.Rotation).Value / MaximumX) * 360;

				var strokeThickness = StrokeThickness(width, height);
				var strokeCurved = Attributes.Get(strings.StrokeCurved).Value > MaximumX / 2.0;

				return string.Format(CultureInfo.InvariantCulture,
					"<g transform=\"translate({2:0.###},{3:0.###}) rotate({0:0.###}) scale({1:0.###})\"><path d=\"{4}\" {5} {6} /></g>",
					rotate, scaleFactor, finalCenterPoint.X, finalCenterPoint.Y, _path, Color.ToSvgFillWithOpacity(),
					strokeThickness > 0
						? string.Format("stroke=\"#{0}\" stroke-width=\"{1:0.###}\" stroke-linejoin=\"{2}\"",
							StrokeColor.ToHexString(false), strokeThickness, strokeCurved ? "round" : "miter")
						: string.Empty);
			}
			catch (Exception)
			{
				MessageBox.Show(string.Format("Couldn't export custom overlay \"{0}\".", Name));
			}

			return string.Empty;
		}

		protected override IEnumerable<Shape> Thumbnail
		{
			get
			{
				const double thumbSize = 30.0;
				double scale = thumbSize / Math.Max(_pathSize.X, _pathSize.Y);
				return new List<Shape>
				       {
					       new Path
					       {
						       RenderTransform = new TransformGroup
						                         {
							                         Children = new TransformCollection
							                                    {
								                                    new ScaleTransform(scale, scale),
								                                    new TranslateTransform(thumbSize / 2, thumbSize / 2)
							                                    }
						                         },
						       Data = Geometry.Parse(_path),
						       SnapsToDevicePixels = true
					       }
				       };
			}
		}
		
		public OverlayPath Copy()
		{
			return new OverlayPath(_name, _path, _pathSize, MaximumX, MaximumY);
		}

		private double StrokeThickness(double canvasWidth, double canvasHeight)
		{
			return canvasWidth * Attributes.Get(strings.Stroke).Value / 32 / ScaleFactor(canvasWidth, canvasHeight) / MaximumX;
		}

		private double ScaleFactor(double canvasWidth, double canvasHeight)
		{
			var idealPixelSize = Attributes.Get(strings.Size).Value / MaximumX * Math.Max(canvasWidth, canvasHeight);
			return idealPixelSize / Math.Max(_pathSize.X, _pathSize.Y);
		}
	}
}
