using SplashKitSDK;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public static bool PromotionFlag;
        private static bool _showPromotionMenu;
        private static Move _promotionMove;
        private static Dictionary<PieceType, Bitmap> _promotionPieces;
        private static Dictionary<PieceType, Rectangle> _promotionButtons;
        private TextLabel _statusLabel;
        private ChessBot _chessBot;
        private NetworkManager _networkManager;
        private bool _botIsThinking = false;
        private bool _isMyTurn = true;
        private const int MAX_FILENAME_LENGTH = 20;

        public GameplayScreen(Game game, Board board, MatchConfiguration config)
        {
            _game = game;
            _board = board;
            _config = config;
            _gameState = game.GetGameState();
            _board.MatchState = _gameState;

            // Create the clock with the configured time settings using singleton pattern
            _clock = Clock.GetInstance(config.GetTimeSpan(), config.GetIncrementSpan());
            
            // Setup UI buttons
            _menuButton = new Button("Menu", 610, 10, 80, 30);
            _undoButton = new Button("Undo", 610, 50, 80, 30);
            _resetButton = new Button("Reset", 610, 90, 80, 30);
            _gameOverNewGameButton = new Button("New Game", 250, 470, 200, 50);
            
            // Setup status label
            _statusLabel = new TextLabel("", 610, 220, 120, 60);

            _gameOver = false;
            _gameOverMessage = "";

            // Initialize AI if in computer mode
            if (config.Mode == Variant.Computer)
            {
                _chessBot = ChessBot.GetInstance(_board, _gameState.CurrentPlayer);
            }

            // Initialize network manager if in network mode
            if (config.NetworkRole != NetworkRole.None)
            {
                _networkManager = new NetworkManager();
                // Set initial turn based on network role
                _isMyTurn = config.NetworkRole == NetworkRole.Host;
                UpdateStatusLabel(_isMyTurn ? "Your turn" : "Opponent's turn");

                // Subscribe to network events
                _networkManager.OnMoveReceived += HandleNetworkMove;
                _networkManager.OnConnectionStatusChanged += HandleConnectionStatus;
            }

            // Initialize BoardEvent
            BoardEvent.Initialize(_board);
            BoardEvent.OnMoveMade += HandleLocalMove;

            _clock.Start();
            _clock.OnTimeExpired += DeclareGameOver;
        }

        private void HandleNetworkMove(string moveData)
        {
            Console.WriteLine($"[GameplayScreen] Received network move: {moveData}");
            // Parse the move data and apply it to the board
            Move move = Move.ConvertNotation(moveData, _board);
            if (move != null)
            {
                Console.WriteLine($"[GameplayScreen] Applying move from {move.From} to {move.To}");
                _gameState.MakeMove(move);
                _clock.SwitchTurn();
                _isMyTurn = true;
                UpdateStatusLabel("Your turn");
                BoardEvent.CheckGameResult();
            }
            else
            {
                Console.WriteLine("[GameplayScreen] Failed to parse received move");
            }
        }

        private void HandleLocalMove(Move move)
        {
            if (_config.NetworkRole != NetworkRole.None && _isMyTurn)
            {
                Console.WriteLine($"[GameplayScreen] Sending local move: {move}");
                // Send move notation to opponent
                _networkManager.SendMove(move.ToString());
                _isMyTurn = false;
                UpdateStatusLabel("Opponent's turn");
            }
        }

        private void HandleConnectionStatus(bool isConnected)
        {
            Console.WriteLine($"[GameplayScreen] Connection status changed: {isConnected}");
            UpdateStatusLabel(isConnected ? 
                (_isMyTurn ? "Your turn" : "Opponent's turn") : 
                "Disconnected");
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
                HandlePromotionSelection();
                return;
            }

            // Handle menu buttons
            if (_menuButton.IsClicked() || SplashKit.KeyTyped(KeyCode.EscapeKey))
            {
                _networkManager?.Cleanup();
                ResetBoard();
                _game.ChangeState(new MainMenuState(_game, _board));
                return;
            }

            if (_undoButton.IsClicked() || SplashKit.KeyTyped(KeyCode.ZKey))
            {
                // Only allow undo in non-network games
                if (_config.NetworkRole == NetworkRole.None)
                {
                    BoardEvent.HandleUndo();
                    _clock.CurrentTurn = _gameState.CurrentPlayer;
                    if (_config.Mode == Variant.Computer && _gameState.CurrentPlayer == Player.Black)
                    {
                        _botIsThinking = false;
                    }
                }
                return;
            }

            if (_resetButton.IsClicked() || SplashKit.KeyTyped(KeyCode.RKey))
            {
                // Only allow reset in non-network games
                if (_config.NetworkRole == NetworkRole.None)
                {
                    ResetBoard();
                }
                return;
            }
            if (SplashKit.KeyTyped(KeyCode.FKey))
            {
                _board.Flip();
            }
            // Handle human input - only if it's the player's turn
            bool canMove = _config.NetworkRole == NetworkRole.None || 
                          (_config.NetworkRole != NetworkRole.None && _isMyTurn);
            
            if (!_botIsThinking && canMove && 
                (_config.Mode != Variant.Computer || _gameState.CurrentPlayer == Player.White))
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
            if (_config.Mode == Variant.Computer && _gameState.CurrentPlayer == Player.Black && !_botIsThinking)
            {
                _botIsThinking = true;
                
                // Use Task.Run to not block the UI thread
                Task.Run(async () =>
                {
                    try
                    {
                        // Get best move directly from ChessBot
                        Move bestMove = await _chessBot.GetBestMove();
                        
                        if (bestMove != null)
                        {
                            // Execute the move on the board
                            _gameState.MakeMove(bestMove);
                            _clock.SwitchTurn();
                            BoardEvent.CheckGameResult();
                        }
                    }
                    finally
                    {
                        _botIsThinking = false;
                    }
                });
            }
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);
            _board.Draw();

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

            // Draw the player indicators and spell counts
            DrawPlayerInfo(Player.White, 20, 5);
            DrawPlayerInfo(Player.Black, 20, SplashKit.ScreenHeight() - 60);
            
            // Draw game buttons
            _menuButton.Draw();
            _undoButton.Draw();
            _resetButton.Draw();

            // Draw game over message if applicable
            if (_gameOver)
            {
                // Draw semi-transparent overlay
                Rectangle overlay = new Rectangle(Color.RGBAColor(0, 0, 0, 128), 0, 0, 
                    SplashKit.ScreenWidth(), SplashKit.ScreenHeight());
                overlay.Draw();
                
                // Draw game over text
                Font font = SplashKit.LoadFont("Arial", "Arial.ttf");
                SplashKit.DrawText(_gameOverMessage, Color.White, font, 36, 
                    SplashKit.ScreenWidth() / 2 - 150, SplashKit.ScreenHeight() / 2 - 50);
                
                _gameOverNewGameButton.Draw();
            }

            // Draw time display
            DrawTimeDisplay(Player.White, _clock.WhiteTime.ToString(@"mm\:ss"), 630, 250);
            DrawTimeDisplay(Player.Black, _clock.BlackTime.ToString(@"mm\:ss"), 630, 300);

            // Draw current mode text
            string modeText = GetModeDisplayText();
            SplashKit.DrawText(modeText, Color.Black, 630, 350);

            // Draw status label
            if (!string.IsNullOrEmpty(_statusLabel.Text))
            {
                _statusLabel.Draw();
            }
            
            SplashKit.RefreshScreen();
        }
        
        private void DrawPlayerInfo(Player player, int x, int y)
        {
            string playerName = player == Player.White ? "White" : "Black";
            Color playerBoxColor = player == Player.White ? Color.White : Color.Black;
            Color textColor = player == Player.White ? Color.Black : Color.White;
            
            // Draw player background
            SplashKit.FillRectangle(playerBoxColor, x - 5, y - 5, 90, 30);
            SplashKit.DrawRectangle(Color.Gray, x - 5, y - 5, 90, 30);
            SplashKit.DrawText(playerName, textColor, x, y);
            
        }

        private void DrawTimeDisplay(Player player, string timeStr, int x, int y)
        {
            string playerText = player == Player.White ? "White: " : "Black: ";
            Color textColor = player == _gameState.CurrentPlayer ? Color.Blue : Color.Black;
            SplashKit.DrawText(playerText + timeStr, textColor, x, y);
        }

        private string GetModeDisplayText()
        {
            return _config.Mode switch
            {
                Variant.TwoPlayer => "Two Player Mode",
                Variant.Computer => "Computer Mode",
                Variant.Custom => "Custom Mode",
                Variant.Network => "Network Mode",
                _ => "Chess"
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
            PromotionFlag = true;

            // Center position of the board
            int menuX = 160; // Center horizontally (640/2 - 320/2)
            int menuY = 280; // Center vertically (640/2 - 80/2)

            // Use existing piece bitmap images instead of creating new ones
            _promotionPieces = new Dictionary<PieceType, Bitmap>
            {
                { PieceType.Queen, Piece.GetPieceBitmap(PieceType.Queen, color) },
                { PieceType.Rook, Piece.GetPieceBitmap(PieceType.Rook, color) },
                { PieceType.Bishop, Piece.GetPieceBitmap(PieceType.Bishop, color) },
                { PieceType.Knight, Piece.GetPieceBitmap(PieceType.Knight, color) }
            };

            // Create transparent rectangles for click detection
            _promotionButtons = new Dictionary<PieceType, Rectangle>
            {
                { PieceType.Queen, new Rectangle(Color.Transparent, menuX, menuY, 80, 80) },
                { PieceType.Rook, new Rectangle(Color.Transparent, menuX + 80, menuY, 80, 80) },
                { PieceType.Bishop, new Rectangle(Color.Transparent, menuX + 160, menuY, 80, 80) },
                { PieceType.Knight, new Rectangle(Color.Transparent, menuX + 240, menuY, 80, 80) }
            };
        }

        public override string GetStateName() => "GamePlay";

        private void ResetBoard()
        {
            // Reset the board using the OOP approach
            _board.ResetBoard();
            _gameState.Reset();
            
            // Reset UI state
            _gameOver = false;
            _gameOverMessage = "";
            _botIsThinking = false;
            
            // Reset clock
            _clock.Reset(_config.GetTimeSpan());
            _clock.Start();
            
            // Reset network state if in network mode
            if (_config.NetworkRole != NetworkRole.None)
            {
                _isMyTurn = _config.NetworkRole == NetworkRole.Host;
                UpdateStatusLabel(_isMyTurn ? "Your turn" : "Opponent's turn");
            }
            
            // Clear UI selections and highlights
            _board.BoardHighlights.Clear();
            
            // Update status message
            UpdateStatusLabel("Game Reset");
        }

        private void HandlePromotionSelection()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                Point2D clickPoint = new Point2D() { X = SplashKit.MouseX(), Y = SplashKit.MouseY() };

                foreach (KeyValuePair<PieceType, Rectangle> button in _promotionButtons)
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

        private void UpdateStatusLabel(string text)
        {
            _statusLabel.Text = text;
        }

        private void DrawSaveMenu()
        {
            // Draw semi-transparent overlay
            SplashKit.FillRectangle(SplashKit.RGBAColor(0, 0, 0, 128), 0, 0, SplashKit.ScreenWidth(), SplashKit.ScreenHeight());
            
            // Draw menu background
            SplashKit.FillRectangle(Color.White, 200, 200, 400, 200);
            SplashKit.DrawRectangle(Color.Black, 200, 200, 400, 200);
            
            // Draw title
            SplashKit.DrawText("Save Game", Color.Black, 350, 220);
            
            // Draw filename input box
            SplashKit.FillRectangle(Color.LightGray, 250, 270, 300, 30);
            SplashKit.DrawRectangle(Color.Black, 250, 270, 300, 30);
        }

        private void DrawLoadMenu()
        {
            // Draw semi-transparent overlay
            SplashKit.FillRectangle(SplashKit.RGBAColor(0, 0, 0, 128), 0, 0, SplashKit.ScreenWidth(), SplashKit.ScreenHeight());
            
            // Draw menu background
            SplashKit.FillRectangle(Color.White, 200, 50, 400, 400);
            SplashKit.DrawRectangle(Color.Black, 200, 50, 400, 400);
            
            // Draw title
            SplashKit.DrawText("Load Game", Color.Black, 350, 70);
        }

        public void HandleAutoSave()
        {
            if (!_gameOver)
            {
                GameSaver.AutoSaveGame(_config, _clock, _board.GetFen());
            }
        }
    }
}
