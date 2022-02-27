using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ContextMenuCustomApp.View.Common
{
    public class EqualsVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (null == value || parameter == null)
            {
                return Visibility.Collapsed;
            }
            return object.Equals(value.ToString(), parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}