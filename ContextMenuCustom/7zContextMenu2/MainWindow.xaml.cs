using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;
using Windows.System;

namespace _7zContextMenu2
{
    public class MainVm {
        public ObservableCollection<CommondItem1> Itmes { get; set; } = new ObservableCollection<CommondItem1>();
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainVm();
            this.DataContext = _vm;
            Load();
        }

        private MainVm _vm;

        private async void Load() {
            _vm.Itmes.Clear();
            var folder=await ApplicationData.Current.LocalFolder.TryGetItemAsync("custom_commands");
            if (folder is StorageFolder configFolder) {
               var files=await configFolder.GetFilesAsync();
               foreach(var file in files){
                   var content= await FileIO.ReadTextAsync(file);
                    try
                    {
                        var item = CommondItem1.ReadFromJson(content);
                        item.File = file;
                        _vm.Itmes.Add(item);
                    }
                    catch (Exception ex)
                    {
                            var item = new CommondItem1();
                            item.Title = "<Error> config";
                            item.File = file;
                            _vm.Itmes.Add(item);
                    }

                }
            }
        }

        private async Task<StorageFile> CreateConfigFile(string name) {
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

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            var folder = await GetConfigsFolder();
            _ =await Launcher.LaunchFolderAsync(folder);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is CommondItem1 item) {
                if (item.File == null) {
                   var fileName = item.Title + ".json";
                   item.File = await CreateConfigFile(fileName);
                }
                try
                {
                    var content = CommondItem1.WriteToJson(item);
                await  FileIO.WriteTextAsync(item.File, content);

                } catch (Exception ex)
                {
                    MessageBox.Show("save error\n"+ex.Message);
                }
            }
            Load();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var item = new CommondItem1() { Title = "new commond" };
            _vm.Itmes.Add(item);
            CommandList.SelectedItem = item;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (CommandList.SelectedItem is CommondItem1 item)
            {
                if (item.File == null)
                {
                    return;
                }
                await item.File.DeleteAsync();
                Load();
            }
        }
    }
}
