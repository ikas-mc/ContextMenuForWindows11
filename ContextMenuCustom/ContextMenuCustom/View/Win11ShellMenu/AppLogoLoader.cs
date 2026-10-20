using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.Storage.Streams;

namespace ContextMenuCustomApp.View.Win11ShellMenu
{
    public static class AppLogoLoader
    {
        public static RandomAccessStreamReference? GetStreamSource(DependencyObject obj)
        {
            return (RandomAccessStreamReference?)obj.GetValue(StreamSourceProperty);
        }

        public static void SetStreamSource(DependencyObject obj, RandomAccessStreamReference? value)
        {
            obj.SetValue(StreamSourceProperty, value);
        }

        public static readonly DependencyProperty StreamSourceProperty =
            DependencyProperty.RegisterAttached("StreamSource", typeof(RandomAccessStreamReference), typeof(AppLogoLoader), new PropertyMetadata(null, ChangedCallback));

        public async static void ChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var image = d as Image;
            if (image != null && e.NewValue is RandomAccessStreamReference stream)
            {
                var readStream = await stream.OpenReadAsync();
                var bitmap = new BitmapImage
                {
                    DecodePixelWidth = 100,
                    DecodePixelHeight = 100
                };
                await bitmap.SetSourceAsync(readStream);
                image.Source = bitmap;
                return;
            }

            if (null != image)
            {
                image.Source = null;
            }
        }


    }
}
