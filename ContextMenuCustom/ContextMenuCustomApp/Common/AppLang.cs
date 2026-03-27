namespace ContextMenuCustomApp.Common
{
    public partial class AppLang
    {


        //setting about
        public string SettingAbout { get; set; } = "About";
        public string SettingAboutVersion { get; set; } = "Version";
        public string SettingAboutSystem { get; set; } = "System";
        public string SettingAboutAuthor { get; set; } = "ikas@2024";
        public string SettingAboutHelp { get; set; } = "Wiki";
        public string SettingAboutIssue { get; set; } = "Issue";
        public string SettingAboutChangeLog { get; set; } = "ChangeLog";
        public string SettingAboutPrivacy { get; set; } = "Privacy";

        //setting style
        public string SettingStyleGroup { get; set; } = "Style";
        public string SettingStyleTheme { get; set; } = "Theme";
        public string SettingStyleThemeAuto { get; set; } = "Auto";
        public string SettingStyleThemeDark { get; set; } = "Dark";
        public string SettingStyleThemeLight { get; set; } = "Light";

        //setting menu
        public string SettingMenuGroup { get; set; } = "Menu";
        public string SettingMenuTitle { get; set; } = "Menu Title";
        public string SettingMenuTitleInputTip { get; set; } = "Menu Title";

        //setting icon
        public string SettingMenuIcon { get; set; } = "Menu Icon";
        public string SettingMenuIconPathOpenTip { get; set; } = "Open";
        public string SettingMenuIconLightInputTip { get; set; } = "Icon path For Light theme";
        public string SettingMenuIconDarkInputTip { get; set; } = "Icon path For Dark theme";

        //setting language
        public string SettingLanguagesGroup { get; set; } = "Languages";
        public string SettingLanguages { get; set; } = "App Language";
        public string SettingLanguagesHelp { get; set; } = "How To Add Custom Languages";
        public string SettingLanguagesShare { get; set; } = "Share or Download Languages";
        public string SettingLanguagesReloadTip { get; set; } = "Reload Custom Languages";
        public string SettingLanguagesExportTip { get; set; } = "Export Current Language";
        public string SettingLanguagesImportTip { get; set; } = "Import Language";
        public string SettingLanguagesFolderOpenTip { get; set; } = "Open Languages Folder";
        public string SettingLanguagesEnableRtl { get; set; } = "Enable RTL";

        //setting data
        public string SettingDataGroup { get; set; } = "Data";
        public string SettingEnableDebug { get; set; } = "Enable Debug";
        public string SettingEnableDebugTip { get; set; } = "Enable debug log, use DebugView to view logs";
        public string SettingEnableDebugHelp { get; set; } = "Wiki";
        public string SettingDataTitle { get; set; } = "App Data";
        public string SettingDataButton { get; set; } = "Open Data Folder";
        public string SettingDataButtonTip { get; set; } = "Open Data Folder";

        //setting app
        public string SettingOther { get; set; } = "Other";
        public string SettingOtherRestart { get; set; } = "Restart App";

        //update tip
        public string UpdateTipTitle { get; set; } = "V5.7";
        public string UpdateTipCloseButton { get; set; } = "Close";
        public string UpdateTip1 { get; set; } = "ChangeLog";
        public string UpdateTip1Content { get; set; } = "1. Add environment variable support for icon path\r\n2. Fix file extension list matching bug\r\n3. Update sdk and other fix";
        public string UpdateTip2 { get; set; } = "Tips";
        public string UpdateTip2Content { get; set; } = "1. Restart explorer after update if no menu";
    }
}
