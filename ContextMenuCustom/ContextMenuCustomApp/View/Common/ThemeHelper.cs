using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace ContextMenuCustomApp.View.Common
{
    public static class ThemeHelper
    {
        private static Window _currentApplicationWindow;

        public static ElementTheme RootTheme
        {
            get
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    return rootElement.RequestedTheme;
                }

                return ElementTheme.Default;
            }
            set
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                }

                UpdateSystemCaptionButtonColors();
            }
        }

        public static void Initialize()
        {
            _currentApplicationWindow = Window.Current;
        }

        private static void UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            if (_currentApplicationWindow != null)
            {
                _ = _currentApplicationWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, UpdateSystemCaptionButtonColors);
            }
        }

        public static bool IsDarkTheme()
        {
            if (RootTheme == ElementTheme.Default)
            {
                return Application.Current.RequestedTheme == ApplicationTheme.Dark;
            }
            return RootTheme == ElementTheme.Dark;
        }

        public static void UpdateSystemCaptionButtonColors()
        {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            if (IsDarkTheme())
            {
                titleBar.ButtonForegroundColor = Colors.White;
            }
            else
            {
                titleBar.ButtonForegroundColor = Colors.Black;
            }

        }

        public static void EnableSound(bool enable, bool withSpatial = false)
        {
            ElementSoundPlayer.State = enable ? ElementSoundPlayerState.On : ElementSoundPlayerState.Off;

            if (!withSpatial)
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }
            else
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
            }

        }

    }
}
