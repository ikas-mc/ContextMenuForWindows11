using Windows.Storage;

namespace ContextMenuCustomApp.Common
{
    public class AppDataSettings
    {
        private readonly ApplicationDataContainer _container;

        public AppDataSettings(string typeName)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            _container = localSettings.CreateContainer(typeName, ApplicationDataCreateDisposition.Always);
        }

        public AppDataSettings()
        {
            _container = ApplicationData.Current.LocalSettings;
        }

        public T GetValue<T>(string propertyName, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return defaultValue;
            }

            var value = _container.Values[propertyName];
            if (null != value)
            {
                return (T)value;
            }

            return defaultValue;
        }

        public bool SetValue<T>(string propertyName, T value = default)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return false;
            }

            _container.Values[propertyName] = value;
            return true;
        }
    }
}