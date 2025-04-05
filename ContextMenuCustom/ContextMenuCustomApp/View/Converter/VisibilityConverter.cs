using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ContextMenuCustomApp.View.Converter
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool visible;
            if (value is bool v)
            {
                visible = v;
            }
            else if (value is Visibility visibility)
            {
                visible = Visibility.Visible == visibility;
            }
            else if (value is int || value is short || value is long)
            {
                visible = 0 != (int)value;
            }
            else if (value is float || value is double)
            {
                visible = 0.0 != (double)value;
            }
            else if (value is string str)
            {
                visible = !string.IsNullOrEmpty(str);
            }
            else {
                visible = value != null;
            }

            if (parameter is string p && p == "!")
            {
                visible = !visible;
            }

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}