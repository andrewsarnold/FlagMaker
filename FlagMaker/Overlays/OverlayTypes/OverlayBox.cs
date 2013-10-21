using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	internal class OverlayBox : Overlay
	{
		public OverlayBox(int maximum)
			: base(new List<Attribute>
			       {
					   new Attribute("X", true, 1),
					   new Attribute("Y", true, 1),
				       new Attribute("Width", true, 1),
				       new Attribute("Height", true, 1)
			       }, maximum)
		{
		}

		public OverlayBox(Color color, int x, int y, int width, int height, int maximum)
			: base(color, new List<Attribute>
			              {
							  new Attribute("X", true, x),
							  new Attribute("Y", true, y),
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
			var width = canvas.Width * (Attributes.Get("Width").Value / Maximum);
			var height = Attributes.Get("Height").Value == 0
							 ? width
							 : canvas.Height * (Attributes.Get("Height").Value / Maximum);

			var rect = new Rectangle
						   {
							   Fill = new SolidColorBrush(Color),
							   Width = width,
							   Height = height,
							   SnapsToDevicePixels = true
						   };
			canvas.Children.Add(rect);
			Canvas.SetTop(rect, canvas.Height * (Attributes.Get("Y").Value / Maximum));
			Canvas.SetLeft(rect, canvas.Width * (Attributes.Get("X").Value / Maximum));
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("X").Value = values[0];
			Attributes.Get("Y").Value = values[1];
			Attributes.Get("Width").Value = values[2];
			Attributes.Get("Height").Value = values[3];
		}

		public override string ExportSvg(int width, int height)
		{
			var w = width * (Attributes.Get("Width").Value / Maximum);
			var h = Attributes.Get("Height").Value == 0
				? w
				: height * (Attributes.Get("Height").Value / Maximum);

			return string.Format("<rect width=\"{0}\" height=\"{1}\" x=\"{2}\" y=\"{3}\" fill=\"#{4}\" />",
				w, h,
				width * (Attributes.Get("X").Value / Maximum),
				height * (Attributes.Get("Y").Value / Maximum),
				Color.ToHexString());
		}
	}
}