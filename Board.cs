using System;
using System.Collections.Generic;
using Chess;
using SplashKitSDK;

public static class Board
{
    private static int _squareSize;
    private static int _startX;
    private static int _startY;
    private static Color _lightColor;
    private static Color _darkColor;
    private static char[,] ChessBoard;

    public static void Create(int squareSize, int startX, int startY, Color lightColor, Color darkColor)
    {
        _squareSize = squareSize;
        _startX = startX;
        _startY = startY;
        _lightColor = lightColor;
        _darkColor = darkColor;
        ChessBoard = InitializeBoard();
    }

    private static char[,] InitializeBoard()
    {
        return new char[8, 8]
        {
            { 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R' },
            { 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' },
            { '.', '.', '.', '.', '.', '.', '.', '.' },
            { '.', '.', '.', '.', '.', '.', '.', '.' },
            { '.', '.', '.', '.', '.', '.', '.', '.' },
            { '.', '.', '.', '.', '.', '.', '.', '.' },
            { 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' },
            { 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' }
        };
    }
    public Piece[,] Pieces
    {

    }
    public static Piece[,] InitializePieces()
    {
        Piece[,] pieces = new Piece[8, 8];

        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                char pieceChar = ChessBoard[rank, file];
                pieces[rank, file] = GetPiece(pieceChar, rank, file);
            }
        }
        return pieces;
    }
    public static void DrawPieces()
    {

    }

    private static Piece GetPiece(char pieceChar, int rank, int file)
    {
        if (pieceChar == '.') return null; // Empty square

        string color = char.IsUpper(pieceChar) ? "White" : "Black";
        Position position = new Position(rank * 80, file * 80);

        switch (char.ToUpper(pieceChar))
        {
            case 'P': return new Pawn("Pawn", color, position);
            case 'R': return new Rook("Rook", color, position);
            case 'N': return new Knight("Knight", color, position);
            case 'B': return new Bishop("Bishop", color, position);
            case 'Q': return new Queen("Queen", color, position);
            case 'K': return new King("King", color, position);
            default: return null;
        }
    }
}
