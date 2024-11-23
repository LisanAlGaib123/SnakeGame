using SnakeGameLibrary;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeWpfApp
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }


        // Obsługa kliknięcia "Start Game"
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // Przejście do gry - możemy stworzyć nową instancję okna gry
            var gameWindow = new GameWindow();
            gameWindow.Show();
            this.Close(); // Zamykamy menu
        }

        // Obsługa kliknięcia "Custom Map"
        private void CustomMap_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Custom Map feature is under development!");
        }

        // Obsługa kliknięcia "High Scores"
        private void HighScores_Click(object sender, RoutedEventArgs e)
        {
            var highScoresWindow = new HighScoresWindow();
            highScoresWindow.Show();
            this.Close();
        }


        // Obsługa kliknięcia "Exit"
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
