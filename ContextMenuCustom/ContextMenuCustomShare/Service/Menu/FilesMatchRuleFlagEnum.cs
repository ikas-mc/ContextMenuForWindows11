using System.ComponentModel;

namespace ContextMenuCustomApp.Service.Menu
{
    public enum FilesMatchRuleFlagEnum
    {
        [Description("Off")]
        Off = 2,
        [Description("Any")]
        Any = 0,
        [Description("All")]
        All = 1,
    }
}
