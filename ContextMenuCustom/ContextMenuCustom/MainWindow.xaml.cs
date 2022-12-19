// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ContextMenuCustom
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
         
            var folder = await GetMenusFolderAsync();
            _ = await Launcher.LaunchFolderAsync(folder);
        }

        private const string MenusFolderName = "custom_commands";
        public async Task<StorageFolder> GetMenusFolderAsync()
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(MenusFolderName);
            if (item is StorageFile)
            {
                await item.DeleteAsync();
            }
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync(MenusFolderName, CreationCollisionOption.OpenIfExists);
        }
    }
}
