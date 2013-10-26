using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FlagMaker.Divisions;
using FlagMaker.Overlays;
using FlagMaker.Overlays.OverlayTypes.ShapeTypes;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace FlagMaker
{
	public partial class MainWindow
	{
		private int _ratioHeight, _ratioWidth;

		private Division _division;
		private ObservableCollection<ColorItem> _standardColors;
		private ObservableCollection<ColorItem> _availableColors;

		private bool _isLoading;
		private bool _showGrid;

		public MainWindow()
		{
			InitializeComponent();

			_showGrid = false;

			SetColorsAndSliders();
			LoadPresets();
		}

		#region Division

		private void DivisionColorChanged()
		{
			if (_isLoading) return;

			_division.SetColors(new List<Color>
			                    {
				                    divisionPicker1.SelectedColor,
				                    divisionPicker2.SelectedColor,
				                    divisionPicker3.SelectedColor
			                    });
			Draw();
		}

		private void DivisionSliderChanged()
		{
			if (_isLoading) return;

			divisionSliderLabel1.Text = divisionSlider1.Value.ToString();
			divisionSliderLabel2.Text = divisionSlider2.Value.ToString();
			divisionSliderLabel3.Text = divisionSlider3.Value.ToString();

			_division.SetValues(new List<double>
			                    {
				                    divisionSlider1.Value,
				                    divisionSlider2.Value,
				                    divisionSlider3.Value
			                    });
			Draw();
		}

		private void SetDivisionVisibility()
		{
			divisionPicker2.Visibility = Visibility.Collapsed;
			divisionPicker3.Visibility = Visibility.Collapsed;
			divisionPicker1.SelectedColor = _division.Colors[0];

			if (_division.Colors.Count > 1)
			{
				divisionPicker2.SelectedColor = _division.Colors[1];
				divisionPicker2.Visibility = Visibility.Visible;
				if (_division.Colors.Count > 2)
				{
					divisionPicker3.SelectedColor = _division.Colors[2];
					divisionPicker3.Visibility = Visibility.Visible;
				}
			}

			divisionSlider1.Visibility = Visibility.Collapsed;
			divisionSlider2.Visibility = Visibility.Collapsed;
			divisionSlider3.Visibility = Visibility.Collapsed;
			divisionSliderLabel1.Visibility = Visibility.Collapsed;
			divisionSliderLabel2.Visibility = Visibility.Collapsed;
			divisionSliderLabel3.Visibility = Visibility.Collapsed;

			if (_division.Values.Count > 0)
			{
				divisionSlider1.Value = _division.Values[0];
				divisionSlider1.Visibility = Visibility.Visible;
				divisionSliderLabel1.Text = _division.Values[0].ToString("#");
				divisionSliderLabel1.Visibility = Visibility.Visible;

				if (_division.Values.Count > 1)
				{
					divisionSlider2.Value = _division.Values[1];
					divisionSlider2.Visibility = Visibility.Visible;
					divisionSliderLabel2.Text = _division.Values[1].ToString("#");
					divisionSliderLabel2.Visibility = Visibility.Visible;

					if (_division.Values.Count > 2)
					{
						divisionSlider3.Value = _division.Values[2];
						divisionSlider3.Visibility = Visibility.Visible;
						divisionSliderLabel3.Text = _division.Values[2].ToString("#");
						divisionSliderLabel3.Visibility = Visibility.Visible;
					}
				}
			}
		}

		private void DivisionGridClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionGrid(divisionPicker1.SelectedColor, divisionPicker2.SelectedColor, (int)divisionSlider1.Value, (int)divisionSlider2.Value);
			SetDivisionVisibility();
			Draw();
		}

		private void DivisionFessesClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionFesses(divisionPicker1.SelectedColor, divisionPicker2.SelectedColor, divisionPicker3.SelectedColor, (int)divisionSlider1.Value, (int)divisionSlider2.Value, (int)divisionSlider3.Value);
			SetDivisionVisibility();
			Draw();
		}

		private void DivisionPalesClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionPales(divisionPicker1.SelectedColor, divisionPicker2.SelectedColor, divisionPicker3.SelectedColor, (int)divisionSlider1.Value, (int)divisionSlider2.Value, (int)divisionSlider3.Value);
			SetDivisionVisibility();
			Draw();
		}

		private void DivisionBendsForwardClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionBendsForward(divisionPicker1.SelectedColor, divisionPicker2.SelectedColor);
			SetDivisionVisibility();
			Draw();
		}

		private void DivisionBendsBackwardClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionBendsBackward(divisionPicker1.SelectedColor, divisionPicker2.SelectedColor);
			SetDivisionVisibility();
			Draw();
		}

		private void DivisionXClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionX(divisionPicker1.SelectedColor, divisionPicker2.SelectedColor);
			SetDivisionVisibility();
			Draw();
		}

		#endregion

		#region Overlays

		private void OverlayAdd(object sender, RoutedEventArgs e)
		{
			OverlayAdd(lstOverlays.Children.Count, null);
		}

		private void SetOverlayMargins()
		{
			for (int i = 0; i < lstOverlays.Children.Count - 1; i++)
			{
				((OverlayControl)lstOverlays.Children[i]).Margin = new Thickness(0, 0, 0, 20);
			}
		}

		private void Draw(object sender, EventArgs e)
		{
			Draw();
		}

		private void Remove(object sender, EventArgs e)
		{
			var controlToRemove = (OverlayControl)sender;
			lstOverlays.Children.Remove(controlToRemove);
			Draw();
		}

		private void MoveUp(object sender, EventArgs e)
		{
			var controlToMove = (OverlayControl)sender;
			int index = lstOverlays.Children.IndexOf(controlToMove);
			if (index == 0) return;

			var controls = new List<OverlayControl>();
			for (int i = 0; i < lstOverlays.Children.Count; i++)
			{
				if (i + 1 == index)
				{
					controls.Add((OverlayControl)lstOverlays.Children[i + 1]);
					controls.Add((OverlayControl)lstOverlays.Children[i]);
					i++;
				}
				else
				{
					controls.Add((OverlayControl)lstOverlays.Children[i]);
				}
			}

			lstOverlays.Children.Clear();
			foreach (var overlayControl in controls)
			{
				lstOverlays.Children.Add(overlayControl);
			}

			SetOverlayMargins();
			Draw();
		}

		private void MoveDown(object sender, EventArgs e)
		{
			var controlToMove = (OverlayControl)sender;
			int index = lstOverlays.Children.IndexOf(controlToMove);
			if (index == lstOverlays.Children.Count - 1) return;

			var controls = new List<OverlayControl>();
			for (int i = 0; i < lstOverlays.Children.Count; i++)
			{
				if (i == index)
				{
					controls.Add((OverlayControl)lstOverlays.Children[i + 1]);
					controls.Add((OverlayControl)lstOverlays.Children[i]);
					i++;
				}
				else
				{
					controls.Add((OverlayControl)lstOverlays.Children[i]);
				}
			}

			lstOverlays.Children.Clear();
			foreach (var overlayControl in controls)
			{
				lstOverlays.Children.Add(overlayControl);
			}

			SetOverlayMargins();
			Draw();
		}

		private void Clone(object sender, EventArgs e)
		{
			var controlToClone = (OverlayControl)sender;
			int index = lstOverlays.Children.IndexOf(controlToClone);
			OverlayAdd(index, controlToClone.Overlay);
		}

		private void OverlayAdd(int index, Overlay overlay)
		{
			var gridSize = ((Ratio)cmbGridSize.SelectedItem);
			var newOverlay = new OverlayControl(_standardColors, _availableColors, gridSize.Width, gridSize.Height);
			if (overlay != null)
			{
				newOverlay.SetType(overlay.Name);

				if (overlay is OverlayFlag)
				{
					newOverlay.Overlay = overlay;
				}

				newOverlay.Color = overlay.Color;
				for (int i = 0; i < overlay.Attributes.Count; i++)
				{
					newOverlay.SetSlider(i, overlay.Attributes[i].Value);
				}
			}

			newOverlay.OnDraw += Draw;
			newOverlay.OnRemove += Remove;
			newOverlay.OnMoveUp += MoveUp;
			newOverlay.OnMoveDown += MoveDown;
			newOverlay.OnClone += Clone;

			lstOverlays.Children.Insert(index, newOverlay);

			SetOverlayMargins();

			if (!_isLoading)
			{
				Draw();
			}
		}

		#endregion

		private void SetColorsAndSliders()
		{
			_standardColors = ColorFactory.Colors(Palette.FlagsOfAllNations, false);
			_availableColors = ColorFactory.Colors(Palette.FlagsOfTheWorld, true);

			divisionPicker1.AvailableColors = _availableColors;
			divisionPicker1.StandardColors = _standardColors;
			divisionPicker1.SelectedColor = divisionPicker1.StandardColors[1].Color;

			divisionPicker2.AvailableColors = _availableColors;
			divisionPicker2.StandardColors = _standardColors;
			divisionPicker2.SelectedColor = divisionPicker2.StandardColors[5].Color;

			divisionPicker3.AvailableColors = _availableColors;
			divisionPicker3.StandardColors = _standardColors;
			divisionPicker3.SelectedColor = divisionPicker3.StandardColors[8].Color;

			divisionPicker1.SelectedColorChanged += (sender, args) => DivisionColorChanged();
			divisionPicker2.SelectedColorChanged += (sender, args) => DivisionColorChanged();
			divisionPicker3.SelectedColorChanged += (sender, args) => DivisionColorChanged();
			divisionSlider1.ValueChanged += (sender, args) => DivisionSliderChanged();
			divisionSlider2.ValueChanged += (sender, args) => DivisionSliderChanged();
			divisionSlider3.ValueChanged += (sender, args) => DivisionSliderChanged();

			_division = new DivisionGrid(divisionPicker1.StandardColors[1].Color, divisionPicker2.StandardColors[5].Color, 2, 2);
			divisionPicker3.Visibility = Visibility.Collapsed;
			divisionSlider3.Visibility = Visibility.Collapsed;
			divisionSliderLabel3.Visibility = Visibility.Collapsed;

			SetRatio(3, 2);
			PlainPreset(2, 2);
		}

		private void SetRatio(int width, int height)
		{
			txtRatioHeight.Text = height.ToString();
			txtRatioWidth.Text = width.ToString();
			_ratioHeight = height;
			_ratioWidth = width;

			FillGridCombobox();
		}

		private void Draw()
		{
			canvas.Width = _ratioWidth * 200;
			canvas.Height = _ratioHeight * 200;
			canvas.Children.Clear();
			_division.Draw(canvas);

			foreach (var overlay in lstOverlays.Children)
			{
				((OverlayControl)overlay).Overlay.Draw(canvas);
			}

			DrawGrid();
		}

		private void DrawGrid()
		{
			canvasGrid.Children.Clear();

			if (!_showGrid) return;

			if (cmbGridSize.Items.Count == 0) return;

			var gridSize = ((Ratio)cmbGridSize.SelectedItem);

			var intervalX = canvas.Width / gridSize.Width;
			for (int x = 0; x <= gridSize.Width; x++)
			{
				var line = new Line
				{
					StrokeThickness = 3,
					X1 = 0,
					X2 = 0,
					Y1 = 0,
					Y2 = canvas.Height,
					SnapsToDevicePixels = false,
					Stroke = new SolidColorBrush(Colors.Silver)
				};
				canvasGrid.Children.Add(line);
				Canvas.SetTop(line, 0);
				Canvas.SetLeft(line, x * intervalX);
			}

			var intervalY = canvas.Height / gridSize.Height;
			for (int y = 0; y <= gridSize.Height; y++)
			{
				var line = new Line
				{
					StrokeThickness = 3,
					X1 = 0,
					X2 = canvas.Width,
					Y1 = 0,
					Y2 = 0,
					SnapsToDevicePixels = false,
					Stroke = new SolidColorBrush(Colors.Silver)
				};
				canvasGrid.Children.Add(line);
				Canvas.SetTop(line, y * intervalY);
				Canvas.SetLeft(line, 0);
			}
		}

		private void FillGridCombobox()
		{
			cmbGridSize.Items.Clear();
			for (int i = 1; i < 10; i++)
			{
				cmbGridSize.Items.Add(new Ratio(_ratioWidth * i, _ratioHeight * i));
			}
			cmbGridSize.SelectedIndex = 0;
		}

		private void RatioTextboxChanged(object sender, TextChangedEventArgs e)
		{
			int newHeight;
			int newWidth;

			if (!int.TryParse(txtRatioHeight.Text, out newHeight))
			{
				_ratioHeight = 1;
			}

			if (!int.TryParse(txtRatioWidth.Text, out newWidth))
			{
				_ratioWidth = 1;
			}

			if (newHeight < 1)
			{
				_ratioHeight = 1;
				txtRatioHeight.Text = "1";
			}
			else
			{
				_ratioHeight = newHeight;
			}

			if (newWidth < 1)
			{
				_ratioWidth = 1;
				txtRatioWidth.Text = "1";
			}
			else
			{
				_ratioWidth = newWidth;
			}

			Draw();
			FillGridCombobox();
		}

		private void GridSizeDropdownChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbGridSize.Items.Count == 0) return;

			var gridSize = ((Ratio)cmbGridSize.SelectedItem);
			int sliderMaxX = gridSize.Width;
			int sliderMaxY = gridSize.Height;
			int sliderMax = Math.Max(sliderMaxX, sliderMaxY);

			divisionSlider1.Maximum = sliderMax;
			divisionSlider2.Maximum = sliderMax;
			divisionSlider3.Maximum = sliderMax;

			_division.SetMaximum(sliderMaxX, sliderMaxY);

			foreach (var overlay in lstOverlays.Children)
			{
				((OverlayControl)overlay).SetMaximum(sliderMaxX, sliderMaxY);
			}

			Draw();
		}

		#region Export

		private void MenuExportPngClick(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog
			{
				FileName = "Untitled",
				DefaultExt = ".png",
				Filter = "PNG (*.png)|*.png"
			};

			bool? result = dlg.ShowDialog();
			if (!((bool)result)) return;

			ExportToPng(new Uri(dlg.FileName), canvas);
		}

		private void ExportToPng(Uri path, FrameworkElement surface)
		{
			if (path == null) return;

			// Save current canvas transform
			Transform transform = surface.LayoutTransform;
			// reset current transform (in case it is scaled or rotated)
			surface.LayoutTransform = null;

			// Get the size of canvas
			var size = new Size(surface.Width, surface.Height);
			// Measure and arrange the surface
			// VERY IMPORTANT
			surface.Measure(size);
			surface.Arrange(new Rect(size));

			// Create a render bitmap and push the surface to it
			var renderBitmap =
				new RenderTargetBitmap(
					(int)size.Width,
					(int)size.Height,
					96d,
					96d,
					PixelFormats.Pbgra32);
			renderBitmap.Render(surface);

			// Create a file stream for saving image
			using (var outStream = new FileStream(path.LocalPath, FileMode.Create))
			{
				// Use png encoder for our data
				var encoder = new PngBitmapEncoder();
				// push the rendered bitmap to it
				encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
				// save the data to the stream
				encoder.Save(outStream);
			}

			// Restore previously saved layout
			surface.LayoutTransform = transform;
		}

		private void MenuExportSvgClick(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog
			{
				FileName = "Untitled",
				DefaultExt = ".svg",
				Filter = "SVG (*.svg)|*.svg"
			};

			bool? result = dlg.ShowDialog();
			if (!((bool)result)) return;

			ExportToSvg(new Uri(dlg.FileName));
		}

		private void ExportToSvg(Uri path)
		{
			const int width = 600;
			var height = (int)(((double)_ratioHeight / _ratioWidth) * width);

			using (var sw = new StreamWriter(path.AbsolutePath))
			{
				sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
				sw.WriteLine("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");
				sw.WriteLine("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"{0}\" height=\"{1}\">", width, height);

				sw.WriteLine(_division.ExportSvg(width, height));

				foreach (var overlay in lstOverlays.Children.OfType<OverlayControl>().Select(o => o.Overlay))
				{
					try
					{
						sw.WriteLine(overlay.ExportSvg(width, height));
					}
					catch (NotImplementedException)
					{
						// Ignore overlays without SVG implementation
					}
				}

				sw.WriteLine("</svg>");
			}
		}

		#endregion

		#region Load / save

		private void MenuNewClick(object sender, RoutedEventArgs e)
		{
			divisionPicker1.SelectedColor = divisionPicker1.StandardColors[1].Color;
			divisionPicker2.SelectedColor = divisionPicker2.StandardColors[5].Color;
			lstOverlays.Children.Clear();
			PlainPreset(2, 2);
		}

		private void MenuSaveClick(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog
						  {
							  FileName = "Untitled",
							  DefaultExt = ".flag",
							  Filter = "Flag (*.flag)|*.flag|All files (*.*)|*.*"
						  };

			bool? result = dlg.ShowDialog();
			if (!((bool)result)) return;

			using (var sr = new StreamWriter(dlg.FileName, false, Encoding.Unicode))
			{
				sr.WriteLine("name={0}", Path.GetFileNameWithoutExtension(dlg.FileName));
				sr.WriteLine("ratio={0}:{1}", txtRatioHeight.Text, txtRatioWidth.Text);
				sr.WriteLine("gridSize={0}", cmbGridSize.SelectedItem);

				sr.WriteLine();

				sr.WriteLine("division");
				sr.WriteLine("type={0}", _division.Name);
				sr.WriteLine("color1={0}", divisionPicker1.SelectedColor.ToHexString());
				sr.WriteLine("color2={0}", divisionPicker2.SelectedColor.ToHexString());
				sr.WriteLine("color3={0}", divisionPicker3.SelectedColor.ToHexString());
				sr.WriteLine("size1={0}", divisionSlider1.Value);
				sr.WriteLine("size2={0}", divisionSlider2.Value);
				sr.WriteLine("size3={0}", divisionSlider3.Value);

				foreach (var overlay in from object child in lstOverlays.Children select ((OverlayControl)child))
				{
					sr.WriteLine();
					sr.WriteLine("overlay");
					sr.WriteLine("type={0}", overlay.Overlay.Name);
					if (overlay.Overlay.Name == "flag") sr.WriteLine("path={0}", ((OverlayFlag)overlay.Overlay).Path);
					sr.WriteLine("color={0}", overlay.Color.ToHexString());

					for (int i = 0; i < overlay.Overlay.Attributes.Count(); i++)
					{
						sr.WriteLine("size{0}={1}", i + 1, overlay.Overlay.Attributes[i].Value);
					}
				}
			}

			LoadPresets();
		}

		private void MenuOpenClick(object sender, RoutedEventArgs e)
		{
			var path = Flag.GetFlagPath();
			if (!string.IsNullOrWhiteSpace(path))
			{
				LoadFlagFromFile(path);
			}
		}

		private void LoadFlagFromFile(string filename)
		{
			var flag = Flag.LoadFromFile(filename);
			_isLoading = true;

			txtRatioHeight.Text = flag.Ratio.Height.ToString(CultureInfo.InvariantCulture);
			txtRatioWidth.Text = flag.Ratio.Width.ToString(CultureInfo.InvariantCulture);
			for (int i = 0; i < cmbGridSize.Items.Count; i++)
			{
				if (((Ratio)cmbGridSize.Items[i]).Width != flag.GridSize.Width) continue;
				cmbGridSize.SelectedIndex = i;
				break;
			}

			_division = flag.Division;
			SetDivisionVisibility();

			lstOverlays.Children.Clear();
			foreach (var overlay in flag.Overlays)
			{
				OverlayAdd(lstOverlays.Children.Count, overlay);
			}

			Draw();
			_isLoading = false;
		}

		#endregion

		#region Presets

		private void PresetChanged(object sender, SelectionChangedEventArgs e)
		{
			cmbPresets.SelectedIndex = -1;
		}

		private void PresetBlank(object sender, RoutedEventArgs e)
		{
			PlainPreset(1, 1);
		}

		private void PresetHorizontal(object sender, RoutedEventArgs e)
		{
			PlainPreset(1, 2);
		}

		private void PresetVertical(object sender, RoutedEventArgs e)
		{
			PlainPreset(2, 1);
		}

		private void PresetQuad(object sender, RoutedEventArgs e)
		{
			PlainPreset(2, 2);
		}

		private void PresetStripes(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < cmbGridSize.Items.Count; i++)
			{
				if (((Ratio)cmbGridSize.Items[i]).Width >= 7)
				{
					cmbGridSize.SelectedIndex = i;
					break;
				}
			}

			PlainPreset(1, 7);
		}

		private void PlainPreset(int slider1, int slider2)
		{
			divisionSlider1.Value = slider1;
			divisionSlider2.Value = slider2;
			divisionSlider3.Value = 1;
			DivisionGridClick(null, null);
		}

		private void LoadPresets()
		{
			try
			{
				var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "presets").Where(f => f.EndsWith(".flag"));

				var presets = new Dictionary<string, string>();
				foreach (var file in files)
				{
					var name = GetPresetFlagName(file);
					if (!string.IsNullOrWhiteSpace(name))
					{
						presets.Add(file, name);
					}
				}

				mnuWorldFlagPresets.Items.Clear();
				foreach (var menuItem in presets.OrderBy(p => p.Value).Select(preset => new MenuItem { Header = preset.Value, ToolTip = preset.Key }))
				{
					menuItem.Click += LoadPreset;
					mnuWorldFlagPresets.Items.Add(menuItem);
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Couldn't load presets. Check for a Presets folder in the application directory.");
			}
		}

		private void LoadPreset(object sender, RoutedEventArgs routedEventArgs)
		{
			var menuItem = (MenuItem)sender;
			LoadFlagFromFile(menuItem.ToolTip.ToString());
		}

		private string GetPresetFlagName(string filename)
		{
			using (var sr = new StreamReader(filename))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					if (line.StartsWith("name="))
					{
						return line.Split('=')[1];
					}
				}
			}

			return string.Empty;
		}

		#endregion

		private void MainWindowOnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			viewbox.MaxHeight = Height - 100;
		}

		private void GridOnChanged(object sender, RoutedEventArgs e)
		{
			_showGrid = chkGridOn.IsChecked ?? false;
			DrawGrid();
		}


	}
}
