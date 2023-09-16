using Windows.Storage;

namespace ContextMenuCustomApp.View.Common
{
    public sealed class SettingHelper
    {
        public static void Set(string key, object value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }

        public static T Get<T>(string propertyName, T defaultValue)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                var value = ApplicationData.Current.LocalSettings.Values[propertyName];
                if (null != value)
                {
                    return (T)value;
                }
            }
            return defaultValue;
        }
    }
}
