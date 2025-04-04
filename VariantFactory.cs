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
                Variant.Online => new OnlineMode(),
                Variant.Computer => new ComputerMode(),
                Variant.Custom => new CustomMode(),
                _ => new TwoPlayerMode()
            };
        }
    }
}
