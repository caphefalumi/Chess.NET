using SplashKitSDK;
using System;
using System.Diagnostics;
using System.Collections.Generic;

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
        private static bool _showPromotionMenu;
        private static Move _promotionMove;
        private static Player _promotionColor;
        private static Dictionary<PieceType, Bitmap> _promotionPieces;
        private static Dictionary<PieceType, Rectangle> _promotionButtons;  // Add this for click detection

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
            // First check for game over state
            if (_gameOver)
            {
                if (_gameOverNewGameButton.IsClicked())
                {
                    ResetBoard();
                }
                return;
            }

            // Then check for promotion menu
            if (_showPromotionMenu && SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                HandlePromotionSelection(); // If promotion menu is active, don't handle other inputs
            }

            // Handle menu buttons
            if (_menuButton.IsClicked())
            {
                _game.ChangeState(new MainMenuState(_game, _board));
                return;
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
                return;
            }
            else if (_resetButton.IsClicked() || SplashKit.KeyTyped(KeyCode.RKey))
            {
                ResetBoard();
                return;
            }

            // Handle human input - only if it's not the computer's turn or not in computer mode
            if (!_botIsThinking && (_config.Mode != Variant.Computer || _gameState.CurrentPlayer == Player.White))
            {
                BoardEvent.HandleMouseEvents(_board, _gameState);
            }
        }

        public override void Update()
        {
            if (_gameOver) 
            {
                _gameOverNewGameButton.Update();
                return;
            }

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
                        thinkTime = 300; // Very shallow search
                    }
                    else if (remainingTime.TotalSeconds < 30)
                    {
                        // Limited time - make reasonable moves quickly
                        thinkTime = 500;
                    }
                    else
                    {
                        // Plenty of time - make strong moves
                        thinkTime = 1000;
                    }

                    // Calculate the move on a separate thread to avoid freezing UI
                    Task.Run(() =>
                    {
                        _botSelectedMove = _bot.GetBestMove(thinkTime);
                    });
                }
                else if (_botThinkTimer.ElapsedMilliseconds >= 500 && _botSelectedMove != null)
                {
                    // Execute the bot's move
                    _gameState.MakeMove(_botSelectedMove);
                    _clock.SwitchTurn();
                    BoardEvent.CheckGameResult();
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
        public static void HandlePromotionSelection()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                Point2D clickPoint = new Point2D() { X = SplashKit.MouseX(), Y = SplashKit.MouseY() };

                foreach (var button in _promotionButtons)
                {
                    if (button.Value.IsAt(clickPoint))
                    {
                        // Create and execute the promotion move
                        Move promotionMove = new PromotionMove(_promotionMove.From, _promotionMove.To, _promotionMove.MovedPiece, button.Key);
                        BoardEvent.HandleMove(promotionMove);
                        _showPromotionMenu = false;
                        PromotionFlag = false;
                        break;
                    }
                }
            }
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

            // Draw promotion menu if active (draw before game over UI)
            if (_showPromotionMenu)
            {
                // Calculate center position of the board
                int menuX = 160; // Center horizontally (640/2 - 320/2)
                int menuY = 280; // Center vertically (640/2 - 80/2)

                // Draw menu background (white color)
                SplashKit.FillRectangle(Color.White, menuX, menuY, 320, 80);
                SplashKit.DrawRectangle(Color.Gray, menuX, menuY, 320, 80);

                // Draw piece options horizontally with their click detection rectangles
                int x = menuX;
                foreach (KeyValuePair<PieceType, Bitmap> piece in _promotionPieces)
                {
                    // Calculate the padding to center the piece in the 80x80 box
                    float pieceSize = 70.0f; // Slightly smaller than the 80x80 box
                    float padding = (80 - pieceSize) / 2; // Center the piece in the box
                    
                    // Draw piece image centered in the box
                    SplashKit.DrawBitmap(
                        piece.Value, 
                        x + padding - 25, 
                        menuY + padding - 45, 
                        SplashKit.OptionScaleBmp(pieceSize / piece.Value.Width, pieceSize / piece.Value.Height)
                    );
                    
                    // Draw transparent rectangle for click detection
                    _promotionButtons[piece.Key].Draw();
                    
                    x += 80; // Move to next piece position
                }
            }

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
            }
            
            // Always refresh the screen except when actually executing the bot's move
            if (!(_botIsThinking && _botSelectedMove != null))
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

        public static void ShowPromotionMenu(Move move, Player color)
        {
            _showPromotionMenu = true;
            _promotionMove = move;
            _promotionColor = color;
            PromotionFlag = true;

            // Center position of the board
            int menuX = 160; // Center horizontally (640/2 - 320/2)
            int menuY = 280; // Center vertically (640/2 - 80/2)

            _promotionPieces = new Dictionary<PieceType, Bitmap>
            {
                { PieceType.Queen, new Bitmap("wQ", $"Resources\\Pieces\\wQ.png") },
                { PieceType.Rook, new Bitmap("wR", $"Resources\\Pieces\\wR.png") },
                { PieceType.Bishop, new Bitmap("wB", $"Resources\\Pieces\\wB.png") },
                { PieceType.Knight, new Bitmap("wN", $"Resources\\Pieces\\wN.png") }
            };

            // Create transparent rectangles for click detection
            _promotionButtons = new Dictionary<PieceType, Rectangle>
            {
                { PieceType.Queen, new Rectangle(Color.RGBAColor(0,0,0,0), menuX, menuY, 80, 80) },
                { PieceType.Rook, new Rectangle(Color.RGBAColor(0,0,0,0), menuX + 80, menuY, 80, 80) },
                { PieceType.Bishop, new Rectangle(Color.RGBAColor(0,0,0,0), menuX + 160, menuY, 80, 80) },
                { PieceType.Knight, new Rectangle(Color.RGBAColor(0,0,0,0), menuX + 240, menuY, 80, 80) }
            };
        }

        public override string GetStateName() => "GamePlay";
    }
}
