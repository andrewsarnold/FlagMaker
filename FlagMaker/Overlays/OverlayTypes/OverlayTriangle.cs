using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays.OverlayTypes
{
	public class OverlayTriangle : Overlay
	{
		public OverlayTriangle(int maximumX, int maximumY)
			: base(new List<Attribute>
			{
				new Attribute("Size", true, 1, true)
			}, maximumX, maximumY)
		{
		}

		public OverlayTriangle(Color color, int size, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			{
				new Attribute("Size", true, size, true)
			}, maximumX, maximumY)
		{
		}

		public override string Name { get { return "triangle"; } }

		public override void Draw(Canvas canvas)
		{
			var width = canvas.Width * (Attributes.Get("Size").Value / MaximumX);

			var path = new Path
							{
								Fill = new SolidColorBrush(Color),
								Width = canvas.Width,
								Height = canvas.Height,
								Data = Geometry.Parse(string.Format("M 0,0 {0},{1} 0,{2}", width, canvas.Height / 2, canvas.Height)),
								SnapsToDevicePixels = true
							};
			canvas.Children.Add(path);
		}

		public override void SetValues(List<double> values)
		{
			Attributes.Get("Size").Value = values[0];
		}

		public override string ExportSvg(int width, int height)
		{
			return string.Format("<polygon points=\"0,0 0,{0} {1},{2}\" fill=\"#{3}\" />",
				height,
				width * (Attributes.Get("Size").Value / MaximumX),
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
							   Data = Geometry.Parse("M 0,0 15,10 0,20 0,0")
						   }
				       };
			}
		}
	}
}