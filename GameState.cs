namespace Chess
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public Stack<Move> MoveHistory { get; private set; }  // Store move history
        private static GameState _instance;

        private GameState(Board board, Player player)
        {
            CurrentPlayer = player;
            Board = board;
            MoveHistory = new Stack<Move>();  // Initialize move stack
            board.GameState = this;
            Sounds.GameStart.Play();
        }

        public static GameState GetInstance(Board board, Player player)
        {
            if (_instance is null)
            {
                _instance = new GameState(board, player);
            }
            return _instance;
        }

        public static GameState GetInstance()
        {
            if (_instance is not null)
            {
                return _instance;
            }
            return null;
        }


        public void MakeMove(Move move)
        {
            move.Execute(Board);
            MoveHistory.Push(move);  // Store the move for undoing
            CurrentPlayer = CurrentPlayer.Opponent();
        }

        public void UnmakeMove()
        {
            if (MoveHistory.Count > 0)
            {
                Move lastMove = MoveHistory.Pop();  // Retrieve last move
                lastMove.Undo(Board);  // Reverse the move
                CurrentPlayer = CurrentPlayer.Opponent();
            }
        }
        public bool MoveResolvesCheck(Move move, Player player)
        {
            MakeMove(move);  // Simulate move
            bool stillInCheck = Board.IsInCheck(player);
            UnmakeMove();  // Undo move
            return !stillInCheck;  // The move is legal only if it removes check
        }

        private bool IsGameOver()
        {
            return false; // Logic for game over detection
        }
    }
}
