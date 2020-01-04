//TODO(용택): 이후 Unity Rect(있던가?)로 대체 고려
namespace minorlife
{
    public struct Rect
    {
        public int row;
        public int col;
        public int width;
        public int height;

        #region Coord Utility Properties
        public int RowMin { get{ return row; } }
        public int RowMax { get{ return row+height-1; } } 
        public int ColMin { get{ return col; } }
        public int ColMax { get{ return col+width-1; } }
        public Coord TopLeft
        {
            get
            {
                Coord coord;
                coord.row=RowMin;
                coord.col=ColMin;
                return coord;
            }
        }
        public Coord BottomRight
        {
            get
            {
                Coord coord;
                coord.row=RowMax;
                coord.col=ColMax;
                return coord;
            }
        }
        public Coord TopRight
        {
            get
            {
                Coord coord;
                coord.row=RowMin;
                coord.col=ColMax;
                return coord;
            }
        }
        public Coord BottomLeft
        {
            get
            {
                Coord coord;
                coord.row=RowMax;
                coord.col=ColMin;
                return coord;
            }
        }
        public Coord Center
        {
            get
            {
                //Coord coord = (BottomRight - TopLeft) * 0.5f;
                Coord coord;
                coord.row = (int)( (row + row+height-1) * 0.5 );
                coord.col = (int)( (col + col+width-1) * 0.5 );
                return coord;
                //10~50             //80~90
                //  30              //  85
                //10~70
            }
        }
        #endregion//Coord Utility Properties
        
        //TODO(용택): Rect 의 Validation 은 Map 에서 진행해야 한다.
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

        public int[] GetColumns(int row)
        {
            int[] columns = null;
            if (RowMin <= row && row <= RowMax)
            {
                columns = new int[width];
                for (int c = 0; c < width; ++c)
                    columns[c] = ColMin + c;
            }
            return columns;
        }
        public int[] GetRows(int column)
        {
            int[] rows = null;
            if (ColMin <= column && column <= ColMax)
            {
                rows = new int[height];
                for (int r = 0; r < height; ++r)
                    rows[r] = RowMin + r;
            }
            return rows;
        }

        public bool Contains(int r, int c)
        {
            if (r < row || c < col || r >= row + height || c >= col + width)
            {
                return false;
            }
            return true;
        }
        public static bool Contains(Rect rect, int r, int c)
        {
            return rect.Contains(r, c);
        }

        public bool HasIntersection(Rect other)
        {
            //REF(용택): https://stackoverflow.com/questions/2752349/fast-rectangle-to-rectangle-intersection
            //return !(other.ColMin > ColMax || other.ColMax < ColMin || other.RowMin > RowMax || other.RowMax < RowMin);
            if (other.row < row + height && other.col < col + width &&
                row < other.row + other.height && col < other.col + other.width)
            {
                return true;
            }
            else
            {
                return false;    
            }
        }

        public static bool HasIntersection(Rect a, Rect b)
        {
            return a.HasIntersection(b);
        }

        public override string ToString()
        {
            return "{r:" + row + ", c:" + col + ", w:" + width + ", h:" + height + "}";
        }
    }
}