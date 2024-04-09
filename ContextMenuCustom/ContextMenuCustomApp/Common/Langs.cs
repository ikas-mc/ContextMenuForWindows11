using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextMenuCustomApp.Common
{
    public class Langs 
    {
        public string AppName { get; set; } = "Custom Context Menu";

        //menu list
        public string MenuListAdd { get; set; } = "Add";
        public string MenuListAddTis { get; set; } = "Add Menu";
        public string MenuListRefresh { get; set; } = "Refresh";
        public string MenuListRefreshTips { get; set; } = "Refresh Menus";
        public string MenuListOpenFolder { get; set; } = "Open Folder";
        public string MenuListOpenFolderTips { get; set; } = "Refresh Menus";

        //menu 
        public string MenuTitle { get; set; } = "Title";
        public string MenuOrder { get; set; } = "Order";
        public string MenuExe { get; set; } = "Exe";
        public string MenuExeTips { get; set; } = "Open";
        public string MenuParam { get; set; } = "Param";
        public string MenuIcon { get; set; } = "Icon";
        public string MenuMatchFolder { get; set; } = "Match Folder";
        public string MenuMatchFile { get; set; } = "Match File";
        public string MenuMatchFileInputExtentions { get; set; } = "File Extentions(use * to match all)";
        public string MenuMatchFileOptionOff { get; set; } = "Off";
        public string MenuMatchFileOptionExt { get; set; } = "Ext";
        public string MenuMatchFileOptionRegex { get; set; } = "Regex";
        public string MenuMatchFiles { get; set; } = "Match Files";
        public string MenuMatchFilesTips { get; set; } = "Read Wiki!";
        public string MenuMatchFilesOptionOff { get; set; } = "Off";
        public string MenuMatchFilesOptionEach { get; set; } = "Each";
        public string MenuMatchFilesOptionJoin { get; set; } = "Join";

        //menu command bar
        public string MenuCommandSave { get; set; } = "Save";
        public string MenuCommandSaveTips { get; set; } = "Save";
        public string MenuCommandOpen { get; set; } = "Open File";
        public string MenuCommandOpenTips { get; set; } = "Open File";
        public string MenuCommandDelete { get; set; } = "Delete";
        public string MenuCommandDeleteTips { get; set; } = "Delete";
        public string MenuCommandHelp { get; set; } = "Help";
        public string MenuCommandHelpTips { get; set; } = "Help";

        //setting
        public string Setting { get; set; } = "Setting";
        public string SettingBackButton { get; set; } = "Back";
        public string SettingBackButtonTips { get; set; } = "Go Back";

        //setting cache
        public string SettingCache { get; set; } = "Cache:";
        public string SettingCacheAbout { get; set; } = "About Cache:";
        public string SettingCacheTips { get; set; } = "";
        public string SettingCacheTime { get; set; } = "Cache Time:";
        public string SettingCacheButtonOk { get; set; } = "Got it !";

        //setting about
        public string SettingAbout { get; set; } = "About";
        public string SettingAboutersion { get; set; } = "Version";
        public string SettingAboutSystem { get; set; } = "System";
        public string SettingAboutHelp { get; set; } = "Help";
        public string SettingAboutSource { get; set; } = "Source";
        public string SettingAboutPrivacy { get; set; } = "Privacy";

        //setting style
        public string SettingStyle { get; set; } = "Style";
        public string SettingStyleTheme { get; set; } = "Theme";
        public string SettingStyleThemeAuto { get; set; } = "Auto";
        public string SettingStyleThemeDark { get; set; } = "Dark";
        public string SettingStyleThemeLight { get; set; } = "Light";

        //setting menu
        public string SettingMenu { get; set; } = "Menu";
        public string SettingMenuTitle { get; set; } = "Context Menu Title";
        public string SettingMenuInputTitle { get; set; } = "Menu Title";

        //setting data
        public string SettingData { get; set; } = "Data";
        public string SettingDataTitle { get; set; } = "App Data";
        public string SettingDataButton { get; set; } = "Open Data Folder";

        //setting lang
        public string SettingLang { get; set; } = "Languages";
        public string SettingLangTitle { get; set; } = "Select Language";

    }
}
