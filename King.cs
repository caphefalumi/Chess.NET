namespace Chess
{
    class King : Piece, IPiece
    {
        HashSet<Position> _moves;
        private bool _hasMoved = false;

        // Directions the King can move (tuples for cleaner code)
        private static readonly (int, int)[] directions =
        {
            (-1, -1), (-1, 0), (-1, 1),
            ( 0, -1),          ( 0, 1),
            ( 1, -1), ( 1, 0), ( 1, 1)
        };

        public King(string color, Position position) : base("King", color, position)
        {
            _moves = new HashSet<Position>();
        }

        public override HashSet<Position> GetLegalMoves()
        {
            _moves.Clear();
            AddLegalMoves(directions, _moves);

            // Add castling moves if possible
            AddCastlingMoves();

            return _moves;
        }

        private void AddCastlingMoves()
        {
            if (_hasMoved)
                return;

            // Check kingside castling
            CheckCastling(7, 1); // Rook file, king move direction

            // Check queenside castling
            CheckCastling(0, -1); // Rook file, king move direction
        }

        private void CheckCastling(int rookFile, int direction)
        {
            // Find the rook
            IPiece rookPiece = Board.FindPieceAt(new Position(rookFile, Position.Rank));

            if (rookPiece == null || !(rookPiece is Rook) || rookPiece.Color != Color)
                return;

            Rook rook = (Rook)rookPiece;

            if (rook.HasMoved)
                return;

            // Check if squares between king and rook are empty
            int start = Position.File + direction;
            int end = direction > 0 ? rookFile - 1 : rookFile + 1;

            for (int file = Math.Min(start, end); file <= Math.Max(start, end); file++)
            {
                if (Board.FindPieceAt(new Position(file, Position.Rank)) != null)
                    return;
            }

            // Check if king passes through check
            for (int file = Position.File; file != Position.File + (2 * direction); file += direction)
            {
                // Temporarily move king
                Position originalPos = Position;
                Position tempPos = new Position(file, Position.Rank);
                Position = tempPos;

                // Check if king is in check
                bool inCheck = IsInCheck();

                // Move king back
                Position = originalPos;

                if (inCheck)
                    return;
            }

            // Add castling move (king moves 2 squares)
            _moves.Add(new Position(Position.File + (2 * direction), Position.Rank));
        }

        private bool IsInCheck()
        {
            // Check if any opponent piece can attack the king
            foreach (IPiece piece in Board.Pieces)
            {
                if (piece.Color != Color)
                {
                    HashSet<Position> attackMoves = piece.GetLegalMoves();
                    if (attackMoves.Contains(Position))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SetHasMoved()
        {
            _hasMoved = true;
        }
    }
}