using ContextMenuCustomApp.View.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

namespace ContextMenuCustomApp.View.Win11ShellMenu
{
    public sealed partial class Win11ShellMenuPage : Page
    {
        public Win11ShellMenuViewModel ViewModel { get; } = new Win11ShellMenuViewModel();

        private Win11ShellPackageRow? _rightTappedPackageRow;
        private Win11ShellComItem? _rightTappedComItem;

        public Win11ShellMenuPage()
        {
            InitializeComponent();
            this.RegisterMessageHandler(ViewModel);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadAsync();
        }

        private async void OnRefreshClicked(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadAsync();
        }

        private async void OnBlockToggled(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox checkBox || checkBox.Tag is not Win11ShellComItem item) return;

            var isChecked = checkBox.IsChecked;
            if (isChecked == null) return;

            bool blocked = !isChecked.Value;
            await ViewModel.SetBlockedAsync(item, blocked);

            if (item.IsEnabled != !blocked)
            {
                checkBox.IsChecked = item.IsEnabled;
            }
        }

        private void OnPackageContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            if (sender is FrameworkElement fe && fe.Tag is Win11ShellPackageRow row)
            {
                _rightTappedPackageRow = row;
                ContextMenuHelper.ShowAt(PackageContextFlyout, sender, args);
            }
        }

        private async void OnOpenAppClicked(object sender, RoutedEventArgs e)
        {
            if (_rightTappedPackageRow is not null)
                await ViewModel.LaunchAppAsync(_rightTappedPackageRow);
        }

        private void OnOpenInstallFolderClicked(object sender, RoutedEventArgs e)
        {
            if (_rightTappedPackageRow is not null)
                ViewModel.OpenInstallFolder(_rightTappedPackageRow);
        }

        private void OnComItemContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            if (sender is FrameworkElement fe && fe.Tag is Win11ShellComItem item)
            {
                _rightTappedComItem = item;
                var flyout = ComItemContextFlyout;
                if (flyout.Items.Count > 1)
                {
                    if (item.IsEnabled)
                    {
                        flyout.Items[0].Visibility = Visibility.Collapsed;
                        flyout.Items[1].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        flyout.Items[0].Visibility = Visibility.Visible;
                        flyout.Items[1].Visibility = Visibility.Collapsed;
                    }
                }

                ContextMenuHelper.ShowAt(flyout, sender, args);
            }
        }

        private async void OnComItemContextToggleClicked(object sender, RoutedEventArgs e)
        {
            if (_rightTappedComItem is not null)
                await ViewModel.SetBlockedAsync(_rightTappedComItem, _rightTappedComItem.IsEnabled);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
