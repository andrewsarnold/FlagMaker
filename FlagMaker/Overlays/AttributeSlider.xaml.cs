using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace FlagMaker.Overlays
{
	public partial class AttributeSlider
	{
		public event EventHandler ValueChanged;

		private bool _isDiscrete;

		public AttributeSlider(string name, bool isDiscrete, double value, int maximum)
		{
			InitializeComponent();

			LblName.Content = name;
			_isDiscrete = isDiscrete;
			chkDiscrete.IsChecked = _isDiscrete;
			LblValue.Content = value;
			Slider.Minimum = 0;
			Slider.Maximum = maximum;
			Slider.IsSnapToTickEnabled = isDiscrete;
			Slider.Value = value;
			Slider.TickFrequency = 1;
			Slider.TickPlacement = TickPlacement.BottomRight;
		}

		public int Maximum
		{
			get { return (int)Slider.Maximum; }
			set { Slider.Maximum = value; }
		}

		public double Value
		{
			get
			{
				return Slider.Value;
			}
			set
			{
				Slider.Value = value;
			}
	}

		private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			LblValue.Content = Slider.Value.ToString(_isDiscrete ? "0" : "0.00");

			if (ValueChanged != null)
			{
				ValueChanged(this, new EventArgs());
			}
		}

		private void CheckChanged(object sender, RoutedEventArgs e)
		{
			_isDiscrete = chkDiscrete.IsChecked ?? false;
			Slider.IsSnapToTickEnabled = _isDiscrete;

			if (_isDiscrete)
			{
				Slider.Value = (int) Math.Round(Slider.Value, 0);
			}
		}
	}
}
