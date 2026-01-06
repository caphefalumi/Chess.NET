using SplashKitSDK;

using Chess.Core;
using Chess.Pieces;
using Chess.Moves;
namespace Chess.Interfaces
{
    public interface IMove 
    {
       public MoveType Type { get; }
       public void Execute(Board board, bool isSimulation);
       public void Undo(Board board, bool isSimulation);
    }
}
