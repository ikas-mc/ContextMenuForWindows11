using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.View.Common;
using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace ContextMenuCustomApp.View.Setting
{

    public sealed partial class SettingPage : Page
    {
        private readonly SettingViewModel _viewModel;
        public SettingPage()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            _viewModel = new SettingViewModel();
            this.DataContext = _viewModel;
            this.RegisterMessageHandler(_viewModel);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

    }
}
