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

namespace ContextMenuCustomApp.View.Menu
{
    public class MenuPageViewModel : BaseViewModel
    {
        private readonly MenuService _menuService;
        public ObservableCollection<MenuItem> MenuItems { get; }

        public readonly int MultipleFilesFlagJOIN = (int)MultipleFilesFlag.JOIN;

        public MenuPageViewModel()
        {
            MenuItems = new ObservableCollection<MenuItem>();
            _menuService = MenuService.Ins;
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
            var item = new MenuItem() { Title = "new menu", Param = @"""{path}""", AcceptFile = true, AcceptDirectory = true };
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
            get
            {
                return Settings.INS.CacheEnabled;
            }
            set
            {
                Settings.INS.CacheEnabled = value;
                OnPropertyChanged(nameof(CacheEnabled));
                _ = UpdateCache();
            }
        }

        private async Task UpdateCache()
        {
            if (Settings.INS.CacheEnabled)
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
                ApplicationData.Current.LocalSettings.Values["Cache_Time"] = DateTime.Now.ToString(CultureInfo.CurrentCulture);
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


        #region setting

        public string Version()
        {
            var version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public string GetCustomMenuName()
        {
            var value = ApplicationData.Current.LocalSettings.Values["Custom_Menu_Name"];
            return (value as string) ?? "Open With";
        }

        public async void SetCustomMenuName(string name)
        {
            await Task.Run(() =>
            {
                ApplicationData.Current.LocalSettings.Values["Custom_Menu_Name"] = name ?? "Open With";
            });
        }

        #endregion setting
    }
}
