using IgrisLib.Mvvm;
using IgrisLib.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace IgrisLib.ViewModels
{
    public class CCAPIViewModel : ViewModelBase
    {
        private readonly ResourceDictionary Resources;
        private readonly CCAPIView Win;

        public IConnectAPI Api { get; }

        public ObservableCollection<CCAPI.ConsoleInfo> Consoles { get => GetValue(() => Consoles); set => SetValue(() => Consoles, value); }

        public CCAPI.ConsoleInfo SelectedConsole { get => GetValue(() => SelectedConsole); set => SetValue(() => SelectedConsole, value); }

        public DelegateCommand AddConsoleCommand { get; }

        public DelegateCommand DeleteConsoleCommand { get; }

        public DelegateCommand ConnectCommand { get; }

        public DelegateCommand RefreshCommand { get; }

        public CCAPIViewModel(CCAPIView win, IConnectAPI api, ResourceDictionary resources)
        {
            Win = win ?? throw new ArgumentNullException(nameof(win));
            Api = api ?? throw new ArgumentNullException(nameof(api));
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
            AddConsoleCommand = new DelegateCommand(AddConsole, CanExecuteRefresh);
            DeleteConsoleCommand = new DelegateCommand(DeleteConsole, CanExecuteSelected);
            ConnectCommand = new DelegateCommand(Connect, CanExecuteSelected);
            RefreshCommand = new DelegateCommand(Refresh, CanExecuteRefresh);
            Refresh();
        }

        private bool CanExecuteSelected()
        {
            return Api != null && SelectedConsole != null;
        }

        private bool CanExecuteRefresh()
        {
            return Api != null;
        }

        private void AddConsole()
        {
            AddConsoleView add = new AddConsoleView(Resources);
            add.ShowDialog();
            if (add.Result == MessageBoxResult.OK)
            {
                ConsoleRegistry.Add(add.ConsoleName, add.ConsoleIp);
                Refresh();
            }
        }

        private void DeleteConsole()
        {
            ConsoleRegistry.Delete(SelectedConsole.Name);
            Refresh();
        }

        private void Connect()
        {
            if (SelectedConsole != null)
            {
                if (Api.ConnectTarget(SelectedConsole.Ip))
                {
                    Win.Result = true;
                }
                else
                    Win.Result = false;
                Win.Close();
                return;
            }
            else
                MessageBox.Show(Resources["errorSelect"].ToString(), Resources["errorSelectTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        internal void Refresh()
        {
            ObservableCollection<CCAPI.ConsoleInfo> list = new ObservableCollection<CCAPI.ConsoleInfo>();
            foreach (CCAPI.ConsoleInfo consoleInfo in Api.GetConsoleList())
            {
                list.Add(new CCAPI.ConsoleInfo() { Name = consoleInfo.Name, Ip = consoleInfo.Ip });
            }
            Consoles = list;
            SelectedConsole = Consoles.FirstOrDefault();
        }
    }
}
