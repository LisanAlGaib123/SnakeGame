using System;
using System.Threading;
using SnakeGameLibrary;
using Spectre.Console;

namespace SnakeConsoleApp
{
    public class ConsoleView
    {
        private GameController controller;
        private (int x, int y)? lastTailPosition; // Zapamiętanie pozycji ogona

        public ConsoleView(GameController controller)
        {
            this.controller = controller;
            Console.CursorVisible = false; // Ukrywa kursor dla lepszej widoczności
        }

        public void Run()
        {
            DisplayMenu();  
        }

        private void CustomMapGame()
        {
            AnsiConsole.Clear();

            // Pobranie szerokości mapy
            int width = AnsiConsole.Prompt(
                new TextPrompt<int>("[yellow]Enter the map width (10-50):[/]")
                    .ValidationErrorMessage("[red]Invalid input. Please enter a number from 10 to 50.[/]")
                    .Validate(value => value >= 10 && value <= 50));

            // Pobranie wysokości mapy
            int height = AnsiConsole.Prompt(
                new TextPrompt<int>("[yellow]Enter the map height (10-50):[/]")
                    .ValidationErrorMessage("[red]Invalid input. Please enter a number from 10 to 50.[/]")
                    .Validate(value => value >= 10 && value <= 50));

            // Ustawienie nowego rozmiaru planszy i uruchomienie gry
            controller.GetGameState().SetBoardSize(width, height);
            StartGame();
        }



