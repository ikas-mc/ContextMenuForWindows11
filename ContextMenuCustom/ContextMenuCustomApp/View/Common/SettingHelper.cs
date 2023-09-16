using Windows.Storage;

namespace ContextMenuCustomApp.View.Common
{
    public sealed class SettingHelper
    {
        public static void Set(string key, object value)
        {
            Set(ApplicationData.Current.LocalSettings, key, value);
        }

        public static T Get<T>(string key, T defaultValue)
        {
            return Get<T>(ApplicationData.Current.LocalSettings, key, defaultValue);
        }

        public static void Set(ApplicationDataContainer container, string key, object value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                container.Values[key] = value;
            }
        }

        public static T Get<T>(ApplicationDataContainer container, string key, T defaultValue)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var value = container.Values[key];
                if (null != value && value is T t)
                {
                    return t;
                }
            }
            return defaultValue;
        }
    }
}
