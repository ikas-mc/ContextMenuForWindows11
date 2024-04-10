using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using ContextMenuCustomApp.Service.Menu;
using System.Linq;
using Windows.Storage.Pickers;
using ContextMenuCustomApp.View.Common;
using Windows.System;
using ContextMenuCustomApp.View.Setting;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace ContextMenuCustomApp.View.Menu
{
    public sealed partial class MenuPage
    {
        private readonly MenuPageViewModel _viewModel;

        public MenuPage()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            InitializeComponent();
            _viewModel = new MenuPageViewModel(MenuService.Ins);
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
            var item = _viewModel.CreateMenu();
            CommandList.SelectedItem = item;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (GetSeletedMenu(true, out MenuItem menuItem))
            {
                await _viewModel.SaveAsync(menuItem);
                if (null != menuItem.File)
                {
                    CommandList.SelectedItem = _viewModel.MenuItems.FirstOrDefault(menu => Equals(menuItem.File.Path, menu.File.Path));
                }
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (GetSeletedMenu(true, out MenuItem menuItem))
            {
                var result = await Alert.ChooseAsync("Delete Menu ?", "Warning");
                if (result)
                {
                    await _viewModel.DeleteAsync(menuItem);
                }
            }
        }

        private async void Open_Folder_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.OpenMenusFolderAsync();
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            if (GetSeletedMenu(true, out MenuItem menuItem))
            {
                await _viewModel.OpenMenuFileAsync(menuItem);
            }
        }

        private async void OpenExeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (GetSeletedMenu(true, out MenuItem menuItem))
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
                    menuItem.Exe = $"\"{file.Path}\"";
                    if (string.IsNullOrEmpty(menuItem.Icon))
                    {
                        menuItem.Icon = $"\"{file.Path}\",0";
                    }
                }
            }
        }

        private async void OpenIconButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && GetSeletedMenu(true, out MenuItem menuItem))
            {
                var fileOpenPicker = new FileOpenPicker
                {
                    SuggestedStartLocation = PickerLocationId.ComputerFolder
                };

                fileOpenPicker.FileTypeFilter.Add(".dll");
                fileOpenPicker.FileTypeFilter.Add(".exe");
                fileOpenPicker.FileTypeFilter.Add(".icon");
                fileOpenPicker.FileTypeFilter.Add(".png");
                fileOpenPicker.FileTypeFilter.Add(".bmp");
                fileOpenPicker.FileTypeFilter.Add(".jpeg");
                fileOpenPicker.FileTypeFilter.Add(".jpg");
                fileOpenPicker.FileTypeFilter.Add(".heic");
                fileOpenPicker.FileTypeFilter.Add(".tif");
                var file = await fileOpenPicker.PickSingleFileAsync();
                if (null != file)
                {
                    string iconPath;
                    if (file.Name.EndsWith(".dll") || file.Name.EndsWith(".exe"))
                    {
                        iconPath = $"\"{file.Path}\",0";
                    }
                    else
                    {
                        iconPath = $"\"{file.Path}\"";
                    }

                    if (button.Tag is string tag && tag == "Dark")
                    {
                        menuItem.IconDark = iconPath;
                    }
                    else
                    {
                        menuItem.Icon = iconPath;
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

        private async void Refresh_Menu_Click(object sender, RoutedEventArgs e)
        {
            if (GetSeletedMenu(true, out MenuItem menuItem) && menuItem.File is StorageFile)
            {
                await _viewModel.RefreshMenuAsync(menuItem);
            }
        }

        private async void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (GetSeletedMenu(true, out MenuItem menuItem))
            {
                var json = await _viewModel.ToJson(menuItem, true);
                if (string.IsNullOrEmpty(json))
                {
                    return;
                }

                var dataPackage = new DataPackage
                {
                    RequestedOperation = DataPackageOperation.Copy
                };
                dataPackage.SetText(json);
                Clipboard.SetContent(dataPackage);
                this.ShowMessage("Copy To Clipboard Successfully", MessageType.Success);
            }

        }


        //TODO refactor
        private async void CopyFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (GetSeletedMenu(true, out MenuItem menuItem))
            {
                var json = string.Empty;
                DataPackageView dataPackageView = Clipboard.GetContent();
                if (dataPackageView.Contains(StandardDataFormats.Text))
                {
                    json = await dataPackageView.GetTextAsync();
                }

                if (string.IsNullOrEmpty(json))
                {
                    this.ShowMessage("Clipboard text is empty", MessageType.Warnning);
                    return;
                }
             
                //bad
                if (await _viewModel.SetMenu(menuItem, json))
                {
                    this.ShowMessage("Copy From Clipboard Successfully", MessageType.Success);
                }
            }
        }

        private bool GetSeletedMenu(bool showWarnning, out MenuItem selectedMenuItem)
        {
            if (CommandList.SelectedItem is MenuItem menuItem)
            {
                selectedMenuItem = menuItem;
                return true;
            }

            if (showWarnning)
            {
                this.ShowMessage("No selected menu", MessageType.Warnning);
            }

            selectedMenuItem = null;
            return false;
        }

    }
}
