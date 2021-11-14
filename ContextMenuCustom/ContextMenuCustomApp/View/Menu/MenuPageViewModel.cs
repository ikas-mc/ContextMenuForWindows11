using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;
using ContextMenuCustomApp.Service.Menu;

namespace ContextMenuCustomApp.View.Menu
{

    public class MenuPageViewModel : BaseModel
    {
        public ObservableCollection<MenuItem> MenuItems { get; }

        private readonly MenuService _menuService;
        private readonly Action<Exception, string> _exceptionHandler;

        public MenuPageViewModel(Action<Exception, string> exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            if (null == _exceptionHandler)
            {
                throw new ArgumentNullException(nameof(exceptionHandler), "exception handler is nul");
            }
            MenuItems = new ObservableCollection<MenuItem>();
            _menuService = MenuService.Ins;
        }

        public async Task LoadAsync()
        {
            MenuItems.Clear();
            try
            {
                var menus = await _menuService.QueryAllAsync();
                menus.ForEach(MenuItems.Add);
            }
            catch (Exception e)
            {
                _exceptionHandler(e, e.Message);
            }
        }

        public MenuItem New()
        {
            var item = new MenuItem() { Title = "new menu", Param = @"""{path}""" };
            MenuItems.Add(item);
            return item;
        }

        public async Task SaveAsync(MenuItem item)
        {
            try
            {
                await _menuService.SaveAsync(item);
                await LoadAsync();
            }
            catch (Exception e)
            {
                _exceptionHandler(e, e.Message);
            }
        }

        public async Task DeleteAsync(MenuItem item)
        {
            try
            {
                await _menuService.DeleteAsync(item);
                await LoadAsync();
            }
            catch (Exception e)
            {
                _exceptionHandler(e, e.Message);
            }
        }

        public async Task OpenMenusFolderAsync()
        {
            try
            {
                var folder = await _menuService.GetMenusFolderAsync();
                _ = await Launcher.LaunchFolderAsync(folder);
            }
            catch (Exception e)
            {
                _exceptionHandler(e, e.Message);
            }
        }

        public async Task OpenMenuFileAsync(MenuItem item)
        {
            try
            {
                if (item.File == null)
                {
                    return;
                }
                _ = await Launcher.LaunchFileAsync(item.File);
            }
            catch (Exception e)
            {
                _exceptionHandler(e, e.Message);
            }
        }

        public async Task Build()
        {
            try
            {
                await _menuService.BuildToCache();
                ApplicationData.Current.LocalSettings.Values["Cache_Time"] = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            }
            catch (Exception e)
            {
                _exceptionHandler(e, e.Message);
            }
        }

        public void ClearCache()
        {
            try
            {
                _menuService.ClearCache();
                ApplicationData.Current.LocalSettings.Values.Remove("Cache_Time");
            }
            catch (Exception e)
            {
                _exceptionHandler(e, e.Message);
            }
        }

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

        public string Version()
        {
            var version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public string GetCustomMenuName()
        {
            var value = ApplicationData.Current.LocalSettings.Values["Custom_Menu_Name"];
            return (value as string) ?? "Custom Menu";
        }

        public async void SetCustomMenuName(string name)
        {
            await Task.Run(() =>
            {
                ApplicationData.Current.LocalSettings.Values["Custom_Menu_Name"] = name ?? "Custom Menu";
            });
        }
    }
}
