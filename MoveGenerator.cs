namespace Chess
{
    public static class MoveGenerator
    {
        public static HashSet<Position> GenerateMoves(IPiece piece)
        {
            return piece.GetLegalMoves();
        }

        public static void AddLegalMoves(IPiece piece, (int, int)[] directions, HashSet<Position> moves, bool isSlidingPiece = false)
        {
            if (isSlidingPiece)
            {
                AddSlidingMoves(piece, directions, moves);
            }
            else
            {
                foreach ((int dx, int dy) in directions)
                {
                    int newFile = piece.Position.File + dx;
                    int newRank = piece.Position.Rank + dy;
                    AddMoveIfLegal(piece, newFile, newRank, moves);
                }
            }
        }

        private static void AddMoveIfLegal(IPiece piece, int file, int rank, HashSet<Position> moves)
        {
            if (IsWithinBounds(file, rank) && Board.FindPieceAt(new Position(file, rank)) == null)
            {
                moves.Add(new Position(file, rank));
            }
        }

        private static void AddSlidingMoves(IPiece piece, (int, int)[] directions, HashSet<Position> moves)
        {
            foreach ((int dx, int dy) in directions)
            {
                int newFile = piece.Position.File;
                int newRank = piece.Position.Rank;

                while (true)
                {
                    newFile += dx;
                    newRank += dy;

                    if (!IsWithinBounds(newFile, newRank)) break;

                    IPiece pieceAtNewPos = Board.FindPieceAt(new Position(newFile, newRank));

                    if (pieceAtNewPos != null)
                    {
                        if (pieceAtNewPos.Color == piece.Color) break;
                        moves.Add(new Position(newFile, newRank));
                        break;
                    }

                    moves.Add(new Position(newFile, newRank));
                }
            }
        }

        public static bool IsWithinBounds(int file, int rank)
        {
            return file >= 0 && file < 8 && rank >= 0 && rank < 8;
        }

        public static bool IsWithinBounds(Position pos)
        {
            return pos.File >= 0 && pos.File < 8 && pos.Rank >= 0 && pos.Rank < 8;
        }
    }
}
