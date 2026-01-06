using Chess.Core;
using Chess.Pieces;
using Chess.Moves;

namespace Chess.Utils
{
    public static class PlayerHelper
    {
        public static Player Opponent(this Player player)
        {
            switch (player)
            {
                case Player.White:
                    return Player.Black;
                case Player.Black:  
                    return Player.White;
                default:
                    return Player.Viewer;
            }
        }
    }
} 
