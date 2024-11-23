using System.Windows;

namespace SnakeWpfApp
{
    public partial class AddScoreWindow : Window
    {
        public string PlayerName { get; private set; }
        public int Score { get; }

        public AddScoreWindow(int score)
        {
            InitializeComponent();
            Score = score;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Pobranie nazwy gracza z pola tekstowego
            PlayerName = NameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(PlayerName))
            {
                MessageBox.Show("Please enter a valid name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
