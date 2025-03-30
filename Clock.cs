using System;

namespace Chess
{
    public class Clock
    {
        private readonly Timer _whiteTimer;
        private readonly Timer _blackTimer;
        private Player _currentTurn;

        public event Action<string> OnTimeExpired;

        public Clock(TimeSpan initialTime, TimeSpan increment)
        {
            _whiteTimer = new Timer(initialTime, increment);
            _blackTimer = new Timer(initialTime, increment);
            _currentTurn = Player.White;

            _whiteTimer.OnTimeExpired += () => HandleTimeExpired(Player.White);
            _blackTimer.OnTimeExpired += () => HandleTimeExpired(Player.Black);
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

        public string GetFormattedTime(Player player)
        {
            return player == Player.White ? _whiteTimer.GetFormattedTime() : _blackTimer.GetFormattedTime();
        }

        private void HandleTimeExpired(Player player)
        {
            OnTimeExpired?.Invoke($"{player} ran out of time");
        }
    }
}
