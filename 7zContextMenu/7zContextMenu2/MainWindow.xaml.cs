using System;

using System.Windows;
using Windows.System;

namespace _7zContextMenu2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            _=await Launcher.LaunchUriAsync(new Uri("ms-settings:about"));
        }
    }
}
