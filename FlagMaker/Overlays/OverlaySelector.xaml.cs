using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlagMaker.Localization;
using FlagMaker.Overlays.OverlayTypes.PathTypes;
using FlagMaker.Overlays.OverlayTypes.RepeaterTypes;

namespace FlagMaker.Overlays
{
	public partial class OverlaySelector
	{
		private int _defaultMaximumX;
		private int _defaultMaximumY;
		public OverlaySelector(int defaultMaximumX, int defaultMaximumY)
		{
			InitializeComponent();

			_defaultMaximumX = defaultMaximumX;
			_defaultMaximumY = defaultMaximumY;
			Title = strings.Overlays;
			FillOverlays();
		}

		private void FillOverlays()
		{
			AddTab(OverlayFactory.GetOverlaysByType(typeof(OverlayRepeater))
				.Select(o => OverlayFactory.GetInstance(o, _defaultMaximumX, _defaultMaximumY)), "Repeaters");
			AddTab(OverlayFactory.GetOverlaysByType(typeof(OverlayPath))
				.Select(o => OverlayFactory.GetInstance(o, _defaultMaximumX, _defaultMaximumY)), "Emblems");
			AddTab(OverlayFactory.CustomTypes.Select(o => o.Value).OrderBy(o => o.DisplayName), "Custom");
		}

		private void AddTab(IEnumerable<Overlay> overlays, string tabName)
		{
			var style = (Style)FindResource("GraphicButton");
			var wrapPanel = new WrapPanel();

			foreach (var overlay in overlays)
			{
				var thumbnail = new Canvas
				{
					MinWidth = 30,
					MinHeight = 30
				};

				IEnumerable<Shape> thumbs = overlay.Thumbnail;
				foreach (var thumb in thumbs)
				{
					if (thumb.Stroke == null) thumb.Stroke = Brushes.Black;
					if (thumb.Fill == null) thumb.Fill = Brushes.Black;
					thumbnail.Children.Add(thumb);
				}

				wrapPanel.Children.Add(new Button
				{
					ToolTip = overlay.DisplayName,
					Content = thumbnail,
					Tag = overlay.Name,
					Padding = new Thickness(2),
					Style = style
				});
			}

			var scrollViewer = new ScrollViewer
			{
				Content = wrapPanel,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto
			};
			var tabItem = new TabItem { Header = tabName, Content = scrollViewer };
			tabs.Items.Add(tabItem);
		}
	}
}
