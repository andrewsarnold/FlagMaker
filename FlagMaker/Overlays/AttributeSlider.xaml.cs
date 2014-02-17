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

			_lblName.Content = name;
			_lblName.ToolTip = name;
			_isDiscrete = isDiscrete && (value % 1 == 0);
			_chkDiscrete.IsChecked = _isDiscrete;
			_lblValue.Content = value;
			_slider.Minimum = 0;
			_slider.Maximum = maximum;
			_slider.IsSnapToTickEnabled = _isDiscrete;
			_slider.Value = value;
			_slider.TickFrequency = 1;
			_slider.TickPlacement = TickPlacement.BottomRight;
			_txtValue.Visibility = Visibility.Hidden;
		}

		public int Maximum
		{
			get { return (int)_slider.Maximum; }
			set { _slider.Maximum = value; }
		}

		public double Value
		{
			get
			{
				return _slider.Value;
			}
			set
			{
				_slider.Value = value;
			}
		}

		private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_lblValue.Content = _slider.Value.ToString(_isDiscrete ? "0" : "0.00");

			if (ValueChanged != null)
			{
				ValueChanged(this, new EventArgs());
			}
		}

		private void CheckChanged(object sender, RoutedEventArgs e)
		{
			_isDiscrete = _chkDiscrete.IsChecked ?? false;
			_slider.IsSnapToTickEnabled = _isDiscrete;

			if (_isDiscrete)
			{
				_slider.Value = (int)Math.Round(_slider.Value, 0);
			}
		}

		private void Clicked(object sender, MouseButtonEventArgs e)
		{
			_txtValue.Visibility = Visibility.Visible;
			_txtValue.Text = _slider.Value.ToString();
			_txtValue.SelectAll();
			_txtValue.Focus();
		}

		private void TxtValueKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					_txtValue.Visibility = Visibility.Hidden;
					if (_txtValue.Text.Contains("%"))
					{
						var stringVal = _txtValue.Text.Split('%')[0];
						double percentValue;
						if (double.TryParse(stringVal, out percentValue))
						{
							SetValueByFraction(percentValue / 100);
						}
					}
					else if (_txtValue.Text.Contains("/"))
					{
						var fraction = _txtValue.Text.Split('/');

						if (fraction.Length != 2)
						{
							return;
						}

						var numerator = fraction[0];
						var denominator = fraction[1];
						double num, den;
						if (double.TryParse(numerator, out num) &&
							double.TryParse(denominator, out den))
						{
							SetValueByFraction(num / den);
						}
					}
					else
					{
						double fractionValue;
						if (double.TryParse(_txtValue.Text, out fractionValue))
						{
							_chkDiscrete.IsChecked = (fractionValue % 1 == 0);
							_slider.Value = fractionValue;
						}
					}
					break;
				case Key.Escape:
					_txtValue.Visibility = Visibility.Hidden;
					break;
				case Key.Down:
				case Key.Up:
					double value;
					if (double.TryParse(_txtValue.Text, out value))
					{
						value = value + (e.Key == Key.Up ? 0.01 : -0.01);
						_chkDiscrete.IsChecked = false;
						_txtValue.Text = value.ToString();
						_slider.Value = value;
					}
					break;
			}
		}

		private void SetValueByFraction(double fraction)
		{
			if (fraction > 1) fraction = 1;
			if (fraction < 0) fraction = 0;
			var result = fraction * Maximum;
			result = Math.Round(result, 3);

			_chkDiscrete.IsChecked = (result % 1 == 0);
			_slider.Value = result;
		}

		private void TxtValueLostFocus(object sender, RoutedEventArgs e)
		{
			_txtValue.Visibility = Visibility.Hidden;
		}
	}
}
