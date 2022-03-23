using ContextMenuCustomApp.View.Common;
using Windows.Data.Json;
using Windows.Storage;

namespace ContextMenuCustomApp.Service.Menu
{
    enum MultipleFilesFlag
    {
        OFF, EACH, JOIN
    }

    public class MenuItem : BaseModel
    {
        public StorageFile File { get; set; }

        private string _title;
        private string _exe;
        private string _param;
        private string _icon;
        private string _acceptExts;
        private bool _acceptDirectory;
        private bool _acceptFile;
        private bool _acceptMultipleFiles;
        private string _pathDelimiter;
        private string _paramForMultipleFiles;
        private int _acceptMultipleFilesFlag;

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Exe { get => _exe; set => SetProperty(ref _exe, value); }
        public string Param { get => _param; set => SetProperty(ref _param, value); }
        public string Icon { get => _icon; set => SetProperty(ref _icon, value); }
        public string AcceptExts { get => _acceptExts; set => SetProperty(ref _acceptExts, string.IsNullOrEmpty(value) ? value : value.ToLower()); }// to lower for match
        public bool AcceptDirectory { get => _acceptDirectory; set => SetProperty(ref _acceptDirectory, value); }
        public bool AcceptFile { get => _acceptFile; set => SetProperty(ref _acceptFile, value); }
        public bool AcceptMultipleFiles { get => _acceptMultipleFiles; set => SetProperty(ref _acceptMultipleFiles, value); }
        public int AcceptMultipleFilesFlag { get => _acceptMultipleFilesFlag; set => SetProperty(ref _acceptMultipleFilesFlag, value); }
        public string PathDelimiter { get => _pathDelimiter; set => SetProperty(ref _pathDelimiter, value); }
        public string ParamForMultipleFiles { get => _paramForMultipleFiles; set => SetProperty(ref _paramForMultipleFiles, value); }

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
                AcceptExts = json.GetNamedString(NameToJsonKey(nameof(AcceptExts)), string.Empty),
                AcceptDirectory = json.GetNamedBoolean(NameToJsonKey(nameof(AcceptDirectory)), false),
                AcceptFile = json.GetNamedBoolean(NameToJsonKey(nameof(AcceptFile)), true),
                AcceptMultipleFiles = json.GetNamedBoolean(NameToJsonKey(nameof(AcceptMultipleFiles)), false),
                AcceptMultipleFilesFlag = (int)json.GetNamedNumber(NameToJsonKey(nameof(AcceptMultipleFilesFlag)), (int)MultipleFilesFlag.OFF),
                PathDelimiter = json.GetNamedString(NameToJsonKey(nameof(PathDelimiter)), string.Empty),
                ParamForMultipleFiles = json.GetNamedString(NameToJsonKey(nameof(ParamForMultipleFiles)), string.Empty),
            };
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
                [NameToJsonKey(nameof(AcceptExts))] = JsonValue.CreateStringValue(content.AcceptExts ?? string.Empty),
                [NameToJsonKey(nameof(AcceptDirectory))] = JsonValue.CreateBooleanValue(content.AcceptDirectory),
                [NameToJsonKey(nameof(AcceptFile))] = JsonValue.CreateBooleanValue(content.AcceptFile),
                [NameToJsonKey(nameof(AcceptMultipleFiles))] = JsonValue.CreateBooleanValue(content.AcceptMultipleFiles),
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
