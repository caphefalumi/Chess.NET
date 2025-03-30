using System;

namespace Chess
{
    public class Timer
    {
        private TimeSpan _clockemaining;
        private readonly TimeSpan _increment;
        private DateTime _lastStartTime;
        private bool _isRunning;

        public event Action OnTimeExpired;

        public Timer(TimeSpan initialTime, TimeSpan increment)
        {
            _clockemaining = initialTime;
            _increment = increment;
            _isRunning = false;
        }

        public TimeSpan TimeRemaining => _clockemaining;
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
            _clockemaining += _increment;
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
            _clockemaining -= elapsed;

            if (_clockemaining <= TimeSpan.Zero)
            {
                _clockemaining = TimeSpan.Zero;
                OnTimeExpired?.Invoke();
                Pause();
            }

            _lastStartTime = DateTime.Now;
        }

        public void Reset(TimeSpan newTime)
        {
            _clockemaining = newTime;
            _isRunning = false;
        }

        public string GetFormattedTime()
        {
            return $"{(int)_clockemaining.TotalMinutes:D2}:{_clockemaining.Seconds:D2}";
        }
    }
}
