using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace IgrisLib.Views
{
    /// <summary>
    /// Logique d'interaction pour AddConsoleView.xaml
    /// </summary>
    public partial class AddConsoleView : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public MessageBoxResult Result { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ConsoleName { get => txtConsoleName.Text; set => txtConsoleName.Text = value; }

        /// <summary>
        /// 
        /// </summary>
        public string ConsoleIp { get => txtConsoleIp.Text; set => txtConsoleIp.Text = value; }

        /// <summary>
        /// 
        /// </summary>
        public AddConsoleView(ResourceDictionary resources)
        {
            InitializeComponent();
            Resources = resources;
        }

        private void BtnAddConsole_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void BtncCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void TxtConsoleIp_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IPAddress.TryParse((sender as TextBox).Text, out IPAddress address))
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    btnAddConsole.IsEnabled = true;
                    return;
                }
            }
            btnAddConsole.IsEnabled = false;
        }
    }
}
