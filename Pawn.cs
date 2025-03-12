namespace Chess
{
    class Pawn : Piece, IPiece
    {
        private HashSet<Position> _moves;
        public bool CanBeEnPassantCaptured { get; set; }

        private static readonly (int, int)[] _captureDirections = { (-1, 1), (1, 1) };

        public Pawn(string color, Position position) : base("Pawn", color, position)
        {
            _moves = new HashSet<Position>();
            CanBeEnPassantCaptured = false;
        }

        public override HashSet<Position> GetLegalMoves()
        {
            _moves.Clear();
            int direction = Color == "White" ? 1 : -1;
            AddForwardMoves(direction);
            AddCaptureMoves(direction);
            AddEnPassantMoves(direction);
            return _moves;
        }

        private void AddForwardMoves(int direction)
        {
            Position oneStep = new Position(Position.File, Position.Rank + direction);

            if (IsWithinBounds(oneStep) && Board.FindPieceAt(oneStep) == null)
            {
                _moves.Add(oneStep);
                if (!HasMoved)
                {
                    Position twoSteps = new Position(Position.File, Position.Rank + (2 * direction));
                    if (IsWithinBounds(twoSteps) && Board.FindPieceAt(twoSteps) == null)
                    {
                        _moves.Add(twoSteps);
                    }
                }
            }
        }

        private void AddCaptureMoves(int direction)
        {
            foreach (var (fileOffset, rankOffset) in _captureDirections)
            {
                Position newPos = new Position(Position.File + fileOffset, Position.Rank + (direction * rankOffset));
                if (IsWithinBounds(newPos))
                {
                    IPiece targetPiece = Board.FindPieceAt(newPos);
                    if (targetPiece != null && targetPiece.Color != this.Color)
                    {
                        _moves.Add(newPos);
                    }
                }
            }
        }

        private void AddEnPassantMoves(int direction)
        {
            foreach (var (fileOffset, _) in _captureDirections)
            {
                Position adjacentPos = new Position(Position.File + fileOffset, Position.Rank);
                if (IsWithinBounds(adjacentPos))
                {
                    IPiece targetPiece = Board.FindPieceAt(adjacentPos);
                    if (targetPiece is Pawn enemyPawn && enemyPawn.CanBeEnPassantCaptured)
                    {
                        Position enPassantPos = new Position(Position.File + fileOffset, Position.Rank + direction);
                        _moves.Add(enPassantPos);
                    }
                }
            }
        }




        public bool IsOnPromotionRank()
        {
            return (Color == "White" && Position.Rank == 0) || (Color == "Black" && Position.Rank == 7);
        }
    }
}
