using IgrisLib;
using IgrisLib.Mvvm;
using IgrisLibTest.Properties;
using System.Collections.Generic;
using System.Windows;

namespace IgrisLibTest
{
    public class MainViewModel : ViewModelBase
    {
        public PS3API PS3 { get; private set; }

        public List<string> ApiList { get => GetProperty(() => ApiList); set => SetProperty(() => ApiList, value); }

        public string SelectedApi
        {
            get => GetProperty(() => SelectedApi);
            set
            {
                PS3.ChangeAPI(value);
                Settings.Default.API = value;
                Settings.Default.Save();
                SetProperty(() => SelectedApi, value);
            }
        }

        public DelegateCommand ConnectionCommand { get; }

        public MainViewModel()
        {
            PS3 = new PS3API(SelectAPI.CCAPI);
            ConnectionCommand = new DelegateCommand(Connection);
            ApiList = PS3.GetAllApiName();
            SelectedApi = Settings.Default.API;
        }

        private void Connection()
        {
            if (PS3.ConnectTarget())
            {
                MessageBox.Show("Connected with success", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("Not connected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
