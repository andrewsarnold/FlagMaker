using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;
using FlagMaker.Overlays.OverlayTypes.RepeaterTypes;
using FlagMaker.Overlays.OverlayTypes.ShapeTypes;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace FlagMaker.Overlays
{
	public partial class OverlayControl
	{
		private Overlay _overlay;
		private int _defaultMaximumX;
		private int _defaultMaximumY;

		public bool IsLoading;

		public event EventHandler OnRemove;
		public event EventHandler OnMoveUp;
		public event EventHandler OnMoveDown;
		public event EventHandler OnDraw;
		public event EventHandler OnClone;

		public OverlayControl(ObservableCollection<ColorItem> standardColors, ObservableCollection<ColorItem> availableColors, ObservableCollection<ColorItem> recentColors, int defaultMaximumX, int defaultMaximumY)
		{
			InitializeComponent();

			_defaultMaximumX = defaultMaximumX;
			_defaultMaximumY = defaultMaximumY;

			SetUpColors(standardColors, availableColors, recentColors);
			FillOverlayList();
		}

		public Overlay Overlay
		{
			get { return _overlay; }
			set
			{
				_overlay = value;

				_overlayPicker.Visibility = (_overlay is OverlayFlag || _overlay is OverlayRepeater) ? Visibility.Collapsed : Visibility.Visible;

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
			int theitem = -1;
			for (int i = 0; i < cmbOverlays.Items.Count; i++)
			{
				if (string.Equals(((ComboBoxItem)cmbOverlays.Items[i]).ToolTip.ToString(), typename, StringComparison.CurrentCultureIgnoreCase))
				{
					theitem = i;
					break;
				}
			}

			if (theitem >= 0) cmbOverlays.SelectedIndex = theitem;
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
				((AttributeSlider)_pnlSliders.Children[slider]).chkDiscrete.IsChecked = false;
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

		private void FillOverlayList()
		{
			foreach (var overlay in OverlayFactory.GetOverlayTypes())
			{
				var instance = OverlayFactory.GetInstance(overlay, _defaultMaximumX, _defaultMaximumY);

				var thumbnail = new Canvas
				{
					MinWidth = 30,
					MinHeight = 30
				};

				IEnumerable<Shape> thumbs = instance.Thumbnail;
				foreach (var thumb in thumbs)
				{
					if (thumb.Stroke == null) thumb.Stroke = Brushes.Black;
					if (thumb.Fill == null) thumb.Fill = Brushes.Black;
					thumbnail.Children.Add(thumb);
				}

				cmbOverlays.Items.Add(new ComboBoxItem
									  {
										  ToolTip = instance.DisplayName,
										  Content = thumbnail,
										  Tag = instance.Name,
										  Padding = new Thickness(2)
									  });
			}

			foreach (var overlayPath in OverlayFactory.CustomTypes.Select(p => p.Value))
			{
				try
				{
					var thumbnail = new Canvas
					{
						MinWidth = 30,
						MinHeight = 30
					};

					IEnumerable<Shape> thumbs = overlayPath.Thumbnail;
					foreach (var thumb in thumbs)
					{
						if (thumb.Stroke == null) thumb.Stroke = Brushes.Black;
						if (thumb.Fill == null) thumb.Fill = Brushes.Black;
						thumbnail.Children.Add(thumb);
					}

					cmbOverlays.Items.Add(new ComboBoxItem
					{
						ToolTip = overlayPath.DisplayName,
						Content = thumbnail,
						Tag = overlayPath.Name,
						Padding = new Thickness(2)
					});
				}
				catch (Exception)
				{
					MessageBox.Show(string.Format("Couldn't load custom overlay \"{0}\".", overlayPath.Name));
				}
			}

			cmbOverlays.SelectedIndex = 0;
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

		private void OverlayChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = (ComboBoxItem)((ComboBox)sender).SelectedItem;

			if (item == null) return;

			var tag = item.Tag as string;
			if (tag == null) return;

			if (tag == "flag")
			{
				if (!IsLoading)
				{
					string path = Flag.GetFlagPath();

					Flag flag;
					try
					{
						flag = Flag.LoadFromFile(path);
					}
					catch (Exception ex)
					{
						MessageBox.Show(string.Format("{0}\n{1} \"{2}\"", strings.CouldNotOpenFileError, strings.ErrorAtLine, ex.Message), "FlagMaker", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}

					Overlay = new OverlayFlag(flag, path, _defaultMaximumX, _defaultMaximumY);
				}
			}
			else
			{
				Overlay = OverlayFactory.GetInstance(tag, _defaultMaximumX, _defaultMaximumY);
			}
			Overlay.SetColors(new List<Color> { _overlayPicker.SelectedColor });

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
