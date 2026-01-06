using SplashKitSDK;
using Chess.Pieces;
using Chess.Moves;
using Chess.Interfaces;
using Chess.UI.Drawing;

using Chess.UI.States;
using Chess.Networking;
namespace Chess.Core
{
    public class Timer
    {
        private TimeSpan _clockRemaining;
        private readonly TimeSpan _increment;
        private DateTime _lastStartTime;
        private bool _isRunning;

        public event Action OnTimeExpired;

        public Timer(TimeSpan initialTime, TimeSpan increment)
        {
            _clockRemaining = initialTime;
            _increment = increment;
            _isRunning = false;
        }

        public TimeSpan TimeRemaining => _clockRemaining;
        public bool IsRunning => _isRunning;

        public void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _lastStartTime = DateTime.Now;
            }
        }

        public void Pause()
        {
            if (_isRunning)
            {
                _isRunning = false;
            }
        }

        public void AddIncrement()
        {
            _clockRemaining += _increment;
        }

        public void Update()
        {
            if (_isRunning)
            {
                UpdateTime();
            }
        }

        private void UpdateTime()
        {
            TimeSpan elapsed = DateTime.Now - _lastStartTime;
            _clockRemaining -= elapsed;

            if (_clockRemaining <= TimeSpan.Zero)
            {
                _clockRemaining = TimeSpan.Zero;
                OnTimeExpired?.Invoke();
                Pause();
            }

            _lastStartTime = DateTime.Now;
        }

        public void Reset(TimeSpan newTime)
        {
            _clockRemaining = newTime;
            _isRunning = false;
        }
    }
}
