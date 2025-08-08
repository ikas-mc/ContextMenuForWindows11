using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using ContextMenuCustomApp.Service.Common.Json;
using System.IO;

namespace ContextMenuCustomApp.Service.Menu
{
    public class MenuService
    {
        private const string MenusFolderName = "custom_commands";

        public async Task<List<MenuItem>> QueryAllAsync()
        {
            var configFolder = await GetMenusFolderAsync();
            var files = await configFolder.GetFilesAsync();
            var result = new List<MenuItem>(files.Count);
            foreach (var file in files)
            {
                if (!(IsEnabled(file) || IsDisabled(file)))
                {
                    continue;
                }

                var content = await FileIO.ReadTextAsync(file);
                try
                {
                    var item = ConvertMenuFromJson(content);
                    item.File = file;
                    item.Enabled = IsEnabled(item);
                    result.Add(item);
                }
                catch (Exception e)
                {
                    var item = new MenuItem
                    {
                        Title = $"<Error> config file:{file.Name}",
                        File = file
                    };
                    result.Add(item);
                    Debug.WriteLine(e.StackTrace);
                }
            }

            result.Sort((l, r) => l.Index - r.Index);
            return result;
        }

        private async Task<StorageFile> CreateMenuFileAsync(string name)
        {
            var folder = await GetMenusFolderAsync();
            return await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
        }

        public async Task<StorageFolder> GetMenusFolderAsync()
        {
            var storageItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync(MenusFolderName);
            switch (storageItem)
            {
                case StorageFile _:
                    throw new Exception($"Menus Folder Error,\"{storageItem.Path}\" is not a folder");
                case StorageFolder storageFolder:
                    return storageFolder;
                default:
                    return await ApplicationData.Current.LocalFolder.CreateFolderAsync(MenusFolderName, CreationCollisionOption.OpenIfExists);
            }
        }

        public async Task SaveAsync(MenuItem item)
        {
            if (null == item)
            {
                throw new Exception("Menu is null");
            }

            var (result, message) = CheckMenu(item);
            if (!result)
            {
                throw new Exception(message);
            }

            var menuFile = item.File;
            if (menuFile == null)
            {
                var fileName = $"{item.Title}.json";
                menuFile = await CreateMenuFileAsync(fileName);
            }

            var content = ConvertMenuToJson(item,true);
            await FileIO.WriteTextAsync(menuFile, content);

            item.File = menuFile;
            item.Enabled = true;
        }

        public async Task<MenuItem> ReadAsync(StorageFile menuFile)
        {
            if (null == menuFile)
            {
                throw new Exception("Menu file is null");
            }

            var content = await FileIO.ReadTextAsync(menuFile);
            try
            {
                var item = ConvertMenuFromJson(content);
                item.File = menuFile;
                item.Enabled = IsEnabled(item);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception("Read From Menu file error", e);
            }
        }

        public async Task<StorageFile> RenameMenuFile(MenuItem item, string name)
        {
            if (null == item)
            {
                throw new Exception("Menu is null");
            }

            var file = (item?.File) ?? throw new Exception("Menu file is null");

            string newName = null;
            if (!string.IsNullOrEmpty(name))
            {
                newName = Path.GetFileName(name);
            }
            if (string.IsNullOrEmpty(newName))
            {
                throw new Exception("New Name is empty");
            }

            await file.RenameAsync(newName, NameCollisionOption.GenerateUniqueName);
            return file;
        }

        public async Task DeleteAsync(MenuItem item)
        {
            if (null == item)
            {
                throw new Exception("Menu is null");
            }

            var menuFile = item.File;
            if (null != menuFile)
            {
                await menuFile.DeleteAsync();
            }
        }

        public bool IsEnabled(MenuItem item)
        {
            if (null == item)
            {
                throw new Exception("Menu is null");
            }

            //TODO
            if (item.File == null)
            {
                return true;
            }

            return IsEnabled(item.File);
        }

        private bool IsEnabled(StorageFile file)
        {
            if (null == file)
            {
                throw new Exception("Menu file is null");
            }
            return file.Name.ToLower().EndsWith(".json") == true;
        }

        private bool IsDisabled(StorageFile file)
        {
            if (null == file)
            {
                throw new Exception("Menu file is null");
            }
            return file.Name.ToLower().EndsWith(".json.disabled") == true;
        }

        public async Task<StorageFile> EnableAsync(MenuItem item, bool enabled)
        {
            if (null == item)
            {
                throw new Exception("Menu is null");
            }

            var file = (item?.File) ?? throw new Exception("Menu file is null");

            var fileName = file.Name;
            if (enabled && IsDisabled(file))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }
            else if (!enabled && IsEnabled(file))
            {
                fileName += ".disabled";
            }

            if (file.Name != fileName)
            {
                await file.RenameAsync(fileName, NameCollisionOption.FailIfExists);
            }

            return file;
        }

        public async Task BuildToCacheAsync()
        {
            var configFolder = await GetMenusFolderAsync();
            var files = await configFolder.GetFilesAsync();

            var menus = ApplicationData.Current.LocalSettings.CreateContainer("menus", ApplicationDataCreateDisposition.Always).Values;
            menus.Clear();

            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (!IsEnabled(file))
                {
                    continue;
                }
                var content = await FileIO.ReadTextAsync(file);
                menus[i.ToString()] = content;
            }
        }

        public void ClearCache()
        {
            var menus = ApplicationData.Current.LocalSettings.CreateContainer("menus", ApplicationDataCreateDisposition.Always).Values;
            menus.Clear();
        }

        public MenuItem ConvertMenuFromJson(string content)
        {
            var menu = JsonUtil.Deserialize<MenuItem>(content);

            //update from old version v3.6
            if (menu.AcceptFileFlag == (int)FileMatchFlagEnum.None && menu.AcceptFile)
            {
                menu.AcceptFileFlag = (int)FileMatchFlagEnum.Ext;
            }

            //update from old version v3.8
            if (menu.AcceptDirectoryFlag == (int)DirectoryMatchFlagEnum.None && menu.AcceptDirectory)
            {
                menu.AcceptDirectoryFlag = (int)DirectoryMatchFlagEnum.Directory |
                                           (int)DirectoryMatchFlagEnum.Background |
                                           (int)DirectoryMatchFlagEnum.Desktop;
            }

            return menu;
        }

        public string ConvertMenuToJson(MenuItem content, bool indented = false)
        {
            var json = JsonUtil.Serialize(content, indented);
            return json;
        }

        private (bool, string) CheckMenu(MenuItem content)
        {
            if (string.IsNullOrEmpty(content.Title))
            {
                return (false, "Title is empty");
            }

            if (string.IsNullOrEmpty(content.Exe))
            {
                return (false, "Exe is empty");
            }

            return (true, string.Empty);
        }
    }
}