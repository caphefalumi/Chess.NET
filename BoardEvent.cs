using SplashKitSDK;
using System;

namespace Chess
{
    public static class BoardEvent
    {
        private static readonly Dictionary<Position, Move> _moveBuffer = new Dictionary<Position, Move>();
        private static Piece _selectedPiece;
        private static Board _board;
        private static NetworkManager _networkManager;
        private static bool _isWaitingForOpponent;
        // private static Square _selectedSquare;
        private static bool _isDragging;
        private static Point2D _dragOffset;
        private static Piece _draggedPiece;

        public static event Action<Move> OnMoveMade;

        public static void Initialize(Board board)
        {
            _board = board;
            _networkManager = new NetworkManager();
            _isWaitingForOpponent = false;
            // _selectedSquare = null;
            _isDragging = false;
            _draggedPiece = null;
        }

        private static Position GetClickedSquare()
        {
            int file = (int)SplashKit.MouseX() / 80;
            int rank = (int)SplashKit.MouseY() / 80;
            return new Position(file, rank);
        }

        private static void SelectPiece(Position pos)
        {
            _selectedPiece = _board.GetPieceAt(pos);

            if (_selectedPiece != null)
            {
                IEnumerable<Move> legalMoves = _selectedPiece.GetLegalMoves();
                BufferMoves(legalMoves);
                HighlightSelectedPiece();
                HighlightLegalMoves();
            }
        }

        public static void MakeMove(Position pos)
        {
            _board.BoardHighlights.Clear();
            _board.BackgroundOverlays[0] = null;
            _board.BackgroundOverlays[1] = null;
            _board.CurrentSound = null;

            if (_moveBuffer.TryGetValue(pos, out Move move))
            {
                HighlightPreviousMove(move.From, move.To);

                if (move.Type == MoveType.Promotion)
                {
                    HandlePromotion(move);
                }
                else
                {
                    HandleMove(move);
                    GameplayScreen.SwitchTurn();
                    CheckGameResult();
                    _board.CurrentSound.Play();

                    // Send FEN to opponent in network mode after a move is made
                    if (_networkManager != null && _networkManager.IsConnected)
                    {
                        _networkManager.SendMove(move.ToString());
                        _isWaitingForOpponent = true;
                    }
                }
            }
            else if (_board.GetPieceAt(pos)?.Color == _board.MatchState.CurrentPlayer)
            {
                SelectPiece(pos);
            }
            else
            {
                _board.BackgroundOverlays[2] = null;
                _board.CurrentSound = Sounds.Illegal;
                _board.CurrentSound.Play();
            }
            _selectedPiece = null;
        }

        public static void CheckGameResult()
        {
            Player currentPlayer = _board.MatchState.CurrentPlayer;
            int availableMoves = _board.GetAllyMoves(currentPlayer).Count;

            if (_board.IsInCheck(currentPlayer))
            {
                if (availableMoves == 0)
                {
                    SetGameResult(GameResult.Win, $"{currentPlayer.Opponent()} wins by checkmate!");
                }
                else
                {
                    _board.CurrentSound = Sounds.MoveCheck;
                }
            }
            else if (availableMoves == 0)
            {
                SetGameResult(GameResult.Draw, "Game is a draw by stalemate!");
            }
            else if (Is50MoveRule())
            {
                SetGameResult(GameResult.Draw, "Game is a draw by repetition!");
            }
            else if (IsThreefoldRepetition())
            {
                SetGameResult(GameResult.Draw, "Game is a draw by threefold repetition!");
            }
            else if (IsInsufficientMaterial())
            {
                SetGameResult(GameResult.Draw, "Game is a draw due to insufficient material!");
            }
        }

        private static void SetGameResult(GameResult result, string message)
        {
            GameplayScreen.DeclareGameOver(message);
            _board.CurrentSound = Sounds.GameEnd;
        }

        public static bool IsThreefoldRepetition()
        {
            if (_board.MatchState.MoveHistory.Count < 6) return false;
            string currentFen = _board.MatchState.MoveHistory.Peek().Value;
            int count = 1;

            foreach (KeyValuePair<Move, string> entry in _board.MatchState.MoveHistory.Reverse().Skip(1))
            {
                if (entry.Value == currentFen)
                {
                    count++;
                    if (count >= 3) return true;
                }
            }
            return false;
        }


