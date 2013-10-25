using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Divisions;

namespace FlagMaker.Overlays
{
	public class OverlayFlag : Overlay
	{
		private string _name;
		private Division _division;
		private IEnumerable<Overlay> _overlays;

		private double _x;
		private double _y;
		private double _width;
		private double _height;

		public OverlayFlag(string name, int maximumX, int maximumY)
			: base(new List<Attribute>
			{
				new Attribute("X", true, 1, true),
				new Attribute("Y", true, 1, false),
				new Attribute("Width", true, 1, true),
				new Attribute("Height", true, 0, true)
			}, maximumX, maximumY)
		{
			_name = name;
		}

		public OverlayFlag(Color color, string name, int maximumX, int maximumY)
			: base(color, new List<Attribute>
			{
				new Attribute("X", true, 1, true),
				new Attribute("Y", true, 1, false),
				new Attribute("Width", true, 1, true),
				new Attribute("Height", true, 0, true)
			}, maximumX, maximumY)
		{
			_name = name;
		}

		public override string Name
		{
			get { return _name; }
		}

		public override void Draw(Canvas canvas)
		{
			throw new NotImplementedException();
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
			return string.Empty;
		}

		public override IEnumerable<Shape> Thumbnail
		{
			get { return new List<Shape>(); }
		}
	}
}
