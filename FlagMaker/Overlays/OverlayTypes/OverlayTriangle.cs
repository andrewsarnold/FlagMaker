using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;

namespace FlagMaker.Overlays.OverlayTypes
{
	public class OverlayTriangle : Overlay
	{
		public OverlayTriangle(int maximumX, int maximumY)
			: base(new List<Attribute>
			{
				new Attribute(strings.Width, true, 1, true),
				new Attribute(strings.Height, true, 1, false)
			}, maximumX, maximumY)
		{
		}

		public OverlayTriangle(Color color, double width, double height, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			{
				new Attribute(strings.Width, true, width, true),
				new Attribute(strings.Height, true, height, false)
			}, maximumX, maximumY)
		{
		}

		public override string Name { get { return "triangle"; } }

		public override void Draw(Canvas canvas)
		{
			var width = canvas.Width * (Attributes.Get(strings.Width).Value / MaximumX);
			var margin = (canvas.Height - canvas.Height * (Attributes.Get(strings.Height).Value / MaximumY)) / 2;

			var path = new Path
							{
								Fill = new SolidColorBrush(Color),
								Width = canvas.Width,
								Height = canvas.Height,
								Data = Geometry.Parse(string.Format(CultureInfo.InvariantCulture, "M 0,{0} {1},{2} 0,{3}", margin, width, canvas.Height / 2, canvas.Height - margin)),
								SnapsToDevicePixels = true
							};
			canvas.Children.Add(path);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get(strings.Width).Value = values[0];
			Attributes.Get(strings.Height).Value = values[1];
		}

		public override string ExportSvg(int width, int height)
		{
			var margin = (height - height * (Attributes.Get(strings.Height).Value / MaximumY)) / 2;

			return string.Format(CultureInfo.InvariantCulture, "<polygon points=\"0,{0} 0,{1} {2},{3}\" fill=\"#{4}\" />",
				margin,
				height - margin,
				width * (Attributes.Get(strings.Width).Value / MaximumX),
				height / 2,
				Color.ToHexString());
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new List<Shape>
				       {
						   new Path
						   {
							   Data = Geometry.Parse("M 0,0 15,15 0,30 0,0")
						   }
				       };
			}
		}
	}
}