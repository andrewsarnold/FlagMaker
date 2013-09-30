using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlagMaker
{
	public interface IElement
	{
		string Name { get; }
		void Draw(Canvas canvas);
		void SetColors(List<Color> colors);
		void SetValues(List<double> values);
		void SetMaximum(int maximum);
		string ExportSvg(int width, int height);
	}
}