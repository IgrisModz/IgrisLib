using IgrisLib;
using System.Windows;

namespace IgrisLibTest
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PS3API PS3 { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnection_Click(object sender, RoutedEventArgs e)
        {
            PS3 = new PS3API(new PS3MAPI());
            if (PS3.ConnectTarget())
            {
                MessageBox.Show("Connecté avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("Pas connecté", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
