namespace Chess
{
    class Rook : Piece
    {
        private HashSet<Position> _moves;
        private bool _hasMoved = false;

        // Directions the Rook can move
        private static readonly (int, int)[] directions =
        {
            (-1, 0), (1, 0),  // Left & Right
            (0, -1), (0, 1)   // Up & Down
        };

        public Rook(string color, Position position) : base("Rook", color, position)
        {
            _moves = new HashSet<Position>();
        }

        public override HashSet<Position> GetLegalMoves()
        {
            HashSet<Position> _moves = new HashSet<Position>();

            // Rooks use sliding movement
            AddLegalMoves(directions, _moves, true);

            return _moves;
        }

        public bool HasMoved
        {
            get { return _hasMoved; }
        }

        public void SetHasMoved()
        {
            _hasMoved = true;
        }
    }
}