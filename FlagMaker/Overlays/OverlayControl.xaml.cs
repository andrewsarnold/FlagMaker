using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace FlagMaker.Overlays
{
	public partial class OverlayControl
	{
		private Overlay _overlay;
		private int _defaultMaximum;

		public event EventHandler OnRemove;
		public event EventHandler OnMoveUp;
		public event EventHandler OnMoveDown;
		public event EventHandler OnDraw;
		public event EventHandler OnClone;

		public OverlayControl(ObservableCollection<ColorItem> standardColors, ObservableCollection<ColorItem> availableColors, int defaultMaximum)
		{
			InitializeComponent();

			_defaultMaximum = defaultMaximum;

			SetUpColors(standardColors, availableColors);
			FillOverlayList();
		}

		public Overlay Overlay
		{
			get { return _overlay; }
			set
			{
				_overlay = value;

				_pnlSliders.Children.Clear();
				foreach (var attribute in _overlay.Attributes)
				{
					var slider = new AttributeSlider(attribute.Name, attribute.IsDiscrete, attribute.Value, _defaultMaximum);
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
				if (((ComboBoxItem)cmbOverlays.Items[i]).ToolTip.ToString().ToLower() == typename)
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

			if (Math.Abs(value - (int) value) > 0.01)
			{
				((AttributeSlider) _pnlSliders.Children[slider]).chkDiscrete.IsChecked = false;
			}

			((AttributeSlider) _pnlSliders.Children[slider]).Value = value;
		}

		public void SetMaximum(int max)
		{
			_defaultMaximum = max;
			Overlay.SetMaximum(max);

			foreach (var slider in _pnlSliders.Children.OfType<AttributeSlider>())
			{
				var ratio = (double)max / slider.Maximum;
				slider.Maximum = max;
				slider.Value *= ratio;
			}
		}

		private void SetUpColors(ObservableCollection<ColorItem> standardColors, ObservableCollection<ColorItem> availableColors)
		{
			_overlayPicker.AvailableColors = availableColors;
			_overlayPicker.StandardColors = standardColors;
			_overlayPicker.SelectedColor = _overlayPicker.StandardColors[10].Color;
			_overlayPicker.SelectedColorChanged += (sender, args) => OverlayColorChanged();
		}

		private void FillOverlayList()
		{
			foreach (var overlay in Overlay.GetOverlays())
			{
				var instance = (Overlay)Activator.CreateInstance(overlay, _defaultMaximum);

				var thumbnail = new Canvas
				{
					MinWidth = 30,
					MinHeight = 20,
				};

				IEnumerable<Shape> thumbs = instance.Thumbnail;
				foreach (var thumb in thumbs)
				{
					thumb.Stroke = Brushes.Black;
					thumb.Fill = Brushes.Black;
					thumbnail.Children.Add(thumb);
				}

				cmbOverlays.Items.Add(new ComboBoxItem
				                      {
					                      ToolTip = instance.DisplayName,
					                      Content = thumbnail,
										  Tag = instance,
										  Padding = new Thickness(2)
				                      });
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

			var tag = item.Tag as Overlay;
			if (tag != null)
			{
				var instance = (Overlay)Activator.CreateInstance(tag.GetType(), _defaultMaximum);
				Overlay = instance;
				Overlay.SetColors(new List<Color> { _overlayPicker.SelectedColor });

				Draw();
			}
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
