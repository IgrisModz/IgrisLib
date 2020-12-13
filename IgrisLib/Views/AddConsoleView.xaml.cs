using System.Windows;

namespace IgrisLib.Views
{
    /// <summary>
    /// Logique d'interaction pour AddConsoleView.xaml
    /// </summary>
    public partial class AddConsoleView : Window
    {
        public MessageBoxResult Result { get; private set; }

        public string ConsoleName { get => txtConsoleName.Text; set => txtConsoleName.Text = value; }

        public string ConsoleIp { get => txtConsoleIp.Text; set => txtConsoleIp.Text = value; }

        public AddConsoleView(ResourceDictionary resources)
        {
            InitializeComponent();
            Resources = resources;
        }

        private void btnAddConsole_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            base.Close();
        }

        private void btncCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            base.Close();
        }
    }
}
