using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace ContextMenuCustomApp.View.Common
{
    public static class Alert
    {
        private static XamlRoot? GetXamlRoot()
        {
            return ContextMenuCustom.MainWindow.Instance?.AppRootFrame?.XamlRoot;
        }

        public static async void InfoAsync(string content, string title = "Info", string closeButton = "Cancel")
        {
            var dialog = new ContentDialog
            {
                Title = title,
                PrimaryButtonText = "",
                CloseButtonText = closeButton,
                DefaultButton = ContentDialogButton.Close,
                Content = content,
                XamlRoot = GetXamlRoot()
            };
            await dialog.ShowAsync();
        }

        public static async Task<bool> ChooseAsync(string content, string title = "", string primaryButton = "Ok", string closeButton = "Cancel")
        {
            var dialog = new ContentDialog
            {
                Title = title,
                PrimaryButtonText = primaryButton,
                CloseButtonText = closeButton,
                DefaultButton = ContentDialogButton.Primary,
                Content = content,
                XamlRoot = GetXamlRoot()
            };
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}
