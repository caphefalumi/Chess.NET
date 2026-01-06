using SplashKitSDK;
using Chess.Pieces;
using Chess.Moves;
using Chess.Interfaces;
using Chess.UI.Drawing;

using Chess.UI.States;
using Chess.Networking;
using Chess.Utils;
namespace Chess.Core
{
    public class Clock
    {
        private readonly Timer _whiteTimer;
        private readonly Timer _blackTimer;
        private Player _currentTurn;
        private static Clock _instance;

        public event Action<string> OnTimeExpired;

        private Clock(TimeSpan initialTime, TimeSpan increment)
        {
            _whiteTimer = new Timer(initialTime, increment);
            _blackTimer = new Timer(initialTime, increment);
            _currentTurn = Player.White;

            _whiteTimer.OnTimeExpired += () => HandleTimeExpired(Player.White);
            _blackTimer.OnTimeExpired += () => HandleTimeExpired(Player.Black);
        }

        public static Clock GetInstance(TimeSpan initialTime, TimeSpan increment)
        {
            if (_instance == null)
            {
                _instance = new Clock(initialTime, increment);
            }
            return _instance;
        }

        private void HandleTimeExpired(Player player)
        {
            OnTimeExpired?.Invoke($"{player} ran out of time!");
        }
        
        public TimeSpan WhiteTime => _whiteTimer.TimeRemaining;
        public TimeSpan BlackTime => _blackTimer.TimeRemaining;
        public Player CurrentTurn
        {
            get => _currentTurn;
            set => _currentTurn = value;
        }

        public void Start()
        {
            if (_currentTurn == Player.White)
                _whiteTimer.Start();
            else
                _blackTimer.Start();
        }

        public void SwitchTurn()
        {
            if (_currentTurn == Player.White)
            {
                _whiteTimer.Pause();
                _whiteTimer.AddIncrement();
                _blackTimer.Start();
            }
            else
            {
                _blackTimer.Pause();
                _blackTimer.AddIncrement();
                _whiteTimer.Start();
            }

            _currentTurn = _currentTurn.Opponent();
        }

        public void Update()
        {
            _whiteTimer.Update();
            _blackTimer.Update();
        }

        public void Reset(TimeSpan newTime)
        {
            _whiteTimer.Reset(newTime);
            _blackTimer.Reset(newTime);
            _currentTurn = Player.White;
        }


        public void SetRemainingTime(Player player, TimeSpan time)
        {
            if (player == Player.White)
            {
                _whiteTimer.Reset(time);
            }
            else if (player == Player.Black)
            {
                _blackTimer.Reset(time);
            }
        }

        public TimeSpan GetRemainingTime(Player player)
        {
            return player == Player.White ? _whiteTimer.TimeRemaining : _blackTimer.TimeRemaining;
        }
    }
}
