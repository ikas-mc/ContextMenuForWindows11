using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.Service.Common.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Globalization;
using Windows.Storage;

namespace ContextMenuCustomApp.Service.Lang
{
    public class LanguageService
    {
        private const string LanguagesFolderName = "languages";

        private readonly List<string> _defaultLanguages = new List<string>() { "en-US" };

        public async Task<AppLang> LoadAsync()
        {
            string langFileName = Settings.Default.AppLang;

            //default lang
            if (string.IsNullOrEmpty(langFileName) || !langFileName.EndsWith(".json"))
            {
                return new AppLang();
            }

            //custom lang
            try
            {
                return await LoadCustomAsync(langFileName);
            }
            catch (Exception)
            {
                return new AppLang();
            }
        }

        public Task<AppLang> LoadDefualtAsync(string langFileName = null)
        {
            return Task.FromResult(new AppLang());
        }

        public Task<AppLang> LoadCustomAsync(string langFileName)
        {
            return Task.Run(async () =>
            {
                var langFile = await GetCustomLanguageFileAsync(langFileName);
                var langContent = await FileIO.ReadTextAsync(langFile);
                return JsonUtil.Deserialize<AppLang>(langContent);
            });
        }

        public async Task<List<LangInfo>> QueryLangList()
        {
            return await Task.Run(async () =>
            {
                var langInfoList = new List<LangInfo>();
                _defaultLanguages.ForEach(name =>
                {
                    var language = tryParseLanguageTag(name);
                    if (null != language)
                    {
                        var langInfo = LangInfo.Create(name, name, language.DisplayName, true);
                        langInfoList.Add(langInfo);
                    }
                });

                var langsFolder = await GetCustomLanguagesFolderAsync();
                var langFiles = await langsFolder.GetFilesAsync();

                foreach (var file in langFiles)
                {
                    var fileName = file.Name;
                    if (fileName.EndsWith(".json"))
                    {
                        var name = Path.GetFileNameWithoutExtension(fileName);
                        var language = tryParseLanguageTag(name);
                        if (null != language)
                        {
                            LangInfo langInfo = LangInfo.Create(name, fileName, language.DisplayName, false);
                            langInfoList.Add(langInfo);
                        }
                    }
                }
                return langInfoList;
            });
        }


        public async Task<StorageFolder> GetCustomLanguagesFolderAsync()
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(LanguagesFolderName);
            if (item is StorageFolder storageFolder)
            {
                return storageFolder;
            }
            else
            {
                StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(LanguagesFolderName, CreationCollisionOption.OpenIfExists);
                return folder;
            }
        }

        public async Task<StorageFile> GetCustomLanguageFileAsync(string langFileName)
        {
            string path = Path.Combine(AppDataPaths.GetDefault().LocalAppData, LanguagesFolderName, langFileName);
            StorageFile file = await StorageFile.GetFileFromPathAsync(path);
            return file;
        }

        public async Task AddCustomLanguageFileAsync(StorageFile file, bool back)
        {
            var fileName = file.Name;
            if (!fileName.EndsWith(".json"))
            {
                throw new Exception("Language file format is not json");
            }

            var langContent = await FileIO.ReadTextAsync(file);
            try
            {
                JsonUtil.Deserialize<AppLang>(langContent);
            }
            catch (Exception e)
            {
                throw new Exception($"Language file parse error,{e.Message}");
            }

            var langsFolder = await GetCustomLanguagesFolderAsync();

            if (back)
            {
                var oldFile = await langsFolder.TryGetItemAsync(fileName);
                if (oldFile != null)
                {
                    await oldFile.RenameAsync(fileName + ".back", NameCollisionOption.GenerateUniqueName);
                }
            }

            await file.CopyAsync(langsFolder, fileName, NameCollisionOption.ReplaceExisting);
        }

        public async Task ExportLanguageToFileAsync(Func<string, Task<StorageFile>> fileFunc)
        {
            string fileName = Settings.Default.AppLang;

            //default lang
            if (string.IsNullOrEmpty(fileName) || !fileName.EndsWith(".json"))
            {
                fileName = _defaultLanguages.First() + ".json";
            }

            var file = await fileFunc(fileName);
            if (null == file)
            {
                return;
            }

            AppLang applang = await LoadAsync();

            await FileIO.WriteTextAsync(file, JsonUtil.Serialize(applang, true));
        }

        public void UpdateLangSetting(LangInfo langInfo)
        {
            Settings.Default.AppLang = langInfo.FileName;
            ApplicationLanguages.PrimaryLanguageOverride = langInfo.Name;
        }

        public Language tryParseLanguageTag(string name)
        {
            try
            {
                return new Language(name);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}