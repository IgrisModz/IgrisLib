using System.Windows;

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
            base.Close();
        }

        private void BtncCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            base.Close();
        }
    }
}
