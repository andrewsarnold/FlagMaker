using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FlagMaker
{
	public partial class ExportPng
	{
		private readonly Ratio _ratio;
		private readonly bool _constrain;
		private int _width;
		private int _height;
		private bool _update;
		
		public ExportPng(Ratio ratio, bool constrain)
		{
			InitializeComponent();

			const int multiplier = 100;
			_ratio = ratio;
			PngWidth = ratio.Width * multiplier;
			PngHeight = ratio.Height * multiplier;
			_update = true;
			_constrain = constrain;
		}

		public int PngWidth
		{
			get { return _width; }
			private set
			{
				_width = value;
				_txtWidth.Text = _width.ToString(CultureInfo.InvariantCulture);
			}
		}

		public int PngHeight
		{
			get { return _height; }
			private set
			{
				_height = value;
				_txtHeight.Text = _height.ToString(CultureInfo.InvariantCulture);
			}
		}

		private void WidthChanged(object sender, TextChangedEventArgs e)
		{
			if (!_update) return;
			_update = false;
			int newWidth;

			if (int.TryParse(_txtWidth.Text, out newWidth))
			{
				_width = newWidth;
				if (_constrain)
				{
					PngHeight = (int)((_ratio.Height / (double)_ratio.Width) * _width);
				}
			}
			else
			{
				_txtWidth.Text = _width.ToString(CultureInfo.InvariantCulture);
			}
			_update = true;
		}

		private void HeightChanged(object sender, TextChangedEventArgs e)
		{
			if (!_update) return;
			_update = false;
			int newHeight;

			if (int.TryParse(_txtHeight.Text, out newHeight))
			{
				_height = newHeight;
				if (_constrain)
				{
					PngWidth = (int)((_ratio.Width / (double)_ratio.Height) * _height);
				}
			}
			else
			{
				_txtHeight.Text = _height.ToString(CultureInfo.InvariantCulture);
			}
			_update = true;
		}

		private void OkClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
