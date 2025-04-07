namespace Chess
{
    public interface IBot
    {
        Task<string> GetBestMoveAsync(string fen, int timeLimit = 1000);
        Task<bool> IsAvailableAsync();
    }
} 