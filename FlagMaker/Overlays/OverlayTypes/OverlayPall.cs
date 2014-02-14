using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayPall : Overlay
	{
		public OverlayPall(int maximumX, int maximumY)
			: base(new List<Attribute>
			       {
				       new Attribute(strings.X, true, 1, true),
				       new Attribute(strings.Width, true, 1, true)
			       }, maximumX, maximumY)
		{
		}

		public OverlayPall(Color color, double x, double size, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			             {
				             new Attribute(strings.X, true, x, true),
				             new Attribute(strings.Width, true, size, true)
			             }, maximumX, maximumY)
		{
		}

		public override string Name { get { return "pall"; } }

		public override void Draw(Canvas canvas)
		{
			var theWidth = (int)(canvas.Width / (Attributes.Get(strings.Width).Value + 3));
			var x = canvas.Width * (Attributes.Get(strings.X).Value / MaximumX);

			/*
			 * 01 0,0
			 * 02 width/2,0
			 * 03 x+width/2,canvas.Height/2-width/2
			 * 04 canvas.Width,canvas.Height/2-width/2
			 * 05 canvas.Width,canvas.Height/2+width/2
			 * 06 x+width/2,canvas.Height/2+width/2
			 * 07 width/2,canvas.Height
			 * 08 0,canvas.Height
			 * 09 0, canvas.Height-width/2
			 * 10 x-width/2,canvas.Height/2
			 * 11 0,width/2
			 */

			/*
			 * 01 0,0
			 * 02 {0},0
			 * 03 {1},{2}
			 * 04 {3},{2}
			 * 05 {3},{5}
			 * 06 {1},{5}
			 * 07 {0},{4}
			 * 08 0,{4}
			 * 09 0,{6}
			 * 10 {8},{7}
			 * 11 0,{0}
			 * 
			 * {0} width/2
			 * {1} x+width/2
			 * {2} canvas.Height/2-width/2
			 * {3} canvas.Width
			 * {4} canvas.Height
			 * {5} canvas.Height/2+width/2
			 * {6} canvas.Height-width/2
			 * {7} canvas.Height/2
			 * {8} x-width/2
			 */

			// needs redone with actual trigonometry

			var path = new Path
			{
				Fill = new SolidColorBrush(Color),
				Width = canvas.Width,
				Height = canvas.Height,
				Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture,
					"M 0,0 {0},0 {1},{2} {3},{2} {3},{5} {1},{5} {0},{4} 0,{4} 0,{6} {8},{7} 0,{0}",
					theWidth / 2,
					x + (double)theWidth / 3,
					canvas.Height / 2 - (double)theWidth / 3,
					canvas.Width,
					canvas.Height,
					canvas.Height / 2 + (double)theWidth / 3,
					canvas.Height - (double)theWidth / 2,
					canvas.Height / 2,
					x - (double)theWidth / 3)),
				SnapsToDevicePixels = true
			};
			canvas.Children.Add(path);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.X).Value = values[0];
			Attributes.Get(strings.Width).Value = values[1];
		}

		public override string ExportSvg(int width, int height)
		{
			var w = (int)(width / (Attributes.Get(strings.Width).Value + 3));
			var x = width * (Attributes.Get(strings.X).Value / MaximumX);

			return string.Format(CultureInfo.InvariantCulture, "<polygon points=\"0,0 {0:0.###},0 {1:0.###},{2:0.###} {3:0.###},{2:0.###} {3:0.###},{5:0.###} {1:0.###},{5:0.###} {0:0.###},{4:0.###} 0,{4:0.###} 0,{6:0.###} {8:0.###},{7:0.###} 0,{0:0.###}\" fill=\"#{9}\" />",
					w / 2,
					x + (double)w / 3,
					height / 2.0 - (double)w / 3,
					width,
					height,
					height / 2.0 + (double)w / 3,
					height - (double)w / 2,
					height / 2,
					x - (double)w / 3,
					Color.ToHexString());
		}

		protected override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
						   new Line
						   {
							   StrokeThickness = 5,
							   X1 = 0,
							   X2 = 15,
							   Y1 = 0,
							   Y2 = 15
						   },
						   new Line
						   {
							   StrokeThickness = 5,
							   X1 = 0,
							   X2 = 15,
							   Y1 = 30,
							   Y2 = 15
						   },
						   new Line
						   {
							   StrokeThickness = 5,
							   X1 = 15,
							   X2 = 30,
							   Y1 = 15,
							   Y2 = 15
						   },
				       };
			}
		}
	}
}