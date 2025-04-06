using SplashKitSDK;

namespace Chess
{
    public class BoardSetupScreen : ScreenState
    {
        private Game _game;
        private Board _board;
        private Button _startButton;
        private Button _backButton;
        private Button _clearButton;
        private Button _timeSettingsButton;
        private PieceType _selectedPieceType = PieceType.Pawn;
        private Player _selectedPlayerColor = Player.White;
        private Button _whitePieceButton;
        private Button _blackPieceButton;
        private Dictionary<PieceType, Button> _pieceTypeButtons = new Dictionary<PieceType, Button>();
        private MatchConfiguration _config;  // Add configuration object

        public BoardSetupScreen(Game game, Board board)
        {
            _game = game;
            _board = board;

            // Initialize configuration with Custom game mode
            _config = new MatchConfiguration
            {
                Mode = Variant.Custom,
                TimeControl = TimeControl.TenMinutes  // Default 10 minutes
            };

            int centerX = SplashKit.ScreenWidth() / 2;
            _startButton = new Button("Start Game", centerX - 100, 600, 200, 50);
            _backButton = new Button("Back", 10, 10, 80, 30);
            _clearButton = new Button("Clear Board", 100, 10, 120, 30);
            _timeSettingsButton = new Button("Time Settings", 230, 10, 120, 30);

            _whitePieceButton = new Button("White", 650, 100, 80, 30);
            _blackPieceButton = new Button("Black", 650, 140, 80, 30);

            int y = 200;
            foreach (PieceType type in Enum.GetValues(typeof(PieceType)))
            {
                _pieceTypeButtons[type] = new Button(type.ToString(), 650, y, 80, 30);
                y += 40;
            }

            _board.Pieces.Clear(); // Start with an empty board
        }

        public override void HandleInput()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                int file = (int)SplashKit.MouseX() / _board.SquareSize;
                int rank = (int)SplashKit.MouseY() / _board.SquareSize;

                if (file >= 0 && file < 8 && rank >= 0 && rank < 8)
                {
                    Position pos = new Position(file, rank);
                    _board.Pieces.RemoveWhere(p => p.Position.Equals(pos));

                    char pieceChar = PieceFactory.GetPieceChar(_selectedPieceType, _selectedPlayerColor);
                    Piece newPiece = PieceFactory.CreatePiece(pieceChar, pos, _board);
                    if (newPiece != null)
                    {
                        _board.Pieces.Add(newPiece);
                    }
                }
            }

            if (_startButton.IsClicked())
            {
                // Use MatchConfiguration instead of Variant
                _game.ChangeState(new GameplayScreen(_game, _board, _config));
            }
            else if (_backButton.IsClicked())
            {
                _game.ChangeState(new VariantSelectionScreen(_game, _board));
            }
            else if (_clearButton.IsClicked())
            {
                _board.Pieces.Clear();
            }
            else if (_timeSettingsButton.IsClicked())
            {
                // Go to time selection screen, passing this state to return to later
                _game.ChangeState(new TimeSelectionScreen(_game, _board, Variant.Custom));
            }
            else if (_whitePieceButton.IsClicked())
            {
                _selectedPlayerColor = Player.White;
            }
            else if (_blackPieceButton.IsClicked())
            {
                _selectedPlayerColor = Player.Black;
            }

            foreach (KeyValuePair<PieceType, Button> entry in _pieceTypeButtons)
            {
                if (entry.Value.IsClicked())
                {
                    _selectedPieceType = entry.Key;
                    break;
                }
            }
        }

        public override void Update()
        {
            _startButton.Update();
            _backButton.Update();
            _clearButton.Update();
            _timeSettingsButton.Update();
            _whitePieceButton.Update();
            _blackPieceButton.Update();
            foreach (KeyValuePair<PieceType, Button> entry in _pieceTypeButtons) 
            {
                entry.Value.Update();
            }
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);

            SplashKit.DrawText("Custom Game Setup", Color.Black, Font.Get, 24, SplashKit.ScreenWidth() / 2 - 120, 20);

            _board.Draw();
            _startButton.Draw();
            _backButton.Draw();
            _clearButton.Draw();
            _timeSettingsButton.Draw();

            // Display the current time settings
            string timeText = GetTimeControlText();
            SplashKit.DrawText($"Time: {timeText}", Color.Black, Font.Get, 14, 360, 15);

            SplashKit.DrawText("Piece Color:", Color.Black, Font.Get, 16, 650, 70);
            _whitePieceButton.Draw();
            _blackPieceButton.Draw();
            HighlightButton(_selectedPlayerColor == Player.White ? _whitePieceButton : _blackPieceButton);

            SplashKit.DrawText("Piece Type:", Color.Black, Font.Get, 16, 650, 180);
            foreach (KeyValuePair<PieceType, Button> entry in _pieceTypeButtons)
            {
                entry.Value.Draw();
                if (entry.Key == _selectedPieceType) HighlightButton(entry.Value);
            }

            SplashKit.DrawText("Click on board to place pieces", Color.Black, Font.Get, 16, 10, 650);
            SplashKit.RefreshScreen();
        }

        private string GetTimeControlText()
        {
            string timeText = _config.TimeControl switch
            {
                TimeControl.Bullet1 => "1 min",
                TimeControl.Bullet3 => "3 min",
                TimeControl.Blitz5 => "5 min",
                TimeControl.TenMinutes => "10 min",
                TimeControl.FifteenMinutes => "15 min",
                TimeControl.ThirtyMinutes => "30 min",
                TimeControl.Unlimited => "∞",
                _ => "10 min"
            };

            if (_config.UseIncrement)
            {
                timeText += $" + {_config.IncrementSeconds}s";
            }

            return timeText;
        }

        private void HighlightButton(Button button)
        {
            Rectangle highlightRect = new Rectangle(Color.Green, button.X - 5, button.Y - 5, button.Width + 10, button.Height + 10);
            highlightRect.Draw();
        }

        // Method to receive configuration updates from other states
        public void SetConfiguration(MatchConfiguration config)
        {
            _config = config;
            _config.Mode = Variant.Custom; // Ensure we're in custom mode
        }

        public override string GetStateName() => "GameSetup";
    }
}
