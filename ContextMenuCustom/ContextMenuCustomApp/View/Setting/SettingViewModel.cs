using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.View.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;

namespace ContextMenuCustomApp.View.Setting
{
    public class SettingViewModel : BaseViewModel
    {
        private readonly Settings _settings;
        public SettingViewModel()
        {
            _settings = Settings.INS;
        }

        public string Version()
        {
            var version = Package.Current.Id.Version;
            return $"Version: {version.Major}.{version.Minor}.{version.Build}";
        }

        public string SdkVersion()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var output = $"Arch: {ArchitectureString(packageId.Architecture)}\n Sdk: 22000";
            return output;
        }

        private string ArchitectureString(Windows.System.ProcessorArchitecture architecture)
        {
            switch (architecture)
            {
                case Windows.System.ProcessorArchitecture.X86:
                    return "x86";
                case Windows.System.ProcessorArchitecture.X64:
                    return "x64";
                case Windows.System.ProcessorArchitecture.Arm:
                    return "arm";
                case Windows.System.ProcessorArchitecture.Arm64:
                    return "arm64";
                case Windows.System.ProcessorArchitecture.X86OnArm64:
                    return "X86OnArm64";
                case Windows.System.ProcessorArchitecture.Neutral:
                    return "neutral";
                case Windows.System.ProcessorArchitecture.Unknown:
                    return "unknown";
                default:
                    return "???";
            }
        }

        public string GetCustomMenuName()
        {
            var value = SettingHelper.Get<string>("Custom_Menu_Name", "Open With");
            return value;
        }

        public async void SetCustomMenuName(string name)
        {
            await Task.Run(() =>
            {
                SettingHelper.Set("Custom_Menu_Name", name ?? "Open With");
            });
        }

        public int ThemeType
        {
            get
            {
                return _settings.ThemeType;
            }
            set
            {
                _settings.ThemeType = value;

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

    }
}
