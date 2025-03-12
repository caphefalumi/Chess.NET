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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Position other = (Position)obj;
            return File == other.File && Rank == other.Rank;
        }

        public override int GetHashCode()
        {
            return File.GetHashCode() ^ Rank.GetHashCode();
        }

        public static bool operator ==(Position a, Position b)
        {
            // If both are null, or both are same instance, return true
            if (ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false
            if (((object)a == null) || ((object)b == null))
                return false;

            // Return true if the fields match
            return a.File == b.File && a.Rank == b.Rank;
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            char fileLetter = (char)('a' + File);
            return $"{fileLetter}{8 - Rank}";
        }
    }
}