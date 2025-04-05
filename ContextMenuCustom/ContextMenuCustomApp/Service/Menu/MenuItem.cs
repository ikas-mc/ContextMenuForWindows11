using ContextMenuCustomApp.View.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Windows.Storage;

namespace ContextMenuCustomApp.Service.Menu
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MenuItem : BaseModel
    {
        [JsonIgnore]
        public StorageFile File { get; set; }

        [JsonIgnore]
        public string FileName
        {
            get => File?.Name;
            set
            {
                OnPropertyChanged(nameof(FileName));
            }
        }

        private bool _enabled;
        [JsonIgnore]
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }

        private string _title;
        private string _exe;
        private string _param;
        private string _icon;
        private string _iconDark;
        private int _index;

        private bool _acceptDirectory;
        private int _acceptDirectoryFlag;

        private bool _acceptFile;
        private int _acceptFileFlag;
        private string _acceptExts;
        private string _acceptFileRegex;

        private string _pathDelimiter;
        private string _paramForMultipleFiles;
        private int _acceptMultipleFilesFlag;
        private int _showWindowFlag;
        private string _workingDirectory;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Exe
        {
            get => _exe;
            set => SetProperty(ref _exe, value);
        }

        public string Param
        {
            get => _param;
            set => SetProperty(ref _param, value);
        }

        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        public string IconDark
        {
            get => _iconDark;
            set => SetProperty(ref _iconDark, value);
        }

        public bool AcceptDirectory
        {
            get => _acceptDirectory;
            set => SetProperty(ref _acceptDirectory, value);
        }

        public int AcceptDirectoryFlag
        {
            get => _acceptDirectoryFlag;
            set => SetProperty(ref _acceptDirectoryFlag, value);
        }

        public bool AcceptFile
        {
            get => _acceptFile;
            set => SetProperty(ref _acceptFile, value);
        }

        public int AcceptFileFlag
        {
            get => _acceptFileFlag;
            set => SetProperty(ref _acceptFileFlag, value);
        }

        public string AcceptExts
        {
            get => _acceptExts;
            set => SetProperty(ref _acceptExts, string.IsNullOrEmpty(value) ? value : value.ToLower());
        } // to lower for match

        public string AcceptFileRegex
        {
            get => _acceptFileRegex;
            set => SetProperty(ref _acceptFileRegex, value);
        }

        public int AcceptMultipleFilesFlag
        {
            get => _acceptMultipleFilesFlag;
            set => SetProperty(ref _acceptMultipleFilesFlag, value);
        }

        public string PathDelimiter
        {
            get => _pathDelimiter;
            set => SetProperty(ref _pathDelimiter, value);
        }

        public string ParamForMultipleFiles
        {
            get => _paramForMultipleFiles;
            set => SetProperty(ref _paramForMultipleFiles, value);
        }

        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        public int ShowWindowFlag
        {
            get => _showWindowFlag;
            set => SetProperty(ref _showWindowFlag, value);
        }

        public string WorkingDirectory
        {
            get => _workingDirectory;
            set => SetProperty(ref _workingDirectory, value);
        }
    }
}