using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.View.Common;
using Microsoft.UI.Xaml;

namespace ContextMenuCustom
{
    public partial class App : Application
    {
        private MainWindow? _window;

        public App()
        {
            ContextMenuCustomApp.AppContext.Init();
            InitializeComponent();
            ContextMenuCustomApp.Patchs.Patch1.Run();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            ContextMenuCustomApp.AppContext.WaitAll();

            _window = new MainWindow();

            SetTheme();
            SetFlowDirection();
            _window.Activate();
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

        public void SetFlowDirection()
        {
            var rootFrame = MainWindow.Instance?.AppRootFrame;
            if (rootFrame == null)
            {
                return;
            }

            var enabldRTL = Settings.Default.RTLEnablded;
            rootFrame.FlowDirection = enabldRTL ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }
    }
}
