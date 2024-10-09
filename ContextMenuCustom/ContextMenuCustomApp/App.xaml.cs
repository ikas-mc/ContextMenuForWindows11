using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.View.Common;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
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
            AppContext.Current.Init();
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            Patchs.Patch1.Run();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            AppContext.Current.WaitAll();

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
                SetFlowDirection(rootFrame);
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

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            if (args.Kind == ActivationKind.CommandLineLaunch)
            {
                if (args is CommandLineActivatedEventArgs commandLineActivatedEventArgs)
                {
                    var arguments = commandLineActivatedEventArgs.Operation.Arguments;
                    var dialog = new MessageDialog(arguments);
                    dialog.Commands.Add(new UICommand("Copy To Clipboard", (e) => {
                        var dataPackage = new DataPackage
                        {
                            RequestedOperation = DataPackageOperation.Copy
                        };
                        dataPackage.SetText(arguments);
                        Clipboard.SetContent(dataPackage);
                    }));
                    dialog.Commands.Add(new UICommand("Close"));
                    _ = await dialog.ShowAsync();
                }
            }
        }

        public void SetTheme()
        {
            ThemeHelper.Initialize();
            var themeType = Settings.Default.ThemeType;

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

        public void SetFlowDirection(Frame rootFrame)
        {
            var enabldRTL = Settings.Default.RTLEnablded;
            rootFrame.FlowDirection= enabldRTL ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

    }
}
