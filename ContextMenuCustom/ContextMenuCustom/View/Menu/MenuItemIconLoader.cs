using ContextMenuCustomApp.Service.Menu;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;

namespace ContextMenuCustomApp.View.Menu
{
    public static class MenuItemIconReader
    {
        public static async Task<BitmapImage?> ReadIconAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            if (url.StartsWith('"') && url.EndsWith('"'))
            {
                url = url.Trim('"');
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return new BitmapImage(uri);
            }

            int lastComma = url.LastIndexOf(',');
            if (lastComma > 0 && int.TryParse(url.AsSpan(lastComma + 1), out int idx))
            {
                var file = url[..lastComma].Trim('"');
                if (file.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = await Task.Run(() => LoadExeIcon(file, idx));
                    if (null != stream)
                    {
                        var bitmap = new BitmapImage
                        {
                            DecodePixelWidth = 40,
                            DecodePixelHeight = 40
                        };
                        bitmap.SetSource(stream.AsRandomAccessStream());
                        return bitmap;
                    }
                }
            }

            return null;
        }

        public static Stream? LoadExeIcon(string filePath, int iconIndex)
        {
            HICON[] largeIcons = new HICON[1];
            try
            {
                var count = PInvoke.ExtractIconEx(filePath, iconIndex, largeIcons);
                if (count > 0 && largeIcons[0] != IntPtr.Zero)
                {
                    using var icon = System.Drawing.Icon.FromHandle(largeIcons[0]);
                    using var bitmap = icon.ToBitmap();
                    var ms = new MemoryStream();
                    bitmap.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    return ms;
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (largeIcons[0] != IntPtr.Zero)
                {
                    PInvoke.DestroyIcon(largeIcons[0]);
                }
            }

            return null;
        }
    }

    public static class MenuItemIconLoader
    {
        public static MenuItem GetSource(DependencyObject obj)
        {
            return (MenuItem)obj.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject obj, MenuItem value)
        {
            obj.SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(MenuItem), typeof(MenuItemIconLoader), new PropertyMetadata(null, OnSourceChanged));


        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Image image)
            {
                if (e.NewValue is MenuItem menuItem)
                {
                    _ = ApplyAsync(image, menuItem);
                }

                else
                {
                    image.Source = null;
                }
            }
        }

        private static async Task ApplyAsync(Image image, MenuItem menuItem)
        {
            var path = image.ActualTheme == ElementTheme.Dark && !string.IsNullOrEmpty(menuItem.IconDark) ? menuItem.IconDark : menuItem.Icon;

            if (string.IsNullOrEmpty(path))
            {
                image.Source = null;
                return;
            }

            try
            {
                image.Source = await MenuItemIconReader.ReadIconAsync(path);
            }
            catch (Exception)
            {
                image.Source = null;
            }
        }
    }
}
