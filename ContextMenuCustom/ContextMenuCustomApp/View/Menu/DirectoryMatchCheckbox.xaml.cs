using ContextMenuCustomApp.Service.Menu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ContextMenuCustomApp.View.Menu
{
    public sealed partial class DirectoryMatchCheckbox : UserControl
    {
        public DirectoryMatchCheckbox()
        {
            this.InitializeComponent();
        }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(DirectoryMatchCheckbox), new PropertyMetadata(0, (o, v) =>
        {
            Debug.WriteLine($"BitMaskCheckbox value={v.NewValue}");
            var self = (DirectoryMatchCheckbox)o;
            if (v.NewValue is int value)
            {
                self.DirectoryCheckbox.IsChecked = (value & (int)DirectoryMatchFlagEnum.Directory) == (int)DirectoryMatchFlagEnum.Directory;
                self.BackgroundCheckbox.IsChecked = (value & (int)DirectoryMatchFlagEnum.Background) == (int)DirectoryMatchFlagEnum.Background;
                self.DesktopCheckbox.IsChecked = (value & (int)DirectoryMatchFlagEnum.Desktop) == (int)DirectoryMatchFlagEnum.Desktop;
            }
            else
            {
                self.DirectoryCheckbox.IsChecked = false;
                self.BackgroundCheckbox.IsChecked = false;
                self.DesktopCheckbox.IsChecked = false;
            }
        }));

        private void Checkbox_Click(object sender, RoutedEventArgs e)
        {
            this.Value = (DirectoryCheckbox.IsChecked == true ? (int)DirectoryMatchFlagEnum.Directory : 0)
                | (BackgroundCheckbox.IsChecked == true ? (int)DirectoryMatchFlagEnum.Background : 0)
                | (DesktopCheckbox.IsChecked == true ? (int)DirectoryMatchFlagEnum.Desktop : 0);
        }
    }
}
