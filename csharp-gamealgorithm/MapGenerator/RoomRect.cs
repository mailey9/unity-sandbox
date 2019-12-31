using System.Collections.Generic;

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
        public int MinRow { get{ return row; } }
        public int MaxRow { get{ return row+height-1; } } 
        public int MinCol { get{ return col; } }
        public int MaxCol { get{ return col+width-1; } }
        
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

        public bool Contains(RoomCoord roomCoord)
        {
            if (roomCoord.row < row || roomCoord.col < col ||
                roomCoord.row >= row + height || roomCoord.col >= col + width)
            {
                return false;
            }

            return true;
        }

        public HashSet<int> GetColumns(int row)
        {
            var columns = new HashSet<int>(width);
            if (MinRow <= row && row <= MaxRow)
            {
                for (int r = MinCol; r <= MaxCol; ++r)
                    columns.Add(r);
            }
            return columns;
        }
        public HashSet<int> GetRows(int column)
        {
            var rows = new HashSet<int>(height);
            if (MinCol <= column && column <= MaxCol)
            {
                for (int r = MinRow; r <= MaxRow; ++r)
                    rows.Add(r);
            }
            return rows;
        }

        public static bool Contains(RoomRect roomRect, RoomCoord roomCoord)
        {
            return roomRect.Contains(roomCoord);
        }

        public bool HasIntersection(RoomRect other)
        {
            //REF(용택): https://stackoverflow.com/questions/2752349/fast-rectangle-to-rectangle-intersection
            if (other.row < row + height &&
                other.col < col + width &&
                row < other.row + other.height &&
                col < other.col + other.width)
            {
                return true;
            }
            else
            {
                return false;    
            }
        }

        public static bool HasIntersection(RoomRect a, RoomRect b)
        {
            return a.HasIntersection(b);
        }

        public override string ToString()
        {
            return "{r:" + row + ", c:" + col + ", w:" + width + ", h:" + height + "}";
        }
    }
}