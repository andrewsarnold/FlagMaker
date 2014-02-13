using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FlagMaker.Localization;
using FlagMaker.Overlays.OverlayTypes.PathTypes;
using FlagMaker.Overlays.OverlayTypes.RepeaterTypes;
using FlagMaker.Overlays.OverlayTypes.ShapeTypes;

namespace FlagMaker.Overlays
{
	public partial class OverlaySelector
	{
		private readonly int _defaultMaximumX;
		private readonly int _defaultMaximumY;
		private Overlay _selectedOverlay;

		public OverlaySelector(int defaultMaximumX, int defaultMaximumY)
		{
			InitializeComponent();

			_defaultMaximumX = defaultMaximumX;
			_defaultMaximumY = defaultMaximumY;
			Title = strings.Overlays;
			FillOverlays();
		}

		public Overlay SelectedOverlay
		{
			get { return _selectedOverlay; }
			set
			{
				_selectedOverlay = value;
				Close();
			}
		}

		private void FillOverlays()
		{
			AddTab(OverlayFactory.GetOverlaysNotInTypes(new[] { typeof(OverlayRepeater), typeof(OverlayPath) })
				.Where(t => t != typeof(OverlayFlag))
				.Select(o => OverlayFactory.GetInstance(o, _defaultMaximumX, _defaultMaximumY)), strings.Shapes);
			AddTab(OverlayFactory.GetOverlaysByType(typeof(OverlayRepeater))
				.Select(o => OverlayFactory.GetInstance(o, _defaultMaximumX, _defaultMaximumY)), strings.Special);
			AddTab(new Overlay[]{ new OverlayFlag(_defaultMaximumY, _defaultMaximumY) }, strings.Flags);
			AddTab(OverlayFactory.GetOverlaysByType(typeof(OverlayPath))
				.Select(o => OverlayFactory.GetInstance(o, _defaultMaximumX, _defaultMaximumY)), strings.Emblems);
			AddTab(OverlayFactory.CustomTypes.Select(o => o.Value).OrderBy(o => o.DisplayName), strings.Custom);
		}

		private void AddTab(IEnumerable<Overlay> overlays, string tabName)
		{
			var style = (Style)FindResource("GraphicButton");
			var wrapPanel = new WrapPanel();

			foreach (var button in overlays.Select(overlay => new Button
			{
				ToolTip = overlay.DisplayName,
				Content = overlay.CanvasThumbnail(),
				Tag = overlay.Name,
				Padding = new Thickness(2),
				Style = style
			}))
			{
				button.Click += (s, e) =>
				{
					var tag = (string)((Button)s).Tag;

					if (tag == "flag")
					{
						string path = Flag.GetFlagPath();

						Flag flag;
						try
						{
							flag = Flag.LoadFromFile(path);
						}
						catch (OperationCanceledException)
						{
							return;
						}
						catch (Exception ex)
						{
							MessageBox.Show(string.Format("{0}\n{1} \"{2}\"", strings.CouldNotOpenFileError, strings.ErrorAtLine, ex.Message), "FlagMaker", MessageBoxButton.OK, MessageBoxImage.Warning);
							return;
						}

						SelectedOverlay = new OverlayFlag(flag, path, _defaultMaximumX, _defaultMaximumY);
					}
					else
					{
						SelectedOverlay = OverlayFactory.GetInstance(tag, _defaultMaximumX, _defaultMaximumY);
					}
				};

				wrapPanel.Children.Add(button);
			}

			var scrollViewer = new ScrollViewer
			{
				Content = wrapPanel,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto
			};
			var tabItem = new TabItem { Header = tabName, Content = scrollViewer };
			tabs.Items.Add(tabItem);
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				Close();
			}
		}

		private void Cancel(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
