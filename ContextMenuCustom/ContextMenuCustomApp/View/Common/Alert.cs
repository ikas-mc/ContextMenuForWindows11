using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ContextMenuCustomApp.View.Common
{
    public static class Alert
    {
        public static async void InfoAsync(string content, string title = "Info", string closeButton = "Cancel")
        {
            var dialog = new ContentDialog
            {
                Title = title,
                PrimaryButtonText = "",
                CloseButtonText = closeButton,
                DefaultButton = ContentDialogButton.Close,
                Content = content
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
                Content = content
            };
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}
