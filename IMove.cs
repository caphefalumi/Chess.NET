namespace Chess
{
    public interface IMove 
    {
       public MoveType Type { get; }
       public void Execute(Board board);
       public void Undo(Board board);
    }
}