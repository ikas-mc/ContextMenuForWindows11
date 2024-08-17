using Newtonsoft.Json;
using System.ComponentModel;

namespace ContextMenuCustomApp.Service.Menu
{
    public enum ShowWindowFlagEnum
    {
        [DescriptionAttribute("Hide")]
        HIDE = 0,
        [DescriptionAttribute("Show Normal")]
        SHOWNORMAL = 1,
        [DescriptionAttribute("Show Minimized")]
        SHOWMINIMIZED = 2,
        [DescriptionAttribute("Show Maximized")]
        SHOWMAXIMIZED = 3,
    }
}