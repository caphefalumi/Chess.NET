using SplashKitSDK;
using System.Collections.Generic;

namespace Chess
{
    public class MatchState
    {
        private static MatchState _instance;
        private Board _board;
        public Player CurrentPlayer { get; private set; }
        public Stack<Move> MoveHistory { get; }

        private MatchState(Board board, Player startingPlayer)
        {
            _board = board;
            CurrentPlayer = startingPlayer;
            MoveHistory = new Stack<Move>();
        }

        public static MatchState GetInstance(Board board, Player startingPlayer)
        {
            if (_instance == null)
            {
                _instance = new MatchState(board, startingPlayer);
            }
            return _instance;
        }
        public static MatchState GetInstance()
        {
            return _instance;
        }
        public void MakeMove(Move move)
        {
            move.Execute(_board);
            MoveHistory.Push(move);
            CurrentPlayer = CurrentPlayer.Opponent();
        }

        public void UnmakeMove()
        {
            if (MoveHistory.Count > 0)
            {
                Move lastMove = MoveHistory.Pop();
                lastMove.Undo(_board);
                CurrentPlayer = CurrentPlayer.Opponent();
            }
        }
        public bool MoveResolvesCheck(Move move, Player player)
        {
            MakeMove(move);
            bool stillInCheck = _board.IsInCheck(player);
            UnmakeMove();
            return !stillInCheck;
        }
    }
}
