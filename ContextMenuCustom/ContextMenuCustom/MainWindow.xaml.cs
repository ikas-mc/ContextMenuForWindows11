using ContextMenuCustomApp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ContextMenuCustom
{
    public sealed partial class MainWindow : Window
    {
        public static MainWindow? Instance
        {
            get; private set;
        }

        public Frame AppRootFrame => RootFrame;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            RootFrame.Navigate(typeof(Shell));
        }
    }
}
