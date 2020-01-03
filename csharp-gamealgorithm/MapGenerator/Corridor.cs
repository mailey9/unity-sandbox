//TODO(용택): (Corridor) 각각의 방 Room 을 연결하는 통로를 건설한다.

using System;
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