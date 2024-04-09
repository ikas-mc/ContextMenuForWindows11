using ContextMenuCustomApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace ContextMenuCustomApp.Patchs
{
    public class Patch1
    {
        public static void Run()
        {
            var appVersion = AppVersion.Current();
            var patchVersion = Settings.Default.PatchVersion;
            if (patchVersion > 0 && patchVersion < 2003)
            {
                FixCacheSetting();
            }
          
            if (patchVersion != appVersion)
            {
                Settings.Default.PatchVersion = appVersion;
            }
        }

        private static void FixCacheSetting()
        {
            try
            {
                var cacheTimeValue = ApplicationData.Current.LocalSettings.Values["Cache_Time"];
                if (cacheTimeValue is string cacheTime)
                {
                    Settings.Default.CacheEnabled = !string.IsNullOrEmpty(cacheTime);
                }
            }
            catch (Exception )
            {
                // ignored
            }
        }
       
    }
}