using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace ContextMenuCustomApp.Common
{
    public abstract class BaseLang
    {
        private readonly ResourceLoader _resourceLoader;
        protected BaseLang()
        {
#if WIN_UI
            _resourceLoader = new ResourceLoader();
#else
            _resourceLoader = ResourceLoader.GetForCurrentView();
#endif
        }

        public virtual void Load()
        {

            if (null != _resourceLoader)
            {
                var properties = GetType().GetTypeInfo().DeclaredProperties;
                foreach (PropertyInfo propertyInfo in properties)
                {
                    var name = propertyInfo.Name;
                    var value = _resourceLoader.GetString(name);
                    if (string.IsNullOrEmpty(value))
                    {
                        value = name;
                    }
                    propertyInfo.SetValue(this, value);
                }
            }
        }

        public string GetString(string key, string de = "")
        {
            string value = _resourceLoader.GetString(key);
            return value ?? de;
        }

        public string GetStringForUri(Uri uri, string de = "")
        {
            string value = _resourceLoader.GetStringForUri(uri);
            return value ?? de;
        }
    }
}
