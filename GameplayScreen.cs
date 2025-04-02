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
        private Button _teleportSpellButton;
        private Button _freezeSpellButton;
        private static Clock _clock;
        private MatchState _gameState;
        private static bool _gameOver;
        private static string _gameOverMessage;
        private Stopwatch _botThinkTimer; // Timer to control bot thinking
        private bool _botIsThinking = false; // Flag to indicate bot is "thinking"
        private bool _isSpellMode = false;
        private SpellType _selectedSpell;
        public static bool PromotionFlag;
        private static bool _showPromotionMenu;
        private static Move _promotionMove;
        private static Player _promotionColor;
        private static Dictionary<PieceType, Bitmap> _promotionPieces;
        private static Dictionary<PieceType, Rectangle> _promotionButtons;  // Add this for click detection

        // Spell UI Resources
        private Bitmap _teleportBottleImage;
        private Bitmap _freezeBottleImage;

        private TextLabel _statusLabel; // Using our custom TextLabel class

        // Add a field for ComputerStrategyManager at the top of the class
        private ComputerStrategyManager _computerStrategy;

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
            
            // Setup spell buttons
            _teleportSpellButton = new Button("Teleport", 610, 130, 80, 30);
            _freezeSpellButton = new Button("Freeze", 610, 170, 80, 30);
            
            // Setup status label - using custom TextLabel class
            _statusLabel = new TextLabel("", 610, 220, 120, 60);

            _gameOver = false;
            _gameOverMessage = "";

            // Load spell bottle images
            _teleportBottleImage = SplashKit.LoadBitmap("TeleportBottle", "Resources/Spell/Teleport_bottle.png");
            _freezeBottleImage = SplashKit.LoadBitmap("FreezeBottle", "Resources/Spell/Freeze_bottle.png");
            
            // Initialize AI if in computer mode
            if (config.Mode == Variant.Computer)
            {
                _botThinkTimer = new Stopwatch();
                
                // Initialize the ComputerStrategyManager
                _computerStrategy = new ComputerStrategyManager();
            }

            // Initialize spells for both players
            _board.InitializeSpells(Player.White);
            _board.InitializeSpells(Player.Black);

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

            // Handle spell buttons in Spell Chess mode
            if (_config.Mode == Variant.SpellChess)
            {
                if (_teleportSpellButton.IsClicked())
                {
                    if (_board.HasUnusedSpell(_gameState.CurrentPlayer, SpellType.Teleport))
                    {
                        _isSpellMode = true;
                        _selectedSpell = SpellType.Teleport;
                    }
                }
                else if (_freezeSpellButton.IsClicked())
                {
                    if (_board.HasUnusedSpell(_gameState.CurrentPlayer, SpellType.Freeze))
                    {
                        _isSpellMode = true;
                        _selectedSpell = SpellType.Freeze;
                    }
                }
            }

            // Handle human input - only if it's not the computer's turn or not in computer mode
            if (!_botIsThinking && (_config.Mode != Variant.Computer || _gameState.CurrentPlayer == Player.White))
            {
                if (_isSpellMode)
                {
                    HandleSpellInput();
                }
                else
                {
                    BoardEvent.HandleMouseEvents(_board, _gameState);
                }
            }
        }

        private void HandleSpellInput()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                Point2D clickPoint = new Point2D() { X = SplashKit.MouseX(), Y = SplashKit.MouseY() };
                Position targetPos = _board.GetPositionFromPoint(clickPoint);

                if (targetPos != null)
                {
                    if (_selectedSpell == SpellType.Teleport)
                    {
                        Piece selectedPiece = _board.GetSelectedPiece();
                        if (selectedPiece != null && _board.CanTeleport(selectedPiece, targetPos))
                        {
                            _board.UseSpell(_gameState.CurrentPlayer, SpellType.Teleport);
                            
                            NormalMove move = new NormalMove(selectedPiece.Position, targetPos, selectedPiece);
                            BoardEvent.HandleMove(move);
                            _isSpellMode = false;
                            
                            // Switch turns
                            _clock.SwitchTurn();
                        }
                    }
                    else if (_selectedSpell == SpellType.Freeze)
                    {
                        _board.UseSpell(_gameState.CurrentPlayer, SpellType.Freeze);
                        
                        
                        _board.ApplyFreezeSpell(targetPos);
                        _isSpellMode = false;
                        
                        // Switch turns
                        _clock.SwitchTurn();
                    }
                }
            }
            
            // Allow canceling spell mode with right-click
            if (SplashKit.MouseClicked(MouseButton.RightButton))
            {
                _isSpellMode = false;
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
            
            if (_config.Mode == Variant.SpellChess)
            {
                _teleportSpellButton.Update();
                _freezeSpellButton.Update();
            }

            // Update clock
            _clock.CurrentTurn = _gameState.CurrentPlayer;
            _clock.Update();

            // Computer Move Logic
            if (_config.Mode == Variant.Computer && _gameState.CurrentPlayer == Player.Black && !_botIsThinking)
            {
                _botIsThinking = true;
                _botThinkTimer.Restart();

                // Call the synchronous GetBestMove method
                Dictionary<string, object> result = _computerStrategy.GetBestMove(_board.GetFen());
                if (result.ContainsKey("error"))
                {
                    UpdateStatusLabel($"Error: {result["error"]}");
                }
                else
                {
                    string bestMove = result["move"].ToString();
                    if (!string.IsNullOrEmpty(bestMove))
                    {
                        // Convert the string move to a Move object
                        Move move = ConvertStringToMove(bestMove, result["flags"].ToString());
                        if (move != null)
                        {
                            _gameState.MakeMove(move);
                            _clock.SwitchTurn();
                            BoardEvent.CheckGameResult();
                            _botIsThinking = false; // Stop further requests
                        }
                        else
                        {
                            UpdateStatusLabel("Invalid move received from API.");
                        }
                    }
                    else
                    {
                        UpdateStatusLabel("Failed to get move from API.");
                    }
                }
            }

            // Special logic for Spell Chess mode
            if (_config.Mode == Variant.SpellChess)
            {
                HandleSpellChessLogic();
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
            
            // Draw spell buttons if in spell chess mode
            if (_config.Mode == Variant.SpellChess)
            {
                _teleportSpellButton.Draw();
                _freezeSpellButton.Draw();

                // Visual feedback for selected spell in spell mode
                if (_isSpellMode)
                {
                    Button selectedButton = _selectedSpell == SpellType.Teleport ? 
                        _teleportSpellButton : _freezeSpellButton;
                    SplashKit.DrawRectangle(Color.Yellow, selectedButton.X - 2, selectedButton.Y - 2, 
                        selectedButton.Width + 4, selectedButton.Height + 4);
                }
            }

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
            
            if (_config.Mode == Variant.SpellChess)
            {
                // Draw teleport bottle and count
                int teleportCount = _board.GetSpellCount(player, SpellType.Teleport);
                SplashKit.DrawText($"x{teleportCount}", Color.Black, x + 110, y + 5);
                SplashKit.DrawBitmap(_teleportBottleImage, x + 80, y, SplashKit.OptionScaleBmp(0.5, 0.5));
                
                // Draw freeze bottle and count
                int freezeCount = _board.GetSpellCount(player, SpellType.Freeze);
                SplashKit.DrawText($"x{freezeCount}", Color.Black, x + 170, y + 5);
                SplashKit.DrawBitmap(_freezeBottleImage, x + 140, y, SplashKit.OptionScaleBmp(0.5, 0.5));
                
                // Highlight current player
                if (_gameState.CurrentPlayer == player && !_gameOver)
                {
                    SplashKit.DrawRectangle(Color.YellowGreen, x - 5, y - 5, 90, 30);
                }
            }
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
                Variant.SpellChess => "Spell Chess Mode",
                Variant.Custom => "Custom Mode",
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

        // Add a stub implementation for the ResetBoard method
        private void ResetBoard()
        {
            _board.ResetBoard();

            _gameOver = false;
            _gameOverMessage = "";
            _botIsThinking = false;
            _isSpellMode = false;
            _clock.Reset(_config.GetTimeSpan());
            _clock.Start();

            // Reinitialize spells
            _board.InitializeSpells(Player.White);
            _board.InitializeSpells(Player.Black);
        }

        // Add a stub implementation for the HandlePromotionSelection method
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

        // Add a stub implementation for the HandleSpellChessLogic method
        private void HandleSpellChessLogic()
        {
            // Implementation of spell chess logic
            // This might include updating spell effects, handling cooldowns, etc.
        }

        /// <summary>
        /// Updates the status label with the provided text
        /// </summary>
        private void UpdateStatusLabel(string text)
        {
            _statusLabel.Text = text;
        }

        /// <summary>
        /// Converts a string move (e.g., "e2e4") to a Move object.
        /// </summary>
        private Move ConvertStringToMove(string moveStr, string flags)
        {
            if (string.IsNullOrEmpty(moveStr) || moveStr.Length < 4)
            {
                return null;
            }

            // Parse the move string
            Position from = new Position(moveStr.Substring(0, 2)); // Assuming Position can be constructed from a string
            Position to = new Position(moveStr.Substring(2, 2));

            Piece movedPiece = _board.GetPieceAt(from);
            if (movedPiece == null)
            {
                return null;
            }

            // Determine the type of move based on flags
            if (flags.Contains("k") || flags.Contains("q"))
            {
                Console.WriteLine(flags.Contains("k") ? "Kingside castling" : "Queenside castling");
                return new CastleMove(flags.Contains("k") ? MoveType.CastleKS : MoveType.CastleQS, from, (King)movedPiece);
            }
            if (flags.Contains("e") && movedPiece is Pawn)
            {
                Console.WriteLine("En passant capture");
                return new EnPassantMove(from, to, (Pawn)movedPiece);
            }

            // Default to normal move
            return new NormalMove(from, to, movedPiece);
        }
    }
}
