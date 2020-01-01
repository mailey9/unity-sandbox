//TODO(용택): 이후 Unity Coord 또는 Vec2 대체 고려
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
    }
}