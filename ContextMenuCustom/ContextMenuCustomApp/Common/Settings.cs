namespace ContextMenuCustomApp.Common
{
    public class Settings
    {
        private static readonly AppDataSettings MainSettingDao;
        private static readonly AppDataSettings DllSettingDao;

        public static readonly Settings Default;
        static Settings()
        {
            MainSettingDao = new AppDataSettings("app-settings");
            DllSettingDao = new AppDataSettings();
            Default = new Settings();
        }

        public bool CacheEnabled
        {
            get => MainSettingDao.GetValue(nameof(CacheEnabled), false);
            set => MainSettingDao.SetValue(nameof(CacheEnabled), value);
        }

        public int PatchVersion
        {
            get => MainSettingDao.GetValue(nameof(PatchVersion), 0);
            set => MainSettingDao.SetValue(nameof(PatchVersion), value);
        }

        public int AppVersion
        {
            get => MainSettingDao.GetValue(nameof(AppVersion), 0);
            set => MainSettingDao.SetValue(nameof(AppVersion), value);
        }

        public string AppLang
        {
            get => MainSettingDao.GetValue(nameof(AppLang), "");
            set => MainSettingDao.SetValue(nameof(AppLang), value);
        }

        public int ThemeType
        {
            get => MainSettingDao.GetValue(nameof(ThemeType), 0);
            set => MainSettingDao.SetValue(nameof(ThemeType), value);
        }

        public bool RTLEnablded
        {
            get => MainSettingDao.GetValue(nameof(RTLEnablded), false);
            set => MainSettingDao.SetValue(nameof(RTLEnablded), value);
        }

        public string MenuName
        {
            get => DllSettingDao.GetValue("Custom_Menu_Name", "Open With");
            set => DllSettingDao.SetValue("Custom_Menu_Name", value);
        }

        public string MenuDarkIcon
        {
            get => DllSettingDao.GetValue("Custom_Menu_Dark_Icon", string.Empty);
            set => DllSettingDao.SetValue("Custom_Menu_Dark_Icon", value);
        }

        public string MenuLightIcon
        {
            get => DllSettingDao.GetValue("Custom_Menu_Light_Icon", string.Empty);
            set => DllSettingDao.SetValue("Custom_Menu_Light_Icon", value);
        }

        public bool EnableDebug
        {
            get => DllSettingDao.GetValue("Custom_Menu_Enable_Debug", false);
            set => DllSettingDao.SetValue("Custom_Menu_Enable_Debug", value);
        }

        public T GetValue<T>(string key, T defaultValue = default)
        {
            return MainSettingDao.GetValue(key, defaultValue);
        }

        public T SetValue<T>(string key, T value)
        {
            MainSettingDao.SetValue(key, value);
            return value;
        }

    }
}
