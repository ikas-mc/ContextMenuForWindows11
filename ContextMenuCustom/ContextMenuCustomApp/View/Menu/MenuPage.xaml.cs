using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using ContextMenuCustomApp.Service.Menu;
using System.Linq;
using Windows.Storage.Pickers;
using ContextMenuCustomApp.View.Common;
using Windows.System;
using ContextMenuCustomApp.View.Setting;

namespace ContextMenuCustomApp.View.Menu
{
    public sealed partial class MenuPage
    {
        private readonly MenuPageViewModel _viewModel;

        public MenuPage()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            InitializeComponent();
            _viewModel = new MenuPageViewModel();
            this.RegisterMessageHandler(_viewModel);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                _ = _viewModel.LoadAsync();
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = CommandList.SelectedItem as MenuItem;
            await _viewModel.LoadAsync();
            if (null != selectedItem?.File)
            {
                CommandList.SelectedItem = _viewModel.MenuItems.FirstOrDefault(item => Equals(selectedItem.File.Path, item.File.Path));
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var item = _viewModel.New();
            CommandList.SelectedItem = item;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is MenuItem item)
            {
                await _viewModel.SaveAsync(item);
                if (null != item.File)
                {
                    CommandList.SelectedItem = _viewModel.MenuItems.FirstOrDefault(menu => Equals(item.File.Path, menu.File.Path));
                }
            }
            else
            {
                this.ShowMessage("no selected item", MessageType.Warnning);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is MenuItem item)
            {
                var result = await Alert.ChooseAsync("confirm to delete", "Warn");
                if (result)
                {
                    await _viewModel.DeleteAsync(item);
                }
            }
            else
            {
                this.ShowMessage("no selected item", MessageType.Warnning);
            }
        }

        private async void Open_Folder_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.OpenMenusFolderAsync();
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is MenuItem item)
            {
                await _viewModel.OpenMenuFileAsync(item);
            }
            else
            {
                this.ShowMessage("no selected item", MessageType.Warnning);
            }
        }
        private async void OpenExeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is MenuItem item)
            {
                var fileOpenPicker = new FileOpenPicker
                {
                    SuggestedStartLocation = PickerLocationId.ComputerFolder
                };
                fileOpenPicker.FileTypeFilter.Add(".com");
                fileOpenPicker.FileTypeFilter.Add(".exe");
                fileOpenPicker.FileTypeFilter.Add(".bat");
                fileOpenPicker.FileTypeFilter.Add(".cmd");
                fileOpenPicker.FileTypeFilter.Add(".vbs");
                fileOpenPicker.FileTypeFilter.Add(".vbe");
                fileOpenPicker.FileTypeFilter.Add(".js");
                fileOpenPicker.FileTypeFilter.Add(".jse");
                var file = await fileOpenPicker.PickSingleFileAsync();
                if (null != file)
                {
                    item.Exe = $"\"{file.Path}\"";
                    if (string.IsNullOrEmpty(item.Icon))
                    {
                        item.Icon = $"\"{file.Path}\",0";
                    }
                }
            }
        }

        private void BuildCacheTipButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateCacheTime();
            CacheTip.IsOpen = true;
        }

        private void CommandList_DragItemsCompleted(Windows.UI.Xaml.Controls.ListViewBase sender, Windows.UI.Xaml.Controls.DragItemsCompletedEventArgs args)
        {

        }

        private void OpenSetting_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage));
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            _ = Launcher.LaunchUriAsync(new Uri("https://github.com/ikas-mc/ContextMenuForWindows11/wiki"));
        }

    }
}
