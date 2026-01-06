using System;
using System.Collections.Generic;

using Chess.Core;
using Chess.Pieces;
using Chess.Moves;
using Chess.Interfaces;
using Chess.UI.Screens;
namespace Chess.UI.Drawing
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

        public static GameEventManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GameEventManager();
            }
            return _instance;
        }

        public void RegisterObserver(IGameObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void NotifyMoveMade(Move move)
        {
            foreach (IGameObserver observer in _observers)
            {
                observer.OnMoveMade(move);
            }
        }

        /// Notifies all registered observers that the game is over
        /// </summary>
        public void NotifyGameOver(GameResult result, string message)
        {
            foreach (IGameObserver observer in _observers)
            {
                observer.OnGameOver(result, message);
            }
        }

        /// Notifies all registered observers that a player is in check
        public void NotifyCheck(Player playerInCheck)
        {
            foreach (IGameObserver observer in _observers)
            {
                observer.OnCheck(playerInCheck);
            }
        }

        /// Notifies all registered observers that the turn has changed
        public void NotifyTurnChanged(Player newPlayer)
        {
            foreach (IGameObserver observer in _observers)
            {
                observer.OnTurnChanged(newPlayer);
            }
        }
    }
} 
