namespace Chess
{
    public class ComputerMode : IVariantStrategy
    {
        public void StartGame(Game game, Board board, MatchConfiguration config)
        {
            game.ChangeState(new GameplayScreen(game, board, config));
        }
    }
} 