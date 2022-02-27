using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;

namespace ContextMenuCustomApp.Service.Menu
{
    public class MenuService
    {
        private const string MenusFolderName = "custom_commands";

        public static readonly MenuService Ins = new MenuService();

        public async Task<List<MenuItem>> QueryAllAsync()
        {
            var configFolder = await GetMenusFolderAsync();
            var files = await configFolder.GetFilesAsync();
            var result = new List<MenuItem>(files.Count);
            foreach (var file in files)
            {
                var content = await FileIO.ReadTextAsync(file);
                try
                {
                    var item = MenuItem.ReadFromJson(content);
                    item.File = file;
                    result.Add(item);
                }
                catch (Exception e)
                {
                    var item = new MenuItem
                    {
                        Title = "<Error> config",
                        File = file
                    };
                    result.Add(item);
                    Debug.WriteLine(e.StackTrace);
                }
            }

            return result;
        }

        private async Task<StorageFile> CreateMenuFileAsync(string name)
        {
            var folder = await GetMenusFolderAsync();
            return await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
        }

        public async Task<StorageFolder> GetMenusFolderAsync()
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(MenusFolderName);
            if (item is StorageFile)
            {
                await item.DeleteAsync();
            }
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync(MenusFolderName, CreationCollisionOption.OpenIfExists);
        }

        public async Task SaveAsync(MenuItem item)
        {
            if (null == item)
            {
                throw new ArgumentNullException();
            }

            var (result, message) = MenuItem.Check(item);
            if (!result)
            {
                throw new Exception($"{message} is empty");
            }

            if (item.File == null)
            {
                var fileName = item.Title + ".json";
                item.File = await CreateMenuFileAsync(fileName);
            }

            var content = MenuItem.WriteToJson(item);
            await FileIO.WriteTextAsync(item.File, content);
        }


        public async Task DeleteAsync(MenuItem item)
        {
            if (null == item)
            {
                throw new ArgumentNullException();
            }

            if (item.File == null)
            {
                return;
            }
            await item.File.DeleteAsync();
        }

        public async Task BuildToCacheAsync()
        {
            var configFolder = await GetMenusFolderAsync();
            var files = await configFolder.GetFilesAsync();

            var menus = ApplicationData.Current.LocalSettings.CreateContainer("menus", ApplicationDataCreateDisposition.Always).Values;
            menus.Clear();

            for (int i = 0; i < files.Count; i++)
            {
                var content = await FileIO.ReadTextAsync(files[i]);
                menus[i.ToString()] = content;
            }
        }

        public void ClearCache()
        {
            var menus = ApplicationData.Current.LocalSettings.CreateContainer("menus", ApplicationDataCreateDisposition.Always).Values;
            menus.Clear();
        }

    }
}
