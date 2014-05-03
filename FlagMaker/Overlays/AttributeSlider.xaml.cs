using System;
using System.Globalization;
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
			ChkDiscrete.IsChecked = _isDiscrete;
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
			_isDiscrete = ChkDiscrete.IsChecked ?? false;
			Slider.IsSnapToTickEnabled = _isDiscrete;

			if (_isDiscrete)
			{
				Slider.Value = (int)Math.Round(Slider.Value, 0);
			}
		}

		private void Clicked(object sender, MouseButtonEventArgs e)
		{
			TxtValue.Visibility = Visibility.Visible;
			TxtValue.Text = Slider.Value.ToString(CultureInfo.InvariantCulture);
			TxtValue.SelectAll();
			TxtValue.Focus();
		}

		private void TxtValueKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					TxtValue.Visibility = Visibility.Hidden;
					if (TxtValue.Text.Contains("%"))
					{
						var stringVal = TxtValue.Text.Split('%')[0];
						double percentValue;
						if (double.TryParse(stringVal, out percentValue))
						{
							SetValueByFraction(percentValue / 100);
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
							if (den <= 0) return;

							SetValueByFraction(num / den);
						}
					}
					else
					{
						double fractionValue;
						if (double.TryParse(TxtValue.Text, out fractionValue))
						{
							ChkDiscrete.IsChecked = (fractionValue % 1 == 0);
							Slider.Value = fractionValue;
						}
					}
					break;
				case Key.Escape:
					TxtValue.Visibility = Visibility.Hidden;
					break;
				case Key.Down:
				case Key.Up:
					double value;
					if (double.TryParse(TxtValue.Text, out value))
					{
						value = value + (e.Key == Key.Up ? 0.01 : -0.01);

						if (value < 0.0) value = 0.0;
						if (value > Maximum) value = Maximum;

						ChkDiscrete.IsChecked = false;
						TxtValue.Text = value.ToString(CultureInfo.InvariantCulture);
						Slider.Value = value;
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

			ChkDiscrete.IsChecked = (result % 1 == 0);
			Slider.Value = result;
		}

		private void TxtValueLostFocus(object sender, RoutedEventArgs e)
		{
			TxtValue.Visibility = Visibility.Hidden;
		}
	}
}