        private static bool Is50MoveRule()
        {
            return _board.MatchState.HalfmoveClock == 100;
        }
        private static bool IsInsufficientMaterial()
        {
            HashSet<Piece> pieces = _board.Pieces;
            int whitePieceCount = pieces.Count(p => p.Color == Player.White);
            int blackPieceCount = pieces.Count(p => p.Color == Player.Black);

            // Both sides only have a king
            if (whitePieceCount == 1 && blackPieceCount == 1) return true;

            // King + Knight vs. King
            if (pieces.Count(p => char.ToLower(p.PieceChar) == 'n') == 1 && whitePieceCount + blackPieceCount == 2) return true;

            // King + Bishop vs. King
            if (pieces.Count(p => char.ToLower(p.PieceChar) == 'b') == 1 && whitePieceCount + blackPieceCount == 2) return true;



            return false;
        }

        public static void HandleMove(Move move)
        {
            if (_board.MatchState.MoveHistory.TryPeek(out KeyValuePair<Move, string> result))
            {
                if (result.Key.Type == MoveType.DoublePawn)
                {
                    Pawn pawn = (Pawn)_board.GetPieceAt(result.Key.To);
                    if (pawn != null)
                    {
                        pawn.CanBeEnpassant = false;
                    }
                }
            }
            _board.MatchState.MakeMove(move);
            OnMoveMade?.Invoke(move);
        }

        public static void HandleUndo()
        {
            _board.MatchState.UnmakeMove();
        }

        private static void HandlePromotion(Move move)
        {
            // Show the promotion menu at the promotion square
            GameplayScreen.ShowPromotionMenu(move, _board.MatchState.CurrentPlayer);
        }

        private static void BufferMoves(IEnumerable<Move> moves)
        {
            _moveBuffer.Clear();
            foreach (Move move in moves)
            {
                _moveBuffer[move.To] = move;
            }
        }

        private static void HighlightSelectedPiece()
        {
            _board.BackgroundOverlays[2] = new Rectangle(SplashKit.RGBAColor(203, 163, 84, 204), _selectedPiece.Position.X, _selectedPiece.Position.Y, _board.SquareSize);
        }

        private static void HighlightLegalMoves()
        {
            _board.BoardHighlights.Clear();
            foreach (Position to in _moveBuffer.Keys)
            {
                int centerX = to.File * 80 + 40;
                int centerY = to.Rank * 80 + 40;
                int radius = _board.IsEmpty(to) ? 9 : 15;
                _board.BoardHighlights.Add(new Circle(Color.SwinburneRed, centerX, centerY, radius));
            }
        }

        private static void HighlightPreviousMove(Position from, Position to)
        {
            Position[] positions = { from, to };

            for (int i = 0; i < positions.Length; i++)
            {
                bool isLightSquare = ((positions[i].Rank + positions[i].File) & 1) != 0;
                Color highlightColor = SplashKit.RGBAColor(203, 163, 84, 204);
                Rectangle rect = new Rectangle(highlightColor, positions[i].X, positions[i].Y, 80, 80);
                _board.BackgroundOverlays[i] = rect;
            }
        }

        public static void HandleMouseEvents(Board board, MatchState gameState)
        {
            _board = board;
            _board.MatchState = gameState;
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                Position pos = GetClickedSquare();

                // If promotion menu is active, don't handle board clicks
                if (GameplayScreen.PromotionFlag)
                {
                    return;
                }

                // If waiting for opponent's move, don't handle clicks
                if (_isWaitingForOpponent)
                {
                    return;
                }

                Piece clickedPiece = _board.GetPieceAt(pos);
                
                // If clicking a piece of the current player's color
                if (clickedPiece?.Color == _board.MatchState.CurrentPlayer)
                {
                    // Clear previous selection and select new piece
                    _selectedPiece = null;
                    _moveBuffer.Clear();
                    _board.BoardHighlights.Clear();
                    _board.BackgroundOverlays[2] = null;
                    SelectPiece(pos);
                }
                // If a piece is selected and clicking a valid move position
                else if (_selectedPiece != null && _moveBuffer.ContainsKey(pos))
                {
                    MakeMove(pos);
                }
                // If clicking an empty square or opponent's piece with no piece selected
                else
                {
                    _selectedPiece = null;
                    _moveBuffer.Clear();
                    _board.BoardHighlights.Clear();
                    _board.BackgroundOverlays[2] = null;
                }
            }
        }
    }
}
