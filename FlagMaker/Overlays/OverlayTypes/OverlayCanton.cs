using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayCanton : Overlay
	{
		public OverlayCanton(int maximum)
			: base(new List<Attribute>
			       {
				       new Attribute("Width", true, 1),
				       new Attribute("Height", true, 1)
			       }, maximum)
		{
		}

		public OverlayCanton(Color color, int width, int height, int maximum)
			: base(color, new List<Attribute>
			              {
				              new Attribute("Width", true, width),
				              new Attribute("Height", true, height)
			              }, maximum)
		{
		}

		public override string Name { get { return "canton"; } }

		public override IEnumerable<Shape> Thumbnail
		{
			get
			{
				return new Shape[]
				       {
					       new Rectangle
					       {
						       Width = 15,
						       Height = 10
					       }
				       };
			}
		}

		public override void Draw(Canvas canvas)
		{
			var rect = new Rectangle
						   {
							   Fill = new SolidColorBrush(Color),
							   Width = canvas.Width * (Attributes.Get("Width").Value / Maximum),
							   Height = canvas.Height * (Attributes.Get("Height").Value / Maximum),
							   SnapsToDevicePixels = true
						   };
			canvas.Children.Add(rect);
			Canvas.SetTop(rect, 0);
			Canvas.SetLeft(rect, 0);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("Width").Value = values[0];
			Attributes.Get("Height").Value = values[1];
		}

		public override string ExportSvg(int width, int height)
		{
			return string.Format("<rect width=\"{0}\" height=\"{1}\" x=\"0\" y=\"0\" fill=\"#{2}\" />",
				width * (Attributes.Get("Width").Value / Maximum),
				height * (Attributes.Get("Height").Value / Maximum),
				Color.ToHexString());
		}
	}
}