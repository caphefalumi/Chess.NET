namespace Chess
{
    public interface IPiece
    {
        string Name { get; }
        string Color { get; }
        Position Position { get; set; }
        bool HasMoved { get; set; }
        HashSet<Position> GetLegalMoves();
        void Draw();
        void DrawAt(float x, float y);
    }
}