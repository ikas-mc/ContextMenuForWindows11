using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.Service.Menu;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ContextMenuCustomApp.View.Menu
{
    public sealed partial class MenuFileRenameDialog : UserControl
    {
        private readonly MenuItem _menuItem;

        private AppLang _appLang;
        public MenuFileRenameDialog(MenuItem menuItem)
        {
            this._menuItem = menuItem;
            _appLang = AppContext.Current.AppLang;
            this.InitializeComponent();
        }

        public async Task<(bool, string)> ShowAsync()
        {
            this.FileNameTextBox.Text = _menuItem.File != null ? _menuItem.File.Name : SyncFromMenuTitle(_menuItem.Title);
            UpdateSelection();

            var dialog = new ContentDialog
            {
                Title = _appLang.MenuFileRenameTitle,
                PrimaryButtonText= _appLang.CommonOk,
                CloseButtonText = _appLang.CommonCancel,
                DefaultButton = ContentDialogButton.Primary,
                Content = this
            };
            var result = await dialog.ShowAsync();
            return (result == ContentDialogResult.Primary, this.FileNameTextBox.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.FileNameTextBox.Text = SyncFromMenuTitle(this.FileNameTextBox.Text);
            UpdateSelection();
            this.FileNameTextBox.Focus(FocusState.Programmatic);
        }

        private string SyncFromMenuTitle(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return $"{_menuItem.Title}.json";
            }

            if (name.EndsWith(".json.disabled"))
            {
                return $"{_menuItem.Title}.json.disabled";
            }

            return $"{_menuItem.Title}{Path.GetExtension(name)}";
        }

        private void UpdateSelection()
        {
            var name = this.FileNameTextBox.Text;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            var key = Path.GetFileNameWithoutExtension(name);
            if (name.EndsWith(".json.disabled"))
            {
                key = Path.GetFileNameWithoutExtension(key);
            }

            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            this.FileNameTextBox.SelectionStart = 0;
            this.FileNameTextBox.SelectionLength = key.Length;
        }
    }
}
