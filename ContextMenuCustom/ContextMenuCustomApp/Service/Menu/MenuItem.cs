using ContextMenuCustomApp.View.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
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
        private int _index;
        private string _exe;
        private string _param;
        private string _icon;
        private string _iconDark;

        private bool _acceptDirectory;
        private int _acceptDirectoryFlag;

        private bool _acceptFile;
        private int _acceptFileFlag;
        private string _acceptExts;
        private string _acceptFileRegex;

        private int _acceptMultipleFilesFlag;
        private string _pathDelimiter;
        private string _paramForMultipleFiles;

        private int _showWindowFlag;
        private string _workingDirectory;

        [JsonProperty(Order = 0)]
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        [JsonProperty(Order = 2)]
        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        [JsonProperty(Order = 4)]
        public string Exe
        {
            get => _exe;
            set => SetProperty(ref _exe, value);
        }

        [JsonProperty(Order = 6)]
        public string Param
        {
            get => _param;
            set => SetProperty(ref _param, value);
        }

        [JsonProperty(Order = 8)]
        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        [JsonProperty(Order = 10)]
        public string IconDark
        {
            get => _iconDark;
            set => SetProperty(ref _iconDark, value);
        }

        [Obsolete()]
        [JsonProperty(Order = 20)]
        public bool AcceptDirectory
        {
            get => _acceptDirectory;
            set => SetProperty(ref _acceptDirectory, value);
        }
        public bool ShouldSerializeAcceptDirectory() => false;

        [JsonProperty(Order = 22)]
        public int AcceptDirectoryFlag
        {
            get => _acceptDirectoryFlag;
            set => SetProperty(ref _acceptDirectoryFlag, value);
        }

        [Obsolete()]
        [JsonProperty(Order = 30)]
        public bool AcceptFile
        {
            get => _acceptFile;
            set => SetProperty(ref _acceptFile, value);
        }
        public bool ShouldSerializeAcceptFile() => false;

        [JsonProperty(Order = 32)]
        public int AcceptFileFlag
        {
            get => _acceptFileFlag;
            set => SetProperty(ref _acceptFileFlag, value);
        }

        [JsonProperty(Order = 34)]
        public string AcceptExts
        {
            get => _acceptExts;
            set => SetProperty(ref _acceptExts, string.IsNullOrEmpty(value) ? value : value.ToLower());
        } 

        [JsonProperty(Order = 36)]
        public string AcceptFileRegex
        {
            get => _acceptFileRegex;
            set => SetProperty(ref _acceptFileRegex, value);
        }

        [JsonProperty(Order = 40)]
        public int AcceptMultipleFilesFlag
        {
            get => _acceptMultipleFilesFlag;
            set => SetProperty(ref _acceptMultipleFilesFlag, value);
        }

        [JsonProperty(Order = 42)]
        public string PathDelimiter
        {
            get => _pathDelimiter;
            set => SetProperty(ref _pathDelimiter, value);
        }

        [JsonProperty(Order = 44)]
        public string ParamForMultipleFiles
        {
            get => _paramForMultipleFiles;
            set => SetProperty(ref _paramForMultipleFiles, value);
        }

        [JsonProperty(Order = 50)]
        public int ShowWindowFlag
        {
            get => _showWindowFlag;
            set => SetProperty(ref _showWindowFlag, value);
        }

        [JsonProperty(Order = 52)]
        public string WorkingDirectory
        {
            get => _workingDirectory;
            set => SetProperty(ref _workingDirectory, value);
        }
    }
}