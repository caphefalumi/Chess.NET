namespace Chess
{
    public class Position
    {
        public int File { get; set; }
        public int Rank { get; set; }

        public Position(int file, int rank)
        {
            File = file;
            Rank = rank;
        }

        public int X
        {
            get => File * 80;
        }
        public int Y
        {
            get => Rank * 80;
        }

        public bool IsLightSquare()
        {
            if ((File + Rank) % 2 == 0)
            {
                return true;
            }
            return false;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Position other = (Position)obj;
            return File == other.File && Rank == other.Rank;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(File, Rank);
        }

        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            char fileLetter = (char)('a' + File);
            return $"{fileLetter}{8 - Rank}";
        }
    }
}