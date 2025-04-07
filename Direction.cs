namespace Chess
{
    public class Direction
    {
        public readonly static Direction Up = new Direction(-1, 0);
        public readonly static Direction Down = new Direction(1, 0);
        public readonly static Direction Left = new Direction(0, 1);
        public readonly static Direction Right = new Direction(0, -1);
        public readonly static Direction UpLeft = Up + Left;
        public readonly static Direction UpRight = Up + Right;
        public readonly static Direction DownLeft = Down + Left;
        public readonly static Direction DownRight = Down + Right;
        public int RankOffset { get; }
        public int FileOffset { get; }
        private Direction(int rankOffset, int fileOffset)
        {
            RankOffset = rankOffset;
            FileOffset = fileOffset;
        }
        public static Direction operator +(Direction dir1, Direction dir2)
        {
            return new Direction(dir1.RankOffset + dir2.RankOffset, dir1.FileOffset + dir2.FileOffset);
        }

        public static Position operator +(Position pos, Direction dir)
        {
            return new Position(pos.File + dir.FileOffset, pos.Rank + dir.RankOffset);
        }
        public static Position operator -(Position pos, Direction dir)
        {
            return new Position(pos.File - dir.FileOffset, pos.Rank - dir.RankOffset);
        }
        public static Direction FromOffsets(int rankOffset, int fileOffset)
        {
            return new Direction(rankOffset, fileOffset);
        }

    }
}
