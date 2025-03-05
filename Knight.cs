namespace Chess
{
    class Knight : Piece, IPiece
    {
        private HashSet<Position> _moves;

        // Possible L-shaped _moves for a knight
        private static readonly (int, int)[] _directions =
        {
            (-2, -1), (-2, 1),
            (-1, -2), (-1, 2),
            ( 1, -2), ( 1, 2),
            ( 2, -1), ( 2, 1)
        };

        public Knight(string color, Position position) : base("Night", color, position) 
        {
            _moves = new HashSet<Position>();
        }

        public override HashSet<Position> GetLegalMoves()
        {
            
            AddLegalMoves(_directions, _moves);
            return _moves;
        }


    }
}
