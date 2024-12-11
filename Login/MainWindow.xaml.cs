using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Login
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // maximize 
            WindowState = WindowState.Maximized;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Simpler Benutzername-/Passwort-Check
            if (UsernameBox.Text == "admin" && PasswordBox.Password == "password")
            {
                // Wechsel zum Dashboard
                LoginPanel.Visibility = Visibility.Collapsed;
                DashboardPanel.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Invalid credentials, please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Wechsel zurück zum Login
            DashboardPanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
            UsernameBox.Clear();
            PasswordBox.Clear();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Füge ein neues Element zur ListView hinzu
            if (!string.IsNullOrWhiteSpace(NewItemTextBox.Text))
            {
                ItemListView.Items.Add(NewItemTextBox.Text);
                NewItemTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Bitte geben Sie einen Text ein.", "Hinzufügen fehlgeschlagen", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Lösche das ausgewählte Element aus der ListView
            if (ItemListView.SelectedItem != null)
            {
                ItemListView.Items.Remove(ItemListView.SelectedItem);
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein Element zum Löschen aus.", "Löschen fehlgeschlagen", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}