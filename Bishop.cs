namespace Chess
{
    class Bishop : Piece
    {

        HashSet<Position> moves;
        private static readonly (int, int)[] directions =
        {
            (-1, -1), (-1, 1),  // Diagonal up-left, up, up-right
            ( 1, -1), ( 1, 1)   // Diagonal down-left, down, down-right
        };

        public Bishop(string color, Position position) : base("Bishop", color, position)
        {
            moves = new HashSet<Position>();
        }

        public override HashSet<Position> GetLegalMoves()
        {
            HashSet<Position> _moves = new HashSet<Position>();
            AddLegalMoves(directions, _moves); // Uses inherited method

            return _moves;
        }
    }
}
