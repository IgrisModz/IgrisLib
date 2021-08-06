using IgrisLib.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Linq;
using System.Windows;

namespace IgrisLib.Views
{
    /// <summary>
    /// Logique d'interaction pour CCAPIView.xaml
    /// </summary>
    public partial class CCAPIView : MetroWindow
    {
        public bool Result { get; internal set; }

        public IConnectAPI Api { get; }

        public CCAPIView(IConnectAPI api, ResourceDictionary resources)
        {
            Api = api;
            ViewModel = new CCAPIViewModel(this, api, resources, DialogCoordinator.Instance);
            InitializeComponent();
            Resources = resources;
            listView.Focus();
        }

        internal CCAPIViewModel ViewModel
        {
            get => DataContext as CCAPIViewModel;
            set => DataContext = value;
        }

        internal new bool Show()
        {
            if (ViewModel.Consoles.Count() == 0)
            {
                AddConsoleView add = new AddConsoleView(Resources);
                add.ShowDialog();
                if (add.Result == MessageBoxResult.OK)
                {
                    ConsoleRegistry.Add(add.ConsoleName, add.ConsoleIp);
                    ViewModel.Refresh();
                    ShowDialog();
                }
                else
                {
                    Result = false;
                    Close();
                    MessageBox.Show(Resources["noConsole"].ToString(), Resources["noConsoleTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return Result;
            }
            ShowDialog();
            return Result;
        }
    }
}
