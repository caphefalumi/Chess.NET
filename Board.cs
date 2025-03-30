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
        
        public bool IsGameOver { get; set; }
        public Sound CurrentSound { get; set; }

        public HashSet<Piece> Pieces
        {
            get => _pieces;
            set => _pieces = value;
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

        public HashSet<Move> GetAllyMoves(Player player)
        {
            var piecesCopy = _pieces.ToHashSet(); // Create a copy of the collection
            return piecesCopy
                .Where(piece => piece.Color == player)
                .SelectMany(piece => piece.GetLegalMoves())
                .ToHashSet();
        }
        public string GetFen()
        {
            StringBuilder fen = new StringBuilder();

            // 1. Board Representation
            for (int rank = 7; rank >= 0; rank--)
            {
                int emptyCount = 0;
                for (int file = 0; file < 8; file++)
                {
                    Position pos = new Position(file, rank);
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
                if (rank > 0) fen.Append('/');
            }

            // 2. Active Color
            fen.Append(" ").Append(MatchState.CurrentPlayer == Player.White ? "w" : "b");

            // 3. Castling Availability
            //StringBuilder castling = new StringBuilder();
            //if (MatchState.CanWhiteCastleKingside) castling.Append('K');
            //if (MatchState.CanWhiteCastleQueenside) castling.Append('Q');
            //if (MatchState.CanBlackCastleKingside) castling.Append('k');
            //if (MatchState.CanBlackCastleQueenside) castling.Append('q');
            //fen.Append(" ").Append(castling.Length > 0 ? castling.ToString() : "-");

            // 4. En Passant Target Square
            // You'll need to track the en passant square in your MatchState
            fen.Append(" ").Append("-"); // Placeholder - implement actual en passant square tracking

            // 5. Halfmove Clock (moves since last capture or pawn advance)
            // You'll need to track this in your MatchState
            fen.Append(" ").Append("0"); // Placeholder

            // 6. Fullmove Number
            // You'll need to track this in your MatchState
            fen.Append(" ").Append("1"); // Placeholder

            return fen.ToString();
        }
    }
}