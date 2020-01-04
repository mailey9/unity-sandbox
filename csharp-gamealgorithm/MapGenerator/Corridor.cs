using System.Collections.Generic;

namespace minorlife
{
    public class Corridor
    {
        public int Id { get; private set; }
        public int Count { get { return _coords.Count; } }
        private static int _id = 0;
        List<Coord> _coords = null;

        public Corridor(List<Coord> coords)
        {
            Id = _id++;
            _coords = coords;
        }

        public Coord GetCoord(int i)
        {
            return _coords[i];
        }
    }
}