using System.ComponentModel;

namespace ContextMenuCustomApp.Service.Menu
{
    public enum FilesMatchRuleFlagEnum
    {
        [Description("Any")]
        Any = 0,
        [Description("One")]
        One = 1,
        [Description("All")]
        All = 2,
    }
}
