using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlagMaker.Overlays
{
	public abstract class Overlay : IElement
	{
		public abstract string Name { get; }
		public abstract void Draw(Canvas canvas);
		public abstract void SetValues(List<double> values);
		public abstract string ExportSvg(int width, int height);

		public Color Color { get; set; }
		public List<Attribute> Attributes { get; set; }
		protected int MaximumX;
		protected int MaximumY;
		public abstract IEnumerable<Shape> Thumbnail { get; }

		protected Overlay(List<Attribute> attributes, int maximumX, int maximumY)
		{
			Color = Colors.Black;
			Attributes = attributes;
			SetMaximum(maximumX, maximumY);
		}

		protected Overlay(Color color, List<Attribute> attributes, int maximumX, int maximumY)
		{
			Color = color;
			Attributes = attributes;
			SetMaximum(maximumX, maximumY);
		}

		public string DisplayName
		{
			get
			{
				return Name.First().ToString().ToUpper() + String.Join("", Name.Skip(1));
			}
		}

		public void SetColors(List<Color> colors)
		{
			Color = colors[0];
		}

		public void SetMaximum(int maximumX, int maximumY)
		{
			MaximumX = maximumX;
			MaximumY = maximumY;
		}
	}
}