        private void DisplayMenu()
        {
            AnsiConsole.Clear();


            // Nagłówek gry
            AnsiConsole.Write(
                new FigletText("Snake Game")
                    .Centered()
                    .Color(Color.Green));

            AnsiConsole.Write(new Markup(@"
       ---_ ......._-_--.
      (|\ /      / /| \  \
      /  /     .'  -=-'   `.
     /  /    .'             )
   _/  /   .'        _.)   /
  / o   o        _.-' /  .'
  \          _.-'    / .'*|
   \______.-'//    .'.' \*|
    \|  \ | //   .'.' _ |*|
     `   \|//  .'.'_ _ _|*|
      .  .// .'.' | _ _ \*|
      \`-|\_/ /    \ _ _ \*\
       `/'\__/      \ _ _ \*\
      /^|            \ _ _ \*
     '  `             \ _ _ \      ASH (+VK)
                       \_
").Centered());

            // Tworzenie interaktywnego menu w ramce
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Main Menu[/]")
                    .PageSize(10) // Liczba widocznych opcji (jeśli jest więcej, doda przewijanie)
                    .AddChoices("Start Game", "Custom Map", "High Scores", "Exit")
                    .WrapAround(false)
                    .HighlightStyle(new Style(Color.Blue, decoration: Decoration.Bold)));

            switch (option)
            {
                case "Start Game":
                    if (controller.GetGameState().IsCustomSize)
                    {
                        // Jeśli rozmiar mapy jest niestandardowy, nie resetujemy do domyślnych wymiarów
                        StartGame();
                    }
                    else
                    {
                        // Jeśli rozmiar nie został zmieniony, przywracamy domyślny rozmiar
                        controller.GetGameState().ResetToDefaultSize();
                        StartGame();
                    }
                    break;
                case "Custom Map":
                    CustomMapGame(); // Konfiguracja i uruchomienie gry
                    break;
                case "High Scores":
                    DisplayHighScores();
                    break;
                case "Exit":
                    AnsiConsole.Markup("[Green]GoodBye[/]");
                    Environment.Exit(0);
                    break;
            }
        }


        private void StartGame()
        {
            controller.GetGameState().ResetGame();
            AnsiConsole.Clear();
            // Inicjalizacja planszy
            DrawBorders();

            StartScreen();

            while (Console.ReadKey(true).Key != ConsoleKey.Spacebar)
            {
                // Czekamy na naciśnięcie spacji
            }

            while (!controller.GetGameState().IsGameOver)
            {
                Render();
                GetInput();
                controller.UpdateGame();
                Thread.Sleep(100); // Opóźnienie dla płynności
            }

            GameOverScreen();
            DisplayMenu(); // Powrót do menu po zakończeniu gry
        }

        private void StartScreen()
        {
            AnsiConsole.Markup("[bold green]Space[/] - Start\n");
            AnsiConsole.Markup("[bold green]W/UpArrow[/] - Up\n");
            AnsiConsole.Markup("[bold green]S/DownArrow[/] - Down\n");
            AnsiConsole.Markup("[bold green]A/LeftArrow[/] - Left\n");
            AnsiConsole.Markup("[bold green]D/RightArrow[/] - Right");
        }

        private void GameOverScreen()
        {
            AnsiConsole.Clear();
            var gameState = controller.GetGameState();
            var score = gameState.Score;

            AnsiConsole.Markup("[bold red]Game Over![/]\n");
            AnsiConsole.Markup($"[yellow]Score:[/] {score}\n\n");

            // Pobranie aktualnych wyników
            var highScores = controller.GetHighScores();

            // Sprawdzenie, czy wynik mieści się w top 10
            bool qualifiesForHighScore = highScores.Count < 10 || score > highScores[^1].Score;

            if (qualifiesForHighScore)
            {
                AnsiConsole.Markup("[yellow]Congratulations! You made it to the high scores![/]\n");
                AnsiConsole.Markup("Enter your name: ");
                string name = Console.ReadLine() ?? "Anonymous";
                controller.SaveHighScore(name, score);
            }
            else
            {
                AnsiConsole.Markup("[gray]Your score did not qualify for the high scores.[/]\n");
            }

            AnsiConsole.Markup("[gray]Press [bold green]Spacebar[/] or [bold green]Escape[/] to return to menu...[/]");

            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
            } while (key != ConsoleKey.Spacebar && key != ConsoleKey.Escape);
        }

        private void DisplayHighScores()
        {
            AnsiConsole.Clear();
            var highScores = controller.GetHighScores(); // Pobierz wyniki z repozytorium

            if (highScores.Count == 0)
            {
                AnsiConsole.Markup("[yellow]No high scores yet.[/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("[green]Rank[/]");
                table.AddColumn("[green]Name[/]");
                table.AddColumn("[green]Score[/]");

                for (int i = 0; i < highScores.Count; i++)
                {
                    var (name, score) = highScores[i];
                    table.AddRow($"#{i + 1}", name, score.ToString());
                }

                AnsiConsole.Write(table);
            }

            AnsiConsole.Markup("\nPress [bold yellow]Escape[/] to return to menu...");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
            }

            DisplayMenu();
        }

        private void Render()
        {
            var game = controller.GetGameState();
            var snake = game.Snake;

            // Usuń ostatni ogon, jeśli istnieje
            if (lastTailPosition.HasValue)
            {
                Console.SetCursorPosition(lastTailPosition.Value.x, lastTailPosition.Value.y);
                Console.Write(" ");
            }

            // Rysowanie głowy węża
            var head = snake[0];
            Console.SetCursorPosition(head.x, head.y);
            Console.Write(game.SnakeDirection switch
            {
                Direction.Right => ">",
                Direction.Left => "<",
                Direction.Up => "^",
                Direction.Down => "v",
                _ => "o"
            });

            // Rysowanie ciała węża
            for (int i = 1; i < snake.Count; i++)
            {
                var segment = snake[i];
                Console.SetCursorPosition(segment.x, segment.y);
                Console.Write("o");
            }

            // Zapamiętanie pozycji ogona przed aktualizacją
            lastTailPosition = snake[^1];

            DrawFood();

            // Jeśli punktacja się zmieniła, narysuj ją
            if (game.ScoreChanged)
            {
                Console.CursorVisible = false; // Ukrycie kursora
                Console.SetCursorPosition(0, game.BoardHeight + 6);
                Console.WriteLine("Score: " + game.Score);
                game.ResetScoreChangeFlag(); // Resetuj flagę po narysowaniu
            }
        }

        private void DrawBorders()
        {
            var game = controller.GetGameState();
            int width = game.BoardWidth;
            int height = game.BoardHeight;

            var borderPanel = new Panel("")
            {
                Border = BoxBorder.Square, 
                BorderStyle = new Style(Color.White),
                Padding = new Padding(0, 0, 0, 0), 
                Width = width+1,
                Height = height+1
            };

            // Wyświetlenie ramki w konsoli
            AnsiConsole.Clear();
            AnsiConsole.Write(borderPanel);
        }


        private void DrawFood()
        {
            var game = controller.GetGameState();
            Console.CursorVisible = false; // Ukrycie kursora
            Console.SetCursorPosition(game.Food.x, game.Food.y);
            Console.Write("X");
        }

        private void GetInput()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.W:
                        controller.ChangeDirection(Direction.Up);
                        break;
                    case ConsoleKey.S:
                        controller.ChangeDirection(Direction.Down);
                        break;
                    case ConsoleKey.A:
                        controller.ChangeDirection(Direction.Left);
                        break;
                    case ConsoleKey.D:
                        controller.ChangeDirection(Direction.Right);
                        break;
                    case ConsoleKey.UpArrow:
                        controller.ChangeDirection(Direction.Up);
                        break;
                    case ConsoleKey.DownArrow:
                        controller.ChangeDirection(Direction.Down);
                        break;
                    case ConsoleKey.LeftArrow:
                        controller.ChangeDirection(Direction.Left);
                        break;
                    case ConsoleKey.RightArrow:
                        controller.ChangeDirection(Direction.Right);
                        break;
                }
            }
        }

    }
}
