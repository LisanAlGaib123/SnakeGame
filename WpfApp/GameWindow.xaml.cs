using ClassLibrary;
using SnakeGameLibrary; // Twoja logika gry w osobnej bibliotece
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeWpfApp
{
    public partial class GameWindow : Window
    {

        private const int CellSize = 20;
        private GameController gameController;
        private DispatcherTimer gameTimer;
        private Direction? lastInputDirection = null;

        public GameWindow(GameController controller)
        {
            InitializeComponent();

            // Inicjalizacja gry
            var snakeGame = new SnakeGame();
            this.gameController = controller;

            // Konfiguracja timera
            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            gameTimer.Tick += UpdateGame;

            // Rysowanie planszy
            DrawMap();
            StartGame();
        }

        private void StartGame()
        {
            gameController.GetGameState().ResetGame();
            lastInputDirection = null;
            gameTimer.Start();
            DrawSnake();
            DrawFood();
            UpdateScore();
        }

        private void EndGame()
        {
            gameTimer.Stop();

            int finalScore = gameController.GetGameState().Score;

            // Sprawdź, czy wynik kwalifikuje się do zapisania
            CheckHighScoreAndSave(finalScore);

            var result = MessageBox.Show(
                $"Game Over! Your score: {finalScore}\nDo you want to play again?",
                "Game Over",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                StartGame();
            }
            else
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
        }

        private void CheckHighScoreAndSave(int finalScore)
        {
            var highScoreRepo = new HighScoreRepository();
            var highScores = highScoreRepo.GetHighScores();

            // Jeśli wynik kwalifikuje się do pierwszej 10-tki
            if (highScores.Count < 10 || finalScore > highScores.Last().Score)
            {
                // Otwórz okno dialogowe
                var addScoreWindow = new AddScoreWindow(finalScore);
                if (addScoreWindow.ShowDialog() == true)
                {
                    // Zapisanie wyniku w bazie danych
                    highScoreRepo.AddHighScore(addScoreWindow.PlayerName, finalScore);
                }
            }
        }


        private void UpdateGame(object sender, EventArgs e)
        {
            // Akceptacja ostatniego kierunku
            if (lastInputDirection != null)
            {
                gameController.ChangeDirection(lastInputDirection.Value);
                lastInputDirection = null;
            }

            gameController.UpdateGame();

            if (gameController.GetGameState().IsGameOver)
            {
                EndGame();
                return;
            }

            DrawSnake();
            DrawFood();
            UpdateScore();
        }

        private void DrawMap()
        {
            GameCanvas.Children.Clear();

            var (width, height) = gameController.GetGameState().GetBoardSize();

            // Dostosowanie wymiarów Canvas
            GameCanvas.Width = width * gameController.GetGameState().CellSize;
            GameCanvas.Height = height * gameController.GetGameState().CellSize;

            // Rysowanie siatki
            for (int x = 1; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    var cell = new Rectangle
                    {
                        Width = gameController.GetGameState().CellSize,
                        Height = gameController.GetGameState().CellSize,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 0.5
                    };

                    Canvas.SetLeft(cell, x * gameController.GetGameState().CellSize);
                    Canvas.SetTop(cell, y * gameController.GetGameState().CellSize);
                    GameCanvas.Children.Add(cell);
                }
            }
        }

        private string GetHeadImageUri(Direction direction)
        {
            return direction switch
            {
                Direction.Up => "pack://application:,,,/Assets/SnakeHeadUp.png",
                Direction.Down => "pack://application:,,,/Assets/SnakeHeadDown.png",
                Direction.Left => "pack://application:,,,/Assets/SnakeHeadLeft.png",
                Direction.Right => "pack://application:,,,/Assets/SnakeHeadRight.png",
                _ => throw new InvalidOperationException("Unknown direction")
            };
        }

        private string GetBodyImageUri((int x, int y) segment, (int x, int y) prevSegment, (int x, int y) nextSegment)
        {
            // Sprawdzanie zakrętów
            if ((prevSegment.x < segment.x && nextSegment.y > segment.y) || (nextSegment.x < segment.x && prevSegment.y > segment.y))
            {
                // Zakręt z dołu w lewo
                return "pack://application:,,,/Assets/SnakeBodyTurnTopLeft.png";
            }
            if ((prevSegment.x > segment.x && nextSegment.y > segment.y) || (nextSegment.x > segment.x && prevSegment.y > segment.y))
            {
                // Zakręt z dołu w prawo
                return "pack://application:,,,/Assets/SnakeBodyTurnBottomRight.png";
            }
            if ((prevSegment.x < segment.x && nextSegment.y < segment.y) || (nextSegment.x < segment.x && prevSegment.y < segment.y))
            {
                // Zakręt z góry w lewo
                return "pack://application:,,,/Assets/SnakeBodyTurnBottomLeft.png";
            }
            if ((prevSegment.x > segment.x && nextSegment.y < segment.y) || (nextSegment.x > segment.x && prevSegment.y < segment.y))
            {
                // Zakręt z góry w prawo
                return "pack://application:,,,/Assets/SnakeBodyTurnTopRight.png";
            }

            // Sprawdzanie segmentów prostych
            if (prevSegment.x == segment.x && nextSegment.x == segment.x)
            {
                // Segment w pionie
                return "pack://application:,,,/Assets/SnakeBodyVertical.png";
            }
            if (prevSegment.y == segment.y && nextSegment.y == segment.y)
            {
                // Segment w poziomie
                return "pack://application:,,,/Assets/SnakeBodyHorizontal.png";
            }

            // Domyślny obrazek
            return "pack://application:,,,/Assets/SnakeBodyHorizontal.png";
        }


        private void DrawSnake()
        {
            // Usuwanie poprzednich elementów węża z planszy
            GameCanvas.Children.OfType<Image>().Where(img => img.Tag?.ToString() == "Snake").ToList()
                .ForEach(img => GameCanvas.Children.Remove(img));

            var snake = gameController.GetGameState().Snake;
            var direction = gameController.GetGameState().SnakeDirection;

            // Rysowanie głowy węża
            var head = snake.First(); // Głowa to pierwszy element listy
            var headImage = new Image
            {
                Source = new BitmapImage(new Uri(GetHeadImageUri(direction))),
                Width = gameController.GetGameState().CellSize,
                Height = gameController.GetGameState().CellSize,
                Tag = "Snake" // Używane do identyfikacji elementów węża
            };
            Canvas.SetLeft(headImage, head.Item1 * gameController.GetGameState().CellSize);
            Canvas.SetTop(headImage, head.Item2 * gameController.GetGameState().CellSize);
            GameCanvas.Children.Add(headImage);

            // Rysowanie ciała węża
            for (int i = 1; i < snake.Count; i++)
            {
                var segment = snake[i];
                string bodyImageUri;

                if (i < snake.Count - 1)
                {
                    // Sprawdzamy orientację segmentu w odniesieniu do poprzedniego i następnego
                    var prevSegment = snake[i - 1];
                    var nextSegment = snake[i + 1];
                    bodyImageUri = GetBodyImageUri(segment, prevSegment, nextSegment);
                }
                else
                {
                    // Ostatni segment (ogon) – traktujemy go jako segment poziomy lub pionowy
                    var prevSegment = snake[i - 1];
                    bodyImageUri = GetBodyImageUri(segment, prevSegment, prevSegment); // Porównujemy tylko z poprzednim
                }

                var bodyImage = new Image
                {
                    Source = new BitmapImage(new Uri(bodyImageUri)),
                    Width = gameController.GetGameState().CellSize,
                    Height = gameController.GetGameState().CellSize,
                    Tag = "Snake"
                };
                Canvas.SetLeft(bodyImage, segment.Item1 * gameController.GetGameState().CellSize);
                Canvas.SetTop(bodyImage, segment.Item2 * gameController.GetGameState().CellSize);
                GameCanvas.Children.Add(bodyImage);
            }
        }

        private Image currentFoodImage;

        private void DrawFood()
        {
            if (currentFoodImage != null)
            {
                GameCanvas.Children.Remove(currentFoodImage);
            }

            // Pobranie pozycji jedzenia
            var food = gameController.GetGameState().Food;

            // Tworzenie nowego obrazka jedzenia
            currentFoodImage = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Apple.png")),
                Width = gameController.GetGameState().CellSize,
                Height = gameController.GetGameState().CellSize
            };

            // Ustawianie pozycji obrazka na planszy
            Canvas.SetLeft(currentFoodImage, food.Item1 * gameController.GetGameState().CellSize);
            Canvas.SetTop(currentFoodImage, food.Item2 * gameController.GetGameState().CellSize);

            // Dodanie obrazka do planszy
            GameCanvas.Children.Add(currentFoodImage);
        }

        private void UpdateScore()
        {
            ScoreTextBlock.Text = $"Score: {gameController.GetGameState().Score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var currentDirection = gameController.GetGameState().SnakeDirection;

            Direction? newDirection = null;

            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    if (currentDirection != Direction.Down)
                        newDirection = Direction.Up;
                    break;

                case Key.S:
                case Key.Down:
                    if (currentDirection != Direction.Up)
                        newDirection = Direction.Down;
                    break;

                case Key.A:
                case Key.Left:
                    if (currentDirection != Direction.Right)
                        newDirection = Direction.Left;
                    break;

                case Key.D:
                case Key.Right:
                    if (currentDirection != Direction.Left)
                        newDirection = Direction.Right;
                    break;
            }

            if (newDirection != null && lastInputDirection == null)
            {
                lastInputDirection = newDirection;
            }
        }
    }
}
