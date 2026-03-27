namespace ContextMenuCustomApp.Common
{
    public partial class Settings
    {
        public bool CacheEnabled
        {
            get => MainSettingDao.GetValue(nameof(CacheEnabled), false);
            set => MainSettingDao.SetValue(nameof(CacheEnabled), value);
        }
    }
}
