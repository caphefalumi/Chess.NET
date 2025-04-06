using System;

namespace Chess
{
    /// <summary>
    /// Interface for game observers that will receive notifications about game events.
    /// Part of the Observer design pattern - observers implement this interface to react to game events.
    /// </summary>
    public interface IGameObserver
    {
        /// <summary>
        /// Called when a move is made on the board
        /// </summary>
        /// <param name="move">The move that was executed</param>
        void OnMoveMade(Move move);

        /// <summary>
        /// Called when the game is over
        /// </summary>
        /// <param name="result">The game result (win, draw, etc.)</param>
        /// <param name="message">Descriptive message about how the game ended</param>
        void OnGameOver(GameResult result, string message);

        /// <summary>
        /// Called when a player is in check
        /// </summary>
        /// <param name="playerInCheck">The player who is in check</param>
        void OnCheck(Player playerInCheck);

        /// <summary>
        /// Called when the turn changes from one player to the other
        /// </summary>
        /// <param name="newPlayer">The player whose turn it now is</param>
        void OnTurnChanged(Player newPlayer);
    }
} 