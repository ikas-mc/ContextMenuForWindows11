using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace ContextMenuCustomApp
{

    public class MainVm : BaseModel
    {
        public ObservableCollection<CommondItem> Itmes { get; set; } = new ObservableCollection<CommondItem>();

        public string Version()
        {
            var version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public async void Load()
        {
            Itmes.Clear();
            var folder = await ApplicationData.Current.LocalFolder.TryGetItemAsync("custom_commands");
            if (folder is StorageFolder configFolder)
            {
                var files = await configFolder.GetFilesAsync();
                foreach (var file in files)
                {
                    var content = await FileIO.ReadTextAsync(file);
                    try
                    {
                        var item = CommondItem.ReadFromJson(content);
                        item.File = file;
                        Itmes.Add(item);
                    }
                    catch (Exception ex)
                    {
                        var item = new CommondItem();
                        item.Title = "<Error> config";
                        item.File = file;
                        Itmes.Add(item);
                    }

                }
            }
        }


        public string GetCustomMenuName()
        {
            var value = ApplicationData.Current.LocalSettings.Values["Custom_Menu_Name"];
            return (value as string) ?? "Custom Menu";
        }

        public async void SetCustomMenuName(string name)
        {
            await Task.Run(() =>
            {
                ApplicationData.Current.LocalSettings.Values["Custom_Menu_Name"] = name ?? "Custom Menu";
            });
        }
    }

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            mainVm = new MainVm();
            mainVm.Load();
        }

        private MainVm mainVm;

        private async Task<StorageFile> CreateConfigFile(string name)
        {
            var folder = await GetConfigsFolder();
            return await folder.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
        }

        private async Task<StorageFolder> GetConfigsFolder()
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync("custom_commands");
            if (item is StorageFile)
            {
                await item.DeleteAsync();
            }
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync("custom_commands", CreationCollisionOption.OpenIfExists);
        }


        private async void Open_Folder_Click(object sender, RoutedEventArgs e)
        {
            var folder = await GetConfigsFolder();
            _ = await Launcher.LaunchFolderAsync(folder);
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is CommondItem item)
            {
                if (item.File == null)
                {
                    return;
                }
                _ = await Launcher.LaunchFileAsync(item.File);
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is CommondItem item)
            {
                var checkResult = CommondItem.Check(item);
                if (!checkResult.Item1)
                {
                    Alert.InfoAsync($"{checkResult.Item2} is empty", "Warn");
                    return;
                }

                if (item.File == null)
                {
                    var fileName = item.Title + ".json";
                    item.File = await CreateConfigFile(fileName);
                }
                try
                {
                    var content = CommondItem.WriteToJson(item);
                    await FileIO.WriteTextAsync(item.File, content);

                }
                catch (Exception ex)
                {
                    Alert.InfoAsync("save faild", "Warn");
                }
                mainVm.Load();
            }
            else
            {
                Alert.InfoAsync("no selected item");
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var item = new CommondItem() { Title = "new commond", Param = @"""{path}""" };
            mainVm.Itmes.Add(item);
            CommandList.SelectedItem = item;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            mainVm.Load();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is CommondItem item)
            {
                if (item.File == null)
                {
                    Alert.InfoAsync("selected item no exists");
                    mainVm.Load();
                }
                else
                {
                    var result = await Alert.ChooseAsync("confirm to delete", "Warn");
                    if (result)
                    {
                        await item.File.DeleteAsync();
                        mainVm.Load();
                    }
                }
            }
            else
            {
                Alert.InfoAsync("no selected item");
            }
        }

    }
}
