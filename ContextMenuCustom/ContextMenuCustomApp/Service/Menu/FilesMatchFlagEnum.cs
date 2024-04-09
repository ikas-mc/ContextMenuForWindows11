using System.ComponentModel;

namespace ContextMenuCustomApp.Service.Menu
{
    public enum FilesMatchFlagEnum
    {
        [DescriptionAttribute("Off")]
        None = 0,
        [DescriptionAttribute("Each")]
        Each = 1,
        [DescriptionAttribute("Join")]
        Join = 2,
    }
}