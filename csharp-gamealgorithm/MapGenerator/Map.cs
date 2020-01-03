using System;
using System.Text;
using System.Collections.Generic;

namespace minorlife
{
    public class Map
    {
        public int Width  { get{ return _map[0].Count; } }
        public int Height { get{ return _map.Count; } }

        //TODO(용택): 메모리레이아웃 고려, int[f,r,c] 나 linear 를 쓴다..
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
                    Rect rect = room.GetRect(i);

                    if (rect.Validate(this) == false)
                    {
                        return false;
                    }

                    for (int r = rect.row; r < rect.row + rect.height; ++r)
                    {
                        for (int c = rect.col; c < rect.col + rect.width; ++c)
                        {
                            _map[r][c] = 1;
                        }
                    }
                }
            }

            return true;
        }
        public void DEBUG_Apply(List<Corridor> corridors)
        {
            foreach(var corridor in corridors)
            {
                for (int i = 0; i < corridor.Count; ++i)
                {
                    Coord coord = corridor.GetCoord(i);
                    _map[coord.row][coord.col] = 2;
                }
            }
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
                    switch (i)
                    {
                        case 0: stringBuilder.Append( ". " ); break;
                        case 1: stringBuilder.Append( "# " ); break;
                        case 2: stringBuilder.Append( "_ " ); break;
                    }
                    //stringBuilder.Append( i==0 ? ". " : "# " );
                }
                stringBuilder.Append( "\n" );
            }

            return stringBuilder.ToString();
        }
    }
}