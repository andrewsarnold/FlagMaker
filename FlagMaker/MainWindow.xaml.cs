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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FlagMaker.Divisions;
using FlagMaker.Localization;
using FlagMaker.Overlays;
using FlagMaker.Overlays.OverlayTypes.PathTypes;
using FlagMaker.Overlays.OverlayTypes.ShapeTypes;
using FlagMaker.Properties;
using FlagMaker.RandomFlag;
using Xceed.Wpf.Toolkit;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Panel = System.Windows.Controls.Panel;
using Path = System.IO.Path;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

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
				return new Flag("flag", new Ratio(_ratioWidth, _ratioHeight), (Ratio)_cmbGridSize.SelectedItem, _division,
					_lstOverlays.Children.OfType<OverlayControl>().Select(c => c.Overlay));
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
				_mnuLanguage.Items.Add(menuItem);
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
				                    _divisionPicker1.SelectedColor,
				                    _divisionPicker2.SelectedColor,
				                    _divisionPicker3.SelectedColor
			                    });
			Draw();
			SetAsUnsaved();
		}

		private void DivisionSliderChanged()
		{
			if (_isLoading) return;

			_divisionSliderLabel1.Text = _divisionSlider1.Value.ToString(CultureInfo.InvariantCulture);
			_divisionSliderLabel2.Text = _divisionSlider2.Value.ToString(CultureInfo.InvariantCulture);
			_divisionSliderLabel3.Text = _divisionSlider3.Value.ToString(CultureInfo.InvariantCulture);

			_division.SetValues(new List<double>
			                    {
				                    _divisionSlider1.Value,
				                    _divisionSlider2.Value,
				                    _divisionSlider3.Value
			                    });
			Draw();
			SetAsUnsaved();
		}

		private void SetDivisionVisibility()
		{
			_divisionPicker2.Visibility = Visibility.Collapsed;
			_divisionPicker3.Visibility = Visibility.Collapsed;
			_divisionPicker1.SelectedColor = _division.Colors[0];

			if (_division.Colors.Count > 1)
			{
				_divisionPicker2.SelectedColor = _division.Colors[1];
				_divisionPicker2.Visibility = Visibility.Visible;
				if (_division.Colors.Count > 2)
				{
					_divisionPicker3.SelectedColor = _division.Colors[2];
					_divisionPicker3.Visibility = Visibility.Visible;
				}
			}

			_divisionSlider1.Visibility = Visibility.Collapsed;
			_divisionSlider2.Visibility = Visibility.Collapsed;
			_divisionSlider3.Visibility = Visibility.Collapsed;
			_divisionSliderLabel1.Visibility = Visibility.Collapsed;
			_divisionSliderLabel2.Visibility = Visibility.Collapsed;
			_divisionSliderLabel3.Visibility = Visibility.Collapsed;

			if (_division.Values.Count <= 0) return;
			_divisionSlider1.Value = _division.Values[0];
			_divisionSlider1.Visibility = Visibility.Visible;
			_divisionSliderLabel1.Text = _division.Values[0].ToString("#");
			_divisionSliderLabel1.Visibility = Visibility.Visible;

			if (_division.Values.Count <= 1) return;
			_divisionSlider2.Value = _division.Values[1];
			_divisionSlider2.Visibility = Visibility.Visible;
			_divisionSliderLabel2.Text = _division.Values[1].ToString("#");
			_divisionSliderLabel2.Visibility = Visibility.Visible;

			if (_division.Values.Count <= 2) return;
			_divisionSlider3.Value = _division.Values[2];
			_divisionSlider3.Visibility = Visibility.Visible;
			_divisionSliderLabel3.Text = _division.Values[2].ToString("#");
			_divisionSliderLabel3.Visibility = Visibility.Visible;
		}

		private void DivisionGridClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionGrid(_divisionPicker1.SelectedColor, _divisionPicker2.SelectedColor, (int)_divisionSlider1.Value, (int)_divisionSlider2.Value);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		private void DivisionFessesClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionFesses(_divisionPicker1.SelectedColor, _divisionPicker2.SelectedColor, _divisionPicker3.SelectedColor, (int)_divisionSlider1.Value, (int)_divisionSlider2.Value, (int)_divisionSlider3.Value);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		private void DivisionPalesClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionPales(_divisionPicker1.SelectedColor, _divisionPicker2.SelectedColor, _divisionPicker3.SelectedColor, (int)_divisionSlider1.Value, (int)_divisionSlider2.Value, (int)_divisionSlider3.Value);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		private void DivisionBendsForwardClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionBendsForward(_divisionPicker1.SelectedColor, _divisionPicker2.SelectedColor);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		private void DivisionBendsBackwardClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionBendsBackward(_divisionPicker1.SelectedColor, _divisionPicker2.SelectedColor);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		private void DivisionXClick(object sender, RoutedEventArgs e)
		{
			_division = new DivisionX(_divisionPicker1.SelectedColor, _divisionPicker2.SelectedColor);
			SetDivisionVisibility();
			Draw();
			SetAsUnsaved();
		}

		#endregion

		#region Overlays

		private void OverlayAdd(object sender, RoutedEventArgs e)
		{
			OverlayAdd(_lstOverlays.Children.Count, null, false);
		}

		private void SetOverlayMargins()
		{
			for (int i = 0; i < _lstOverlays.Children.Count - 1; i++)
			{
				((OverlayControl)_lstOverlays.Children[i]).Margin = new Thickness(0, 0, 0, 20);
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
			_lstOverlays.Children.Remove(controlToRemove);
			Draw();
			SetAsUnsaved();
		}

		private void MoveUp(object sender, EventArgs e)
		{
			var controlToMove = (OverlayControl)sender;
			int index = _lstOverlays.Children.IndexOf(controlToMove);
			if (index == 0) return;

			var controls = new List<OverlayControl>();
			for (int i = 0; i < _lstOverlays.Children.Count; i++)
			{
				if (i + 1 == index)
				{
					controls.Add((OverlayControl)_lstOverlays.Children[i + 1]);
					controls.Add((OverlayControl)_lstOverlays.Children[i]);
					i++;
				}
				else
				{
					controls.Add((OverlayControl)_lstOverlays.Children[i]);
				}
			}

			_lstOverlays.Children.Clear();
			foreach (var overlayControl in controls)
			{
				_lstOverlays.Children.Add(overlayControl);
			}

			SetOverlayMargins();
			Draw();
			SetAsUnsaved();
		}

		private void MoveDown(object sender, EventArgs e)
		{
			var controlToMove = (OverlayControl)sender;
			int index = _lstOverlays.Children.IndexOf(controlToMove);
			if (index == _lstOverlays.Children.Count - 1) return;

			var controls = new List<OverlayControl>();
			for (int i = 0; i < _lstOverlays.Children.Count; i++)
			{
				if (i == index)
				{
					controls.Add((OverlayControl)_lstOverlays.Children[i + 1]);
					controls.Add((OverlayControl)_lstOverlays.Children[i]);
					i++;
				}
				else
				{
					controls.Add((OverlayControl)_lstOverlays.Children[i]);
				}
			}

			_lstOverlays.Children.Clear();
			foreach (var overlayControl in controls)
			{
				_lstOverlays.Children.Add(overlayControl);
			}

			SetOverlayMargins();
			Draw();
			SetAsUnsaved();
		}

		private void Clone(object sender, EventArgs e)
		{
			var controlToClone = (OverlayControl)sender;
			int index = _lstOverlays.Children.IndexOf(controlToClone);

			var type = controlToClone.Overlay.GetType();
			var copy = OverlayFactory.GetInstance(type, 1, 1, controlToClone.Overlay.Name);

			for (int i = 0; i < controlToClone.Overlay.Attributes.Count; i++)
			{
				copy.Attributes[i].Value = controlToClone.Overlay.Attributes[i].Value;
				copy.Attributes[i].IsDiscrete = controlToClone.Overlay.Attributes[i].IsDiscrete;
			}

			copy.SetColor(controlToClone.Overlay.Color);

			if (type == typeof(OverlayPath) || type.IsSubclassOf(typeof(OverlayPath)))
			{
				((OverlayPath)copy).StrokeColor = ((OverlayPath)controlToClone.Overlay).StrokeColor;
			}
			else if (type == typeof (OverlayFlag))
			{
				((OverlayFlag)copy).Flag = ((OverlayFlag)controlToClone.Overlay).Flag;
			}

			var gridSize = ((Ratio)_cmbGridSize.SelectedItem);
			copy.SetMaximum(gridSize.Width, gridSize.Height);

			OverlayAdd(index + 1, copy, true);
		}

		private void OverlayAdd(int index, Overlay overlay, bool isLoading)
		{
			var gridSize = ((Ratio)_cmbGridSize.SelectedItem);
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

			_lstOverlays.Children.Insert(index, control);

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

			_divisionPicker1.AvailableColors = _availableColors;
			_divisionPicker1.StandardColors = _standardColors;
			_divisionPicker1.SelectedColor = _divisionPicker1.StandardColors[1].Color;
			_divisionPicker1.ShowRecentColors = true;
			_divisionPicker1.RecentColors = _recentColors;

			_divisionPicker2.AvailableColors = _availableColors;
			_divisionPicker2.StandardColors = _standardColors;
			_divisionPicker2.SelectedColor = _divisionPicker2.StandardColors[5].Color;
			_divisionPicker2.ShowRecentColors = true;
			_divisionPicker2.RecentColors = _recentColors;

			_divisionPicker3.AvailableColors = _availableColors;
			_divisionPicker3.StandardColors = _standardColors;
			_divisionPicker3.SelectedColor = _divisionPicker3.StandardColors[8].Color;
			_divisionPicker3.ShowRecentColors = true;
			_divisionPicker3.RecentColors = _recentColors;

			_divisionPicker1.SelectedColorChanged += (sender, args) => DivisionColorChanged();
			_divisionPicker2.SelectedColorChanged += (sender, args) => DivisionColorChanged();
			_divisionPicker3.SelectedColorChanged += (sender, args) => DivisionColorChanged();
			_divisionSlider1.ValueChanged += (sender, args) => DivisionSliderChanged();
			_divisionSlider2.ValueChanged += (sender, args) => DivisionSliderChanged();
			_divisionSlider3.ValueChanged += (sender, args) => DivisionSliderChanged();

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
			bool skip2 = _division is DivisionGrid && _divisionSlider1.Value == 1 && _divisionSlider2.Value == 1;
			var colors = Flag.ColorsUsed();

			_divisionPicker1.SelectedColor = GetNextColor(_divisionPicker1.SelectedColor, colors);
			if (!skip2) _divisionPicker2.SelectedColor = GetNextColor(_divisionPicker2.SelectedColor, colors);
			if (_divisionPicker3.Visibility == Visibility.Visible)
				_divisionPicker3.SelectedColor = GetNextColor(_divisionPicker3.SelectedColor, colors);

			foreach (var overlay in _lstOverlays.Children.Cast<OverlayControl>())
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
			_txtRatioHeight.Text = height.ToString(CultureInfo.InvariantCulture);
			_txtRatioWidth.Text = width.ToString(CultureInfo.InvariantCulture);
			_ratioHeight = height;
			_ratioWidth = width;

			FillGridCombobox();
		}

		private void GridOnChanged(object sender, RoutedEventArgs e)
		{
			_showGrid = !_showGrid;

			if (_showGrid)
			{
				_btnGrid.Background = new SolidColorBrush(Colors.LightSkyBlue);
			}
			else
			{
				_btnGrid.ClearValue(BackgroundProperty);
			}

			DrawGrid();
		}

		private void DrawGrid()
		{
			_canvasGrid.Children.Clear();

			if (!_showGrid) return;

			if (_cmbGridSize.Items.Count == 0) return;

			var gridSize = ((Ratio)_cmbGridSize.SelectedItem);

			var intervalX = _canvas.Width / gridSize.Width;
			for (int x = 0; x <= gridSize.Width; x++)
			{
				var line = new Line
				{
					StrokeThickness = 3,
					X1 = 0,
					X2 = 0,
					Y1 = 0,
					Y2 = _canvas.Height,
					SnapsToDevicePixels = false,
					Stroke = new SolidColorBrush(Colors.Silver)
				};
				_canvasGrid.Children.Add(line);
				Canvas.SetTop(line, 0);
				Canvas.SetLeft(line, x * intervalX);
			}

			var intervalY = _canvas.Height / gridSize.Height;
			for (int y = 0; y <= gridSize.Height; y++)
			{
				var line = new Line
				{
					StrokeThickness = 3,
					X1 = 0,
					X2 = _canvas.Width,
					Y1 = 0,
					Y2 = 0,
					SnapsToDevicePixels = false,
					Stroke = new SolidColorBrush(Colors.Silver)
				};
				_canvasGrid.Children.Add(line);
				Canvas.SetTop(line, y * intervalY);
				Canvas.SetLeft(line, 0);
			}
		}

		private void FillGridCombobox()
		{
			_cmbGridSize.Items.Clear();
			for (int i = 1; i <= 20; i++)
			{
				_cmbGridSize.Items.Add(new Ratio(_ratioWidth * i, _ratioHeight * i));
			}
			_cmbGridSize.SelectedIndex = 0;
		}

		private void RatioTextboxChanged(object sender, TextChangedEventArgs e)
		{
			int newHeight;
			int newWidth;

			if (!int.TryParse(_txtRatioHeight.Text, out newHeight))
			{
				_ratioHeight = 1;
			}

			if (!int.TryParse(_txtRatioWidth.Text, out newWidth))
			{
				_ratioWidth = 1;
			}

			if (newHeight < 1)
			{
				_ratioHeight = 1;
				_txtRatioHeight.Text = "1";
			}
			else
			{
				_ratioHeight = newHeight;
			}

			if (newWidth < 1)
			{
				_ratioWidth = 1;
				_txtRatioWidth.Text = "1";
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
			if (_cmbGridSize.Items.Count == 0) return;

			var gridSize = ((Ratio)_cmbGridSize.SelectedItem);
			int sliderMaxX = gridSize.Width;
			int sliderMaxY = gridSize.Height;
			int sliderMax = Math.Max(sliderMaxX, sliderMaxY);

			_divisionSlider1.Maximum = sliderMax;
			_divisionSlider2.Maximum = sliderMax;
			_divisionSlider3.Maximum = sliderMax;

			foreach (var overlay in _lstOverlays.Children)
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
			_canvas.Width = _ratioWidth * 200;
			_canvas.Height = _ratioHeight * 200;
			Flag.Draw(_canvas);
			DrawTexture(_canvas);
			DrawGrid();
			SetUsedColorPalettes();
		}

		private void DrawTexture(Panel canvas)
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
			var dimensions = GetPngDimensions(true);
			if (dimensions.Height == 0 || dimensions.Width == 0) return;

			var dlg = new SaveFileDialog
			{
				FileName = string.IsNullOrWhiteSpace(_filename) ? strings.Untitled : Path.GetFileNameWithoutExtension(_filename),
				DefaultExt = ".png",
				Filter = "PNG (*.png)|*.png"
			};

			if (!(dlg.ShowDialog() ?? false)) return;

			Flag.ExportToPng(dimensions, dlg.FileName);
		}

		private Size GetPngDimensions(bool constrain)
		{
			var dialog = new ExportPng(new Ratio(_ratioWidth, _ratioHeight), constrain) { Owner = this };
			return !(dialog.ShowDialog() ?? false) 
				? new Size(0, 0)
				: new Size(dialog.PngWidth, dialog.PngHeight);
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
			Flag.ExportToSvg(dlg.FileName);
		}

		private void MenuExportBulkPngClick(object sender, RoutedEventArgs e)
		{
			var error = false;
			var files = GetFlagFiles();
			if (!files.Any()) return;

			var defaultDirectory = Path.GetDirectoryName(files.First());
			var directory = GetBulkSaveDirectory(defaultDirectory);
			if (directory == string.Empty) return;

			var dimensions = GetPngDimensions(false);
			if (dimensions.Height == 0 || dimensions.Width == 0) return;

			foreach (var file in files)
			{
				try
				{
					Flag.LoadFromFile(file).ExportToPng(dimensions, string.Format("{0}\\{1}.png", directory, Path.GetFileNameWithoutExtension(file)));
				}
				catch (Exception)
				{
					error = true;
				}
			}

			ExportFinished(strings.ExportAsPng, error);
		}

		private void MenuExportBulkSvgClick(object sender, RoutedEventArgs e)
		{
			var error = false;
			var files = GetFlagFiles();
			if (!files.Any()) return;

			var defaultDirectory = Path.GetDirectoryName(files.First());
			var directory = GetBulkSaveDirectory(defaultDirectory);
			if (directory == string.Empty) return;

			foreach (var file in files)
			{
				try
				{
					Flag.LoadFromFile(file).ExportToSvg(string.Format("{0}\\{1}.svg", directory, Path.GetFileNameWithoutExtension(file)));
				}
				catch (Exception)
				{
					error = true;
				}
			}

			ExportFinished(strings.ExportAsSvg, error);
		}

		private static IEnumerable<string> GetFlagFiles()
		{
			var dlg = new OpenFileDialog
			{
				Multiselect = true,
				Filter = "Flag (*.flag)|*.flag",
				CheckFileExists = true
			};
			dlg.ShowDialog();
			return dlg.FileNames;
		}

		private static string GetBulkSaveDirectory(string defaultDirectory)
		{
			var dlg = new FolderBrowserDialog
			          {
						  SelectedPath = defaultDirectory,
						  ShowNewFolderButton = true
			          };
			return dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel
				? string.Empty 
				: dlg.SelectedPath;
		}

		private static void ExportFinished(string title, bool errorOccurred)
		{
			if (errorOccurred)
			{
				MessageBox.Show(strings.ExportBulkError, title, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				MessageBox.Show(strings.ExportBulkSuccess, title, MessageBoxButton.OK, MessageBoxImage.Information);
			}
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
			_divisionPicker1.SelectedColor = _divisionPicker1.StandardColors[1].Color;
			_divisionPicker2.SelectedColor = _divisionPicker2.StandardColors[5].Color;
			_lstOverlays.Children.Clear();
			SetRatio(3, 2);
			_txtName.Text = strings.Untitled;
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
				sr.WriteLine("name={0}", string.IsNullOrWhiteSpace(_txtName.Text) ? Path.GetFileNameWithoutExtension(_filename) : _txtName.Text);
				sr.WriteLine("ratio={0}:{1}", _txtRatioHeight.Text, _txtRatioWidth.Text);
				sr.WriteLine("gridSize={0}", _cmbGridSize.SelectedItem);

				sr.WriteLine();

				sr.WriteLine("division");
				sr.WriteLine("type={0}", _division.Name);
				sr.WriteLine("color1={0}", _divisionPicker1.SelectedColor.ToHexString());
				sr.WriteLine("color2={0}", _divisionPicker2.SelectedColor.ToHexString());
				sr.WriteLine("color3={0}", _divisionPicker3.SelectedColor.ToHexString());
				sr.WriteLine("size1={0}", _divisionSlider1.Value);
				sr.WriteLine("size2={0}", _divisionSlider2.Value);
				sr.WriteLine("size3={0}", _divisionSlider3.Value);

				foreach (var overlay in from object child in _lstOverlays.Children select ((OverlayControl)child))
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

			_txtRatioHeight.Text = flag.Ratio.Height.ToString(CultureInfo.InvariantCulture);
			_txtRatioWidth.Text = flag.Ratio.Width.ToString(CultureInfo.InvariantCulture);
			for (int i = 0; i < _cmbGridSize.Items.Count; i++)
			{
				if (((Ratio)_cmbGridSize.Items[i]).Width != flag.GridSize.Width) continue;
				_cmbGridSize.SelectedIndex = i;
				break;
			}

			_division = flag.Division;
			SetDivisionVisibility();

			_lstOverlays.Children.Clear();
			foreach (var overlay in flag.Overlays)
			{
				OverlayAdd(_lstOverlays.Children.Count, overlay, true);
			}

			_txtName.Text = flag.Name;
			_isUnsaved = false;

			Draw();
			_isLoading = false;
			foreach (var control in _lstOverlays.Children.OfType<OverlayControl>())
			{
				control.IsLoading = false;
			}
		}

		#endregion

		#region Presets

		private void PresetChanged(object sender, SelectionChangedEventArgs e)
		{
			_cmbPresets.SelectedIndex = -1;
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
			for (int i = 0; i < _cmbGridSize.Items.Count; i++)
			{
				if (((Ratio)_cmbGridSize.Items[i]).Width >= 7)
				{
					_cmbGridSize.SelectedIndex = i;
					break;
				}
			}

			PlainPreset(1, 7);
		}

		private void PlainPreset(int slider1, int slider2)
		{
			DivisionGridClick(null, null);
			_divisionSlider1.Value = slider1;
			_divisionSlider2.Value = slider2;
			_divisionSlider3.Value = 1;
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

				_mnuWorldFlagPresets.Items.Clear();
				foreach (var menuItem in presets.OrderBy(p => p.Value).Select(preset => new MenuItem { Header = preset.Value, ToolTip = preset.Key }))
				{
					menuItem.Click += LoadPreset;
					_mnuWorldFlagPresets.Items.Add(menuItem);
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
			LoadFlag(new RandomFlagFactory().GenerateFlag());
			_filename = string.Empty;
			SetTitle();
		}

		#endregion

		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			if (CheckUnsaved())
			{
				e.Cancel = true;
			}
		}

		private void LanguageChange(object sender, RoutedEventArgs e)
		{
			foreach (var langMenu in _mnuLanguage.Items.OfType<MenuItem>())
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
