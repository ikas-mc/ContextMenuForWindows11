using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.View.Common;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ContextMenuCustomApp
{
    sealed partial class App
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            Patchs.Patch1.Run();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }
                Window.Current.Content = rootFrame;
                SetTheme();
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(Shell), e.Arguments);
                }
                Window.Current.Activate();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            if (args.Kind == ActivationKind.CommandLineLaunch)
            {
                if (args is CommandLineActivatedEventArgs commandLineActivatedEventArgs)
                {
                    var arguments = commandLineActivatedEventArgs.Operation.Arguments;
                    MessageDialog dialog = new MessageDialog(arguments);
                    _ = dialog.ShowAsync();
                }
            }
        }

        public void SetTheme()
        {
            ThemeHelper.Initialize();
            var themeType = SettingHelper.Get<int>("Settings:Style:themeType", 0);

            if (themeType == 1)
            {
                ThemeHelper.RootTheme = ElementTheme.Dark;
            }
            else if (themeType == 2)
            {
                ThemeHelper.RootTheme = ElementTheme.Light;
            }
            else
            {
                ThemeHelper.RootTheme = ElementTheme.Default;
            }
        
        }

    }
}
