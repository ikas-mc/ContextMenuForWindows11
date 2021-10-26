using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using ContextMenuCustomApp.Service.Menu;

namespace ContextMenuCustomApp.View.Menu
{
    public sealed partial class MenuPage
    {
        private MenuPageViewModel _viewModel;

        public MenuPage()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            InitializeComponent();
            _viewModel = new MenuPageViewModel(OnException);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _ = _viewModel.LoadAsync();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var item = _viewModel.New();
            CommandList.SelectedItem = item;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is MenuItem item)
            {
                await _viewModel.SaveAsync(item);
                //TODO 
                //CommandList.SelectedItem = item;
            }
            else
            {
                Alert.InfoAsync("no selected item");
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is MenuItem item)
            {
                await _viewModel.DeleteAsync(item);
            }
            else
            {
                Alert.InfoAsync("no selected item");
            }
        }

        private async void Open_Folder_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.OpenMenusFolderAsync();
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is MenuItem item)
            {
                await _viewModel.OpenMenuFileAsync(item);
            }
            else
            {
                Alert.InfoAsync("no selected item");
            }
        }

        private void OnException(Exception e, string message)
        {
            Alert.InfoAsync(message ?? e.Message, "Error");
        }

    }
}
