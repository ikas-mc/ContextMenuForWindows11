using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ContextMenuCustomApp.View.Menu;
using ContextMenuCustomApp.View.Common;
using Microsoft.UI.Xaml.Controls;
using Windows.System.Threading;
using System;
using Windows.ApplicationModel;

namespace ContextMenuCustomApp
{
    public sealed partial class Shell : IStatusSupport, IMessageSupport
    {
        public Shell()
        {
            this.InitializeComponent();
            InitHeader();
        }

        public string AppName() {
          return  Package.Current.DisplayName;
        }

        private void InitHeader()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var uiSettings = new UISettings();
            uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
            UpdateTitleBarColor(uiSettings, titleBar);

            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;

        }

        private void UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                UpdateTitleBarColor(sender, titleBar);
            });
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarColor(UISettings uISettings, ApplicationViewTitleBar titleBar)
        {
            var accentColor = uISettings.GetColorValue(UIColorType.Accent);
            var accentColorLight = uISettings.GetColorValue(UIColorType.AccentLight2);
            titleBar.ButtonForegroundColor = accentColor;
            titleBar.ButtonInactiveForegroundColor = accentColorLight;
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            var currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            var defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            var inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            AppTitle.Foreground = e.WindowActivationState == CoreWindowActivationState.Deactivated ? inactiveForegroundBrush : defaultForegroundBrush;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ShellFrame.Navigate(typeof(MenuPage));
        }


        public void UpdateMessage(bool show, MessageType messageType, string message = "")
        {
            switch (messageType)
            {
                case MessageType.Info:
                    this.AppInfoBar.Severity = InfoBarSeverity.Informational;
                    break;
                case MessageType.Success:
                    this.AppInfoBar.Severity = InfoBarSeverity.Success;
                    break;
                case MessageType.Warnning:
                    this.AppInfoBar.Severity = InfoBarSeverity.Warning;
                    break;
                case MessageType.Error:
                    this.AppInfoBar.Severity = InfoBarSeverity.Error;
                    break;
            }
            this.AppInfoBar.Message = show ? message : string.Empty;
            this.AppInfoBar.IsOpen = show;

            if (show)
            {
                ThreadPoolTimer.CreateTimer((timer) =>
                {
                    _ = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.AppInfoBar.IsOpen = false;
                    });
                }, TimeSpan.FromSeconds(MessageType.Error != messageType ? 1 : 5));
            }
        }

        void IStatusSupport.UpdateStatus(bool busy, string message)
        {
            if (busy)
            {
                StatusInfo.IsActive = true;
                //StatusInfo.IsIndeterminate = true;
                StatusInfo.Visibility = Visibility.Visible;
            }
            else
            {
                StatusInfo.IsActive = false;
                //StatusInfo.IsIndeterminate = false;
                StatusInfo.Visibility = Visibility.Collapsed;
            }
        }
    }
}
