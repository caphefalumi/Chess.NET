namespace Chess
{
    class King : Piece, IPiece
    {
        HashSet<Position> _moves;

        // Directions the King can move (tuples for cleaner code)
        private static readonly (int, int)[] directions =
        {
            (-1, -1), (-1, 0), (-1, 1),
            ( 0, -1),          ( 0, 1),
            ( 1, -1), ( 1, 0), ( 1, 1)
        };

        public King(string color, Position position) : base("King", color, position) {
            _moves = new HashSet<Position>();
        }

        public override HashSet<Position> GetLegalMoves()
        {
            AddLegalMoves(directions, _moves);
            return _moves;
        }

    }
}
