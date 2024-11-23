using SnakeGameLibrary;
using SnakeWpfApp;
using System;
using System.Windows;

namespace WpfApp
{
    public partial class CustomMapWindow : Window
    {
        private GameController controller;

        public CustomMapWindow(GameController gameController)
        {
            InitializeComponent();
            this.controller = gameController;
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Walidacja szerokości i wysokości
            if (int.TryParse(WidthTextBox.Text, out int width) &&
                int.TryParse(HeightTextBox.Text, out int height) &&
                width >= 10 && width <= 50 &&
                height >= 10 && height <= 50)
            {
                // Ustawienie nowego rozmiaru planszy
                controller.GetGameState().SetBoardSize(width, height);
                controller.GetGameState().IsCustomSize = true;

                // Uruchomienie gry
                var gameWindow = new GameWindow(controller);
                gameWindow.Show();
                this.Close(); // Zamknięcie okna CustomMapWindow
            }
            else
            {
                MessageBox.Show("Please enter valid dimensions between 10 and 50.");
            }
        }
    }
}
