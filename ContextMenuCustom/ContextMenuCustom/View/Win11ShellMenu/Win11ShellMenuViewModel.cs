using ContextMenuCustomApp.View.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;

namespace ContextMenuCustomApp.View.Win11ShellMenu
{
    public class Win11ShellMenuViewModel : BaseViewModel
    {
        private readonly Win11ShellMenuService _service = new();
        private List<Win11ShellComItem> _allItems = new();

        public ObservableCollection<object> Items { get; } = new();

        private string _filterText = string.Empty;
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (SetProperty(ref _filterText, value))
                    ApplyFilter();
            }
        }

        public async Task LoadAsync()
        {
            await RunWith(async () =>
            {
                var items = await _service.LoadAllAsync();
                _allItems = items.ToList();
                ApplyFilter();

                var packageCount = 0;
                foreach (var row in Items)
                    if (row is Win11ShellPackageRow) packageCount++;

                Message = string.Format(AppContext.AppLang.Win11ShellMenuStatusLoaded, _allItems.Count, packageCount);
            });
        }

        private void ApplyFilter()
        {
            var filter = _filterText.Trim();
            IEnumerable<Win11ShellComItem> query = string.IsNullOrEmpty(filter)
                ? _allItems
                : _allItems.Where(x =>
                    x.IdFullName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    x.MenuComId.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    x.MenuName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    x.PackageDisplayName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    x.ApplicationDisplayName.Contains(filter, StringComparison.OrdinalIgnoreCase));

            var groups = query
                .GroupBy(x => x.IdFullName)
                .OrderBy(g => g.Key)
                .ToList();

            Items.Clear();
            foreach (var group in groups)
            {
                var comItems = group.OrderBy(x => x.IdFullName).ToList();

                if (comItems.Count > 0)
                {
                    var comItem = comItems[0];

                    Items.Add(new Win11ShellPackageRow
                    {
                        IdName = comItem.IdName,
                        IdFullName = comItem.IdFullName,
                        PackageDisplayName = comItem.PackageDisplayName,
                        ApplicationDisplayName = comItem.ApplicationDisplayName,
                        ItemCount = comItems.Count,
                        PackageLogo = comItem.PackageLogo,
                        InstallPath = comItem.PackageInstallPath,
                    });

                    foreach (var item in comItems)
                        Items.Add(item);
                }

            }
        }

        public Task SetBlockedAsync(Win11ShellComItem item, bool blocked)
        {
            return RunWith(() =>
            {
                try
                {
                    _service.SetBlocked(item.MenuComId, blocked);
                    item.IsEnabled = !blocked;
                }
                catch (UnauthorizedAccessException)
                {
                    throw new InvalidOperationException(AppContext.AppLang.Win11ShellMenuStatusAdminRequired);
                }
                return Task.CompletedTask;
            });
        }

        public Task LaunchAppAsync(Win11ShellPackageRow row) =>
            RunWith(() => _service.LaunchAppAsync(row.IdFullName));

        public async void OpenInstallFolder(Win11ShellPackageRow row)
        {
            if (!string.IsNullOrEmpty(row.InstallPath))
                await Launcher.LaunchFolderPathAsync(row.InstallPath);
        }
    }
}
