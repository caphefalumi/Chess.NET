namespace Chess
{
    public interface IPiece
    {
        string Name { get; }
        string Color { get; }
        Position Position { get; set; }
        bool HasMoved { get; set; }
        IEnumerable<Position> GetMoves();
    }
}