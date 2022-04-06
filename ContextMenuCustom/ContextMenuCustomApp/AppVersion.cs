using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace ContextMenuCustomApp
{
    public static class AppVersion
    {
        public static int Current()
        {
            var version = Package.Current.Id.Version;
            return version.Major * 1000 + version.Minor;
        }
    }
}
