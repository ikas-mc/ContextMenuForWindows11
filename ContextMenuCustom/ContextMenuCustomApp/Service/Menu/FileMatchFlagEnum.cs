using System.ComponentModel;

namespace ContextMenuCustomApp.Service.Menu
{
    public enum FileMatchFlagEnum
    {
        [Description("Off")]
        None = 0,
        [Description("Extension Like")]
        Ext = 1,
        [Description("Name Regex")]
        Regex = 2,
        [Description("Extension")]
        ExtList = 3,
        [Description("All")]
        All = 4
    }
}