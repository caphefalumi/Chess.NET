using SplashKitSDK;

namespace Chess
{
    public class GameplayScreen : ScreenState, IGameObserver
    {
        private readonly Game _game;
        private readonly Board _board;
        private readonly MatchConfiguration _config;
        private Button _menuButton;
        private Button _undoButton;
        private Button _resetButton;
        private Button _gameOverNewGameButton;
        private Clock _clock;
        private MatchState _gameState;
        private bool _gameOver;
        private string _gameOverMessage;
        private bool _promotionFlag;
        private bool _showPromotionMenu;
        private Move _promotionMove;
        private Dictionary<PieceType, Button> _promotionButtons;
        private TextLabel _statusLabel;
        private ChessBot _chessBot;
        private NetworkManager _networkManager;
        private bool _botIsThinking = false;
        private bool _isMyTurn = true;
        private GameEventManager _eventManager;

        // Properties to replace static access
        public bool PromotionFlag => _promotionFlag;

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
            _menuButton = new Button("Menu", 650, 10, 80, 30);
            _undoButton = new Button("Undo", 650, 50, 80, 30);
            _resetButton = new Button("Reset", 650, 90, 80, 30);
            _gameOverNewGameButton = new Button("New Game", 250, 470, 200, 50);
            
            // Setup status label
            _statusLabel = new TextLabel("", 610, 220);

            _gameOver = false;
            _gameOverMessage = "";
            _promotionFlag = false;
            _showPromotionMenu = false;

