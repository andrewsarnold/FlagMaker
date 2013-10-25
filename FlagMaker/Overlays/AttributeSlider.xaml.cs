using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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
			TxtValue.Visibility = Visibility.Hidden;
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

		private void Clicked(object sender, MouseButtonEventArgs e)
		{
			TxtValue.Visibility = Visibility.Visible;
			TxtValue.Text = Slider.Value.ToString();
			TxtValue.SelectAll();
			TxtValue.Focus();
		}

		private void TxtValueKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				TxtValue.Visibility = Visibility.Hidden;

				double value;
				if (double.TryParse(TxtValue.Text, out value))
				{
					chkDiscrete.IsChecked = ((int) value == value);
					Slider.Value = value;
				}
			}
			else if (e.Key == Key.Escape)
			{
				TxtValue.Visibility = Visibility.Hidden;
			}
		}

		private void TxtValueLostFocus(object sender, RoutedEventArgs e)
		{
			TxtValue.Visibility = Visibility.Hidden;
		}
	}
}
