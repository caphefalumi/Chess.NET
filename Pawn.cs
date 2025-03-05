namespace Chess
{
    class Pawn : Piece
    {
        private HashSet<Position> _moves;

        public Pawn(string color, Position position) : base("Pawn", color, position)
        {
            _moves = new HashSet<Position>();

        }

        // Possible L-shaped _moves for a knight
        private static readonly (int, int)[] _directions =
        {
                (-2, -1), (-2, 1),
                (-1, -2), (-1, 2),
                ( 1, -2), ( 1, 2),
                ( 2, -1), ( 2, 1)
            };


        public override HashSet<Position> GetLegalMoves()
        {

            AddLegalMoves(_directions, _moves);
            return _moves;
        }

    }
}
