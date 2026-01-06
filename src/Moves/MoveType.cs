using Chess.Core;
using Chess.Pieces;
using Chess.Interfaces;
using Chess.UI.Media;

namespace Chess.Moves
{
    public enum MoveType
    {
        Normal,
        CastleKS,
        CastleQS,
        DoublePawn,
        EnPassant,
        Promotion
    }
} 
