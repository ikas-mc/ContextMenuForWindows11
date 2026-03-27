using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.Service.Menu;
using System;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;
#if WINUI3
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using AppContext = ContextMenuBuilder.AppContext;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

namespace ContextMenuCustomApp.View.Menu
{
    public partial class WindowFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int intValue)
            {
                return intValue + 1;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is int intValue)
            {
                return intValue - 1;
            }
            return 0;
        }
    }
    public sealed partial class MenuEditorControl : UserControl
    {

        public readonly Settings _settings;
        public readonly AppLang _appLang;
        public ObservableCollection<EnumItem> FileMatchEnumItems { get; }
        public ObservableCollection<EnumItem> FilesMatchFlagEnumItems { get; }
        public ObservableCollection<EnumItem> ShowWindowFlagEnumItems { get; }
        public MenuEditorControl()
        {
            _settings = AppContext.AppSettings;
            _appLang = AppContext.AppLang;
            FileMatchEnumItems = new ObservableCollection<EnumItem>(
              new System.Collections.Generic.List<EnumItem>() {
                    new EnumItem() { Label = _appLang.MenuMatchFileOptionOff, Value = (int)FileMatchFlagEnum.None },
                    new EnumItem() { Label = _appLang.MenuMatchFileOptionExtentionLike, Value = (int)FileMatchFlagEnum.Ext },
                    new EnumItem() { Label = _appLang.MenuMatchFileOptionNameRegex, Value = (int)FileMatchFlagEnum.Regex },
                    new EnumItem() { Label = _appLang.MenuMatchFileOptionExtention, Value = (int)FileMatchFlagEnum.ExtList },
                    new EnumItem() { Label = _appLang.MenuMatchFileOptionAll, Value = (int)FileMatchFlagEnum.All },
              }
              );
            FilesMatchFlagEnumItems = new ObservableCollection<EnumItem>(
                  new System.Collections.Generic.List<EnumItem>() {
                    new EnumItem() { Label = _appLang.MenuMatchFilesOptionOff, Value = (int)FilesMatchFlagEnum.None },
                    new EnumItem() { Label = _appLang.MenuMatchFilesOptionEach, Value = (int)FilesMatchFlagEnum.Each },
                    new EnumItem() { Label = _appLang.MenuMatchFilesOptionJoin, Value = (int)FilesMatchFlagEnum.Join },
                }
                );

            ShowWindowFlagEnumItems = new ObservableCollection<EnumItem>(
                 new System.Collections.Generic.List<EnumItem>() {
                    new EnumItem() { Label = _appLang.MenuShowWindowOptionHide, Value = (int)ShowWindowFlagEnum.Hide },
                    new EnumItem() { Label = _appLang.MenuShowWindowOptionNormal, Value = (int)ShowWindowFlagEnum.ShowNormal },
                    new EnumItem() { Label = _appLang.MenuShowWindowOptionMin, Value = (int)ShowWindowFlagEnum.ShowMinimized },
                    new EnumItem() { Label = _appLang.MenuShowWindowOptionMax, Value = (int)ShowWindowFlagEnum.ShowMaximized },
               }
               );
            this.InitializeComponent();
        }

        public MenuItem MenuItem
        {
            get { return (MenuItem)GetValue(MenuItemProperty); }
            set { SetValue(MenuItemProperty, value); }
        }

        public static readonly DependencyProperty MenuItemProperty =
            DependencyProperty.Register(nameof(MenuItem), typeof(MenuItem), typeof(MenuEditorControl), new PropertyMetadata(0));

        private async void OpenExeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = MenuItem;
            if (menuItem != null)
            {
                var fileOpenPicker = new FileOpenPicker
                {
                    SuggestedStartLocation = PickerLocationId.ComputerFolder
                };
                fileOpenPicker.FileTypeFilter.Add("*");
                fileOpenPicker.FileTypeFilter.Add(".com");
                fileOpenPicker.FileTypeFilter.Add(".exe");
                fileOpenPicker.FileTypeFilter.Add(".bat");
                fileOpenPicker.FileTypeFilter.Add(".cmd");
                fileOpenPicker.FileTypeFilter.Add(".vbs");
                fileOpenPicker.FileTypeFilter.Add(".vbe");
                fileOpenPicker.FileTypeFilter.Add(".js");
                fileOpenPicker.FileTypeFilter.Add(".jse");
#if WINUI3
                ContextMenuBuilder.Modules.File.PickerHelper.TryAttachWindow(fileOpenPicker);
#endif
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
            var menuItem = MenuItem;
            if (sender is Button button && menuItem != null)
            {
                var fileOpenPicker = new FileOpenPicker
                {
                    SuggestedStartLocation = PickerLocationId.ComputerFolder
                };

                string[] fileTypes = { "*", ".dll", ".exe", ".ico", ".png", ".bmp", ".jpeg", ".jpg", ".heic", ".tif" };
                foreach (string fileType in fileTypes)
                {
                    fileOpenPicker.FileTypeFilter.Add(fileType);
                }
#if WINUI3
                ContextMenuBuilder.Modules.File.PickerHelper.TryAttachWindow(fileOpenPicker);
#endif
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
    }
}
