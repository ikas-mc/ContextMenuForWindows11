using System.ComponentModel;

namespace ContextMenuCustomApp.Service.Menu
{
    public enum ShowWindowFlagEnum
    {
        [DescriptionAttribute("Hide")]
        Hide = -1,
        [DescriptionAttribute("Show Normal")]
        ShowNormal = 0,
        [DescriptionAttribute("Show Minimized")]
        ShowMinimized = 1,
        [DescriptionAttribute("Show Maximized")]
        ShowMaximized = 2,
    }
}