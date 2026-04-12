namespace ContextMenuCustomApp.Common
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
        public string MenuListOpenFolder { get; set; } = "Open Menus Folder";
        public string MenuListOpenFolderTips { get; set; } = "Open Menus Folder";

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
        public string MenuShowWindowOptionHide { get; set; } = "Hide";
        public string MenuShowWindowOptionNormal { get; set; } = "Normal";
        public string MenuShowWindowOptionMin { get; set; } = "Minimized";
        public string MenuShowWindowOptionMax { get; set; } = "Maximized";
        public string MenuRunAsFlag { get; set; } = "Run As";
        public string MenuRunAsFlagTips { get; set; } = "Run as with other user";
        public string MenuRunAsOptionDefault { get; set; } = "Default";
        public string MenuRunAsOptionAdmin { get; set; } = "Admin";
        public string MenuRunAsOptionOther { get; set; } = "Custom user";
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
        public string MenuMatchFilesRule { get; set; } = "Multi File Rule";
        public string MenuMatchFilesRuleOptionOff { get; set; } = "Off (No Check)";
        public string MenuMatchFilesRuleOptionAny { get; set; } = "Any Matched";
        public string MenuMatchFilesRuleOptionAll { get; set; } = "All Matched";
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

        //
        public string MenuFileRenameTitle { get; set; } = "Rename Menu File";
        public string MenuFileRenameInputTips { get; set; } = "Menu File Name";
        public string MenuFileRenameSyncName { get; set; } = "Sync From Title";

        //setting
        public string Setting { get; set; } = "Setting";
        public string SettingOpenButtonTips { get; set; } = "Open Setting";
        public string SettingBackButton { get; set; } = "Back";
        public string SettingBackButtonTips { get; set; } = "Go Back";

        //setting cache
        public string SettingCache { get; set; } = "Enable Cache";
        public string SettingCacheTips { get; set; } = "Enabling caching can improve menu speed. However, menus will no longer be loaded directly from the menu JSON files";
        public string SettingCacheContent { get; set; } = "Caching Menus to Optimize Loading Speed";
        public string SettingCacheTime { get; set; } = "Cache Time:";
        public string SettingCacheButtonOk { get; set; } = "Ok";
    }
}
