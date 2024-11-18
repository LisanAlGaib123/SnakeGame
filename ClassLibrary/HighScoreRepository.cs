using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace ClassLibrary
{
    public class HighScoreRepository
    {
        private readonly string connectionString;

        public HighScoreRepository()
        {
            string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscores.db");
            connectionString = $"Data Source={databasePath};Version=3;";
            EnsureDatabaseExists();
        }

        private void EnsureDatabaseExists()
        {
            if (!File.Exists(connectionString.Split('=')[1]))
            {
                SQLiteConnection.CreateFile(connectionString.Split('=')[1]);

                using var connection = new SQLiteConnection(connectionString);
                connection.Open();

                var command = new SQLiteCommand(@"
                CREATE TABLE HighScores (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Score INTEGER NOT NULL
                )", connection);
                command.ExecuteNonQuery();
            }
        }

        public void AddHighScore(string name, int score)
        {
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();

            var command = new SQLiteCommand("INSERT INTO HighScores (Name, Score) VALUES (@Name, @Score)", connection);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Score", score);
            command.ExecuteNonQuery();
        }

        public List<(string Name, int Score)> GetHighScores()
        {
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();

            var command = new SQLiteCommand("SELECT Name, Score FROM HighScores ORDER BY Score DESC LIMIT 10", connection);
            using var reader = command.ExecuteReader();

            var highScores = new List<(string Name, int Score)>();
            while (reader.Read())
            {
                highScores.Add((reader.GetString(0), reader.GetInt32(1)));
            }

            return highScores;
        }
    }
}
