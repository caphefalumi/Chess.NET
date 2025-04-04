namespace Chess
{
    public interface IVariantStrategy
    {
        void StartGame(Game game, Board board, MatchConfiguration config);
    }
}
