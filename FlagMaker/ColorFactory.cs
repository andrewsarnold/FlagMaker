using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace FlagMaker
{
	public static class ColorFactory
	{
		public static ObservableCollection<ColorItem> Colors(Palette palette, bool sort)
		{
			IEnumerable<ColorItem> unsorted;
			switch (palette)
			{
				case Palette.FlagsOfAllNations:
					unsorted = FlagsOfAllNations;
					break;
				default:
					unsorted = FlagsOfTheWorld;
					break;
			}

			return sort
				? new ObservableCollection<ColorItem>(unsorted.OrderBy(c => c.Color.Hue()))
				: new ObservableCollection<ColorItem>(unsorted);
		}

		public static IEnumerable<ColorItem> FlagsOfTheWorld
		{
			get
			{
				return new List<ColorItem>
				       {
					       new ColorItem(Color.FromRgb(255, 102, 102), "Very light red"),
					       new ColorItem(Color.FromRgb(255, 51, 51), "Light red"),
					       new ColorItem(Color.FromRgb(255, 0, 0), "Red"),
					       new ColorItem(Color.FromRgb(204, 0, 0), "Dark red"),
					       new ColorItem(Color.FromRgb(153, 0, 0), "Very dark red"),
					       new ColorItem(Color.FromRgb(153, 102, 0), "Light brown"),
					       new ColorItem(Color.FromRgb(102, 51, 0), "Brown"),
					       new ColorItem(Color.FromRgb(51, 0, 0), "Dark brown"),
					       new ColorItem(Color.FromRgb(255, 153, 0), "Light orange"),
					       new ColorItem(Color.FromRgb(255, 102, 0), "Orange"),
					       new ColorItem(Color.FromRgb(255, 204, 51), "Gold"),
					       new ColorItem(Color.FromRgb(255, 255, 204), "Very light yellow"),
					       new ColorItem(Color.FromRgb(255, 255, 153), "Light yellow"),
					       new ColorItem(Color.FromRgb(255, 255, 0), "Yellow"),
					       new ColorItem(Color.FromRgb(255, 204, 0), "Dark yellow"),
					       new ColorItem(Color.FromRgb(0, 255, 51), "Very light green"),
					       new ColorItem(Color.FromRgb(0, 204, 0), "Light green"),
					       new ColorItem(Color.FromRgb(0, 153, 0), "Green"),
					       new ColorItem(Color.FromRgb(0, 102, 0), "Dark green"),
					       new ColorItem(Color.FromRgb(0, 51, 0), "Very dark green"),
					       new ColorItem(Color.FromRgb(51, 204, 255), "Very light blue"),
					       new ColorItem(Color.FromRgb(51, 153, 255), "Light blue"),
					       new ColorItem(Color.FromRgb(0, 0, 255), "Blue"),
					       new ColorItem(Color.FromRgb(0, 0, 204), "Dark blue"),
					       new ColorItem(Color.FromRgb(0, 0, 153), "Very dark blue"),
					       new ColorItem(Color.FromRgb(102, 0, 153), "Purple"),
					       new ColorItem(Color.FromRgb(255, 255, 255), "White"),
					       new ColorItem(Color.FromRgb(204, 204, 204), "Light grey"),
					       new ColorItem(Color.FromRgb(153, 153, 153), "Grey"),
					       new ColorItem(Color.FromRgb(102, 102, 102), "Dark grey"),
					       new ColorItem(Color.FromRgb(51, 51, 51), "Very dark grey"),
					       new ColorItem(Color.FromRgb(0, 0, 0), "Black"),
				       };
			}
		}

		public static IEnumerable<ColorItem> FlagsOfAllNations
		{
			get
			{
				return new List<ColorItem>
				       {
					       new ColorItem(Color.FromRgb(93, 53, 39), "Red brown"),
					       new ColorItem(Color.FromRgb(130, 36, 51), "Crimson"),
					       new ColorItem(Color.FromRgb(198, 12, 48), "Red"),
					       new ColorItem(Color.FromRgb(255, 99, 25), "Orange"),
					       new ColorItem(Color.FromRgb(253, 200, 47), "Deep yellow"),
					       new ColorItem(Color.FromRgb(254, 221, 0), "Yellow"),
					       new ColorItem(Color.FromRgb(51, 115, 33), "Green"),
					       new ColorItem(Color.FromRgb(20, 77, 41), "Tartan green"),
					       new ColorItem(Color.FromRgb(40, 78, 54), "Dark green"),
					       new ColorItem(Color.FromRgb(99, 153, 171), "Azure blue"),
					       new ColorItem(Color.FromRgb(0, 101, 189), "Intermediate blue"),
					       new ColorItem(Color.FromRgb(0, 57, 166), "Heraldic blue"),
					       new ColorItem(Color.FromRgb(0, 38, 100), "Royal blue"),
					       new ColorItem(Color.FromRgb(0, 33, 71), "Navy blue"),
					       new ColorItem(Color.FromRgb(0, 0, 0), "Black"),
					       new ColorItem(Color.FromRgb(141, 129, 123), "Grey"),
					       new ColorItem(Color.FromRgb(255, 255, 255), "White")
				       };
			}
		}
	}

	public enum Palette
	{
		FlagsOfAllNations,
		FlagsOfTheWorld
	}
}
