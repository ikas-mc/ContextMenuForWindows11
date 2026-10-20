using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Management.Deployment;
using Windows.Storage.Streams;

namespace ContextMenuCustomApp.View.Win11ShellMenu
{
    internal class Win11ShellMenuService
    {
        private const string PackagedComRegistryKey = @"SOFTWARE\Classes\PackagedCom\Package";
        private const string BlockedRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";

        public Task<IReadOnlyList<Win11ShellComItem>> LoadAllAsync()
        {
            return Task.Run(LoadAll);
        }

        private async Task<IReadOnlyList<Win11ShellComItem>> LoadAll()
        {
            var result = new List<Win11ShellComItem>();
            var manager = new PackageManager();

            using var hklm = Registry.LocalMachine;
            using var packagedComKey = hklm.OpenSubKey(PackagedComRegistryKey, writable: false);
            if (packagedComKey == null) return result;

            var blockedClsids = ReadBlockedClsids();

            foreach (var packageFullName in packagedComKey.GetSubKeyNames())
            {
                try
                {
                    using var pkgRegKey = packagedComKey.OpenSubKey(packageFullName, writable: false);
                    using var classKey = pkgRegKey?.OpenSubKey("Class", writable: false);
                    if (classKey == null) continue;

                    var clsids = classKey.GetSubKeyNames();
                    if (clsids.Length == 0) continue;

                    var pkg = manager.FindPackageForUser(null, packageFullName);
                    if (null == pkg) continue;

                    var installPath = pkg?.InstalledLocation?.Path ?? FindInstallPath(packageFullName);
                    if (installPath is null) continue;

                    var menuClsids = ReadManifestMenuClsids(installPath);
                    if (menuClsids.Count == 0) continue;

                    var manifestNames = ReadManifestComDisplayNames(installPath);

                    RandomAccessStreamReference? logo = null;
                    try
                    {
                        logo = pkg?.GetLogoAsRandomAccessStreamReference(new Windows.Foundation.Size(48, 48));
                    }
                    catch (Exception)
                    {
                        //
                    }

                    foreach (var clsid in clsids)
                    {
                        var normalized = FormatClsid(clsid);
                        if (!menuClsids.Contains(normalized)) continue;

                        manifestNames.TryGetValue(normalized, out var menuName);

                        var entries = await pkg!.GetAppListEntriesAsync();
                        var appDisplayName = entries?.FirstOrDefault()?.DisplayInfo?.DisplayName ?? string.Empty;

                        result.Add(new Win11ShellComItem
                        {
                            MenuComId = normalized,
                            MenuName = string.IsNullOrEmpty(menuName) ? normalized : menuName,
                            IdFullName = packageFullName,
                            IdName = pkg!.Id.Name,
                            PackageDisplayName = pkg.DisplayName,
                            ApplicationDisplayName = appDisplayName,
                            PackageLogo = logo,
                            PackageInstallPath = installPath,
                            IsEnabled = !blockedClsids.Contains(normalized)
                        });
                    }
                }
                catch
                {
                    // Skip packages that cannot be read
                }
            }

            return result;
        }

        public async Task LaunchAppAsync(string packageFullName)
        {
            var manager = new PackageManager();
            var pkg = manager.FindPackageForUser(null, packageFullName);
            if (pkg is null) return;
            var entries = await pkg.GetAppListEntriesAsync();
            if (entries.Count > 0)
                await entries[0].LaunchAsync();
        }

        public void SetBlocked(string clsid, bool blocked)
        {
            var normalized = FormatClsid(clsid);
            if (blocked)
            {
                using var key = Registry.CurrentUser.CreateSubKey(BlockedRegistryKey, writable: true);
                key?.SetValue(normalized, "##custom-menu-builder", RegistryValueKind.String);
            }
            else
            {
                using var key = Registry.CurrentUser.OpenSubKey(BlockedRegistryKey, writable: true);
                key?.DeleteValue(normalized, throwOnMissingValue: false);
            }
        }

        private static HashSet<string> ReadBlockedClsids()
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(BlockedRegistryKey, writable: false);
                if (key != null)
                {
                    foreach (var name in key.GetValueNames())
                        result.Add(FormatClsid(name));
                }
            }
            catch { }
            return result;
        }

        private static HashSet<string> ReadManifestMenuClsids(string installPath)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var manifestPath = Path.Combine(installPath, "AppxManifest.xml");
            if (!File.Exists(manifestPath)) return result;

            try
            {
                var doc = XDocument.Load(manifestPath);
                foreach (var ext in doc.Descendants().Where(e => e.Name.LocalName == "Extension"))
                {
                    var category = ext.Attribute("Category")?.Value;
                    if (!string.Equals(category, "windows.fileExplorerContextMenus", StringComparison.OrdinalIgnoreCase))
                        continue;

                    foreach (var verb in ext.Descendants().Where(e => e.Name.LocalName == "Verb"))
                    {
                        var clsid = verb.Attribute("Clsid")?.Value;
                        if (!string.IsNullOrEmpty(clsid))
                            result.Add(FormatClsid(clsid));
                    }
                }
            }
            catch { }

            return result;
        }

        private static Dictionary<string, string> ReadManifestComDisplayNames(string installPath)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var manifestPath = Path.Combine(installPath, "AppxManifest.xml");
            if (!File.Exists(manifestPath)) return result;

            try
            {
                var doc = XDocument.Load(manifestPath);
                foreach (var ext in doc.Descendants().Where(e => e.Name.LocalName == "Extension"))
                {
                    var category = ext.Attribute("Category")?.Value;
                    if (!string.Equals(category, "windows.comServer", StringComparison.OrdinalIgnoreCase))
                        continue;

                    foreach (var cls in ext.Descendants().Where(e => e.Name.LocalName == "Class"))
                    {
                        var id = cls.Attribute("Id")?.Value;
                        if (string.IsNullOrEmpty(id)) continue;

                        var name = cls.Attribute("DisplayName")?.Value
                                ?? cls.Parent?.Attribute("DisplayName")?.Value;
                        result[FormatClsid(id)] = string.IsNullOrEmpty(name) ? id : name;
                    }
                }
            }
            catch { }

            return result;
        }

        private static string? FindInstallPath(string packageFullName)
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var candidate = Path.Combine(programFiles, "WindowsApps", packageFullName);
            return Directory.Exists(candidate) ? candidate : null;
        }


        private static string FormatClsid(string clsid)
        {
            clsid = clsid.Trim();
            if (!clsid.StartsWith('{')) clsid = "{" + clsid;
            if (!clsid.EndsWith('}')) clsid += "}";
            return clsid.ToUpperInvariant();
        }
    }
}
