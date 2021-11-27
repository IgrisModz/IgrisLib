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
    /// <summary>
    /// 
    /// </summary>
    public class CCAPIViewModel : ViewModelBase
    {
        private readonly IDialogCoordinator Dialog;
        private readonly ResourceDictionary Resources;
        private readonly CCAPIView Win;

        /// <summary>
        /// 
        /// </summary>
        public IConnectAPI Api { get; }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<CCAPI.ConsoleInfo> Consoles { get => GetProperty(() => Consoles); set => SetProperty(() => Consoles, value); }

        /// <summary>
        /// 
        /// </summary>
        public CCAPI.ConsoleInfo SelectedConsole { get => GetProperty(() => SelectedConsole); set => SetProperty(() => SelectedConsole, value); }

        /// <summary>
        /// 
        /// </summary>
        public ICommand AddConsoleCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand DeleteConsoleCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ConnectCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        public CCAPIViewModel(CCAPIView win, IConnectAPI api, ResourceDictionary resources, IDialogCoordinator instance)
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

        private async void AddConsole()
        {
            AddConsoleView add = new AddConsoleView(Resources);
            add.ShowDialog();
            if (add.Result == MessageBoxResult.OK)
            {
                foreach (CCAPI.ConsoleInfo console in Consoles)
                {
                    if (console.Name == add.ConsoleName)
                    {
                        await Dialog.ShowMessageAsync(this, Resources["failed"].ToString(), Resources["consoleNameExist"].ToString());
                        return;
                    }
                }
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
