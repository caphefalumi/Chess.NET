namespace Chess
{ 
    internal class Program
    {
        static void Main()
        {
            Game game = new Game("Chess Game", 800, 700);
            game.Run();
        }
    }
}
