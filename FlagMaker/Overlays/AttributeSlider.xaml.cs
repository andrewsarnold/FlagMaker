using System;
using System.Linq;
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
			LblName.ToolTip = name;
			_isDiscrete = isDiscrete && (value % 1 == 0);
			chkDiscrete.IsChecked = _isDiscrete;
			LblValue.Content = value;
			Slider.Minimum = 0;
			Slider.Maximum = maximum;
			Slider.IsSnapToTickEnabled = _isDiscrete;
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
				Slider.Value = (int)Math.Round(Slider.Value, 0);
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

				if (TxtValue.Text.Contains("%"))
				{
					var stringVal = TxtValue.Text.Split('%')[0];
					double value;
					if (double.TryParse(stringVal, out value))
					{
						SetValueByFraction(value / 100);
					}
				}
				else if (TxtValue.Text.Contains("/"))
				{
					var fraction = TxtValue.Text.Split('/');
					
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
						SetValueByFraction(num/den);
					}
				}
				else
				{
					double value;
					if (double.TryParse(TxtValue.Text, out value))
					{
						chkDiscrete.IsChecked = ((int)value == value);
						Slider.Value = value;
					}
				}
			}
			else if (e.Key == Key.Escape)
			{
				TxtValue.Visibility = Visibility.Hidden;
			}
			else if (e.Key == Key.Up || e.Key == Key.Down)
			{
				double value;
				if (double.TryParse(TxtValue.Text, out value))
				{
					value = value + (e.Key == Key.Up ? 0.01 : -0.01);
					chkDiscrete.IsChecked = false;
					TxtValue.Text = value.ToString();
					Slider.Value = value;
				}
			}
		}

		private void SetValueByFraction(double fraction)
		{
			if (fraction > 1) fraction = 1;
			if (fraction < 0) fraction = 0;
			var result = fraction * Maximum;
			result = Math.Round(result, 3);

			chkDiscrete.IsChecked = ((int)result == result);
			Slider.Value = result;
		}

		private void TxtValueLostFocus(object sender, RoutedEventArgs e)
		{
			TxtValue.Visibility = Visibility.Hidden;
		}
	}
}
