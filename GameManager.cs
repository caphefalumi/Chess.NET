using System;
using System.Collections.Generic;

namespace Chess
{
    public class GameManager
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public GameManager(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;
        }

        public IEnumerable<Move> LegalMoves(Position pos)
        {
            if (Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer)
            {
                return Enumerable.Empty<Move>();
            }
            Piece piece = Board[pos];
            return piece.GetMoves(pos, Board);
        }

        public void MakeMove(Move move)
        {
            move.Execute(Board);
            CurrentPlayer = CurrentPlayer.Opponent();

        }


        private bool IsGameOver()
        {
            // Logic to determine if the game is over (e.g., checkmate, stalemate)
            return false;
        }
    }
}
