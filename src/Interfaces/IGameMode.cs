using Chess;

using Chess.Core;
using Chess.Pieces;
using Chess.Moves;
public interface IVariantStrategy
{
    void StartGame(Game game, Board board);
}
