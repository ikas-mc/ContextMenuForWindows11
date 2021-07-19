using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace _7zContextMenu2
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
           
            var primaryColor = SwatchHelper.Lookup[MaterialDesignColor.PinkA400];
            var accentColor = SwatchHelper.Lookup[MaterialDesignColor.PinkA400];
            var theme = Theme.Create(new MaterialDesignLightTheme(), primaryColor, accentColor);
            Resources.SetTheme(theme);

            var activationArgs = AppInstance.GetActivatedEventArgs();
            if (activationArgs != null)
            {
                switch (activationArgs.Kind)
                {
                    case ActivationKind.ShareTarget:
                        HandleShareAsync(activationArgs as ShareTargetActivatedEventArgs);
                        break;
                }
            }

            static async void HandleShareAsync(ShareTargetActivatedEventArgs args)
            {
                var shareOperation = args.ShareOperation;
                shareOperation.ReportCompleted();
            }
        }
    }
}

