using Microsoft.UI.Xaml;

namespace ContextMenuCustomApp.View.Common
{
    public static class ThemeHelper
    {
        private static FrameworkElement? RootElement
        {
            get
            {
                return ContextMenuCustom.MainWindow.Instance?.AppRootFrame;
            }
        }

        public static ElementTheme RootTheme
        {
            get
            {
                if (RootElement is FrameworkElement rootElement)
                {
                    return rootElement.RequestedTheme;
                }

                return ElementTheme.Default;
            }
            set
            {
                if (RootElement is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                }
            }
        }

        public static void Initialize()
        {
        }

        public static bool IsDarkTheme()
        {
            if (RootTheme == ElementTheme.Default)
            {
                return Application.Current.RequestedTheme == ApplicationTheme.Dark;
            }
            return RootTheme == ElementTheme.Dark;
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
