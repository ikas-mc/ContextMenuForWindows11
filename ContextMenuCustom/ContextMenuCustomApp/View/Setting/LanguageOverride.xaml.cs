using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.Service.Lang;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ContextMenuCustomApp.View.Setting
{
    public sealed partial class LanguageOverride : UserControl
    {
        private SettingViewModel _settingViewModel;

        public readonly AppLang AppLang;

        public LanguageOverride()
        {
            AppLang = AppContext.Current.AppLang;
            this.InitializeComponent();
            Loaded += Control_Loaded;
        }

        private async void Control_Loaded(object sender, RoutedEventArgs e)
        {
            _settingViewModel = (SettingViewModel)DataContext;
            await _settingViewModel.LoadLanguages();

            LanguageOverrideComboBox.SelectedItem = _settingViewModel.GetCurrentLang();
            LanguageOverrideComboBox.SelectionChanged += LanguageOverrideComboBox_SelectionChanged;
        }

        private void LanguageOverrideComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem is LangInfo langInfo)
            {
                _settingViewModel.UpdateLangSetting(langInfo);
            }
        }
    }
}