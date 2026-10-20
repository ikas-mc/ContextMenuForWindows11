using WinRT.Interop;

namespace ContextMenuCustomApp.View.Common
{
    /// <summary>
    /// WinUI 3 requires file pickers to be associated with the window (HWND) they are shown for.
    /// </summary>
    public static class PickerHelper
    {
        public static bool TryAttachWindow(object picker)
        {
            var window = ContextMenuCustom.MainWindow.Instance;
            if (window == null)
            {
                return false;
            }

            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(picker, hwnd);
            return true;
        }
    }
}
