using System.ComponentModel;

namespace ContextMenuCustomApp.Service.Menu
{
    public enum FilesMatchFlagEnum
    {
        [Description("Off")]
        None = 0,
        [Description("Each")]
        Each = 1,
        [Description("Join")]
        Join = 2,
    }
}