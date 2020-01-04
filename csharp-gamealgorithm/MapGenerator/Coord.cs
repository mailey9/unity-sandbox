using System;

//TODO(용택): (Coord) Unity 적용시 Vector2 로 대체
namespace minorlife
{
    public struct Coord
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
    }
}