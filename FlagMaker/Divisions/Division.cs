using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlagMaker.Divisions
{
	public abstract class Division : IElement
	{
		public abstract string Name { get; }

		public abstract void Draw(Canvas canvas);
		public abstract void SetColors(List<Color> colors);
		public abstract void SetValues(List<double> values);
		public abstract string ExportSvg(int width, int height);

		public void SetMaximum(int maximumX, int maximumY)
		{
		}
	}
}
