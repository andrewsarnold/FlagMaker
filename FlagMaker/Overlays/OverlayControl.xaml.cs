using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FlagMaker.Overlays.OverlayTypes.RepeaterTypes;
using FlagMaker.Overlays.OverlayTypes.ShapeTypes;
using Xceed.Wpf.Toolkit;

namespace FlagMaker.Overlays
{
	public partial class OverlayControl
	{
		private Overlay _overlay;
		private int _defaultMaximumX;
		private int _defaultMaximumY;
		private readonly bool _isFirst;

		public bool IsLoading;
		public bool WasCanceled { get; private set; }

		public event EventHandler OnRemove;
		public event EventHandler OnMoveUp;
		public event EventHandler OnMoveDown;
		public event EventHandler OnDraw;
		public event EventHandler OnClone;

		public OverlayControl(ObservableCollection<ColorItem> standardColors, ObservableCollection<ColorItem> availableColors, ObservableCollection<ColorItem> recentColors, int defaultMaximumX, int defaultMaximumY, bool isLoading)
		{
			InitializeComponent();

			IsLoading = isLoading;
			_defaultMaximumX = defaultMaximumX;
			_defaultMaximumY = defaultMaximumY;
			_isFirst = true;

			SetUpColors(standardColors, availableColors, recentColors);
			Overlay = OverlayFactory.GetDefaultOverlay(_defaultMaximumX, _defaultMaximumY);

			if (!IsLoading)
			{
				OverlaySelect(this, null);
			}

			_isFirst = false;
		}

		public Overlay Overlay
		{
			get { return _overlay; }
			set
			{
				_overlay = value;
				BtnOverlays.Content = _overlay.CanvasThumbnail();
				BtnOverlays.ToolTip = _overlay.DisplayName;

				_overlay.SetColors(new List<Color> { OverlayPicker.SelectedColor });

				// Save old slider/color values
				if (!_isFirst && !(_overlay is OverlayFlag || _overlay is OverlayImage))
				{
					var sliderValues = PnlSliders.Children.OfType<AttributeSlider>().Select(s => s.Value).ToList();
					if (sliderValues.Count > 0)
					{
						for (int i = sliderValues.Count; i < _overlay.Attributes.Count; i++)
						{
							sliderValues.Add(0);
						}
						_overlay.SetValues(sliderValues);
					}
				}

				OverlayPicker.Visibility = (_overlay is OverlayFlag || _overlay is OverlayRepeater || _overlay is OverlayImage) ? Visibility.Collapsed : Visibility.Visible;
				SetVisibilityButton();

				PnlSliders.Children.Clear();
				foreach (var slider in _overlay.Attributes.Select(attribute => new AttributeSlider(attribute.Name, attribute.IsDiscrete, attribute.Value, attribute.UseMaxX ? _defaultMaximumX : _defaultMaximumY)))
				{
					slider.ValueChanged += OverlaySliderChanged;
					PnlSliders.Children.Add(slider);
				}
			}
		}

		public void SetType(string typename)
		{
			var type = OverlayFactory.GetOverlayType(typename);
			Overlay = OverlayFactory.GetInstance(type, _defaultMaximumX, _defaultMaximumY, typename);
		}

		public Color Color
		{
			get { return OverlayPicker.SelectedColor; }
			set { OverlayPicker.SelectedColor = value; }
		}

		public void SetSlider(int slider, double value)
		{
			if (slider >= PnlSliders.Children.Count) return;

			if (Math.Abs(value - (int)value) > 0.01)
			{
				((AttributeSlider)PnlSliders.Children[slider]).ChkDiscrete.IsChecked = false;
			}

			((AttributeSlider)PnlSliders.Children[slider]).Value = value;
		}

		public void SetMaximum(int maximumX, int maximumY)
		{
			_defaultMaximumX = maximumX;
			_defaultMaximumY = maximumY;

			Overlay.SetMaximum(maximumX, maximumY);

			var sliders = PnlSliders.Children.OfType<AttributeSlider>().ToList();
			for (int i = 0; i < _overlay.Attributes.Count; i++)
			{
				var slider = sliders[i];
				var max = _overlay.Attributes[i].UseMaxX ? maximumX : maximumY;
				var newValue = slider.Value * ((double)max / slider.Maximum);
				slider.ChkDiscrete.IsChecked = newValue % 1 == 0;
				slider.Maximum = max;
				slider.Value = newValue;
			}
		}

		private void SetUpColors(ObservableCollection<ColorItem> standardColors, ObservableCollection<ColorItem> availableColors, ObservableCollection<ColorItem> recentColors)
		{
			OverlayPicker.AvailableColors = availableColors;
			OverlayPicker.StandardColors = standardColors;
			OverlayPicker.RecentColors = recentColors;
			OverlayPicker.ShowRecentColors = true;
			OverlayPicker.SelectedColor = OverlayPicker.StandardColors[10].Color;
			OverlayPicker.SelectedColorChanged += (sender, args) => OverlayColorChanged();
		}

		private void OverlayColorChanged()
		{
			if (Overlay == null) return;

			Overlay.SetColors(new List<Color> { OverlayPicker.SelectedColor, Colors.Transparent });
			Draw();
		}

		private void OverlaySliderChanged(object sender, EventArgs e)
		{
			Overlay.SetValues(PnlSliders.Children.OfType<AttributeSlider>().Select(s => s.Value).ToList());
			Draw();
		}

		private void OverlaySelect(object sender, EventArgs e)
		{
			var selector = new OverlaySelector(_defaultMaximumX, _defaultMaximumY)
			{
				Owner = Application.Current.MainWindow
			};

			selector.ShowDialog();
			if (selector.SelectedOverlay == null)
			{
				WasCanceled = true;
				return;
			}

			Overlay = selector.SelectedOverlay;
			if (!IsLoading) Draw();
		}

		private void Draw()
		{
			if (OnDraw != null)
			{
				OnDraw(null, new EventArgs());
			}
		}

		private void Remove(object sender, EventArgs e)
		{
			if (OnRemove != null)
			{
				OnRemove(this, new EventArgs());
			}
		}

		private void MoveUp(object sender, EventArgs e)
		{
			if (OnMoveUp != null)
			{
				OnMoveUp(this, new EventArgs());
			}
		}

		private void MoveDown(object sender, EventArgs e)
		{
			if (OnMoveDown != null)
			{
				OnMoveDown(this, new EventArgs());
			}
		}

		private void Clone(object sender, EventArgs e)
		{
			if (OnClone != null)
			{
				OnClone(this, new EventArgs());
			}
		}

		private void SetVisibility(object sender, RoutedEventArgs e)
		{
			Overlay.IsEnabled = !Overlay.IsEnabled;
			SetVisibilityButton();
			Draw();
		}

		private void SetVisibilityButton()
		{
			((Image)BtnVisibility.Content).Source = new BitmapImage(
				Overlay.IsEnabled
					? new Uri(@"..\Images\check_on.png", UriKind.Relative)
					: new Uri(@"..\Images\check_off.png", UriKind.Relative));
		}
	}
}
