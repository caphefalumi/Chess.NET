using SplashKitSDK;
using Chess.Pieces;
using Chess.Moves;
using Chess.Interfaces;
using Chess.UI.Drawing;

using Chess.UI.States;
using Chess.Networking;
namespace Chess.Core
{
    public class MatchConfiguration
    {
        private Variant _mode;
        private TimeControl _timeControl;
        private bool _useIncrement;
        private int _incrementSeconds;
        private NetworkRole _networkRole;
        private Player _playerColor;

        public Variant Mode
        {
            get => _mode;
            set => _mode = value;
        }

        public TimeControl TimeControl
        {
            get => _timeControl;
            set => _timeControl = value;
        }

        public bool UseIncrement
        {
            get => _useIncrement;
            set => _useIncrement = value;
        }

        public int IncrementSeconds
        {
            get => _incrementSeconds;
            set => _incrementSeconds = value;
        }

        public NetworkRole NetworkRole
        {
            get => _networkRole;
            set => _networkRole = value;
        }

        public Player PlayerColor
        {
            get => _playerColor;
            set => _playerColor = value;
        }

        public MatchConfiguration()
        {
            _mode = Variant.TwoPlayer;
            _timeControl = TimeControl.TenMinutes;
            _useIncrement = false;
            _incrementSeconds = 0;
            _networkRole = NetworkRole.None;
            _playerColor = Player.White;
        }

        public TimeSpan GetTimeSpan()
        {
            return _timeControl switch
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
            return _useIncrement ? TimeSpan.FromSeconds(_incrementSeconds) : TimeSpan.Zero;
        }
    }
}
