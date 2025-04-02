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

        public Position(string algebraicNotation)
        {
            char fileChar = algebraicNotation[0];
            char rankChar = algebraicNotation[1];
            
            // Convert file character (a-h) to index (0-7)
            File = char.ToLower(fileChar) - 'a';
            
            // Convert rank character (1-8) to index (0-7), with 0 at the top
            Rank = '8' - rankChar;
        }

        public int X
        {
            get => File * 80;
        }
        public int Y
        {
            get => Rank * 80;
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