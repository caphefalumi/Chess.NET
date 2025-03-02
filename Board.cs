using System;
using SplashKitSDK;

[Flags]
public enum PieceEval
{
    NONE = 0,
    KING = 1,
    PAWN = 2,
    BISHOP = 3,
    KNIGHT = 4,
    ROOK = 5,
    QUEEN = 6,
    WHITE = 8,
    BLACK = 16
}

public static class Board
{
    private static int _squareSize;
    private static int _startX;
    private static int _startY;
    private static Color _lightColor;
    private static Color _darkColor;
    private static string[,] ChessBoard;

    public static void Create(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
    {
        _squareSize = squareSize;
        _startX = startX;
        _startY = startY;
        _lightColor = lightColor;
        _darkColor = darkColor;
        ChessBoard = InitializeBoard();
    }

    private static string[,] InitializeBoard()
    {
        return new string[8, 8]
        {
            { "R", "N", "B", "Q", "K", "B", "N", "R" },
            { "P", "P", "P", "P", "P", "P", "P", "P" },
            { ".", ".", ".", ".", ".", ".", ".", "." },
            { ".", ".", ".", ".", ".", ".", ".", "." },
            { ".", ".", ".", ".", ".", ".", ".", "." },
            { ".", ".", ".", ".", ".", ".", ".", "." },
            { "p", "p", "p", "p", "p", "p", "p", "p" },
            { "r", "n", "b", "q", "k", "b", "n", "r" }
        };
    }

    public static void DrawBoard()
    {
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                bool isLightSquare = ((rank + file) & 1) != 0;
                Color squareColor = isLightSquare ? _lightColor : _darkColor;

                int x = _startX + (file * _squareSize);
                int y = _startY + (rank * _squareSize);

                SplashKit.FillRectangle(squareColor, x, y, _squareSize, _squareSize);
            }
        }
    }

    public static void DrawPieces()
    {
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                string piece = ChessBoard[rank, file];
                if (piece != ".")
                {
                    
                    
                }
            }
        }
    }
}
