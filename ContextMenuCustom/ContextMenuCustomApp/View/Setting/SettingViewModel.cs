using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.Service.Common.Json;
using ContextMenuCustomApp.Service.Lang;
using ContextMenuCustomApp.View.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Globalization;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;

namespace ContextMenuCustomApp.View.Setting
{
    public class SettingViewModel : BaseViewModel
    {
        public readonly Settings Settings;
        public readonly AppLang AppLang;
        private readonly LanguageService _languageService;
        public SettingViewModel()
        {
            Settings = AppContext.Current.AppSetting;
            AppLang = AppContext.Current.AppLang;
            _languageService = AppContext.Current.GetService<LanguageService>();
        }

        public string Version()
        {
            var version = Package.Current.Id.Version;
            return $"Version: {version.Major}.{version.Minor}.{version.Build}";
        }

        public int ThemeType
        {
            get
            {
                return Settings.ThemeType;
            }
            set
            {
                Settings.ThemeType = value;

                if (value == 1)
                {
                    ThemeHelper.RootTheme = ElementTheme.Dark;
                }
                else if (value == 2)
                {
                    ThemeHelper.RootTheme = ElementTheme.Light;
                }
                else
                {
                    ThemeHelper.RootTheme = ElementTheme.Default;
                }
            }
        }

        public async void OpenDataFolder()
        {
            var folder = ApplicationData.Current.LocalFolder;
            _ = await Launcher.LaunchFolderAsync(folder);
        }

        #region language

        private List<LangInfo> _languages;

        public List<LangInfo> Languages
        {
            get => _languages;
            set => SetProperty(ref _languages, value);
        }

        public async Task LoadLanguages()
        {
            var languages = await RunWith(async () =>
            {
                return await _languageService.QueryLangList();
            });
            Languages = languages ?? new List<LangInfo>();
        }

        public void UpdateLangSetting(LangInfo langInfo)
        {
            RunWith(() =>
            {
                _languageService.UpdateLangSetting(langInfo);
            });
        }

        public async Task ExportLang()
        {
            await RunWith(() =>
            {
                return _languageService.ExportLanguageToFileAsync(async (string suggestedFileName) =>
                {
                    FileSavePicker fileSavePicker = new FileSavePicker
                    {
                        SuggestedStartLocation = PickerLocationId.Desktop,
                        SuggestedFileName = suggestedFileName ?? ""
                    };

                    fileSavePicker.FileTypeChoices.Add("Json", new List<string>() { ".json" });
                    var file = await fileSavePicker.PickSaveFileAsync();
                    return file;
                });
            });
        }

        public async Task ImportLang()
        {
            await RunWith(async () =>
            {
                var fileOpenPicker = new FileOpenPicker
                {
                    SuggestedStartLocation = PickerLocationId.Desktop,
                };

                fileOpenPicker.FileTypeFilter.Add(".json");
                var file = await fileOpenPicker.PickSingleFileAsync();
                if (file == null)
                {
                    return;
                }

                await _languageService.AddCustomLanguageFileAsync(file, true);
            });

            await LoadLanguages();
        }

        public LangInfo GetCurrentLang()
        {
            var langFileName = AppContext.Current.AppSetting.AppLang;
            var langInfo = Languages.Find(x => x.FileName == langFileName);
            if (null == langInfo)
            {
                langInfo = Languages.FirstOrDefault();
            }
            return langInfo;
        }

        public async void OpenLanguagesFolder()
        {
            await RunWith(async () =>
            {
                var folder = await _languageService.GetCustomLanguagesFolderAsync();
                await Launcher.LaunchFolderAsync(folder);
            });
        }

        #endregion

    }
}
