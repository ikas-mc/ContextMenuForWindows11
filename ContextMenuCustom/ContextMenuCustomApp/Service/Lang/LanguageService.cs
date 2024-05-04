using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.Service.Common.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Storage;

namespace ContextMenuCustomApp.Service.Lang
{
    public class LanguageService
    {
        private readonly List<string> DefaultLanguages = new List<string>() { "en-US" };

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
                var langFile = await StorageFile.GetFileFromPathAsync(Path.Combine(AppDataPaths.GetDefault().LocalAppData, "Langs", langFileName));
                var langContent = await FileIO.ReadTextAsync(langFile);
                return JsonUtil.Deserialize<AppLang>(langContent);
            });
        }

        public async Task<List<LangInfo>> QueryLangList()
        {
            return await Task.Run(async () =>
            {
                var langInfoList = new List<LangInfo>();
                DefaultLanguages.ForEach(name => langInfoList.Add(LangInfo.Create(name, name, true)));

                var langsFolder = await GetCustomLanguagesFolderAsync();
                var langFiles = await langsFolder.GetFilesAsync();
                foreach (var file in langFiles)
                {
                    var fileName = file.Name;
                    if (fileName.EndsWith(".json"))
                    {
                        var name = Path.GetFileNameWithoutExtension(fileName);
                        LangInfo langInfo = LangInfo.Create(name, fileName, false);
                        langInfoList.Add(langInfo);
                    }
                }
                return langInfoList;
            });
        }

        public async Task<StorageFolder> GetCustomLanguagesFolderAsync()
        {
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync("languages", CreationCollisionOption.OpenIfExists);
        }

        public void UpdateLangSetting(LangInfo langInfo)
        {
            Settings.Default.AppLang = langInfo.FileName;
            ApplicationLanguages.PrimaryLanguageOverride = langInfo.Name;
        }

    }
}