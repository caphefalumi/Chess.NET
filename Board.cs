using SplashKitSDK;
using System.Numerics;
using System.Text;
using System.Linq;

namespace Chess
{
    public class Board : IDrawable
    {
        private HashSet<Piece> _pieces;
        private HashSet<Circle> _boardHighlights;
        private Rectangle[] _backgroundOverlays;
        private static Board _instance;
        private BoardRenderer _boardDrawer;
        private int _squareSize;
        private King _whiteKing;
        private King _blackKing;
        private HashSet<Position> _frozenSquares;
        private SpellManager _spellManager;
        
        public bool IsGameOver { get; set; }
        public Sound CurrentSound { get; set; }

        public HashSet<Piece> Pieces
        {
            get => _pieces;
            private set => _pieces = value;
        }
        public HashSet<Circle> BoardHighlights
        {
            get => _boardHighlights;
            set => _boardHighlights = value;
        }
        public Rectangle[] BackgroundOverlays
        {
            get => _backgroundOverlays;
            set => _backgroundOverlays = value;
        }
        public int SquareSize => _squareSize;
        public GameResult GameResult { get; set; }
        public MatchState MatchState { get; set; }

        private Board(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            _squareSize = squareSize;
            _pieces = PieceFactory.CreatePieces(this);
            _boardHighlights = new HashSet<Circle>();
            _backgroundOverlays = new Rectangle[3];
            _boardDrawer = BoardRenderer.GetInstance(squareSize, startX, startY, lightColor, darkColor);
            _whiteKing = FindKing(Player.White);
            _blackKing = FindKing(Player.Black);
            _frozenSquares = new HashSet<Position>();
            _spellManager = new SpellManager();
            GameResult = GameResult.Ongoing;
        }
        public static Board GetInstance(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            if (_instance is null)
            {
                _instance = new Board(squareSize, startX, startY, lightColor, darkColor);
            }
            return _instance;
        }

        public void Draw()
        {
            _boardDrawer.Draw();
            _boardDrawer.Draw(_backgroundOverlays);
            _boardDrawer.Draw(_pieces);
            _boardDrawer.Draw(_boardHighlights);
        }

        public Piece GetPieceAt(int rank, int file)
        {
            foreach (Piece piece in Pieces)
            {
                if (piece.Position.Rank == rank && piece.Position.File == file)
                {
                    return piece;
                }
            }
            return null;
        }
        public Piece GetPieceAt(Position pos)
        {
            foreach (Piece piece in Pieces)
            {
                if (piece.Position.Equals(pos))
                {
                    return piece;
                }
            }
            return null;
        }
        public static bool IsInside(Position pos)
        {
            return pos.File >= 0 && pos.File < 8 && pos.Rank >= 0 && pos.Rank < 8;
        }

        public bool IsEmpty(Position pos)
        {
            return GetPieceAt(pos) is null;
        }

        private King FindKing(Player player)
        {
            return (King)_pieces.FirstOrDefault(piece => piece is King && piece.Color == player);
        }

        public bool IsInCheck(Player player)
        {
            King king = player == Player.White ? _whiteKing : _blackKing;
            return _pieces
                .Where(piece => piece.Color == player.Opponent())
                .SelectMany(piece => piece.GetAttackedSquares())
                .Any(move => move.To == king.Position);
        }

        public void InitializeSpells(Player player)
        {
            _spellManager.InitializeSpells(player);
        }

        public bool HasUnusedSpell(Player player, SpellType type)
        {
            return _spellManager.HasUnusedSpell(player, type);
        }

        public void UseSpell(Player player, SpellType type)
        {
            _spellManager.UseSpell(player, type);
        }

        public void ApplyFreezeSpell(Position center)
        {
            for (int rank = -1; rank <= 1; rank++)
            {
                for (int file = -1; file <= 1; file++)
                {
                    Position pos = new Position(center.File + file, center.Rank + rank);
                    if (IsInside(pos))
                    {
                        _frozenSquares.Add(pos);
                    }
                }
            }
        }

