using System.Collections.Generic;
using SplashKitSDK;

namespace Chess
{
    public class Board
    {
        private static Board _instance; // Singleton instance
        private static IPiece _selectedPiece; // Track the selected piece
        BoardDrawer _boardDrawer;

        public static HashSet<IPiece> Pieces;

        private Board(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            Pieces = PieceFactory.CreatePieces();
            _boardDrawer = BoardDrawer.GetInstance(squareSize, startX, startY, lightColor, darkColor);
        }

        public static Board GetInstance(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            if (_instance == null)
            {
                _instance = new Board(squareSize, startX, startY, lightColor, darkColor);
            }
            return _instance;
        }


        public static Piece FindPieceAt(Position pos)
        {
            foreach (Piece piece in Pieces)
            {
                if (piece.Position == pos)
                {
                    return piece;
                }
            }
            return null;
        }



    }
}
