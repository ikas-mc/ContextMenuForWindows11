using System.ComponentModel;

namespace ContextMenuCustomApp.Service.Menu
{
    public enum FileMatchFlagEnum
    {
        [DescriptionAttribute("Off")]
        None = 0,
        [DescriptionAttribute("Extension Like")]
        Ext = 1,
        [DescriptionAttribute("Name Regex")]
        Regex = 2,
        [DescriptionAttribute("Extension")]
        ExtList = 3,
        [DescriptionAttribute("All")]
        All = 4,
    }
}