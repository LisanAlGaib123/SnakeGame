using SnakeGameLibrary;
using System.Data.SQLite;

namespace SnakeConsoleApp
{

    class Program
    {
        static void Main(string[] args)
        {
            var game = new SnakeGame();                 // Inicjalizacja modelu gry
            var controller = new GameController(game);  // Inicjalizacja kontrolera
            var consoleView = new ConsoleView(controller); // Inicjalizacja widoku konsoli

            consoleView.Run();  // Uruchomienie gry
        }
    }
}
