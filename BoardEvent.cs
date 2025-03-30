using SplashKitSDK;

namespace Chess
{
    public static class BoardEvent
    {
        private static readonly Dictionary<Position, Move> _moveBuffer = new Dictionary<Position, Move>();
        private static Piece _selectedPiece;
        private static Board _board;
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
                    HandlePromotion(move.From, move.To);
                }
                else
                {
                    HandleMove(move);
                }
                GameplayScreen.SwitchTurn();
                CheckGameResult();
                _board.CurrentSound.Play();
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

        private static void CheckGameResult()
        {
            Player currentPlayer = _board.MatchState.CurrentPlayer;
            int availableMoves = _board.GetAllyMoves(currentPlayer).Count;

            if (_board.IsInCheck(currentPlayer))
            {
                if (availableMoves == 0)
                {
                    SetGameResult(GameResult.Checkmate, $"{currentPlayer.Opponent()} wins by checkmate!");
                }
                else
                {
                    _board.CurrentSound = Sounds.MoveCheck;
                }
            }
            else if (availableMoves == 0)
            {
                SetGameResult(GameResult.Stalemate, "Game is a draw by stalemate!");
            }
            else if (IsThreefoldRepetition())
            {
                SetGameResult(GameResult.ThreefoldRepetition, "Game is a draw by threefold repetition!");
            }
            else if (IsInsufficientMaterial())
            {
                SetGameResult(GameResult.InsufficientMaterial, "Game is a draw due to insufficient material!");
            }
        }

        private static void SetGameResult(GameResult result, string message)
        {
            GameplayScreen.DeclareGameOver(message);
            _board.CurrentSound = Sounds.GameEnd;
        }

        private static bool IsThreefoldRepetition()
        {
            List<Move> history = _board.MatchState.MoveHistory.ToList();
            return false;
            
        }

        private static bool IsInsufficientMaterial()
        {
            HashSet<Piece> pieces = _board.Pieces.ToHashSet();
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
            if (_board.MatchState.MoveHistory.TryPeek(out Move result))
            {
                if (result.Type == MoveType.DoublePawn)
                {
                    Pawn pawn = (Pawn)_board.GetPieceAt(result.To);
                    if (pawn != null)
                        pawn.CanBeEnpassant = false;
                }
            }
            _board.MatchState.MakeMove(move);
        }

        public static void HandleUndo()
        {
            _board.MatchState.UnmakeMove();
        }

        private static void HandlePromotion(Position from, Position to)
        {
            Move proMove = new PromotionMove(from, to, PieceType.Queen);
            HandleMove(proMove);
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

                if (_selectedPiece == null)
                {
                    SelectPiece(pos);
                }
                else
                {
                    MakeMove(pos);
                }
            }
        }
    }
}
