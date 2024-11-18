using System;
using System.Collections.Generic;


namespace SnakeGameLibrary
{
    public enum Direction { Up, Down, Left, Right }

    public class SnakeGame
    {
        private const int DefaultBoardWidth = 20;
        private const int DefaultBoardHeight = 10;
        public bool IsCustomSize { get; private set; } = false; // Flaga informująca, czy mapa ma niestandardowy rozmiar
        public int BoardWidth { get; private set; } = DefaultBoardWidth;
        public int BoardHeight { get; private set; } = DefaultBoardHeight;
        public List<(int x, int y)> Snake { get; private set; }
        public (int x, int y) Food { get; private set; }
        public Direction SnakeDirection { get; set; }
        public bool IsGameOver { get; private set; }
        public int Score { get; private set; }
        public bool ScoreChanged { get; private set; }

        private Random random;

        public SnakeGame(int boardWidth = 20, int boardHeight = 10)
        {
            Score = 0;
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            Snake = new List<(int x, int y)> { (boardWidth / 2, boardHeight / 2) };
            random = new Random();
            PlaceFood();
            SnakeDirection = Direction.Right;
            IsGameOver = false;
        }

        public void ResetGame()
        {
            Score = 0;
            IsGameOver = false;
            ScoreChanged = true; // Aby zaktualizować początkowy wynik
            Snake.Clear();
            Snake.Add((BoardWidth / 2, BoardHeight / 2)); // Ustaw początkową pozycję węża na środek planszy
            SnakeDirection = Direction.Right;
            PlaceFood();
        }

        public void MoveSnake()
        {
            if (IsGameOver) return;

            var head = Snake[0];
            (int x, int y) newHead = head;

            switch (SnakeDirection)
            {
                case Direction.Up: newHead.y--; break;
                case Direction.Down: newHead.y++; break;
                case Direction.Left: newHead.x--; break;
                case Direction.Right: newHead.x++; break;
            }

            // Sprawdzanie kolizji z krawędzią planszy
            if (newHead.x <= 0 || newHead.x >= BoardWidth || newHead.y <= 0 || newHead.y >= BoardHeight || Snake.Contains(newHead))
            {
                IsGameOver = true;
                return;
            }

            Snake.Insert(0, newHead);

            // Sprawdzanie czy wąż zjadł jedzenie
            if (newHead == Food)
            {
                EatFood();
                PlaceFood();
            }
            else
            {
                Snake.RemoveAt(Snake.Count - 1); // Usuwa ogon węża jeśli nie było jedzenia
            }
        }

        private void PlaceFood()
        {
            (int x, int y) position;
            do
            {
                position = (random.Next(1, BoardWidth-1), random.Next(1, BoardHeight-1));

            } while (Snake.Contains(position));

            Food = position;
        }

        public void EatFood()
        {
            Score++;
            ScoreChanged = true; // Flaga, że punkty się zmieniły
        }

        public void ResetScoreChangeFlag()
        {
            ScoreChanged = false; // Resetowanie flagi po narysowaniu punktów
        }

        public void SetBoardSize(int width, int height)
        {
            BoardWidth = width;
            BoardHeight = height;
            ResetGame();
        }

        public void ResetToDefaultSize()
        {
            BoardWidth = DefaultBoardWidth;
            BoardHeight = DefaultBoardHeight;
            ResetGame();  // Resetujemy grę po przywróceniu domyślnych wymiarów
        }

    }
}
