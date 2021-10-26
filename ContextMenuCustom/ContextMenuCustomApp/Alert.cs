using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ContextMenuCustomApp
{
    public static class Alert
    {
        public static async void InfoAsync(string content, string title = "Info")
        {
            var dialog = new ContentDialog
            {
                Title = title,
                PrimaryButtonText = "",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                Content = content
            };
            await dialog.ShowAsync();
        }

        public static async Task<bool> ChooseAsync(string content, string title = "")
        {
            var dialog = new ContentDialog
            {
                Title = title,
                PrimaryButtonText = "Ok",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = content
            };
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}
