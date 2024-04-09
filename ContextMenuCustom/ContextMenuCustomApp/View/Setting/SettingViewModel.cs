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
        public readonly Settings settings = Settings.Default;
        public SettingViewModel()
        {

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

        public int ThemeType
        {
            get
            {
                return settings.ThemeType;
            }
            set
            {
                settings.ThemeType = value;

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
