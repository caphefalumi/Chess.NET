using SplashKitSDK;
using System;
using System.Diagnostics;

namespace Chess
{
    public class GameplayScreen : ScreenState
    {
        private readonly Game _game;
        private readonly Board _board;
        private readonly MatchConfiguration _config;
        private Button _menuButton;
        private Button _undoButton;
        private Button _resetButton;
        private Button _gameOverNewGameButton;
        private static Clock _clock;
        private MatchState _gameState;
        private static bool _gameOver;
        private static string _gameOverMessage;
        private ChessBot _bot; // For computer mode
        private Stopwatch _botThinkTimer; // Timer to control bot thinking
        private bool _botIsThinking = false; // Flag to indicate bot is "thinking"
        private Move _botSelectedMove = null; // The move the bot has selected
        public static bool PromotionFlag;
        public GameplayScreen(Game game, Board board, MatchConfiguration config)
        {
            _game = game;
            _board = board;
            _config = config;
            _gameState = game.GetGameState();
            _board.MatchState = _gameState;

            // Create the clock with the configured time settings
            _clock = new Clock(config.GetTimeSpan(), config.GetIncrementSpan());

            // Setup UI buttons
            _menuButton = new Button("Menu", 610, 10, 80, 30);
            _undoButton = new Button("Undo", 610, 50, 80, 30);
            _resetButton = new Button("Reset", 610, 90, 80, 30);
            _gameOverNewGameButton = new Button("New Game", 250, 470, 200, 50);

            _gameOver = false;
            _gameOverMessage = "";

            // Initialize AI if in computer mode
            if (config.Mode == Variant.Computer)
            {
                _bot = new ChessBot(_board);
                _botThinkTimer = new Stopwatch();
            }

            _clock.Start();

            // Subscribe to time expiration event
            _clock.OnTimeExpired += DeclareGameOver;
        }

        public override void HandleInput()
        {
            if (_gameOver)
            {
                if (_gameOverNewGameButton.IsClicked())
                {
                    ResetBoard();
                }
                return;
            }

            // Handle human input - only if it's not the computer's turn or not in computer mode
            if (!_botIsThinking && (_config.Mode != Variant.Computer || _gameState.CurrentPlayer == Player.White))
            {
                BoardEvent.HandleMouseEvents(_board, _gameState);
            }

            // Handle menu buttons
            if (_menuButton.IsClicked())
            {
                _game.ChangeState(new MainMenuState(_game, _board));
            }
            else if (_undoButton.IsClicked() || SplashKit.KeyTyped(KeyCode.ZKey))
            {
                BoardEvent.HandleUndo();
                _clock.CurrentTurn = _gameState.CurrentPlayer;

                // If it's now computer's turn after undo, reset thinking state
                if (_config.Mode == Variant.Computer && _gameState.CurrentPlayer == Player.Black)
                {
                    _botIsThinking = false;
                }
            }
            else if (_resetButton.IsClicked() || SplashKit.KeyTyped(KeyCode.RKey))
            {
                ResetBoard();
            }
        }

        public override void Update()
        {
            if (_gameOver) return;

            _menuButton.Update();
            _undoButton.Update();
            _resetButton.Update();

            // Update clock
            _clock.CurrentTurn = _gameState.CurrentPlayer;
            _clock.Update();

            // Computer Move Logic
            if (_config.Mode == Variant.Computer && _gameState.CurrentPlayer == Player.Black)
            {
                // Handle computer move in stages for a better user experience
                if (!_botIsThinking)
                {
                    // Start thinking process
                    _botIsThinking = true;
                    _botThinkTimer.Restart();

                    // Determine how much time the bot should "think"
                    TimeSpan remainingTime = _clock.BlackTime;
                    int thinkTime;

                    // Adjust search strategy based on remaining time
                    if (remainingTime.TotalSeconds < 10)
                    {
                        // Low time - make quick moves
                        _bot.AdjustDepthForTime(500); // Very shallow search
                        thinkTime = 300; // Appear to think for 300ms
                    }
                    else if (remainingTime.TotalSeconds < 30)
                    {
                        // Limited time - make reasonable moves quickly
                        _bot.AdjustDepthForTime(1000);
                        thinkTime = 500; // Think for 500ms
                    }
                    else
                    {
                        // Plenty of time - make strong moves
                        _bot.AdjustDepthForTime((int)remainingTime.TotalMilliseconds);
                        thinkTime = 1000; // Think for 1 second
                    }

                    // Calculate the move on a separate thread to avoid freezing UI
                    Task.Run(() =>
                    {
                        _botSelectedMove = _bot.GetBestMove(thinkTime);
                    });
                }
                else if (_botThinkTimer.ElapsedMilliseconds >= 500 && _botSelectedMove is not null)
                {
                    // Bot has finished "thinking" - make the move
                    _gameState.MakeMove(_botSelectedMove);
                    _clock.SwitchTurn();
                    BoardEvent.CheckGameResult(); // Check if the move resulted in game over
                    _botIsThinking = false;
                    _botSelectedMove = null;
                }
            }

            // Special logic for Spell Chess mode
            if (_config.Mode == Variant.SpellChess)
            {
                HandleSpellChessLogic();
            }
        }
        public static PieceType HandlePromotionSelection()
        {
            PieceType promotedPiece = PieceType.Pawn;
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                int x = 250, y = 200, width = 80, height = 80;
                float mx = SplashKit.MouseX(), my = SplashKit.MouseY();

                if (mx >= x && mx <= x + width)
                {
                    if (my >= y && my <= y + height * 4)
                    {
                        int choice = (int)((my - y) / height);
                        PieceType[] choices = { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight };
                        promotedPiece = choices[choice];
                        PromotionFlag = false; // Reset the promotion flag
                    }
                }
            }
            return promotedPiece;
        }

