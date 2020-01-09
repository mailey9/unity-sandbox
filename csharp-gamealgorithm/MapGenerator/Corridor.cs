using System.Collections.Generic;

namespace minorlife
{
    public class Corridor
    {
        public int Id { get; private set; }
        public int Count { get { return _coords.Count; } }
        private static int _id = 0;
        List<Point> _coords = null;

        public Corridor(List<Point> coords)
        {
            Id = _id++;
            _coords = coords;
        }

        public Point GetPoint(int i)
        {
            return _coords[i];
        }
    }
}