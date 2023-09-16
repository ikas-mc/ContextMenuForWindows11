using ContextMenuCustomApp.View.Common;
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
        private readonly static ApplicationDataContainer settings;

        public readonly static Settings INS;
        static Settings()
        {
            settings = ApplicationData.Current.LocalSettings.CreateContainer("app-settings", ApplicationDataCreateDisposition.Always);
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

        public string AppLang
        {
            get
            {
                return GetValue(nameof(AppLang), "");
            }
            set
            {
                SetValue(nameof(AppLang), value);
            }
        }

        public int ThemeType
        {
            get
            {
                return GetValue(nameof(ThemeType), 0);
            }
            set
            {
                SetValue(nameof(ThemeType), value);
            }
        }

        private T GetValue<T>(string key, T defaultValue = default)
        {
            return SettingHelper.Get<T>(settings, key, defaultValue);
        }


        private T SetValue<T>(string key, T value)
        {
            SettingHelper.Set(settings, key, value);
            return value;
        }


    }
}