        private void HandleSpellChessLogic()
        {
            // Implement special spell chess logic
            // This would include spell effects, power-ups, etc.
        }

        private void ResetBoard()
        {
            _board.Pieces = PieceFactory.CreatePieces(_board);
            _board.BoardHighlights.Clear();
            for (int i = 0; i < _board.BackgroundOverlays.Length; i++)
            {
                _board.BackgroundOverlays[i] = null;
            }

            _gameOver = false;
            _gameOverMessage = "";
            _botIsThinking = false;
            _botSelectedMove = null;
            _clock.Reset(_config.GetTimeSpan());
            _clock.Start();
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);
            _board.Draw();

            if (_gameOver)
            {
                // Game Over UI
                SplashKit.FillRectangle(Color.RGBAColor(30, 30, 30, 200), 200, 200, 300, 200);
                SplashKit.DrawRectangle(Color.Black, 200, 200, 300, 200);
                SplashKit.DrawText("Game Over", Color.White, "Arial", 24, 270, 220);
                SplashKit.DrawText(_gameOverMessage, Color.White, "Arial", 18, 230, 260);
                _gameOverNewGameButton.Draw();
            }
            else
            {
                // Game UI
                _menuButton.Draw();
                _undoButton.Draw();
                _resetButton.Draw();

                // Draw clock
                string whiteTime = _clock.GetFormattedTime(Player.White);
                string blackTime = _clock.GetFormattedTime(Player.Black);

                // Create a nicer time display
                DrawTimeDisplay(Player.White, whiteTime, 600, 150);
                DrawTimeDisplay(Player.Black, blackTime, 600, 200);

                // Draw current player indicator
                string currentPlayer = _gameState.CurrentPlayer == Player.White ? "White" : "Black";
                string modeText = GetModeDisplayText();
                SplashKit.DrawText($"Mode: {modeText}", Color.Black, "Arial", 16, 10, 610);
                SplashKit.DrawText($"Current Player: {currentPlayer}", Color.Black, "Arial", 16, 10, 640);

                // Show "thinking" indicator when bot is thinking
                if (_botIsThinking)
                {
                    SplashKit.DrawText("Computer is thinking...", Color.Black, "Arial", 16, 10, 580);
                }

                // Draw increment info if using increment
                if (_config.UseIncrement)
                {
                    SplashKit.DrawText($"Increment: +{_config.IncrementSeconds}s", Color.Black, "Arial", 14, 10, 670);
                }
            }
            if (!_botIsThinking)
            {
                SplashKit.RefreshScreen();
            }
        }

        private void DrawTimeDisplay(Player player, string time, int x, int y)
        {
            string playerLabel = player == Player.White ? "White" : "Black";
            Color bgColor = player == Player.White ? Color.White : Color.Black;
            Color textColor = player == Player.White ? Color.Black : Color.White;

            // Draw background
            SplashKit.FillRectangle(bgColor, x, y, 80, 30);
            SplashKit.DrawRectangle(Color.Gray, x, y, 80, 30);

            // Draw text
            SplashKit.DrawText($"{playerLabel}: {time}", textColor, "Arial", 14, x + 5, y + 8);

            // Highlight current player
            if (_gameState.CurrentPlayer == player)
            {
                SplashKit.DrawRectangle(Color.RGBAColor(0, 255, 0, 150), x, y, 80, 30);
            }
        }

        private string GetModeDisplayText()
        {
            return _config.Mode switch
            {
                Variant.TwoPlayer => "Two Player",
                Variant.Computer => "vs Computer",
                Variant.SpellChess => "Spell Chess",
                Variant.Custom => "Custom",
                _ => "Standard"
            };
        }

        public static void SwitchTurn()
        {
            _clock.SwitchTurn();
        }

        public static void DeclareGameOver(string msg)
        {
            _gameOver = true;
            _gameOverMessage = msg;
        }

        public override string GetStateName() => "GamePlay";
    }
}
