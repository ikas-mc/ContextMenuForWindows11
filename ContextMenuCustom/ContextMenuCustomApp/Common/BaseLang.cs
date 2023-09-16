using ContextMenuCustomApp.View.Common;
using System;
using System.Reflection;

using Windows.Data.Json;
using Windows.Storage;

namespace ContextMenuCustomApp.Common
{
    public abstract class BaseLang
    {

        protected BaseLang()
        {

        }

        public virtual async void Load()
        {
            var lang = SettingHelper.Get<string>("Settings:Local:Lang", "");
            if (string.IsNullOrEmpty(lang)) {
                return;
            }

            var folder = await ApplicationData.Current.LocalFolder.TryGetItemAsync("langs");
            if (null == folder || !folder.IsOfType(StorageItemTypes.Folder))
            {
                return;
            }

            var file = await ((StorageFolder)folder).TryGetItemAsync(lang);
            if (null == file || !file.IsOfType(StorageItemTypes.File))
            {
                return;
            }

            var content = await FileIO.ReadTextAsync((StorageFile)file);
            if (!JsonObject.TryParse(content, out var langJson))
            {
                return;
            }

            var properties = GetType().GetTypeInfo().DeclaredProperties;
            foreach (PropertyInfo propertyInfo in properties)
            {
                var name = propertyInfo.Name;
                var value = langJson.GetNamedString(name);
                if (!string.IsNullOrEmpty(value))
                {
                    propertyInfo.SetValue(this, value);
                }
            }

        }
    }
}
