using SplashKitSDK;

using Chess.Core;
using Chess.Pieces;
using Chess.Moves;
namespace Chess.Interfaces
{
    public interface IBot
    {
        Task<string> GetBestMoveAsync(string fen, int timeLimit = 1000);
        Task<bool> IsAvailableAsync();
    }
} 
