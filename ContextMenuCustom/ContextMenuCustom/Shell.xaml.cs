using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.View.Common;
using ContextMenuCustomApp.View.Menu;
using ContextMenuCustomApp.View.Setting;
using ContextMenuCustomApp.View.Tip;
using ContextMenuCustomApp.View.Win11ShellMenu;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.System.Threading;

namespace ContextMenuCustomApp
{
    public sealed partial class Shell : IStatusSupport, IMessageSupport
    {
        public Shell()
        {
            this.InitializeComponent();
            InitHeader();
            this.Loaded += Shell_Loaded;
        }

        public string AppName()
        {
            var entries = Package.Current.GetAppListEntries();
            if (entries.FirstOrDefault() is AppListEntry entry)
            {
                return entry.DisplayInfo.DisplayName;
            }
            return Package.Current.DisplayName;
        }

        private void InitHeader()
        {
            var window = ContextMenuCustom.MainWindow.Instance;
            if (window == null)
            {
                return;
            }

            var titleBar = window.AppWindow.TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ExtendsContentIntoTitleBar = true;
            window.SetTitleBar(AppTitleBar);
        }

        private void NavEntry_Click(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton btn && btn.Tag is string tag)
            {
                try
                {
                    switch (tag)
                    {
                        case "1":
                        {
                            ShellFrame.Navigate(typeof(MenuPage));
                            break;
                        }
                        case "2":
                        {
                            ShellFrame.Navigate(typeof(Win11ShellMenuPage));
                            break;
                        }
                        case "3":
                        {
                            _ = Launcher.LaunchUriAsync(new Uri("https://github.com/ikas-mc/ContextMenuForWindows11/wiki"));
                            break;
                        }
                        case "4":
                        {
                            ShellFrame.Navigate(typeof(SettingPage));
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }
            }
        }

        private void ShellFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            DispatcherQueue.TryEnqueue(() =>
            {
                ShellFrame.Navigate(typeof(MenuPage));
            });
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Shell_Loaded;

            var appVersion = AppVersion.Current();
            if (Settings.Default.AppVersion < appVersion)
            {
                Settings.Default.AppVersion = appVersion;
                ShowUpdateTip();
            }
        }

        private async void ShowUpdateTip()
        {
            try
            {
                await TipDialog.CreateUpdateTipDialog().ShowAsync();
            }
            catch (Exception)
            {
                //
            }
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
                case MessageType.Warning:
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
                    _ = this.DispatcherQueue.TryEnqueue(() =>
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
