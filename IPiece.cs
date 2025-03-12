namespace Chess
{
    public interface IPiece
    {
        string Name { get; }
        string Color { get; }
        Position Position { get; set; }
        bool HasMoved { get; set; }
        string Type { get; }
        void Draw();

        HashSet<Position> GetLegalMoves();

        //bool IsValidMove(Position newPosition);
    }
}
