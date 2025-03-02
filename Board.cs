using System.Collections.Generic;
using SplashKitSDK;

namespace Chess
{
    public class Board
    {
        private static Board _instance; // Singleton instance
        private static IPiece _selectedPiece; // Track the selected piece
        public static HashSet<IPiece> Pieces;

        private Board(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            BoardDrawer.Initialize(squareSize, startX, startY, lightColor, darkColor);
            Pieces = PieceFactory.CreatePieces();
        }

        public static Board GetInstance(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
        {
            if (_instance == null)
            {
                _instance = new Board(squareSize, startX, startY, lightColor, darkColor);
            }
            return _instance;
        }

        public void Draw()
        {
            BoardDrawer.DrawBoard();

            foreach (IPiece piece in Pieces)
            {
                piece.Draw();
            }
        }

    }
}
