using System;

namespace minorlife
{
    public struct Rect : IEquatable<Rect>
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
                Coord coord;
                coord.row = (int)( (row + row+height-1) * 0.5 );
                coord.col = (int)( (col + col+width-1) * 0.5 );
                return coord;
            }
        }
        #endregion//Coord Utility Properties
        
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

        public bool IsEdge(int r, int c)
        {
            if (r == RowMin || r == RowMax || c == ColMin || c == ColMax)
                return true;
            else
                return false;
        }

        public static Rect Intersect(Rect a, Rect b)
        {
            //TODO(용택): 이게 문제다. a 와 b 를 어떤 방식으로든 sort 해두고 intersection 을 구해야한다.
            Rect intersectArea;
            intersectArea.row = Math.Max(a.row, b.row);
            intersectArea.col = Math.Max(a.col, b.col);
            intersectArea.width = Math.Min(a.ColMax, b.ColMax) + 1 - intersectArea.col;
            intersectArea.height = Math.Min(a.RowMax, b.RowMax) + 1 - intersectArea.row;
            return intersectArea;
        }

        public static Comparison<Rect> RowColumnWidthHeightComparison = delegate (Rect a, Rect b)
        {
            int rowCompare = a.row.CompareTo(b.row);
            if (rowCompare == 0)
            {
                int colCompare = a.col.CompareTo(b.col);
                if (colCompare == 0)
                {
                    int widthCompare = a.width.CompareTo(b.width);
                    return widthCompare == 0 ? a.height.CompareTo(b.height) : widthCompare;
                }
                return colCompare;
            }
            return rowCompare;
        };

        public override string ToString()
        {
            return "{r:" + row + ", c:" + col + ", w:" + width + ", h:" + height + "}";
        }

        public override int GetHashCode()
        {
            return row.GetHashCode() + col.GetHashCode() - width.GetHashCode() + height.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Rect? rect = obj as Rect?;
            if (rect == null) return false;
            return Equals(rect);
        }
        public bool Equals(Rect other)
        {
            if (row != other.row || col != other.col || width != other.width || height != other.height)
                return false;
            else
                return true;
        }
        public static bool operator==(Rect a, Rect b)
        {
            return a.Equals(b);
        }
        public static bool operator!=(Rect a, Rect b)
        {
            return !a.Equals(b);
        }
    }
}