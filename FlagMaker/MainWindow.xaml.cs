using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using FlagMaker.Divisions;
using FlagMaker.Localization;
using FlagMaker.Overlays;
using FlagMaker.Overlays.OverlayTypes.PathTypes;
using FlagMaker.Overlays.OverlayTypes.ShapeTypes;
using FlagMaker.Properties;
using FlagMaker.RandomFlag;
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
		private ObservableCollection<ColorItem> _recentColors;

		private bool _isLoading;
		private bool _showGrid;
		private int _texture;

		private Flag Flag
		{
			get
			{
				return new Flag("flag", new Ratio(_ratioWidth, _ratioHeight), (Ratio)CmbGridSize.SelectedItem, _division,
					LstOverlays.Children.OfType<OverlayControl>().Select(c => c.Overlay));
			}
		}

		private readonly string _headerText;
		private string _filename;
		private bool _isUnsaved;

		public static readonly RoutedCommand NewCommand = new RoutedCommand();
		public static readonly RoutedCommand SaveCommand = new RoutedCommand();
		public static readonly RoutedCommand SaveAsCommand = new RoutedCommand();
		public static readonly RoutedCommand OpenCommand = new RoutedCommand();

		public MainWindow()
		{
			Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(Settings.Default.Culture);
			InitializeComponent();
			SetLanguages();

			var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			_headerText = string.Format(" - FlagMaker {0}.{1}{2}", version.Major, version.Minor, version.Build > 0 ? string.Format(".{0}", version.Build) : string.Empty);
			SetTitle();

			_showGrid = false;

			SetColorsAndSliders();
			LoadPresets();
			OverlayFactory.SetUpTypeMap();

			try
			{
				OverlayFactory.FillCustomOverlays();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void SetLanguages()
		{
			foreach (var menuItem in new List<CultureInfo>
			{
				new CultureInfo("en-US"),
				new CultureInfo("es-ES"),
				new CultureInfo("fr-FR"),
				new CultureInfo("ru-RU")
			}.Select(lang => new MenuItem
			{
				Header = lang.TextInfo.ToTitleCase(lang.Parent.NativeName),
				Tag = lang.Name,
				IsChecked = Settings.Default.Culture == lang.Name
			}))
			{
				menuItem.Click += LanguageChange;
				MnuLanguage.Items.Add(menuItem);
			}
		}

		private void SetTitle()
		{
			Title = string.Format("{0}{1}{2}",
				string.IsNullOrWhiteSpace(_filename)
					? strings.Untitled
					: Path.GetFileNameWithoutExtension(_filename),
					_isUnsaved ? "*" : string.Empty,
					_headerText);
		}

		#region Division

		private void DivisionColorChanged()
		{
			if (_isLoading) return;

			_division.SetColors(new List<Color>
			                    {
				                    DivisionPicker1.SelectedColor,
				                    DivisionPicker2.SelectedColor,
				                    DivisionPicker3.SelectedColor
			                    });
			Draw();
			SetAsUnsaved();
		}

		private void DivisionSliderChanged()
		{
			if (_isLoading) return;

			DivisionSliderLabel1.Text = DivisionSlider1.Value.ToString(CultureInfo.InvariantCulture);
			DivisionSliderLabel2.Text = DivisionSlider2.Value.ToString(CultureInfo.InvariantCulture);
			DivisionSliderLabel3.Text = DivisionSlider3.Value.ToString(CultureInfo.InvariantCulture);

			_division.SetValues(new List<double>
			                    {
				                    DivisionSlider1.Value,
				                    DivisionSlider2.Value,
				                    DivisionSlider3.Value
			                    });
			Draw();
			SetAsUnsaved();
		}

		private void SetDivisionVisibility()
		{
			DivisionPicker2.Visibility = Visibility.Collapsed;
			DivisionPicker3.Visibility = Visibility.Collapsed;
			DivisionPicker1.SelectedColor = _division.Colors[0];

			if (_division.Colors.Count > 1)
			{
				DivisionPicker2.SelectedColor = _division.Colors[1];
				DivisionPicker2.Visibility = Visibility.Visible;
				if (_division.Colors.Count > 2)
				{
					DivisionPicker3.SelectedColor = _division.Colors[2];
					DivisionPicker3.Visibility = Visibility.Visible;
				}
			}

			DivisionSlider1.Visibility = Visibility.Collapsed;
			DivisionSlider2.Visibility = Visibility.Collapsed;
			DivisionSlider3.Visibility = Visibility.Collapsed;
			DivisionSliderLabel1.Visibility = Visibility.Collapsed;
			DivisionSliderLabel2.Visibility = Visibility.Collapsed;
			DivisionSliderLabel3.Visibility = Visibility.Collapsed;

			if (_division.Values.Count > 0)
			{
				DivisionSlider1.Value = _division.Values[0];
				DivisionSlider1.Visibility = Visibility.Visible;
				DivisionSliderLabel1.Text = _division.Values[0].ToString("#");
				DivisionSliderLabel1.Visibility = Visibility.Visible;

				if (_division.Values.Count > 1)
				{
					DivisionSlider2.Value = _division.Values[1];
					DivisionSlider2.Visibility = Visibility.Visible;
					DivisionSliderLabel2.Text = _division.Values[1].ToString("#");
					DivisionSliderLabel2.Visibility = Visibility.Visible;

					if (_division.Values.Count > 2)
					{
						DivisionSlider3.Value = _division.Values[2];
						DivisionSlider3.Visibility = Visibility.Visible;
						DivisionSliderLabel3.Text = _division.Values[2].ToString("#");
						DivisionSliderLabel3.Visibility = Visibility.Visible;
					}
				}
			}
		}

		private void DivisionGridClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionGrid(DivisionPicker1.SelectedColor, DivisionPicker2.SelectedColor, (int)DivisionSlider1.Value, (int)DivisionSlider2.Value);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		private void DivisionFessesClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionFesses(DivisionPicker1.SelectedColor, DivisionPicker2.SelectedColor, DivisionPicker3.SelectedColor, (int)DivisionSlider1.Value, (int)DivisionSlider2.Value, (int)DivisionSlider3.Value);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		private void DivisionPalesClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionPales(DivisionPicker1.SelectedColor, DivisionPicker2.SelectedColor, DivisionPicker3.SelectedColor, (int)DivisionSlider1.Value, (int)DivisionSlider2.Value, (int)DivisionSlider3.Value);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		private void DivisionBendsForwardClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionBendsForward(DivisionPicker1.SelectedColor, DivisionPicker2.SelectedColor);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		private void DivisionBendsBackwardClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionBendsBackward(DivisionPicker1.SelectedColor, DivisionPicker2.SelectedColor);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		private void DivisionXClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionX(DivisionPicker1.SelectedColor, DivisionPicker2.SelectedColor);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		#endregion

		#region Overlays

		private void OverlayAdd(object sender, RoutedEventArgs e)
		{
			OverlayAdd(LstOverlays.Children.Count, null, false);
		}

		private void SetOverlayMargins()
		{
			for (int i = 0; i < LstOverlays.Children.Count - 1; i++)
			{
				((OverlayControl)LstOverlays.Children[i]).Margin = new Thickness(0, 0, 0, 20);
			}
		}

		private void Draw(object sender, EventArgs e)
		{
			Draw();
			SetAsUnsaved();
		}

		private void Remove(object sender, EventArgs e)
		{
			var controlToRemove = (OverlayControl)sender;
			LstOverlays.Children.Remove(controlToRemove);
			Draw();
			SetAsUnsaved();
		}

		private void MoveUp(object sender, EventArgs e)
		{
			var controlToMove = (OverlayControl)sender;
			int index = LstOverlays.Children.IndexOf(controlToMove);
			if (index == 0) return;

			var controls = new List<OverlayControl>();
			for (int i = 0; i < LstOverlays.Children.Count; i++)
			{
				if (i + 1 == index)
				{
					controls.Add((OverlayControl)LstOverlays.Children[i + 1]);
					controls.Add((OverlayControl)LstOverlays.Children[i]);
					i++;
				}
				else
				{
					controls.Add((OverlayControl)LstOverlays.Children[i]);
				}
			}

			LstOverlays.Children.Clear();
			foreach (var overlayControl in controls)
			{
				LstOverlays.Children.Add(overlayControl);
			}

			SetOverlayMargins();
			Draw();
			SetAsUnsaved();
		}

		private void MoveDown(object sender, EventArgs e)
		{
			var controlToMove = (OverlayControl)sender;
			int index = LstOverlays.Children.IndexOf(controlToMove);
			if (index == LstOverlays.Children.Count - 1) return;

			var controls = new List<OverlayControl>();
			for (int i = 0; i < LstOverlays.Children.Count; i++)
			{
				if (i == index)
				{
					controls.Add((OverlayControl)LstOverlays.Children[i + 1]);
					controls.Add((OverlayControl)LstOverlays.Children[i]);
					i++;
				}
				else
				{
					controls.Add((OverlayControl)LstOverlays.Children[i]);
				}
			}

			LstOverlays.Children.Clear();
			foreach (var overlayControl in controls)
			{
				LstOverlays.Children.Add(overlayControl);
			}

			SetOverlayMargins();
			Draw();
			SetAsUnsaved();
		}

		private void Clone(object sender, EventArgs e)
		{
			var controlToClone = (OverlayControl)sender;
			int index = LstOverlays.Children.IndexOf(controlToClone);

			var type = controlToClone.Overlay.GetType();
			var copy = OverlayFactory.GetInstance(type, 1, 1, controlToClone.Overlay.Name);

			for (int i = 0; i < controlToClone.Overlay.Attributes.Count; i++)
			{
				copy.Attributes[i].Value = controlToClone.Overlay.Attributes[i].Value;
				copy.Attributes[i].IsDiscrete = controlToClone.Overlay.Attributes[i].IsDiscrete;
			}

			copy.SetColor(controlToClone.Overlay.Color);

			if (type.IsSubclassOf(typeof(OverlayPath)))
			{
				((OverlayPath)copy).StrokeColor = ((OverlayPath)controlToClone.Overlay).StrokeColor;
			}
			else if (type == typeof (OverlayFlag))
			{
				((OverlayFlag)copy).Flag = ((OverlayFlag)controlToClone.Overlay).Flag;
			}

			var gridSize = ((Ratio)CmbGridSize.SelectedItem);
			copy.SetMaximum(gridSize.Width, gridSize.Height);

			OverlayAdd(index + 1, copy, true);
		}

		private void OverlayAdd(int index, Overlay overlay, bool isLoading)
		{
			var gridSize = ((Ratio)CmbGridSize.SelectedItem);
			var control = new OverlayControl(_standardColors, _availableColors, _recentColors, gridSize.Width, gridSize.Height, isLoading);

			if (control.WasCanceled)
			{
				return;
			}

			if (overlay != null)
			{
				control.Overlay = overlay;
			}

			control.OnDraw += Draw;
			control.OnRemove += Remove;
			control.OnMoveUp += MoveUp;
			control.OnMoveDown += MoveDown;
			control.OnClone += Clone;

			LstOverlays.Children.Insert(index, control);

			SetOverlayMargins();

			if (!_isLoading)
			{
				Draw();
				SetAsUnsaved();
			}
		}

		#endregion

		#region Colors

		private void SetColorsAndSliders()
		{
			_standardColors = ColorFactory.Colors(Palette.FlagsOfAllNations, false);
			_availableColors = ColorFactory.Colors(Palette.FlagsOfTheWorld, false);
			_recentColors = new ObservableCollection<ColorItem>();

			DivisionPicker1.AvailableColors = _availableColors;
			DivisionPicker1.StandardColors = _standardColors;
			DivisionPicker1.SelectedColor = DivisionPicker1.StandardColors[1].Color;
			DivisionPicker1.ShowRecentColors = true;
			DivisionPicker1.RecentColors = _recentColors;

			DivisionPicker2.AvailableColors = _availableColors;
			DivisionPicker2.StandardColors = _standardColors;
			DivisionPicker2.SelectedColor = DivisionPicker2.StandardColors[5].Color;
			DivisionPicker2.ShowRecentColors = true;
			DivisionPicker2.RecentColors = _recentColors;

			DivisionPicker3.AvailableColors = _availableColors;
			DivisionPicker3.StandardColors = _standardColors;
			DivisionPicker3.SelectedColor = DivisionPicker3.StandardColors[8].Color;
			DivisionPicker3.ShowRecentColors = true;
			DivisionPicker3.RecentColors = _recentColors;

			DivisionPicker1.SelectedColorChanged += (sender, args) => DivisionColorChanged();
			DivisionPicker2.SelectedColorChanged += (sender, args) => DivisionColorChanged();
			DivisionPicker3.SelectedColorChanged += (sender, args) => DivisionColorChanged();
			DivisionSlider1.ValueChanged += (sender, args) => DivisionSliderChanged();
			DivisionSlider2.ValueChanged += (sender, args) => DivisionSliderChanged();
			DivisionSlider3.ValueChanged += (sender, args) => DivisionSliderChanged();

			New();
		}

		private void SetUsedColorPalettes()
		{
			_recentColors.Clear();

			var colors = Flag.ColorsUsed();
			foreach (var color in colors)
			{
				_recentColors.Add(new ColorItem(color, null));
			}
		}

		private void ShuffleColors(object sender, RoutedEventArgs e)
		{
			bool skip2 = _division is DivisionGrid && DivisionSlider1.Value == 1 && DivisionSlider2.Value == 1;
			var colors = Flag.ColorsUsed();

			DivisionPicker1.SelectedColor = GetNextColor(DivisionPicker1.SelectedColor, colors);
			if (!skip2) DivisionPicker2.SelectedColor = GetNextColor(DivisionPicker2.SelectedColor, colors);
			if (DivisionPicker3.Visibility == Visibility.Visible)
				DivisionPicker3.SelectedColor = GetNextColor(DivisionPicker3.SelectedColor, colors);

			foreach (var overlay in LstOverlays.Children.Cast<OverlayControl>())
			{
				overlay.Color = GetNextColor(overlay.Color, colors);
			}
		}

		private static Color GetNextColor(Color c, List<Color> colors)
		{
			var index = colors.FindIndex(i => i == c);
			return colors[((index + 1) % colors.Count)];
		}

		#endregion

		#region Grid

		private void SetRatio(int width, int height)
		{
			TxtRatioHeight.Text = height.ToString(CultureInfo.InvariantCulture);
			TxtRatioWidth.Text = width.ToString(CultureInfo.InvariantCulture);
			_ratioHeight = height;
			_ratioWidth = width;

			FillGridCombobox();
		}

		private void GridOnChanged(object sender, RoutedEventArgs e)
		{
			_showGrid = !_showGrid;

			if (_showGrid)
			{
				BtnGrid.Background = new SolidColorBrush(Colors.LightSkyBlue);
			}
			else
			{
				BtnGrid.ClearValue(BackgroundProperty);
			}

			DrawGrid();
		}

		private void DrawGrid()
		{
			CanvasGrid.Children.Clear();

			if (!_showGrid) return;

			if (CmbGridSize.Items.Count == 0) return;

			var gridSize = ((Ratio)CmbGridSize.SelectedItem);

			var intervalX = Canvas.Width / gridSize.Width;
			for (int x = 0; x <= gridSize.Width; x++)
			{
				var line = new Line
				{
					StrokeThickness = 3,
					X1 = 0,
					X2 = 0,
					Y1 = 0,
					Y2 = Canvas.Height,
					SnapsToDevicePixels = false,
					Stroke = new SolidColorBrush(Colors.Silver)
				};
				CanvasGrid.Children.Add(line);
				Canvas.SetTop(line, 0);
				Canvas.SetLeft(line, x * intervalX);
			}

			var intervalY = Canvas.Height / gridSize.Height;
			for (int y = 0; y <= gridSize.Height; y++)
			{
				var line = new Line
				{
					StrokeThickness = 3,
					X1 = 0,
					X2 = Canvas.Width,
					Y1 = 0,
					Y2 = 0,
					SnapsToDevicePixels = false,
					Stroke = new SolidColorBrush(Colors.Silver)
				};
				CanvasGrid.Children.Add(line);
				Canvas.SetTop(line, y * intervalY);
				Canvas.SetLeft(line, 0);
			}
		}

		private void FillGridCombobox()
		{
			CmbGridSize.Items.Clear();
			for (int i = 1; i <= 20; i++)
			{
				CmbGridSize.Items.Add(new Ratio(_ratioWidth * i, _ratioHeight * i));
			}
			CmbGridSize.SelectedIndex = 0;
		}

		private void RatioTextboxChanged(object sender, TextChangedEventArgs e)
		{
			int newHeight;
			int newWidth;

			if (!int.TryParse(TxtRatioHeight.Text, out newHeight))
			{
				_ratioHeight = 1;
			}

			if (!int.TryParse(TxtRatioWidth.Text, out newWidth))
			{
				_ratioWidth = 1;
			}

			if (newHeight < 1)
			{
				_ratioHeight = 1;
				TxtRatioHeight.Text = "1";
			}
			else
			{
				_ratioHeight = newHeight;
			}

			if (newWidth < 1)
			{
				_ratioWidth = 1;
				TxtRatioWidth.Text = "1";
			}
			else
			{
				_ratioWidth = newWidth;
			}

			if (!_isLoading)
			{
				Draw();
				SetAsUnsaved();
			}

			FillGridCombobox();
		}

		private void GridSizeDropdownChanged(object sender, SelectionChangedEventArgs e)
		{
			if (CmbGridSize.Items.Count == 0) return;

			var gridSize = ((Ratio)CmbGridSize.SelectedItem);
			int sliderMaxX = gridSize.Width;
			int sliderMaxY = gridSize.Height;
			int sliderMax = Math.Max(sliderMaxX, sliderMaxY);

			DivisionSlider1.Maximum = sliderMax;
			DivisionSlider2.Maximum = sliderMax;
			DivisionSlider3.Maximum = sliderMax;

			foreach (var overlay in LstOverlays.Children)
			{
				((OverlayControl)overlay).SetMaximum(sliderMaxX, sliderMaxY);
			}

			if (!_isLoading)
			{
				Draw();
				SetAsUnsaved();
			}
		}

		#endregion

		private void SetAsUnsaved()
		{
			_isUnsaved = true;
			SetTitle();
		}

		private void Draw()
		{
			Canvas.Width = _ratioWidth * 200;
			Canvas.Height = _ratioHeight * 200;
			Flag.Draw(Canvas);
			DrawTexture(Canvas);
			DrawGrid();
			SetUsedColorPalettes();
		}

		private void DrawTexture(Canvas canvas)
		{
			if (_texture == 0) return;

			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(string.Format(@"pack://application:,,,/Images/texture/{0}.png", _texture));
			bitmap.CacheOption = BitmapCacheOption.OnLoad;
			bitmap.EndInit();

			var image = new Image
			{
				Source = bitmap,
				Width = canvas.Width,
				Height = canvas.Height,
				Stretch = Stretch.Fill
			};

			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

			canvas.Children.Add(image);
			Canvas.SetLeft(image, 0);
			Canvas.SetTop(image, 0);
		}

		private void ToggleTexture(object sender, RoutedEventArgs e)
		{
			_texture = (_texture + 1) % 6;
			Draw();
		}

		#region Export

		private void MenuExportPngClick(object sender, RoutedEventArgs e)
		{
			var dialog = new ExportPng(new Ratio(_ratioWidth, _ratioHeight)) { Owner = this };
			if (!(dialog.ShowDialog() ?? false)) return;

			var dimensions = new Size(dialog.PngWidth, dialog.PngHeight);

			var dlg = new SaveFileDialog
			{
				FileName = string.IsNullOrWhiteSpace(_filename) ? strings.Untitled : Path.GetFileNameWithoutExtension(_filename),
				DefaultExt = ".png",
				Filter = "PNG (*.png)|*.png"
			};

			if (!(dlg.ShowDialog() ?? false)) return;

			// Create a full copy of the canvas so the
			// scaling of the existing canvas and
			// grid don't ge messed up
			string gridXaml = XamlWriter.Save(Canvas);
			var stringReader = new StringReader(gridXaml);
			XmlReader xmlReader = XmlReader.Create(stringReader);
			var newGrid = (Canvas)XamlReader.Load(xmlReader);

			ExportToPng(dlg.FileName, newGrid, dimensions);
		}

		private static void ExportToPng(string path, FrameworkElement surface, Size newSize)
		{
			if (path == null) return;

			// Get original size of canvas
			var size = new Size(surface.Width, surface.Height);

			// Appy scaling for desired PNG size
			surface.LayoutTransform = new ScaleTransform(newSize.Width / size.Width, newSize.Height / size.Height);

			surface.Measure(size);
			surface.Arrange(new Rect(newSize));

			var renderBitmap =
				new RenderTargetBitmap(
					(int)newSize.Width,
					(int)newSize.Height,
					96d,
					96d,
					PixelFormats.Pbgra32);
			renderBitmap.Render(surface);

			using (var outStream = new FileStream(path, FileMode.Create))
			{
				var encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
				encoder.Save(outStream);
			}
		}

		private void MenuExportSvgClick(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog
			{
				FileName = string.IsNullOrWhiteSpace(_filename) ? strings.Untitled : Path.GetFileNameWithoutExtension(_filename),
				DefaultExt = ".svg",
				Filter = "SVG (*.svg)|*.svg"
			};

			if (!(dlg.ShowDialog() ?? false)) return;
			ExportToSvg(dlg.FileName);
		}

		private void ExportToSvg(string path)
		{
			Flag.ExportToSvg(path);
		}

		#endregion

		#region Load / save

		private void MenuNewClick(object sender, RoutedEventArgs e)
		{
			New();
			SetTitle();
		}

		private void New()
		{
			if (CheckUnsaved()) return;
			PlainPreset(2, 2);
			DivisionPicker1.SelectedColor = DivisionPicker1.StandardColors[1].Color;
			DivisionPicker2.SelectedColor = DivisionPicker2.StandardColors[5].Color;
			LstOverlays.Children.Clear();
			SetRatio(3, 2);
			TxtName.Text = strings.Untitled;
			_filename = string.Empty;
			_isUnsaved = false;
			SetTitle();
		}

		private void MenuSaveClick(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(_filename))
			{
				MenuSaveAsClick(sender, e);
			}
			else
			{
				Save();
			}

			SetTitle();
		}

		private void MenuSaveAsClick(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog
						  {
							  FileName = string.IsNullOrWhiteSpace(_filename) ? strings.Untitled : Path.GetFileNameWithoutExtension(_filename),
							  DefaultExt = ".flag",
							  Filter = string.Format("{0} (*.flag)|*.flag|{1} (*.*)|*.*", strings.Flag, strings.AllFiles)
						  };

			if (!(dlg.ShowDialog() ?? false)) return;
			_filename = dlg.FileName;
			Save();
			SetTitle();
		}

		private void Save()
		{
			using (var sr = new StreamWriter(_filename, false, Encoding.Unicode))
			{
				sr.WriteLine("name={0}", string.IsNullOrWhiteSpace(TxtName.Text) ? Path.GetFileNameWithoutExtension(_filename) : TxtName.Text);
				sr.WriteLine("ratio={0}:{1}", TxtRatioHeight.Text, TxtRatioWidth.Text);
				sr.WriteLine("gridSize={0}", CmbGridSize.SelectedItem);

				sr.WriteLine();

				sr.WriteLine("division");
				sr.WriteLine("type={0}", _division.Name);
				sr.WriteLine("color1={0}", DivisionPicker1.SelectedColor.ToHexString());
				sr.WriteLine("color2={0}", DivisionPicker2.SelectedColor.ToHexString());
				sr.WriteLine("color3={0}", DivisionPicker3.SelectedColor.ToHexString());
				sr.WriteLine("size1={0}", DivisionSlider1.Value);
				sr.WriteLine("size2={0}", DivisionSlider2.Value);
				sr.WriteLine("size3={0}", DivisionSlider3.Value);

				foreach (var overlay in from object child in LstOverlays.Children select ((OverlayControl)child))
				{
					sr.WriteLine();
					sr.WriteLine("overlay");
					sr.WriteLine("type={0}", overlay.Overlay.Name);
					if (overlay.Overlay.Name == "flag") sr.WriteLine("path={0}", ((OverlayFlag)overlay.Overlay).Path);
					if (overlay.Overlay.Name == "image") sr.WriteLine("path={0}", ((OverlayImage)overlay.Overlay).Path);
					else sr.WriteLine("color={0}", overlay.Color.ToHexString());

					for (int i = 0; i < overlay.Overlay.Attributes.Count(); i++)
					{
						sr.WriteLine("size{0}={1}", i + 1, overlay.Overlay.Attributes[i].Value.ToString(CultureInfo.InvariantCulture));
					}

					var path = overlay.Overlay as OverlayPath;
					if (path != null) sr.WriteLine("stroke={0}", path.StrokeColor.ToHexString());
				}
			}

			_isUnsaved = false;
			LoadPresets();
		}

		private void MenuOpenClick(object sender, RoutedEventArgs e)
		{
			if (CheckUnsaved()) return;
			var path = Flag.GetFlagPath();
			if (!string.IsNullOrWhiteSpace(path))
			{
				LoadFlagFromFile(path);
			}
			SetTitle();
		}

		// Cancel if returns true
		private bool CheckUnsaved()
		{
			if (!_isUnsaved) return false;

			string message = string.Format(strings.SaveChangesPrompt,
				string.IsNullOrWhiteSpace(_filename)
					? "untitled"
					: Path.GetFileNameWithoutExtension(_filename));

			var result = MessageBox.Show(message, "FlagMaker", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
			if (result == MessageBoxResult.Yes)
			{
				MenuSaveClick(null, null);
			}

			return result == MessageBoxResult.Cancel;
		}

		private void LoadFlagFromFile(string filename)
		{
			try
			{
				LoadFlag(Flag.LoadFromFile(filename));
				_filename = filename;
			}
			catch (Exception e)
			{
				MessageBox.Show(string.Format(strings.CouldNotOpenError, e.GetBaseException().Message), "FlagMaker", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void LoadFlag(Flag flag)
		{
			_isLoading = true;

			TxtRatioHeight.Text = flag.Ratio.Height.ToString(CultureInfo.InvariantCulture);
			TxtRatioWidth.Text = flag.Ratio.Width.ToString(CultureInfo.InvariantCulture);
			for (int i = 0; i < CmbGridSize.Items.Count; i++)
			{
				if (((Ratio)CmbGridSize.Items[i]).Width != flag.GridSize.Width) continue;
				CmbGridSize.SelectedIndex = i;
				break;
			}

			_division = flag.Division;
			SetDivisionVisibility();

			LstOverlays.Children.Clear();
			foreach (var overlay in flag.Overlays)
			{
				OverlayAdd(LstOverlays.Children.Count, overlay, true);
			}

			TxtName.Text = flag.Name;
			_isUnsaved = false;

			Draw();
			_isLoading = false;
			foreach (var control in LstOverlays.Children.OfType<OverlayControl>())
			{
				control.IsLoading = false;
			}
		}

		#endregion

		#region Presets

		private void PresetChanged(object sender, SelectionChangedEventArgs e)
		{
			CmbPresets.SelectedIndex = -1;
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
			for (int i = 0; i < CmbGridSize.Items.Count; i++)
			{
				if (((Ratio)CmbGridSize.Items[i]).Width >= 7)
				{
					CmbGridSize.SelectedIndex = i;
					break;
				}
			}

			PlainPreset(1, 7);
		}

		private void PlainPreset(int slider1, int slider2)
		{
			DivisionGridClick(null, null);
			DivisionSlider1.Value = slider1;
			DivisionSlider2.Value = slider2;
			DivisionSlider3.Value = 1;
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

				MnuWorldFlagPresets.Items.Clear();
				foreach (var menuItem in presets.OrderBy(p => p.Value).Select(preset => new MenuItem { Header = preset.Value, ToolTip = preset.Key }))
				{
					menuItem.Click += LoadPreset;
					MnuWorldFlagPresets.Items.Add(menuItem);
				}
			}
			catch (Exception)
			{
				MessageBox.Show(strings.CouldNotLoadPresetsError);
			}
		}

		private void LoadPreset(object sender, RoutedEventArgs routedEventArgs)
		{
			if (CheckUnsaved()) return;
			var menuItem = (MenuItem)sender;
			LoadFlagFromFile(menuItem.ToolTip.ToString());
			SetTitle();
		}

		private static string GetPresetFlagName(string filename)
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

		private void GenerateRandomFlag(object sender, RoutedEventArgs e)
		{
			if (CheckUnsaved()) return;
			LoadFlag(RandomFlagFactory.GenerateFlag());
			_filename = string.Empty;
			SetTitle();
		}

		#endregion

		private void MainWindowOnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			Viewbox.MaxHeight = Height - 100;
		}

		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			if (CheckUnsaved())
			{
				e.Cancel = true;
			}
		}

		private void LanguageChange(object sender, RoutedEventArgs e)
		{
			foreach (var langMenu in MnuLanguage.Items.OfType<MenuItem>())
			{
				langMenu.IsChecked = false;
			}

			var item = (MenuItem)sender;
			item.IsChecked = true;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(item.Tag.ToString());
			Settings.Default.Culture = item.Tag.ToString();
			Settings.Default.Save();
			MessageBox.Show(strings.RestartForChanges);
		}
	}
}
