using ContextMenuCustomApp.View.Common;
using Windows.Data.Json;
using Windows.Storage;

namespace ContextMenuCustomApp.Service.Menu
{
    enum MultipleFilesFlagEnum
    {
        OFF, EACH, JOIN
    }
    enum AcceptFileFlagEnum
    {
        OFF, EXT, REGEX
    }


    public class MenuItem : BaseModel
    {
        public StorageFile File { get; set; }

        private string _title;
        private string _exe;
        private string _param;
        private string _icon;
        private int _index;

        private bool _acceptDirectory;

        private bool _acceptFile;
        private int _acceptFileFlag;
        private string _acceptExts;
        private string _acceptFileRegex;

        private string _pathDelimiter;
        private string _paramForMultipleFiles;
        private int _acceptMultipleFilesFlag;


        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Exe { get => _exe; set => SetProperty(ref _exe, value); }
        public string Param { get => _param; set => SetProperty(ref _param, value); }
        public string Icon { get => _icon; set => SetProperty(ref _icon, value); }

        public bool AcceptDirectory { get => _acceptDirectory; set => SetProperty(ref _acceptDirectory, value); }

        public bool AcceptFile { get => _acceptFile; set => SetProperty(ref _acceptFile, value); }
        public int AcceptFileFlag { get => _acceptFileFlag; set => SetProperty(ref _acceptFileFlag, value); }
        public string AcceptExts { get => _acceptExts; set => SetProperty(ref _acceptExts, string.IsNullOrEmpty(value) ? value : value.ToLower()); }// to lower for match
        public string AcceptFileRegex { get => _acceptFileRegex; set => SetProperty(ref _acceptFileRegex, value); }

        public int AcceptMultipleFilesFlag { get => _acceptMultipleFilesFlag; set => SetProperty(ref _acceptMultipleFilesFlag, value); }
        public string PathDelimiter { get => _pathDelimiter; set => SetProperty(ref _pathDelimiter, value); }
        public string ParamForMultipleFiles { get => _paramForMultipleFiles; set => SetProperty(ref _paramForMultipleFiles, value); }

        public int Index { get => _index; set => SetProperty(ref _index, value); }

        private static string NameToJsonKey(string name)
        {
            return name[0].ToString().ToLower() + name.Substring(1);
        }

        public static MenuItem ReadFromJson(string content)
        {
            var json = JsonObject.Parse(content);
            var menu = new MenuItem
            {
                Title = json.GetNamedString(NameToJsonKey(nameof(Title)), "menu"),
                Exe = json.GetNamedString(NameToJsonKey(nameof(Exe)), string.Empty),
                Param = json.GetNamedString(NameToJsonKey(nameof(Param)), string.Empty),
                Icon = json.GetNamedString(NameToJsonKey(nameof(Icon)), string.Empty),
                Index = (int)json.GetNamedNumber(NameToJsonKey(nameof(Index)), 0),

                //sigle folder
                AcceptDirectory = json.GetNamedBoolean(NameToJsonKey(nameof(AcceptDirectory)), false),

                //sigle file
                AcceptFile = json.GetNamedBoolean(NameToJsonKey(nameof(AcceptFile)), false),//set false default v3.6
                AcceptFileFlag = (int)json.GetNamedNumber(NameToJsonKey(nameof(AcceptFileFlag)), (int)AcceptFileFlagEnum.OFF),
                AcceptExts = json.GetNamedString(NameToJsonKey(nameof(AcceptExts)), string.Empty),
                AcceptFileRegex = json.GetNamedString(NameToJsonKey(nameof(AcceptFileRegex)), string.Empty),

                //mult files
                AcceptMultipleFilesFlag = (int)json.GetNamedNumber(NameToJsonKey(nameof(AcceptMultipleFilesFlag)), (int)MultipleFilesFlagEnum.OFF),
                PathDelimiter = json.GetNamedString(NameToJsonKey(nameof(PathDelimiter)), string.Empty),
                ParamForMultipleFiles = json.GetNamedString(NameToJsonKey(nameof(ParamForMultipleFiles)), string.Empty),

            };

            //update from old version v3.6
            if (menu.AcceptFileFlag == (int)AcceptFileFlagEnum.OFF && menu.AcceptFile)
            {
                menu.AcceptFileFlag = (int)AcceptFileFlagEnum.EXT;
            }

            return menu;
        }


        public static string WriteToJson(MenuItem content)
        {
            var json = new JsonObject
            {
                [NameToJsonKey(nameof(Title))] = JsonValue.CreateStringValue(content.Title),
                [NameToJsonKey(nameof(Exe))] = JsonValue.CreateStringValue(content.Exe ?? string.Empty),
                [NameToJsonKey(nameof(Param))] = JsonValue.CreateStringValue(content.Param ?? string.Empty),
                [NameToJsonKey(nameof(Icon))] = JsonValue.CreateStringValue(content.Icon ?? string.Empty),
                [NameToJsonKey(nameof(Index))] = JsonValue.CreateNumberValue(content.Index),

                [NameToJsonKey(nameof(AcceptDirectory))] = JsonValue.CreateBooleanValue(content.AcceptDirectory),

                [NameToJsonKey(nameof(AcceptExts))] = JsonValue.CreateStringValue(content.AcceptExts ?? string.Empty),
                [NameToJsonKey(nameof(AcceptFileFlag))] = JsonValue.CreateNumberValue(content.AcceptFileFlag),
                [NameToJsonKey(nameof(AcceptFileRegex))] = JsonValue.CreateStringValue(content.AcceptFileRegex ?? string.Empty),

                [NameToJsonKey(nameof(AcceptMultipleFilesFlag))] = JsonValue.CreateNumberValue(content.AcceptMultipleFilesFlag),
                [NameToJsonKey(nameof(PathDelimiter))] = JsonValue.CreateStringValue(content.PathDelimiter ?? string.Empty),
                [NameToJsonKey(nameof(ParamForMultipleFiles))] = JsonValue.CreateStringValue(content.ParamForMultipleFiles ?? string.Empty),

            };
            return json.Stringify();
        }

        public static (bool, string) Check(MenuItem content)
        {
            if (string.IsNullOrEmpty(content.Title))
            {
                return (false, nameof(content.Title));
            }

            if (string.IsNullOrEmpty(content.Exe))
            {
                return (false, nameof(content.Exe));
            }

            if (string.IsNullOrEmpty(content.Param))
            {
                return (false, nameof(content.Param));
            }

            return (true, string.Empty);
        }
    }
}
