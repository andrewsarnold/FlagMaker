using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
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
				_btnOverlays.Content = _overlay.CanvasThumbnail();
				_btnOverlays.ToolTip = _overlay.DisplayName;

				_overlay.SetColors(new List<Color> { _overlayPicker.SelectedColor });

				// Save old slider/color values
				if (!_isFirst && !(_overlay is OverlayFlag || _overlay is OverlayImage))
				{
					var sliderValues = _pnlSliders.Children.OfType<AttributeSlider>().Select(s => s.Value).ToList();
					if (sliderValues.Count > 0)
					{
						for (int i = sliderValues.Count; i < _overlay.Attributes.Count; i++)
						{
							sliderValues.Add(0);
						}
						_overlay.SetValues(sliderValues);
					}
				}

				_overlayPicker.Visibility = (_overlay is OverlayFlag || _overlay is OverlayRepeater || _overlay is OverlayImage) ? Visibility.Collapsed : Visibility.Visible;

				_pnlSliders.Children.Clear();
				foreach (var slider in _overlay.Attributes.Select(attribute => new AttributeSlider(attribute.Name, attribute.IsDiscrete, attribute.Value, attribute.UseMaxX ? _defaultMaximumX : _defaultMaximumY)))
				{
					slider.ValueChanged += OverlaySliderChanged;
					_pnlSliders.Children.Add(slider);
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
			get { return _overlayPicker.SelectedColor; }
			set { _overlayPicker.SelectedColor = value; }
		}

		public void SetSlider(int slider, double value)
		{
			if (slider >= _pnlSliders.Children.Count) return;

			if (Math.Abs(value - (int)value) > 0.01)
			{
				((AttributeSlider)_pnlSliders.Children[slider])._chkDiscrete.IsChecked = false;
			}

			((AttributeSlider)_pnlSliders.Children[slider]).Value = value;
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
				var ratio = (double)max / slider.Maximum;
				slider.Maximum = max;
				slider.Value *= ratio;
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
		}

		private void OverlayColorChanged()
		{
			if (Overlay == null) return;

			Overlay.SetColors(new List<Color> { _overlayPicker.SelectedColor, Colors.Transparent });
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
			if (selector.SelectedOverlay == null) return;
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
	}
}
