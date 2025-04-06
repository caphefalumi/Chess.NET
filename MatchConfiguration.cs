namespace Chess
{
    public class MatchConfiguration
    {
        public Variant Mode { get; set; }
        public TimeControl TimeControl { get; set; }
        public bool UseIncrement { get; set; }
        public int IncrementSeconds { get; set; }
        public NetworkRole NetworkRole { get; set; }
        public Player PlayerColor { get; set; }
        
        public MatchConfiguration()
        {
            Mode = Variant.TwoPlayer;
            TimeControl = TimeControl.TenMinutes;
            UseIncrement = false;
            IncrementSeconds = 0;
            NetworkRole = NetworkRole.None;
            PlayerColor = Player.White;
        }

        public TimeSpan GetTimeSpan()
        {
            return TimeControl switch
            {
                TimeControl.Bullet1 => TimeSpan.FromMinutes(1),
                TimeControl.Bullet3 => TimeSpan.FromMinutes(3),
                TimeControl.Blitz5 => TimeSpan.FromMinutes(5),
                TimeControl.TenMinutes => TimeSpan.FromMinutes(10),
                TimeControl.FifteenMinutes => TimeSpan.FromMinutes(15),
                TimeControl.ThirtyMinutes => TimeSpan.FromMinutes(30),
                TimeControl.Unlimited => TimeSpan.FromHours(24),
                _ => TimeSpan.FromMinutes(10)
            };
        }

        public TimeSpan GetIncrementSpan()
        {
            return UseIncrement ? TimeSpan.FromSeconds(IncrementSeconds) : TimeSpan.Zero;
        }
    }

    public enum TimeControl
    {
        Bullet1,
        Bullet3,
        Blitz5,
        TenMinutes,
        FifteenMinutes,
        ThirtyMinutes,
        Unlimited
    }
}
