﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		protected int Maximum;
		public abstract IEnumerable<Shape> Thumbnail { get; }

		protected Overlay(List<Attribute> attributes, int maximum)
		{
			Color = Colors.Black;
			Attributes = attributes;
			SetMaximum(maximum);
		}

		protected Overlay(Color color, List<Attribute> attributes, int maximum)
		{
			Color = color;
			Attributes = attributes;
			SetMaximum(maximum);
		}

		public string DisplayName
		{
			get
			{
				return Name.First().ToString().ToUpper() + String.Join("", Name.Skip(1));
			}
		}

		public static IEnumerable<Type> GetOverlays()
		{
			return Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.Namespace != null && t.Namespace.Contains("OverlayTypes")).OrderBy(t => t.Name);
		}

		public void SetColors(List<Color> colors)
		{
			Color = colors[0];
		}

		public void SetMaximum(int maximum)
		{
			Maximum = maximum;
		}
	}
}