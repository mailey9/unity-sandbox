using System;
using System.Collections.Generic;
//TODO(용택): (Coord) Unity 적용시 Vector2 로 대체
namespace minorlife
{
    public struct Coord : IEquatable<Coord>, IComparer<Coord>
    {
        public int row;
        public int col;

        public static Coord operator-(Coord a, Coord b)
        {
            Coord ret;
            ret.row = a.row - b.row;
            ret.col = a.col - b.col;
            return ret;
        }

        public static Coord operator*(Coord a, float f)
        {
            Coord ret;
            ret.row = (int)(a.row * f);
            ret.col = (int)(a.col * f);
            return ret;
        }

        public static int CalculateManhattanDistance(Coord a, Coord b)
        {
            Coord diff = a - b;
            return Math.Abs(diff.row) + Math.Abs(diff.col);
        }

        public override int GetHashCode()
        {
            return 42 * row.GetHashCode() + 33 * col.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Coord? coord = obj as Coord?;
            return coord == null ? false : Equals(coord);
        }
        public bool Equals(Coord other)
        {
            if (row == other.row && col == other.col)
                return true;
            else
                return false;
        }
        public static bool operator==(Coord a, Coord b)
        {
            return a.Equals(b);
        }
        public static bool operator!=(Coord a, Coord b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return "r:" + row + ", c:" + col;
        }

        public int Compare(Coord a, Coord b)
        {
            int rowCompare = a.row.CompareTo(b.row);
            if (rowCompare != 0)
                return rowCompare;
            else
                return a.col.CompareTo(b.col);
        }

        public static Comparison<Coord> RowColumnComparison = delegate (Coord a, Coord b)
        {
            int rowCompare = a.row.CompareTo(b.row);
            if (rowCompare != 0)
                return rowCompare;
            else
                return a.col.CompareTo(b.col);
        };
        public static Comparison<Coord> ColumnRowComparison = delegate (Coord a, Coord b)
        {
            int columnCompare = a.col.CompareTo(b.col);
            if (columnCompare != 0)
                return columnCompare;
            else
                return a.row.CompareTo(b.row);
        };
    }
}