using System;
using System.Collections.Generic;

//NOTE(용택):   일단 취소.. 의도는 RoomRect 의 합집합을 Concave Polygon Points 로 관리하려는 것... 이었는데,
//              기본적으로 포인트만 주어졌을 때 Concave Polygon 을 이루는 가지수가 많다.
//              Rect 라서 Well-formed 라서 필터가 되기는 하나, Polygon.Contains( point ); 등의 메서드를 생각해보았을 때,
//              이런저런 Math 연산보다는 Rect.Contains( point ); 를 여러번 돌리는 것이 성능 상도 낫다.

/*

namespace minorlife
{
    //NOTE(용택): Concave Polygon 이며 Rect 의 합집합으로 이루어진다.
    public class RoomPolygon
    {
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

        List<RoomCoord> _coords = null;

        public RoomPolygon(int capacity=0)
        {
            _coords = new List<RoomCoord>(capacity);
        }

        public void Append(RoomCoord roomCoord)
        {

        }

        public bool Contains(RoomCoord roomCoord)
        {
            foreach(var coord in _coords)
            {
                if (coord.Equals(roomCoord) == true)
                {
                    return true;
                }
            }
            return false;
        }

        private void SortClockwise()
        {

            //_coords.OrderBy(x => Math.Atan2())
        }
    }
}

*/