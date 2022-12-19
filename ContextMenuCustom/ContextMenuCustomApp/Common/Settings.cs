using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace ContextMenuCustomApp.Common
{

    public class Settings
    {
        private readonly static IPropertySet settings;

        public readonly static Settings INS;
        static Settings()
        {
            settings = ApplicationData.Current.LocalSettings.CreateContainer("app-settings", ApplicationDataCreateDisposition.Always).Values;
            INS = new Settings();
        }

        public bool CacheEnabled
        {
            get
            {
                return GetValue(nameof(CacheEnabled), false);
            }
            set
            {
                SetValue(nameof(CacheEnabled), value);
            }
        }


        public int PatchVersion
        {
            get
            {
                return GetValue(nameof(PatchVersion), 0);
            }
            set
            {
                SetValue(nameof(PatchVersion), value);
            }
        }

        private T GetValue<T>(string key, T defaultValue = default)
        {
            if (settings[key] is T value)
            {
                return value;
            }

            return defaultValue;
        }


        private T SetValue<T>(string key, T value)
        {
            settings[key] = value;
            return value;
        }


    }
}
