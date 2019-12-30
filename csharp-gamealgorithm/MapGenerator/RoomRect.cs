namespace minorlife
{
    public struct RoomRect
    {
        public int row;
        public int col;
        public int width;
        public int height;

        public RoomCoord TopLeft        { get{ RoomCoord coord; coord.row=row;          coord.col=col;         return coord; } }
        public RoomCoord BottomRight    { get{ RoomCoord coord; coord.row=row+height-1; coord.col=col+width-1; return coord; } }
        
        public bool Validate(Map map)
        {
            if (row < 0 || col < 0 || row + height > map.Height || col + width > map.Width)
            {
                return false;
            }

            if (width <= 0 || height <= 0)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return "{r:" + row + ", c:" + col + ", w:" + width + ", h:" + height + "}";
        }
    }
}