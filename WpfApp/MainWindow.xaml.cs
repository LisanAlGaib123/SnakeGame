using SnakeGameLibrary;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApp;

namespace SnakeWpfApp
{
    public partial class MainWindow : Window
    {
        private GameController controller;

        public MainWindow()
        {
            InitializeComponent();
        }


        // Obsługa kliknięcia "Start Game"
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {

            var snakeGame = new SnakeGame();
            var gameController = new GameController(snakeGame);
            var gameWindow = new GameWindow(gameController);
            gameWindow.Show();
            this.Close(); // Zamykamy menu
        }

        // Obsługa kliknięcia "Custom Map"
        private void CustomMap_Click(object sender, RoutedEventArgs e)
        {
            var snakeGame = new SnakeGame(); // Tworzymy nową instancję logiki gry
            var gameController = new GameController(snakeGame); // Przekazujemy ją do kontrolera
            var customMapWindow = new CustomMapWindow(gameController);
            customMapWindow.Show();
            this.Close();
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
