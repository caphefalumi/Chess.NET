namespace Chess
{
    class Rook : Piece
    {
        private HashSet<Position> _moves;
        private static readonly (int, int)[] directions =

        {
            (-1, 0),  // Diagonal up-left, up, up-right
            ( 0, -1),  (0, 1),  // Left, Right
            ( 1, 0)   // Diagonal down-left, down, down-right
        };


        public Rook(string color, Position position) : base("Rook", color, position)
        {
            _moves = new HashSet<Position>();

        }

        public override HashSet<Position> GetLegalMoves()
        {
            HashSet<Position> _moves = new HashSet<Position>();
            AddLegalMoves(directions, _moves); // Uses inherited method

            return _moves;
        }
    }
}
