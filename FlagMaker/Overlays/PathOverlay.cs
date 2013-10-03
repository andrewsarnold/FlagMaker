using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays
{
	public class PathOverlay : Overlay
	{
		private readonly string _name;
		private readonly Vector _pathSize;
		private readonly string _path;

		public PathOverlay(string name, string path, Vector pathSize, int maximum)
			: base(new List<Attribute>
			       {
				       new Attribute("X", true, 1),
				       new Attribute("Y", true, 1),
				       new Attribute("Size", true, 1),
				       new Attribute("Rotation", true, 0)
			       }, maximum)
		{
			_name = name;
			_path = path;
			_pathSize = pathSize;
		}

		public PathOverlay(Color color, string name, string path, Vector pathSize, int maximum)
			: base(color, new List<Attribute>
			       {
				       new Attribute("X", true, 1),
				       new Attribute("Y", true, 1),
				       new Attribute("Size", true, 1),
				       new Attribute("Rotation", true, 0)
			       }, maximum)
		{
			_name = name;
			_path = path;
			_pathSize = pathSize;
		}

		public override string Name
		{
			get { return _name; }
		}

		public override void Draw(Canvas canvas)
		{
			double extraNotch = Maximum % 2 == 0 ? 0 : 0.5;

			double xGridSize = canvas.Width / Maximum;
			double yGridSize = canvas.Height / Maximum;

			double x = Attributes.Get("X").Value;
			double y = Attributes.Get("Y").Value;

			var finalCenterPoint = new Point((x - extraNotch) * xGridSize, (y - extraNotch) * yGridSize);

			var idealPixelSize = Attributes.Get("Size").Value / Maximum * Math.Max(canvas.Width, canvas.Height);

			var scaleFactor = idealPixelSize / _pathSize.X;

			var transformGroup = new TransformGroup();
			var rotateTransform = new RotateTransform((Attributes.Get("Rotation").Value / (Maximum + 1)) * 360);
			transformGroup.Children.Add(rotateTransform);
			var scaleTransform = new ScaleTransform(scaleFactor, scaleFactor);
			transformGroup.Children.Add(scaleTransform);

			var path = new Path
			{
				Fill = new SolidColorBrush(Color),
				RenderTransform = transformGroup,
				Data = Geometry.Parse(_path),
				SnapsToDevicePixels = true
			};

			canvas.Children.Add(path);

			Canvas.SetLeft(path, finalCenterPoint.X);
			Canvas.SetTop(path, finalCenterPoint.Y);
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
			double extraNotch = Maximum % 2 == 0 ? 0 : 0.5;

			double xGridSize = width / Maximum;
			double yGridSize = height / Maximum;

			double x = Attributes.Get("X").Value;
			double y = Attributes.Get("Y").Value;

			var finalCenterPoint = new Point((x - extraNotch) * xGridSize, (y - extraNotch) * yGridSize);

			var idealPixelSize = Attributes.Get("Size").Value / Maximum * Math.Max(width, height);
			var scaleFactor = idealPixelSize / _pathSize.X;
			var rotate = (Attributes.Get("Rotation").Value / (Maximum + 1)) * 360;

			return string.Format("<g transform=\"translate({2},{3}) rotate({0}) scale({1})\"><path d=\"{4}\" fill=\"#{5}\" /></g>",
					rotate, scaleFactor, finalCenterPoint.X, finalCenterPoint.Y, _path, Color.ToHexString());
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				double scale = 20.0 / _pathSize.X;
				return new List<Shape>
				       {
					       new Path
					       {
						       RenderTransform = new TransformGroup
						                         {
							                         Children = new TransformCollection
							                                    {
								                                    new ScaleTransform(scale, scale),
								                                    new TranslateTransform(10, 10)
							                                    }
						                         },
						       Data = Geometry.Parse(_path),
						       SnapsToDevicePixels = true
					       }
				       };
			}
		}
	}
}
