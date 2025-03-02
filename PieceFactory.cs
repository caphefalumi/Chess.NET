using Chess;

public static class PieceFactory
{
    public static Piece CreatePiece(string color, string type)
    {
        switch (type)
        {
            case "King": return new King(color);
            case "Queen": return new Queen(color);
            case "Bishop": return new Bishop(color);
            case "Knight": return new Knight(color);
            case "Rook": return new Rook(color);
            case "Pawn": return new Pawn(color);
            default: return null;
        }
    }
}
