using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.Service.Menu;
using ContextMenuCustomApp.View.Common;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
#if WINUI3
using AppContext = ContextMenuBuilder.AppContext;
#else
#endif
namespace ContextMenuCustomApp.View.Menu
{
    public partial class MenuPageViewModel : BaseViewModel
    {
        private readonly MenuService _menuService;
        public ObservableCollection<MenuItem> MenuItems { get; }
        public AppLang AppLang { get; private set; }
        public Settings AppSetting { get; private set; }

        private readonly bool _ignoredCache;

        public MenuPageViewModel(MenuService menuService, bool ignoreCache = false)
        {
            this._ignoredCache = ignoreCache;
            AppLang = AppContext.AppLang;
            AppSetting = AppContext.AppSettings;
            MenuItems = new ObservableCollection<MenuItem>();
            _menuService = menuService;
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

        public void Clear()
        {
            MenuItems.Clear();
        }

        public MenuItem CreateMenu()
        {
            var item = new MenuItem()
            {
                Title = $"Menu-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                Param = @"""{path}""",
                AcceptFileFlag = (int)FileMatchFlagEnum.All,
                AcceptDirectoryFlag = (int)(DirectoryMatchFlagEnum.Directory | DirectoryMatchFlagEnum.Background | DirectoryMatchFlagEnum.Desktop),
                AcceptMultipleFilesFlag = (int)FilesMatchFlagEnum.Each,
                AcceptMultipleFilesMatchFlag = (int)FilesMatchRuleFlagEnum.Any,
                Index = 0,
                Enabled = true
            };
            MenuItems.Insert(0, item);
            return item;
        }

        public async Task SaveAsync(MenuItem item)
        {
            await RunWith(async () =>
            {
                await _menuService.SaveAsync(item);
                await UpdateCache();
                await LoadAsync();
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
            menuItem.CopyMenuFrom(newMenuItem);
        }

        public async Task DeleteAsync(MenuItem item)
        {
            await RunWith(async () =>
            {
                await _menuService.DeleteAsync(item);
                //await LoadAsync();
                await UpdateCache();
                MenuItems.Remove(item);
                OnMessage("Delete Successfully");
            });
        }

        public async Task RenameMenuFile(MenuItem item, string name)
        {
            await RunWith(async () =>
            {
                var newFile = await _menuService.RenameMenuFile(item, name);
                item.File = newFile;
                item.FileName = newFile.Name;//TODO 
                item.Enabled = _menuService.IsEnabled(item);//TODO 
                OnMessage("Rename Successfully");
            });
        }

        public async Task EnableMenuFile(MenuItem item, bool enabled)
        {
            await RunWith(async () =>
            {
                var newFile = await _menuService.EnableAsync(item, enabled);
                item.File = newFile;
                item.FileName = newFile.Name;//TODO 
                item.Enabled = _menuService.IsEnabled(item);//TODO 
                await UpdateCache();
                OnMessage($"{(enabled ? "Enable " : "Disable")} Successfully");
            });
        }

        public async Task OpenMenusFolderAsync()
        {
            var folder = _menuService.GetMenusFolder();
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

        public async Task<bool> UpdateMenuFromJson(MenuItem menuItem, String json)
        {
            return await RunWith(async () =>
               {
                   var newMenuItem = await Task.Run(() =>
                   {
                       return _menuService.ConvertMenuFromJson(json);
                   });

                   if (null == newMenuItem)
                   {
                       return false;
                   }

                   //TODO 
                   newMenuItem.File = menuItem.File;
                   newMenuItem.Enabled = menuItem.Enabled;

                   ReplaceMenu(menuItem, newMenuItem);
                   return true;
               });
        }

        public async Task<string> ToJson(MenuItem menuItem, bool indented)
        {
            return await RunWith(() =>
             {
                 return Task.Run(() =>
                 {
                     return _menuService.ConvertMenuToJson(menuItem, indented);
                 });
             });
        }

        #endregion menu


        #region menu cache
        // move to MenuService 
        public bool CacheEnabled
        {
            get { return AppSetting.CacheEnabled; }
            set
            {
                AppSetting.CacheEnabled = value;
                OnPropertyChanged(nameof(CacheEnabled));
                _ = UpdateCache();
            }
        }

        private async Task UpdateCache()
        {
            if (_ignoredCache)
            {
                return;
            }

            if (AppSetting.CacheEnabled)
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
            if (_ignoredCache)
            {
                return;
            }

            await RunWith(async () =>
            {
                await _menuService.BuildToCacheAsync();
                ApplicationData.Current.LocalSettings.Values["Cache_Time"] = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                OnMessage("Build Successfully");
            });
        }

        public async Task ClearCache()
        {
            if (_ignoredCache)
            {
                return;
            }

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
