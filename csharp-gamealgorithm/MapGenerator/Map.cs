using System;
using System.Collections.Generic;

namespace minorlife
{
    public class Map
    {
        public int Width  { get{ return _map[0].Count; } }
        public int Height { get{ return _map.Count; } }

        private List< List<int> >   _map;

        public Map(int width, int height)//Create2dArray(int row, int col)
        {
            var array2d = new List< List<int> >(height);
            for(int r = 0; r < height; ++r)
            {
                array2d.Add( new List<int>(new int[width]) );
                for(int c = 0; c < array2d[r].Count; ++c)
                {
                    array2d[r][c] = 0;
                }
            }

            _map = array2d;
        }

        public bool Apply(List<RoomRect> roomRects)
        {
            foreach(RoomRect roomRect in roomRects)
            {
                if (roomRect.Validate(this) == false)
                {
                    return false;
                }

                for (int r = roomRect.row; r < roomRect.row + roomRect.height; ++r)
                {
                    for (int c = roomRect.col; c < roomRect.col + roomRect.width; ++c)
                    {
                        _map[r][c] = 1;
                    }
                }
            }

            return true;
        }

        public override string ToString()
        {
            string ret = "";
            foreach(List<int> rows in _map)
            {
                foreach(int i in rows)
                {
                    ret += ( i==0 ? ". " : "# " );
                }
                ret += "\n";
            }
            return ret;
        }
    }
}