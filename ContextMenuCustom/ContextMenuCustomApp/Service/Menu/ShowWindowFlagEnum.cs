using System.ComponentModel;

namespace ContextMenuCustomApp.Service.Menu
{
    public enum ShowWindowFlagEnum
    {
        [DescriptionAttribute("Hide")]
        Hide = 0,
        [DescriptionAttribute("Show Normal")]
        ShowNormal = 1,
        [DescriptionAttribute("Show Minimized")]
        ShowMinimized = 2,
        [DescriptionAttribute("Show Maximized")]
        ShowMaximized = 3,
    }
}