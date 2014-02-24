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

		public Color Color { get; private set; }
		public List<Attribute> Attributes { get; private set; }
		protected int MaximumX;
		protected int MaximumY;
		protected abstract IEnumerable<Shape> Thumbnail { get; }

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

		public Canvas CanvasThumbnail()
		{
			var thumbnail = new Canvas
			{
				MinWidth = 30,
				MinHeight = 30
			};

			foreach (var thumb in Thumbnail)
			{
				if (thumb.Stroke == null) thumb.Stroke = Brushes.Black;
				if (thumb.Fill == null) thumb.Fill = Brushes.Black;
				thumbnail.Children.Add(thumb);
			}

			return thumbnail;
		}
	}
}
