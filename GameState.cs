namespace Chess
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }

        public GameState(Board board, Player player)
        {
            CurrentPlayer = player;
            Board = board;
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