        public bool IsSquareFrozen(Position pos)
        {
            return _frozenSquares.Contains(pos);
        }

        public void ClearFrozenSquares()
        {
            _frozenSquares.Clear();
        }

        public bool CanTeleport(Piece piece, Position target)
        {
            if (!IsInside(target)) return false;
            
            // Check if there's a friendly piece between the current position and target
            Position currentPos = piece.Position;
            int dx = target.File - currentPos.File;
            int dy = target.Rank - currentPos.Rank;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
            
            for (int i = 1; i < steps; i++)
            {
                Position checkPos = new Position(
                    currentPos.File + (dx * i) / steps,
                    currentPos.Rank + (dy * i) / steps
                );
                
                Piece pieceAtPos = GetPieceAt(checkPos);
                if (pieceAtPos != null && pieceAtPos.Color == piece.Color)
                {
                    return true; // Found a friendly piece to teleport past
                }
            }
            
            return false;
        }

        public HashSet<Move> GetAllyMoves(Player player)
        {
            HashSet<Piece> piecesCopy = _pieces.ToHashSet();
            HashSet<Move> moves = piecesCopy
                .Where(piece => piece.Color == player)
                .SelectMany(piece => piece.GetLegalMoves())
                .ToHashSet();

            // Filter out moves to frozen squares
            moves.RemoveWhere(move => IsSquareFrozen(move.To));

            return moves;
        }

        public string GetFen()
        {
            StringBuilder fen = new StringBuilder();

            // 1. Board Representation (ranks 8 to 1, files a to h)
            for (int rank = 0; rank < 8; rank++)  // Start from rank 0 (which corresponds to the bottom in FEN)
            {
                int emptyCount = 0;
                for (int file = 0; file < 8; file++)
                {
                    Position pos = new Position(file, rank);  // Use the rank as it is, no reverse needed here
                    Piece piece = GetPieceAt(pos);

                    if (piece == null)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            fen.Append(emptyCount);
                            emptyCount = 0;
                        }
                        fen.Append(piece.PieceChar);
                    }
                }
                if (emptyCount > 0) fen.Append(emptyCount);
                if (rank < 7) fen.Append('/');  // Append slash except for the last rank
            }

            // 2. Active Color
            fen.Append(' ').Append(MatchState.CurrentPlayer == Player.White ? "w" : "b");

            // 3. Castling Availability
            StringBuilder castling = new StringBuilder();
            if (MatchState.CanWhiteCastleKingside) castling.Append('K');
            if (MatchState.CanWhiteCastleQueenside) castling.Append('Q');
            if (MatchState.CanBlackCastleKingside) castling.Append('k');
            if (MatchState.CanBlackCastleQueenside) castling.Append('q');
            fen.Append(' ').Append(castling.Length > 0 ? castling.ToString() : "-");

            // 4. En Passant Target Square
            fen.Append(' ');
            fen.Append('-');

            // 5. Halfmove Clock (moves since last capture or pawn advance)
            fen.Append(' ').Append(MatchState.HalfmoveClock);

            // 6. Fullmove Number
            fen.Append(' ').Append(MatchState.FullmoveNumber);

            return fen.ToString();
        }



        public Position GetPositionFromPoint(Point2D point)
        {
            // Convert screen coordinates to board coordinates
            int file = (int)((point.X - _boardDrawer.StartX) / _squareSize);
            int rank = (int)((point.Y - _boardDrawer.StartY) / _squareSize);
            return new Position(file, rank);
        }

        public Piece GetSelectedPiece()
        {
            return _pieces.FirstOrDefault(piece => piece.IsSelected);
        }

        public int GetSpellCount(Player player, SpellType type)
        {
            return _spellManager.GetSpellCount(player, type);
        }

        public void ResetBoard()
        {
            _pieces = PieceFactory.CreatePieces(this);
            _boardHighlights.Clear();
            _frozenSquares.Clear();
            _backgroundOverlays = new Rectangle[3];
        }
    }
}