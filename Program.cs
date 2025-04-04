namespace Chess
{ 
    internal class Program
    {
        static void Main()
        {
            Game game = Game.GetInstance("Chess Game", 800, 700);
            game.Run();
        }
    }
}
