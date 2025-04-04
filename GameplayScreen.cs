using SplashKitSDK;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess
{
    // Simple TextLabel class for displaying status messages
    public class TextLabel
    {
        public string Text { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        public TextLabel(string text, int x, int y, int width, int height)
        {
            Text = text;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        
        public void Draw()
        {
            SplashKit.DrawText(Text, Color.DarkBlue, X, Y);
        }
    }
    
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
        private Stopwatch _botThinkTimer;
        private bool _botIsThinking = false;
        public static bool PromotionFlag;
        private static bool _showPromotionMenu;
        private static Move _promotionMove;
        private static Player _promotionColor;
        private static Dictionary<PieceType, Bitmap> _promotionPieces;
        private static Dictionary<PieceType, Rectangle> _promotionButtons;
        private TextLabel _statusLabel;
        private ChessBot _chessBot;
        private NetworkChessManager _networkManager;

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
            
            // Setup status label
            _statusLabel = new TextLabel("", 610, 220, 120, 60);

            _gameOver = false;
            _gameOverMessage = "";

            // Initialize AI if in computer mode
            if (config.Mode == Variant.Computer)
            {
                _botThinkTimer = new Stopwatch();
                _chessBot = new ChessBot(_board);
            }

            // Initialize network manager if in network mode
            if (config.Mode == Variant.Network)
            {
                _networkManager = NetworkChessManager.GetInstance();
                _networkManager.Initialize(_game, _board, _config, OnFenReceived);
                if (_networkManager.IsServer)
                {
                    _networkManager.StartServer();
                }
                else
                {
                    string serverIP = _networkManager.DiscoverServerIP();
                    if (serverIP != null)
                    {
                        _networkManager.StartClient(serverIP);
                    }
                    else
                    {
                        UpdateStatusLabel("No server found");
                    }
                }
            }

            // Initialize BoardEvent with network manager
            BoardEvent.Initialize(_board);

            _clock.Start();
            _clock.OnTimeExpired += DeclareGameOver;
        }

        private void OnFenReceived(string fen)
        {
            _board.UpdateFromFen(fen);
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
                BoardEvent.HandleUndo();
                _clock.CurrentTurn = _gameState.CurrentPlayer;
                if (_config.Mode == Variant.Computer && _gameState.CurrentPlayer == Player.Black)
                {
                    _botIsThinking = false;
                }
                return;
            }

            if (_resetButton.IsClicked() || SplashKit.KeyTyped(KeyCode.RKey))
            {
                ResetBoard();
                return;
            }

            // Handle human input - only if it's not waiting for opponent or computer's turn
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

            // Network mode updates
            if (_config.Mode == Variant.Network && _networkManager != null)
            {
                if (_networkManager.IsConnected)
                {
                    UpdateStatusLabel("Connected");
                }
                else
                {
                    UpdateStatusLabel("Disconnected");
                }
            }

            // Computer Move Logic
            if (_config.Mode == Variant.Computer && _gameState.CurrentPlayer == Player.Black && !_botIsThinking)
            {
                _botIsThinking = true;
                _botThinkTimer.Restart();
                
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
                        else
                        {
                            UpdateStatusLabel("Failed to get move from engine.");
                        }
                    }
                    catch (Exception ex)
                    {
                        UpdateStatusLabel($"Error: {ex.Message}");
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
            DrawTimeDisplay(Player.White, _clock.WhiteTime.ToString(@"mm\:ss"), 610, 250);
            DrawTimeDisplay(Player.Black, _clock.BlackTime.ToString(@"mm\:ss"), 610, 300);

            // Draw current mode text
            string modeText = GetModeDisplayText();
            SplashKit.DrawText(modeText, Color.Black, 610, 350);

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
            _promotionColor = color;
            PromotionFlag = true;

            // Center position of the board
            int menuX = 160; // Center horizontally (640/2 - 320/2)
            int menuY = 280; // Center vertically (640/2 - 80/2)

            // Use existing piece bitmap images instead of creating new ones
            _promotionPieces = new Dictionary<PieceType, Bitmap>
            {
                { PieceType.Queen, GetPieceBitmap(PieceType.Queen, color) },
                { PieceType.Rook, GetPieceBitmap(PieceType.Rook, color) },
                { PieceType.Bishop, GetPieceBitmap(PieceType.Bishop, color) },
                { PieceType.Knight, GetPieceBitmap(PieceType.Knight, color) }
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

        // Helper method to get piece bitmap without creating new bitmap objects
        private static Bitmap GetPieceBitmap(PieceType pieceType, Player color)
        {
            // Get the piece character from the PieceFactory
            char pieceChar = PieceFactory.GetPieceChar(pieceType, color);
            
            // Get the bitmap filename directly without creating a temporary piece
            char pieceColor = (color == Player.White) ? 'w' : 'b';
            string bitmapName = pieceColor.ToString() + pieceChar.ToString();
            
            // Try to load the bitmap using SplashKit's bitmap management
            // This will reuse existing bitmaps rather than creating new ones
            return SplashKit.LoadBitmap(bitmapName, $"Resources\\Pieces\\{bitmapName}.png");
        }

        public override string GetStateName() => "GamePlay";

        private void ResetBoard()
        {
            // Reset the board using the OOP approach
            _board.ResetBoard();
            
            // Get a new game state
            _gameState = _game.GetGameState();
            _board.MatchState = _gameState;
            
            // Reset UI state
            _gameOver = false;
            _gameOverMessage = "";
            _botIsThinking = false;
            
            // Reset clock
            _clock.Reset(_config.GetTimeSpan());
            _clock.Start();
            
            // Send initial FEN in network mode
            if (_config.Mode == Variant.Network && _networkManager != null && _networkManager.IsConnected)
            {
                _networkManager.SendFEN(_board.GetFen());
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
    }
}
