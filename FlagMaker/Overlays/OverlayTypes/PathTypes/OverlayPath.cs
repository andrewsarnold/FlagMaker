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
					   new Attribute(strings.Stroke, true, 0, true)
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
					   new Attribute(strings.Stroke, true, 0, true)
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

				var idealPixelSize = Attributes.Get(strings.Size).Value / MaximumX * Math.Max(canvas.Width, canvas.Height);

				var scaleFactor = idealPixelSize / _pathSize.X;

				var transformGroup = new TransformGroup();
				var rotateTransform = new RotateTransform((Attributes.Get(strings.Rotation).Value / MaximumX) * 360);
				transformGroup.Children.Add(rotateTransform);
				var scaleTransform = new ScaleTransform(scaleFactor, scaleFactor);
				transformGroup.Children.Add(scaleTransform);

				var path = new Path
				           {
					           Fill = new SolidColorBrush(Color),
					           RenderTransform = transformGroup,
					           Data = Geometry.Parse(_path),
					           SnapsToDevicePixels = true,
					           Stroke = new SolidColorBrush(StrokeColor),
							   StrokeThickness = canvas.Width * Attributes.Get(strings.Stroke).Value / 32 / scaleFactor / MaximumX,
					           StrokeLineJoin = PenLineJoin.Round
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
				var scaleFactor = idealPixelSize / _pathSize.X;
				var rotate = (Attributes.Get(strings.Rotation).Value / MaximumX) * 360;

				return string.Format(CultureInfo.InvariantCulture, "<g transform=\"translate({2:0.###},{3:0.###}) rotate({0:0.###}) scale({1:0.###})\"><path d=\"{4}\" {5} /></g>",
						rotate, scaleFactor, finalCenterPoint.X, finalCenterPoint.Y, _path, Color.ToSvgFillWithOpacity());
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
	}
}
