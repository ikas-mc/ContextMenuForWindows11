using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;
using ContextMenuCustomApp.Service.Menu;
using ContextMenuCustomApp.View.Common;
using ContextMenuCustomApp.Common;
using System.Data;
using System.Reflection;

namespace ContextMenuCustomApp.View.Menu
{
    public class MenuPageViewModel : BaseViewModel
    {
        private readonly MenuService _menuService;
        public ObservableCollection<MenuItem> MenuItems { get; }
        public ObservableCollection<EnumItem> FileMatchEnumItems { get; }
        public ObservableCollection<EnumItem> DirectoryMatchEnumItems { get; }
        public ObservableCollection<EnumItem> FilesMatchFlagEnumItems { get; }

        public MenuPageViewModel(MenuService menuService)
        {
            _menuService = menuService;

            MenuItems = new ObservableCollection<MenuItem>();
            FileMatchEnumItems = new ObservableCollection<EnumItem>(EnumItemUtil.GetEnumItems<FileMatchFlagEnum>());
            DirectoryMatchEnumItems = new ObservableCollection<EnumItem>(EnumItemUtil.GetEnumItems<DirectoryMatchFlagEnum>());
            FilesMatchFlagEnumItems = new ObservableCollection<EnumItem>(EnumItemUtil.GetEnumItems<FilesMatchFlagEnum>());
        }

        #region menu

        public async Task LoadAsync()
        {
            MenuItems.Clear();
            await RunWith(async () =>
            {
                var menus = await _menuService.QueryAllAsync();
                menus.ForEach(MenuItems.Add);
            });
        }

        public MenuItem New()
        {
            var item = new MenuItem()
            { Title = "new menu", Param = @"""{path}""", AcceptFile = true, AcceptDirectory = true };
            MenuItems.Add(item);
            return item;
        }

        public async Task SaveAsync(MenuItem item)
        {
            await RunWith(async () =>
            {
                await _menuService.SaveAsync(item);
                await LoadAsync();
                await UpdateCache();
                OnMessage("Save Successfully");
            });
        }

        public async Task RefreshMenuAsync(MenuItem menuItem)
        {
            await RunWith(async () =>
           {
               var newMenuItem = await _menuService.ReadAsync(menuItem.File);
               ReplaceMenu(menuItem, newMenuItem);
           });
        }

        public void ReplaceMenu(MenuItem menuItem, MenuItem newMenuItem)
        {

            PropertyInfo[] propsSource = typeof(MenuItem).GetProperties();
            foreach (PropertyInfo infoSource in propsSource)
            {
                object value = infoSource.GetValue(newMenuItem, null);
                infoSource.SetValue(menuItem, value, null);
            }
        }

        public async Task DeleteAsync(MenuItem item)
        {
            await RunWith(async () =>
            {
                await _menuService.DeleteAsync(item);
                await LoadAsync();
                await UpdateCache();
                OnMessage("Delete Successfully");
            });
        }

        public async Task OpenMenusFolderAsync()
        {
            var folder = await _menuService.GetMenusFolderAsync();
            _ = await Launcher.LaunchFolderAsync(folder);
        }

        public async Task OpenMenuFileAsync(MenuItem item)
        {
            if (item.File == null)
            {
                return;
            }

            _ = await Launcher.LaunchFileAsync(item.File);
        }

        #endregion menu


        #region menu cache

        public string CacheTime
        {
            get
            {
                var value = ApplicationData.Current.LocalSettings.Values["Cache_Time"];
                return (value as string) ?? "No Cache";
            }
        }

        public void UpdateCacheTime()
        {
            OnPropertyChanged(nameof(CacheTime));
        }

        public bool CacheEnabled
        {
            get { return Settings.Default.CacheEnabled; }
            set
            {
                Settings.Default.CacheEnabled = value;
                OnPropertyChanged(nameof(CacheEnabled));
                _ = UpdateCache();
            }
        }

        private async Task UpdateCache()
        {
            if (Settings.Default.CacheEnabled)
            {
                await BuildCache();
            }
            else
            {
                await ClearCache();
            }
        }


        public async Task BuildCache()
        {
            await RunWith(async () =>
            {
                await _menuService.BuildToCacheAsync();
                ApplicationData.Current.LocalSettings.Values["Cache_Time"] =
                    DateTime.Now.ToString(CultureInfo.CurrentCulture);
                OnMessage("Build Successfully");
            });
        }

        public async Task ClearCache()
        {
            await RunWith(() =>
            {
                return Task.Run(() =>
                {
                    _menuService.ClearCache();
                    ApplicationData.Current.LocalSettings.Values.Remove("Cache_Time");
                    OnMessage("Clear Successfully");
                });
            });
        }

        #endregion cache
    }
}