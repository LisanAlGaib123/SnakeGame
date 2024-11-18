using ClassLibrary;
using System;

namespace SnakeGameLibrary
{
    public class GameController
    {
        private readonly SnakeGame game;
        private readonly HighScoreRepository highScoreRepository;

        public GameController(SnakeGame game)
        {
            this.game = game;
            this.highScoreRepository = new HighScoreRepository();
        }

        public void ChangeDirection(Direction direction)
        {
            if ((direction == Direction.Up && game.SnakeDirection != Direction.Down) ||
                (direction == Direction.Down && game.SnakeDirection != Direction.Up) ||
                (direction == Direction.Left && game.SnakeDirection != Direction.Right) ||
                (direction == Direction.Right && game.SnakeDirection != Direction.Left))
            {
                game.SnakeDirection = direction;
            }
        }

        public void UpdateGame()
        {
            game.MoveSnake();
        }

        public SnakeGame GetGameState()
        {
            return game;
        }

        public List<(string Name, int Score)> GetHighScores()
        {
            return highScoreRepository.GetHighScores();
        }

        public void SaveHighScore(string name, int score)
        {
            highScoreRepository.AddHighScore(name, score);
        }
    }
}
