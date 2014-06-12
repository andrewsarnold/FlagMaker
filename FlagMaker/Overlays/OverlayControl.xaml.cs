using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FlagMaker.Overlays.OverlayTypes.PathTypes;
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
		private bool _isFirst;

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

			if (!IsLoading)
			{
				OverlaySelect(this, null);
			}
		}

		public Overlay Overlay
		{
			get { return _overlay; }
			set
			{
				_overlay = value;
				var path = _overlay as OverlayPath;
				_btnOverlays.Content = _overlay.CanvasThumbnail();
				_btnOverlays.ToolTip = _overlay.DisplayName;

				// Save old slider/color values
				if (!_isFirst && !IsLoading)
				{
					var sliderValues = _pnlSliders.Children.OfType<AttributeSlider>().Select(s => s.Value).ToList();
					if (sliderValues.Count > 0)
					{
						for (int i = sliderValues.Count; i < _overlay.Attributes.Count; i++)
						{
							sliderValues.Add(0);
						}
						_overlay.SetValues(sliderValues);

						_overlay.SetColor(_overlayPicker.SelectedColor);

						if (path != null)
						{
							path.StrokeColor = _strokePicker.SelectedColor;
						}
					}
				}
				else if (path != null)
				{
					_strokePicker.SelectedColor = path.StrokeColor;
				}

				_overlayPicker.Visibility = (_overlay is OverlayFlag || _overlay is OverlayRepeater || _overlay is OverlayImage) ? Visibility.Collapsed : Visibility.Visible;
				_overlayPicker.SelectedColor = _overlay.Color;
				SetVisibilityButton();

				_pnlSliders.Children.Clear();
				foreach (var slider in _overlay.Attributes.Select(attribute => new AttributeSlider(attribute.Name, attribute.IsDiscrete, attribute.Value, attribute.UseMaxX ? _defaultMaximumX : _defaultMaximumY)))
				{
					slider.ValueChanged += OverlaySliderChanged;
					_pnlSliders.Children.Add(slider);
				}
				
				_strokePicker.Visibility = path != null ? Visibility.Visible : Visibility.Collapsed;

				_isFirst = false;
				IsLoading = false;
			}
		}

		public Color Color
		{
			get { return _overlayPicker.SelectedColor; }
			set { _overlayPicker.SelectedColor = value; }
		}

		public void SetMaximum(int maximumX, int maximumY)
		{
			_defaultMaximumX = maximumX;
			_defaultMaximumY = maximumY;

			Overlay.SetMaximum(maximumX, maximumY);

			var sliders = _pnlSliders.Children.OfType<AttributeSlider>().ToList();
			for (int i = 0; i < _overlay.Attributes.Count; i++)
			{
				var slider = sliders[i];
				var max = _overlay.Attributes[i].UseMaxX ? maximumX : maximumY;
				var newValue = slider.Value * ((double)max / slider.Maximum);
				slider._chkDiscrete.IsChecked = newValue % 1 == 0;
				slider.Maximum = max;
				slider.Value = newValue;
			}
		}

		private void SetUpColors(ObservableCollection<ColorItem> standardColors, ObservableCollection<ColorItem> availableColors, ObservableCollection<ColorItem> recentColors)
		{
			_overlayPicker.AvailableColors = availableColors;
			_overlayPicker.StandardColors = standardColors;
			_overlayPicker.RecentColors = recentColors;
			_overlayPicker.ShowRecentColors = true;
			_overlayPicker.SelectedColor = _overlayPicker.StandardColors[10].Color;
			_overlayPicker.SelectedColorChanged += (sender, args) => OverlayColorChanged();

			_strokePicker.AvailableColors = availableColors;
			_strokePicker.StandardColors = standardColors;
			_strokePicker.RecentColors = recentColors;
			_strokePicker.ShowRecentColors = true;
			_strokePicker.SelectedColor = _strokePicker.StandardColors[16].Color;
			_strokePicker.SelectedColorChanged += (sender, args) =>
			{
				((OverlayPath)_overlay).StrokeColor = _strokePicker.SelectedColor;
				Draw();
			};
		}

		private void OverlayColorChanged()
		{
			if (Overlay == null) return;

			Overlay.SetColor(_overlayPicker.SelectedColor);
			Draw();
		}

		private void OverlaySliderChanged(object sender, EventArgs e)
		{
			Overlay.SetValues(_pnlSliders.Children.OfType<AttributeSlider>().Select(s => s.Value).ToList());
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
			((Image)_btnVisibility.Content).Source = new BitmapImage(
				Overlay.IsEnabled
					? new Uri(@"..\Images\check_on.png", UriKind.Relative)
					: new Uri(@"..\Images\check_off.png", UriKind.Relative));
		}
	}
}
