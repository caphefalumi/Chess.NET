namespace Chess
{
    class Bishop : Piece
    {

        HashSet<Position> moves;
        private static readonly (int, int)[] directions =
        {
            (-1, -1), (-1, 1),
            ( 1, -1), ( 1, 1)
        };

        public Bishop(string color, Position position) : base("Bishop", color, position)
        {
            moves = new HashSet<Position>();
        }

        public override HashSet<Position> GetLegalMoves()
        {
            HashSet<Position> _moves = new HashSet<Position>();
            AddLegalMoves(directions, _moves, true); // Uses inherited method

            return _moves;
        }
    }
}
