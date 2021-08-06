using IgrisLib.Mvvm;
using IgrisLib.Views;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace IgrisLib.ViewModels
{
    public class CCAPIViewModel : ViewModelBase
    {
        private readonly IDialogCoordinator Dialog;
        private readonly ResourceDictionary Resources;
        private readonly CCAPIView Win;

        public IConnectAPI Api { get; }

        public ObservableCollection<CCAPI.ConsoleInfo> Consoles { get => GetValue(() => Consoles); set => SetValue(() => Consoles, value); }

        public CCAPI.ConsoleInfo SelectedConsole { get => GetValue(() => SelectedConsole); set => SetValue(() => SelectedConsole, value); }

        internal ICommand AddConsoleCommand { get; }

        internal ICommand DeleteConsoleCommand { get; }

        internal ICommand ConnectCommand { get; }

        internal ICommand RefreshCommand { get; }

        internal CCAPIViewModel(CCAPIView win, IConnectAPI api, ResourceDictionary resources, IDialogCoordinator instance)
        {
            Win = win ?? throw new ArgumentNullException(nameof(win));
            Api = api ?? throw new ArgumentNullException(nameof(api));
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
            Dialog = instance ?? throw new ArgumentNullException(nameof(instance));
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

        private async void Connect()
        {
            if (SelectedConsole != null)
            {
                Win.Result = Api.ConnectTarget(SelectedConsole.Ip);

                Win.Close();
                return;
            }
            else
            {
                await Dialog.ShowMessageAsync(this, Resources["errorSelectTitle"].ToString(), Resources["errorSelect"].ToString());
            }
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
