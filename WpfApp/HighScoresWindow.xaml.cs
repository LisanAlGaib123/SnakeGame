using ClassLibrary;
using SnakeGameLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SnakeWpfApp
{
    public partial class HighScoresWindow : Window
    {
        private GameController controller;

        public HighScoresWindow()
        {
            InitializeComponent();
            controller = new GameController(new SnakeGame()); // Tworzymy nową instancję kontrolera
            LoadHighScores();
        }

        private void LoadHighScores()
        {
            // Inicjalizacja repozytorium
            var repo = new HighScoreRepository();

            // Pobieranie wyników
            var highScores = repo.GetHighScores();

            if (highScores.Count == 0)
            {
                MessageBox.Show("No high scores to display.");
                return;
            }

            // Tworzenie danych dla DataGrid
            var highScoresData = highScores
                .Select((score, index) => new
                {
                    Rank = $"#{index + 1}",
                    Name = score.Name,
                    Score = score.Score
                })
                .ToList();

            // Powiązanie danych z DataGrid
            HighScoresDataGrid.ItemsSource = highScoresData;
        }


        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            // Powrót do menu głównego
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
