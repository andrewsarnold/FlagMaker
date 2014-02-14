using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes.ShapeTypes
{
	internal class OverlayImage : OverlayShape
	{
		private string _path;
		private readonly string _directory;
		private BitmapImage _bitmap;

		public string Path
		{
			get { return _path; }
			private set
			{
				_path = value;

				if (string.IsNullOrWhiteSpace(_path))
				{
					return;
				}

				if (!System.IO.Path.IsPathRooted(_path))
				{
					_path = string.Format("{0}\\{1}", _directory, _path);
				}

				_bitmap = new BitmapImage();
				_bitmap.BeginInit();
				_bitmap.UriSource = new Uri(_path, UriKind.Absolute);
				_bitmap.CacheOption = BitmapCacheOption.OnLoad;
				_bitmap.EndInit();
			}
		}

		public OverlayImage(int maximumX, int maximumY)
			: base(maximumX, maximumY)
		{
			Path = string.Empty;
		}

		public OverlayImage(string path, string directory, int maximumX, int maximumY)
			: base(maximumX, maximumY)
		{
			_directory = directory;
			Path = path;
		}

		public OverlayImage(string path, double x, double y, double width, double height, int maximumX, int maximumY)
			: base(Colors.Transparent, x, y, width, height, maximumX, maximumY)
		{
			Path = path;
		}

		public override string Name { get { return "image"; } }

		public override void Draw(Canvas canvas)
		{
			var width = canvas.Width * Attributes.Get(strings.Width).Value / MaximumX;
			var height = canvas.Height * Attributes.Get(strings.Height).Value / MaximumY;
			if (height == 0)
			{
				var ratio = _bitmap.Height / _bitmap.Width;
				height = width * ratio;
			}

			var image = new Image
						{
							Source = _bitmap,
							Width = width,
							Height = height,
							Stretch = Stretch.Fill
						};

			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

			canvas.Children.Add(image);
			Canvas.SetLeft(image, (canvas.Width * (Attributes.Get(strings.X).Value / MaximumX)) - width / 2);
			Canvas.SetTop(image, (canvas.Height * (Attributes.Get(strings.Y).Value / MaximumY)) - height / 2);
		}

		public override string ExportSvg(int width, int height)
		{
			return string.Empty;
		}

		protected override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
					       new Rectangle
					       {
						       Width = 25,
						       Height = 30,
						       Stroke = Brushes.Black,
						       StrokeThickness = 3,
						       Fill = Brushes.Transparent,
						       Margin = new Thickness(2.5, 0, 0, 0)
					       },
					       new Rectangle
					       {
						       Width = 8,
						       Height = 8,
						       Margin = new Thickness(7, 5, 0, 0)
					       },
					       new Ellipse
					       {
						       Width = 8,
						       Height = 8,
						       Margin = new Thickness(15, 11, 0, 0)
					       },
					       new Polygon
					       {
						       Points = new PointCollection
						                {
							                new Point(8, 25),
							                new Point(12, 17),
							                new Point(16, 25),
						                }
					       }
				       };
			}
		}
	}
}
