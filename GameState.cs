namespace Chess
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        private Stack<Move> _moveHistory;  // Store move history

        public GameState(Board board, Player player)
        {
            CurrentPlayer = player;
            Board = board;
            _moveHistory = new Stack<Move>();  // Initialize move stack
            board.GameState = this;
        }

        public void MakeMove(Move move)
        {
            move.Execute(Board);
            _moveHistory.Push(move);  // Store the move for undoing
            CurrentPlayer = CurrentPlayer.Opponent();
        }

        public void UnmakeMove()
        {
            if (_moveHistory.Count > 0)
            {
                Move lastMove = _moveHistory.Pop();  // Retrieve last move
                lastMove.Undo(Board);  // Reverse the move
                CurrentPlayer = CurrentPlayer.Opponent();
            }
        }
        public bool MoveResolvesCheck(Move move)
        {
            MakeMove(move);  // Simulate move
            bool stillInCheck = Board.IsInCheck(CurrentPlayer);
            UnmakeMove();  // Undo move
            return !stillInCheck;  // The move is legal only if it removes check
        }

        private bool IsGameOver()
        {
            return false; // Logic for game over detection
        }
    }
}
