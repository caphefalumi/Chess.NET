// Rename your existing GameState.cs file to ChessGameState.cs
// Change class name and references appropriately

using SplashKitSDK;

namespace Chess
{
    public class ChessGameState
    {
        // Your existing GameState code, renamed to ChessGameState

        private static ChessGameState _instance;
        private Board _board;
        public Player CurrentPlayer { get; private set; }
        public Stack<Move> MoveHistory { get; }

        private ChessGameState(Board board, Player startingPlayer)
        {
            _board = board;
            CurrentPlayer = startingPlayer;
            MoveHistory = new Stack<Move>();
        }

        public static ChessGameState GetInstance(Board board, Player startingPlayer)
        {
            if (_instance == null)
            {
                _instance = new ChessGameState(board, startingPlayer);
            }
            return _instance;
        }
        public void MakeMove(Move move)
        {
            move.Execute(_board);
            MoveHistory.Push(move);  // Store the move for undoing
            CurrentPlayer = CurrentPlayer.Opponent();
        }

        public void UnmakeMove()
        {
            if (MoveHistory.Count > 0)
            {
                Move lastMove = MoveHistory.Pop();  // Retrieve last move
                lastMove.Undo(_board);  // Reverse the move
                CurrentPlayer = CurrentPlayer.Opponent();
            }
        }
        public bool MoveResolvesCheck(Move move, Player player)
        {
            MakeMove(move);  // Simulate move
            bool stillInCheck = _board.IsInCheck(player);
            UnmakeMove();  // Undo move
            return !stillInCheck;  // The move is legal only if it removes check
        }

        private bool IsGameOver()
        {
            return false; // Logic for game over detection
        }
    }
}