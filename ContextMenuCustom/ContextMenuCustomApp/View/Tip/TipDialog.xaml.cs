using System.Threading.Tasks;
using System;
using Windows.UI.Xaml.Controls;

namespace ContextMenuCustomApp
{
    public sealed partial class TipDialog : UserControl
    {
        public TipDialog()
        {
            this.InitializeComponent();
        }

        public string Tip1 { get; set; }
        public string Tip1Content { get; set; }
        public string Tip2 { get; set; }
        public string Tip2Content { get; set; }
        public async Task ShowAsync()
        {
            var dialog = new ContentDialog
            {
                Title = AppContext.Current.AppLang.UpdateTipTitle,
                CloseButtonText = AppContext.Current.AppLang.UpdateTipCloseButton,
                DefaultButton = ContentDialogButton.Close,
                Content = this
            };
            await dialog.ShowAsync();
        }

        public static TipDialog CreateUpldateTipDialog()
        {
            var appLang=AppContext.Current.AppLang;
            return new TipDialog
            {
                Tip1 = appLang.UpdateTip1,
                Tip1Content = appLang.UpdateTip1Content,
                Tip2 = appLang.UpdateTip2,
                Tip2Content = appLang.UpdateTip2Content
            };
        }
    }
}