using System;
using System.Text;
using System.Collections.Generic;

namespace minorlife
{
    public class Map
    {
        public int Width  { get{ return _map[0].Count; } }
        public int Height { get{ return _map.Count; } }

        private List<List<int>>   _map    = null;   //TODO(용택): 이걸 층(Floor)별로 다시 2d array 를 보관하는 걸 생각해본다. arr[level][y][x]; --int 는 struct Tile{} 로 대체.
        private List<List<Room>>  _floors = null;   //TODO(용택): 상동, 필요없을 수 있다.
        private List<List<Path>>  _paths  = null;   //TODO(용택): 상동, 필요없을 수 있다.

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

        public bool Apply(List<Room> rooms)
        {
            foreach(Room room in rooms)
            {
                for (int i = 0; i < room.RectCount; ++i)
                {
                    RoomRect roomRect = room.GetRect(i);

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
            }

            return true;
        }

        public override string ToString()
        {
            int capa = _map.Count;
            foreach(var rows in _map)
            {
                capa += rows.Count;
            }
            capa *= 2;
            capa += _map.Count;

            StringBuilder stringBuilder = new StringBuilder(capa);
            foreach(var rows in _map)
            {
                foreach(int i in rows)
                {
                    stringBuilder.Append( i==0 ? ". " : "# " );
                }
                stringBuilder.Append( "\n" );
            }

            return stringBuilder.ToString();
        }
    }
}