            // Initialize AI if in computer mode
            if (config.Mode == Variant.Computer)
            {
                // Initialize the AI to play as the opposite color of the player's choice
                Player computerPlayer = config.PlayerColor.Opponent();
                _chessBot = ChessBot.GetInstance(_board, computerPlayer);
                
                // If player chose Black and it's White's turn, make the computer move immediately
                if (config.PlayerColor == Player.Black && _gameState.CurrentPlayer == Player.White)
                {
                    _botIsThinking = true;
                    UpdateStatusLabel("Computer is thinking...");
                    
                    // Make the computer move
                    Task.Run(async () =>
                    {
                        try
                        {
                            // Wait a moment for UI to be fully set up
                            await Task.Delay(500);
                            
                            // Get computer move
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

            // Initialize network manager if in network mode
            if (config.NetworkRole != NetworkRole.None)
            {
                _networkManager = NetworkManager.GetInstance();
                // Set initial turn based on network role
                _isMyTurn = config.NetworkRole == NetworkRole.Host;
                UpdateStatusLabel(_isMyTurn ? "Your turn" : "Opponent's turn");

                // Subscribe to network events
                _networkManager.OnMoveReceived += HandleNetworkMove;
                _networkManager.OnConnectionStatusChanged += HandleConnectionStatus;
            }

            // Initialize BoardEvent
            BoardEvent.Initialize(_board);
            BoardEvent.SetGameplayScreen(this);  // Register this instance with BoardEvent

            // Initialize and register with the event manager
            _eventManager = GameEventManager.GetInstance();
            _eventManager.RegisterObserver(this);
            
            // No need to create additional observers here

            _clock.Start();
            _clock.OnTimeExpired += message => DeclareGameOver(message);
        }

        // IGameObserver implementation
        public void OnMoveMade(Move move)
        {
            // Update UI when a move is made
            UpdateStatusLabel($"{move.MovedPiece.Color.Opponent()}'s turn");
            
            // If we're playing against a computer and it's computer's turn, show "Computer is thinking..."
            if (_config.Mode == Variant.Computer && _gameState.CurrentPlayer == Player.Black)
            {
                UpdateStatusLabel("Computer is thinking...");
            }
            
            HandleLocalMove(move);
            
            // Switch the clock
            SwitchTurn();
        }

        public void OnGameOver(GameResult result, string message)
        {
            // Update game state and UI
            UpdateStatusLabel(message);
            DeclareGameOver(message);
        }

        public void OnCheck(Player playerInCheck)
        {
            // Update UI with check state
            UpdateStatusLabel($"{playerInCheck} is in check!");
        }

        public void OnTurnChanged(Player newPlayer)
        {
            // Update the clock to match the current player
            _clock.CurrentTurn = newPlayer;
        }

        private void HandleNetworkMove(string moveData)
        {
            Console.WriteLine($"[GameplayScreen] Received network move: {moveData}");
            // Parse the move data and apply it to the board
            Move move = Move.ConvertNotation(moveData, _board, true);
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
                _game.ChangeState(new MainMenuScreen(_game, _board));
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
            if (_showPromotionMenu)
            {
                foreach (Button button in _promotionButtons.Values)
                {
                    button.Update();
                }
            }
            _menuButton.Update();
            _undoButton.Update();
            _resetButton.Update();
            
            // Update clock
            _clock.CurrentTurn = _gameState.CurrentPlayer;
            _clock.Update();

            // Computer Move Logic
            if (_config.Mode == Variant.Computer && 
                _gameState.CurrentPlayer != _config.PlayerColor && 
                !_botIsThinking)
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


        // Method to switch turns
        public void SwitchTurn()
        {
            _clock.SwitchTurn();
        }

        // Method to handle game over
        public void DeclareGameOver(string msg)
        {
            _gameOver = true;
            _gameOverMessage = msg;
        }

        // Method to show promotion menu
        public void ShowPromotionMenu(Move move, Player color)
        {
            _showPromotionMenu = true;
            _promotionMove = move;
            _promotionFlag = true;
            
            // Use the provided color (or default to current player if not specified)
            Player pieceColor = color;
            
            
            // Center position of the board
            int menuX = 160; // Center horizontally (640/2 - 320/2)
            int menuY = 280; // Center vertically (640/2 - 80/2)
            
            // Create buttons using the piece bitmaps
            _promotionButtons = new Dictionary<PieceType, Button>
            {
                { PieceType.Queen, new Button(Piece.GetPieceBitmap(PieceType.Queen, pieceColor), menuX, menuY, 80, 80) },
                { PieceType.Rook, new Button(Piece.GetPieceBitmap(PieceType.Rook, pieceColor), menuX + 80, menuY, 80, 80) },
                { PieceType.Bishop, new Button(Piece.GetPieceBitmap(PieceType.Bishop, pieceColor), menuX + 160, menuY, 80, 80) },
                { PieceType.Knight, new Button(Piece.GetPieceBitmap(PieceType.Knight, pieceColor), menuX + 240, menuY, 80, 80) }
            };
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);
            _board.Draw();

            if (_showPromotionMenu)
            {
                foreach (Button button in _promotionButtons.Values)
                {
                    button.Draw();
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
                Rectangle overlay = new Rectangle(Color.RGBAColor(0, 0, 0, 128), 0, 0, SplashKit.ScreenWidth(), SplashKit.ScreenHeight());
                overlay.Draw();
                
                // Draw game over text
                SplashKit.DrawText(_gameOverMessage, Color.White, Font.Arial, 36, SplashKit.ScreenWidth() / 2 - 150, SplashKit.ScreenHeight() / 2 - 50);
                
                _gameOverNewGameButton.Draw();
            }

            // Draw time display
            DrawTimeDisplay(Player.White, _clock.WhiteTime.ToString(@"mm\:ss"), 650, 250);
            DrawTimeDisplay(Player.Black, _clock.BlackTime.ToString(@"mm\:ss"), 650, 300);

            // Draw current mode text
            string modeText = GetModeDisplayText();
            SplashKit.DrawText(modeText, Color.Black, Font.Arial, 18, 650, 350);

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
            SplashKit.DrawText(playerName, textColor, Font.Arial, 14, x, y);
            
        }

        private void DrawTimeDisplay(Player player, string timeStr, int x, int y)
        {
            string playerText = player == Player.White ? "White: " : "Black: ";
            Color textColor = player == _gameState.CurrentPlayer ? Color.Blue : Color.Black;
            SplashKit.DrawText(playerText + timeStr, textColor, Font.Arial, 20, x, y);
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
            
            UpdateStatusLabel("");
        }

        private void HandlePromotionSelection()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                Point2D clickPoint = new Point2D() { X = SplashKit.MouseX(), Y = SplashKit.MouseY() };
                
                foreach (KeyValuePair<PieceType, Button> button in _promotionButtons)
                {
                    // Update the button first to check hover state
                    button.Value.Update();
                    
                    // Check if the button was clicked
                    if (button.Value.IsAt(clickPoint) && SplashKit.MouseClicked(MouseButton.LeftButton))
                    {
                        // Create and execute the promotion move
                        Move promotionMove = new PromotionMove(_promotionMove.From, _promotionMove.To, _promotionMove.MovedPiece, button.Key);
                        BoardEvent.HandleMove(promotionMove);
                        _showPromotionMenu = false;
                        _promotionFlag = false;
                        break;
                    }
                }
            }
        }

        private void UpdateStatusLabel(string text)
        {
            _statusLabel.Text = text;
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
