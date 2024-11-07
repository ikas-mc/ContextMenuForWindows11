﻿namespace ContextMenuCustomApp.Common
{
    public partial class AppLang
    {
        //Common
        public string CommonOk { get; set; } = "Ok";
        public string CommonCancel { get; set; } = "Cancel";
        public string CommonWarning { get; set; } = "Warning";
        public string CommonError { get; set; } = "Error";

        //menu list
        public string MenuListAdd { get; set; } = "Add";
        public string MenuListAddTis { get; set; } = "Add Menu";
        public string MenuListRefresh { get; set; } = "Refresh";
        public string MenuListRefreshTips { get; set; } = "Refresh Menus";
        public string MenuListOpenFolder { get; set; } = "Open Folder";
        public string MenuListOpenFolderTips { get; set; } = "Open Menu Config Folder";

        //menu 
        public string MenuConfigGroup { get; set; } = "Menu Config";
        public string MenuTitle { get; set; } = "Title";
        public string MenuOrder { get; set; } = "Order";
        public string MenuExe { get; set; } = "Exe";
        public string MenuExeOpenTips { get; set; } = "Open";
        public string MenuParam { get; set; } = "Param";
        public string MenuParamTips { get; set; } = "supported variables: {path},{name},{parent},{extension},{nameNoExt}";
        public string MenuIcon { get; set; } = "Icon";
        public string MenuIconLightInputTip { get; set; } = "Icon For Light Theme Or Default";
        public string MenuIconDarkInputTip { get; set; } = "Icon For Dark Theme";
        public string MenuIconOpenTip { get; set; } = "Open";

        //Advanced
        public string MenuConfigAdvancedGroup { get; set; } = "Menu Advanced Config";
        public string MenuShowWindowFlag { get; set; } = "Show Window";
        public string MenuShowWindowFlagTips { get; set; } = "Show or Hide Window...";
        public string MenuWorkingDirectory { get; set; } = "Working Directory";
        public string MenuWorkingDirectoryTips { get; set; } = "empty as default, supported variables: {parent}";

        //folder
        public string MenuMatchFolder { get; set; } = "Match Folder";
        public string MenuMatchFolderDirectory { get; set; } = "Directory";
        public string MenuMatchFolderBackground { get; set; } = "Background";
        public string MenuMatchFolderDesktop { get; set; } = "Desktop";
        public string MenuMatchFolderDrive { get; set; } = "Drive";

        //file
        public string MenuMatchFile { get; set; } = "Match File";
        public string MenuMatchFileOptionExtentionLike { get; set; } = "Extention Like";
        public string MenuMatchFileInputExtentionLikeTips { get; set; } = "Extension Like (empty = all, * = all, .cpp|.cxx match .c or .cpp or .cxx)";
        public string MenuMatchFileOptionNameRegex { get; set; } = "Name Regex";
        public string MenuMatchFileInputNameRegexTips { get; set; } = "Regex for name (.+?\\.txt match x.txt)";
        public string MenuMatchFileOptionExtention { get; set; } = "Extention";
        public string MenuMatchFileInputExtentionTips { get; set; } = "Extension List (use | as delimiter) (.cpp|.cxx match .cpp or .cxx)";
        public string MenuMatchFileInputExtentionWiki { get; set; } = "file extentions";
        public string MenuMatchFileOptionAll { get; set; } = "All";
        public string MenuMatchFileOptionOff { get; set; } = "Off";

        //files
        public string MenuMatchFiles { get; set; } = "Match Files";
        public string MenuMatchFilesTips { get; set; } = "Wiki";
        public string MenuMatchFilesOptionOff { get; set; } = "Off";
        public string MenuMatchFilesOptionEach { get; set; } = "Each";
        public string MenuMatchFilesOptionJoin { get; set; } = "Join";
        public string MenuMatchFilesJoinDelimiterTip { get; set; } = "Path Delimiter";
        public string MenuMatchFilesJoinParamTip { get; set; } = "Param";
        public string MenuMatchFilesJoin { get; set; } = "Join";

        //config file
        public string MenuFileGroup { get; set; } = "Menu File";
        public string MenuFileFileName { get; set; } = "File Name";
        public string MenuFileFileNameTip { get; set; } = "Menu Config File Name";

        //menu command bar
        public string MenuCommandRefresh { get; set; } = "Refresh";
        public string MenuCommandRefreshTips { get; set; } = "Refresh Menu From Config File";
        public string MenuCommandSave { get; set; } = "Save";
        public string MenuCommandSaveTips { get; set; } = "Save Menu";
        public string MenuCommandDelete { get; set; } = "Delete";
        public string MenuCommandDeleteTips { get; set; } = "Delete Menu";
        public string MenuCommandOpen { get; set; } = "Open";
        public string MenuCommandOpenTips { get; set; } = "Open Menu Config File";
        public string MenuCommandRename { get; set; } = "Rename";
        public string MenuCommandRenameTips { get; set; } = "Rename Menu Config File";
        public string MenuCommandClipboardCopy { get; set; } = "Copy";
        public string MenuCommandClipboardCopyTips { get; set; } = "Copy To Clipboard (JSON text)";
        public string MenuCommandClipboardPaste { get; set; } = "Paste";
        public string MenuCommandClipboardPasteTips { get; set; } = "Paste From Clipboard (JSON text)";
        public string MenuCommandHelp { get; set; } = "Wiki";
        public string MenuCommandHelpTips { get; set; } = "Open Wiki";
        public string MenuCommandToggleEnabled { get; set; } = "Toggle Enabled";
        public string MenuCommandToggleEnabledTips { get; set; } = "Enable Or Disable Menu";
        //setting
        public string Setting { get; set; } = "Setting";
        public string SettingOpenButtonTips { get; set; } = "Open Setting";
        public string SettingBackButton { get; set; } = "Back";
        public string SettingBackButtonTips { get; set; } = "Go Back";

        //setting cache
        public string SettingCache { get; set; } = "Cache:";
        public string SettingCacheTips { get; set; } = "Open Cache Settings";
        public string SettingCacheContent { get; set; } = "Caching Menus to Optimize Loading Speed";
        public string SettingCacheTime { get; set; } = "Cache Time:";
        public string SettingCacheButtonOk { get; set; } = "Ok";

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
        public string UpdateTipTitle { get; set; } = "V5.5";
        public string UpdateTipCloseButton { get; set; } = "Close";
        public string UpdateTip1 { get; set; } = "ChangeLog";
        public string UpdateTip1Content { get; set; } = "1. Add ARM64 support \r\n2. Add menu config: working directory\r\n3. Add menu config: show or hide window\r\n4. Add menu config: disable or enable menu\r\n5. Add drive menu support (windows 11 22621+)\r\n6. Optimize language import and add RTL support\r\n7. Optimize UI\r\n8 . Other fix ";
        public string UpdateTip2 { get; set; } = "Tips";
        public string UpdateTip2Content { get; set; } = "1. Restart explorer after update if no menu";
    }
}
