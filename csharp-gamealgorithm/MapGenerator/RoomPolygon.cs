using System;
using System.Collections.Generic;

namespace minorlife
{
    //NOTE(용택): Concave Polygon 이며 Rect 의 합집합으로 이루어진다.
    public class RoomPolygon
    {
        //TODO(용택): struct { List<T>; } 의 memory-layout 에 대한 조사 필요. class { std::vector<T>; } 와 같은 결과일 것 같기는 하다.
        List<RoomCoord> _coords;

        //TODO(용택): 다음 인터페이스의 선언
        //  -- void Append(RoomCoord roomCoord)     ==> CW Sort 를 해둔다. (혹은 CCW, whatever)
        //  -- bool Contains(RoomCoord roomCoord)   ==> 이후 coords 에서 중복제외.

        public RoomCoord TopLeftMost
        {
            get
            {
                RoomCoord roomCoord;
                roomCoord.row = int.MaxValue;
                roomCoord.col = int.MaxValue;
                foreach(RoomCoord coord in _coords)
                {
                    if (Math.Min(roomCoord.row, coord.row) == roomCoord.row &&
                        Math.Min(roomCoord.col, coord.col) == roomCoord.col)
                    {
                        roomCoord = coord;
                    }
                }
                return roomCoord;
            }
        }
        public RoomCoord BottomRightMost
        {
            get
            {
                RoomCoord roomCoord;
                roomCoord.row = int.MinValue;
                roomCoord.col = int.MinValue;
                foreach(RoomCoord coord in _coords)
                {
                    if (Math.Max(roomCoord.row, coord.row) == roomCoord.row &&
                        Math.Max(roomCoord.col, coord.col) == roomCoord.col)
                    {
                        roomCoord = coord;
                    }
                }
                return roomCoord;
            }
        }
    }
}