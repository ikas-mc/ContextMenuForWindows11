using ContextMenuCustomApp.View.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Streams;

namespace ContextMenuCustomApp.View.Win11ShellMenu
{
    public partial class Win11ShellComItem : BaseModel
    {
        public string MenuComId { get; init; } = string.Empty;
        public string MenuName { get; init; } = string.Empty;
        public string IdName { get; init; } = string.Empty;
        public string IdFullName { get; init; } = string.Empty;
        public string PackageDisplayName { get; init; } = string.Empty;
        public string ApplicationDisplayName { get; init; } = string.Empty;
        public RandomAccessStreamReference? PackageLogo { get; init; }
        public string? PackageInstallPath { get; init; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value, nameof(IsEnabled));
        }
    }

    public class Win11ShellPackageRow
    {
        public string ApplicationDisplayName { get; init; } = string.Empty;
        public string IdFullName { get; init; } = string.Empty;
        public string IdName { get; init; } = string.Empty;
        public string PackageDisplayName { get; init; } = string.Empty;
        public int ItemCount { get; init; }
        public RandomAccessStreamReference? PackageLogo { get; init; }
        public string? InstallPath { get; init; }
        public string DisplayLabel => string.IsNullOrEmpty(ApplicationDisplayName) ? PackageDisplayName : ApplicationDisplayName;
        public string ItemCountLabel => $"{IdName} : {ItemCount}";
    }

    public partial class Win11ShellRowTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PackageTemplate { get; set; } = null!;
        public DataTemplate ComItemTemplate { get; set; } = null!;

        protected override DataTemplate SelectTemplateCore(object item)
            => item is Win11ShellPackageRow ? PackageTemplate : ComItemTemplate;

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }
}
