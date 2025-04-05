using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using ContextMenuCustomApp.Service.Menu;
using ContextMenuCustomApp.View.Common;
using ContextMenuCustomApp.Common;
using System.Reflection;

namespace ContextMenuCustomApp.View.Menu
{
    public class MenuPageViewModel : BaseViewModel
    {
        private readonly MenuService _menuService;
        public ObservableCollection<MenuItem> MenuItems { get; }
        public ObservableCollection<EnumItem> FileMatchEnumItems { get; }
        public ObservableCollection<EnumItem> FilesMatchFlagEnumItems { get; }
        public ObservableCollection<EnumItem> ShowWindowFlagEnumItems { get; }

        public AppLang AppLang { get; private set; }

        public MenuPageViewModel()
        {
            AppLang = AppContext.Current.AppLang;
            _menuService = AppContext.Current.GetService<MenuService>();

            MenuItems = new ObservableCollection<MenuItem>();
            FileMatchEnumItems = new ObservableCollection<EnumItem>(
                new System.Collections.Generic.List<EnumItem>() {
                    new EnumItem() { Label = AppLang.MenuMatchFileOptionOff, Value = (int)FileMatchFlagEnum.None },
                    new EnumItem() { Label = AppLang.MenuMatchFileOptionExtentionLike, Value = (int)FileMatchFlagEnum.Ext },
                    new EnumItem() { Label = AppLang.MenuMatchFileOptionNameRegex, Value = (int)FileMatchFlagEnum.Regex },
                    new EnumItem() { Label = AppLang.MenuMatchFileOptionExtention, Value = (int)FileMatchFlagEnum.ExtList },
                    new EnumItem() { Label = AppLang.MenuMatchFileOptionAll, Value = (int)FileMatchFlagEnum.All },
                }
                );
            FilesMatchFlagEnumItems = new ObservableCollection<EnumItem>(
                  new System.Collections.Generic.List<EnumItem>() {
                    new EnumItem() { Label = AppLang.MenuMatchFilesOptionOff, Value = (int)FilesMatchFlagEnum.None },
                    new EnumItem() { Label = AppLang.MenuMatchFilesOptionEach, Value = (int)FilesMatchFlagEnum.Each },
                    new EnumItem() { Label = AppLang.MenuMatchFilesOptionJoin, Value = (int)FilesMatchFlagEnum.Join },
                }
                );

            ShowWindowFlagEnumItems = new ObservableCollection<EnumItem>(
                 new System.Collections.Generic.List<EnumItem>() {
                    new EnumItem() { Label = AppLang.MenuShowWindowOptionHide, Value = (int)ShowWindowFlagEnum.Hide },
                    new EnumItem() { Label = AppLang.MenuShowWindowOptionNormal, Value = (int)ShowWindowFlagEnum.ShowNormal },
                    new EnumItem() { Label = AppLang.MenuShowWindowOptionMin, Value = (int)ShowWindowFlagEnum.ShowMinimized },
                    new EnumItem() { Label = AppLang.MenuShowWindowOptionMax, Value = (int)ShowWindowFlagEnum.ShowMaximized },
               }
               );
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

        public MenuItem CreateMenu()
        {
            var item = new MenuItem()
            {
                Title = $"Menu-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                Param = @"""{path}""",
                AcceptFileFlag = (int)FileMatchFlagEnum.All,
                AcceptDirectoryFlag = (int)(DirectoryMatchFlagEnum.Directory | DirectoryMatchFlagEnum.Background | DirectoryMatchFlagEnum.Desktop),
                AcceptMultipleFilesFlag = (int)FilesMatchFlagEnum.Each,
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

        public async Task<bool> SetMenu(MenuItem menuItem, String json)
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
    }
}