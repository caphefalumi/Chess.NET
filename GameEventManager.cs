using System;
using System.Collections.Generic;

namespace Chess
{

    public class GameEventManager
    {
        private static GameEventManager _instance;
        private readonly List<IGameObserver> _observers;

        /// <summary>
        /// Private constructor for the singleton pattern
        /// </summary>
        private GameEventManager()
        {
            _observers = new List<IGameObserver>();
        }

        /// <summary>
        /// Gets the singleton instance of the GameEventManager
        /// </summary>
        /// <returns>The singleton instance</returns>
        public static GameEventManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GameEventManager();
            }
            return _instance;
        }

        /// <summary>
        /// Registers an observer to receive game event notifications
        /// </summary>
        /// <param name="observer">The observer to register</param>
        public void RegisterObserver(IGameObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        /// <summary>
        /// Removes an observer so it no longer receives notifications
        /// </summary>
        /// <param name="observer">The observer to remove</param>
        public void RemoveObserver(IGameObserver observer)
        {
            _observers.Remove(observer);
        }

        /// <summary>
        /// Notifies all registered observers that a move was made
        /// </summary>
        /// <param name="move">The move that was executed</param>
        public void NotifyMoveMade(Move move)
        {
            foreach (IGameObserver observer in _observers)
            {
                observer.OnMoveMade(move);
            }
        }

        /// <summary>
        /// Notifies all registered observers that the game is over
        /// </summary>
        /// <param name="result">The game result (win, draw, etc.)</param>
        /// <param name="message">Descriptive message about how the game ended</param>
        public void NotifyGameOver(GameResult result, string message)
        {
            foreach (IGameObserver observer in _observers)
            {
                observer.OnGameOver(result, message);
            }
        }

        /// <summary>
        /// Notifies all registered observers that a player is in check
        /// </summary>
        /// <param name="playerInCheck">The player who is in check</param>
        public void NotifyCheck(Player playerInCheck)
        {
            foreach (IGameObserver observer in _observers)
            {
                observer.OnCheck(playerInCheck);
            }
        }

        /// <summary>
        /// Notifies all registered observers that the turn has changed
        /// </summary>
        /// <param name="newPlayer">The player whose turn it now is</param>
        public void NotifyTurnChanged(Player newPlayer)
        {
            foreach (IGameObserver observer in _observers)
            {
                observer.OnTurnChanged(newPlayer);
            }
        }
    }
} 