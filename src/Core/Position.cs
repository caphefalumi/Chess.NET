using SplashKitSDK;
using Chess.Pieces;
using Chess.Moves;
using Chess.Interfaces;
using Chess.UI.Drawing;

using Chess.UI.States;
using Chess.Networking;
namespace Chess.Core
{
    public class Position
    {
        private int _file;
        private int _rank;

        public int File 
        { 
            get => _file; 
            set => _file = value; 
        }
        
        public int Rank 
        { 
            get => _rank; 
            set => _rank = value; 
        }

        public Position(int file, int rank)
        {
            _file = file;
            _rank = rank;
        }

        public Position(string notation)
        {
            char fileChar = notation[0];
            char rankChar = notation[1];
            
            // Convert file character (a-h) to index (0-7)
            _file = char.ToLower(fileChar) - 'a';
            
            // Convert rank character (1-8) to index (0-7), with 0 at the top
            _rank = '8' - rankChar;
        }

        public int X
        {
            get => _file * 80;
        }
        public int Y
        {
            get => _rank * 80;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Position other = (Position)obj;
            return _file == other._file && _rank == other._rank;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_file, _rank);
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
            char fileLetter = (char)('a' + _file);
            return $"{fileLetter}{8 - _rank}";
        }
    }
}
