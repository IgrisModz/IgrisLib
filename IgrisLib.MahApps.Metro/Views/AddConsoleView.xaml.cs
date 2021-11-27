using MahApps.Metro.Controls;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace IgrisLib.Views
{
    /// <summary>
    /// Logique d'interaction pour AddConsoleView.xaml
    /// </summary>
    public partial class AddConsoleView : MetroWindow
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
            //string str = string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
            //    "(^127(\\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])){3}$)",
            //    "(^10(\\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])){3}$)",
            //    "(^172\\.1[6-9]{1}[0-9]{0,1}(\\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])){2}$)",
            //    "(^172\\.2[0-9]{1}[0-9]{0,1}(\\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])){2}$)",
            //    "(^172\\.3[0-1]{1}[0-9]{0,1}(\\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])){2}$)",
            //    "(^192\\.168(\\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])){2}$)");
            string[] str = (sender as TextBox).Text.Split('.');
            if (str.Length == 4 && IPAddress.TryParse((sender as TextBox).Text, out IPAddress address))
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
