using IgrisLib;
using IgrisLib.Mvvm;
using IgrisLibTest.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace IgrisLibTest
{
    public class MainViewModel : ViewModelBase
    {
        public PS3API PS3 { get; private set; }

        public List<string> ApiList { get => GetValue(() => ApiList); set => SetValue(() => ApiList, value); }

        public string SelectedApi
        {
            get => GetValue(() => SelectedApi);
            set
            {
                switch (value)
                {
                    case "CCAPI":
                        PS3.ChangeAPI(new CCAPI());
                        break;
                    case "PS3MAPI":
                        PS3.ChangeAPI(new PS3MAPI());
                        break;
                    case "TMAPI":
                        PS3.ChangeAPI(new TMAPI());
                        break;
                    default:
                        PS3.ChangeAPI(new TMAPI());
                        break;
                }
                Settings.Default.API = value;
                Settings.Default.Save();
                SetValue(() => SelectedApi, value);
            }
        }

        public DelegateCommand ConnectionCommand { get; }

        public MainViewModel()
        {
            PS3 = new PS3API(null);
            ConnectionCommand = new DelegateCommand(Connection);
            ApiList = PS3.GetAllApi();
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
