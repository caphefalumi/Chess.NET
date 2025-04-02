namespace Chess
{
    public interface IVariantStrategy
    {
        void StartGame(Game game, Board board, MatchConfiguration config);
    }
    
    // Update implementations
    public class TwoPlayerMode : IVariantStrategy
    {
        public void StartGame(Game game, Board board, MatchConfiguration config)
        {
            game.ChangeState(new GameplayScreen(game, board, config));
        }
    }
    
    public class ComputerMode : IVariantStrategy
    {
        public void StartGame(Game game, Board board, MatchConfiguration config)
        {
            game.ChangeState(new GameplayScreen(game, board, config));
        }
    }
    
    public class SpellChessMode : IVariantStrategy
    {
        public void StartGame(Game game, Board board, MatchConfiguration config)
        {
            // Ensure Spell Chess is set to two-player mode, not computer mode
            config.Mode = Variant.SpellChess;
            
            // Initialize the board and spells
            board.InitializeSpells(Player.White);
            board.InitializeSpells(Player.Black);
            
            game.ChangeState(new GameplayScreen(game, board, config));
        }
    }
    
    public class CustomMode : IVariantStrategy
    {
        public void StartGame(Game game, Board board, MatchConfiguration config)
        {
            game.ChangeState(new GameplayScreen(game, board, config));
        }
    }
}
