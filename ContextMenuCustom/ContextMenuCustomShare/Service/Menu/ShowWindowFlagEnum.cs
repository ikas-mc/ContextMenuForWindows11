using System.ComponentModel;

namespace ContextMenuCustomApp.Service.Menu
{
    public enum ShowWindowFlagEnum
    {
        [Description("Hide")]
        Hide = -1,
        [Description("Show Normal")]
        ShowNormal = 0,
        [Description("Show Minimized")]
        ShowMinimized = 1,
        [Description("Show Maximized")]
        ShowMaximized = 2,
    }
}