using System;

using Chess.Core;
using Chess.Pieces;
using Chess.Moves;
namespace Chess.Interfaces
{
    /// <summary>
    /// Interface for game observers that will receive notifications about game events.
    /// Part of the Observer design pattern - observers implement this interface to react to game events.
    /// </summary>
    public interface IGameObserver
    {
        void OnMoveMade(Move move);

        void OnGameOver(GameResult result, string message);

        void OnCheck(Player playerInCheck);

        void OnTurnChanged(Player newPlayer);
    }
} 
