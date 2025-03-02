
namespace Chess
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Rank => Y % 80;
        public int File => X % 80;
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
