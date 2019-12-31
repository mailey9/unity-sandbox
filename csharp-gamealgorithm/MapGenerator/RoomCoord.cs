namespace minorlife
{
    public struct RoomCoord
    {
        public int row;
        public int col;

        public static RoomCoord operator-(RoomCoord a, RoomCoord b)
        {
            RoomCoord ret;
            ret.row = a.row - b.row;
            ret.col = a.col - b.col;
            return ret;
        }

        public static RoomCoord operator*(RoomCoord a, float f)
        {
            RoomCoord ret;
            ret.row = (int)(a.row * f);
            ret.col = (int)(a.col * f);
            return ret;
        }
    }
}