// GameModeFactory.cs
namespace Chess
{
    public static class VariantFactory
    {
        public static IVariantStrategy CreateGameMode(MatchConfiguration config)
        {
            return config.Mode switch
            {
                Variant.TwoPlayer => new TwoPlayerMode(),
                Variant.Computer => new ComputerMode(),
                Variant.SpellChess => new SpellChessMode(),
                Variant.Custom => new CustomMode(),
                _ => new TwoPlayerMode()
            };
        }
    }
}
