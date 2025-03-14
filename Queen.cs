namespace Chess
{
    class Queen : Piece
    {
        // Directions the Queen can move (combining Rook & Bishop _moves)
        private static readonly (int, int)[] directions =
        {
            (-1, -1), (-1, 0), (-1, 1),  // Diagonal up-left, up, up-right
            ( 0, -1),          ( 0, 1),  // Left, Right
            ( 1, -1), ( 1, 0), ( 1, 1)   // Diagonal down-left, down, down-right
        };

        public Queen(string color, Position position) : base("Queen", color, position) { }

        public override HashSet<Position> GetLegalMoves()
        {
            HashSet<Position> _moves = new HashSet<Position>();
            AddLegalMoves(directions, _moves, true);

            return _moves;
        }
    }
}
