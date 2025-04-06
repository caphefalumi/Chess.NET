using System;

namespace Chess
{
    /// <summary>
    /// A simple observer that outputs game events to the console for debugging.
    /// This is a concrete Observer in the Observer design pattern that responds
    /// to game events by logging them to the console.
    /// </summary>
    public class ConsoleDebugObserver : IGameObserver
    {
        private bool _isEnabled;

        /// <summary>
        /// Creates a new instance of the ConsoleDebugObserver
        /// </summary>
        /// <param name="isEnabled">Whether the observer should log events immediately</param>
        public ConsoleDebugObserver(bool isEnabled = true)
        {
            _isEnabled = isEnabled;
        }

        /// <summary>
        /// Handles move events by logging the move details to the console
        /// </summary>
        /// <param name="move">The move that was executed</param>
        public void OnMoveMade(Move move)
        {
            if (!_isEnabled) return;
            Console.WriteLine($"MOVE: {move.MovedPiece.Color} {move.MovedPiece.Type} from {move.From} to {move.To}");
        }

        /// <summary>
        /// Handles game over events by logging the result to the console
        /// </summary>
        /// <param name="result">The game result</param>
        /// <param name="message">Descriptive message about how the game ended</param>
        public void OnGameOver(GameResult result, string message)
        {
            if (!_isEnabled) return;
            Console.WriteLine($"GAME OVER: {result} - {message}");
        }

        /// <summary>
        /// Handles check events by logging which player is in check
        /// </summary>
        /// <param name="playerInCheck">The player who is in check</param>
        public void OnCheck(Player playerInCheck)
        {
            if (!_isEnabled) return;
            Console.WriteLine($"CHECK: {playerInCheck} is in check");
        }

        /// <summary>
        /// Handles turn change events by logging whose turn it is
        /// </summary>
        /// <param name="newPlayer">The player whose turn it now is</param>
        public void OnTurnChanged(Player newPlayer)
        {
            if (!_isEnabled) return;
            Console.WriteLine($"TURN: {newPlayer}'s turn");
        }

        /// <summary>
        /// Enables console logging for this observer
        /// </summary>
        public void Enable()
        {
            _isEnabled = true;
        }

        /// <summary>
        /// Disables console logging for this observer
        /// </summary>
        public void Disable()
        {
            _isEnabled = false;
        }
    }
} 