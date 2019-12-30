using System;
using System.Collections.Generic;

namespace minorlife
{
    //TODO(용택): 어떻게 채워 넣을 것이냐..?
    //NOTE(용택): BFS 를 응용하는 방법이 있겠다. ... 구체적인 건 떠올려보자.

    public class Room
    {
        #region Properties: Min/Max of RoomRect
        public int MinRow
        {
            get
            {
                int minRow = int.MaxValue;
                foreach(var roomRect in _roomRects)
                {
                    minRow = Math.Min(minRow, roomRect.MinRow);
                }
                return minRow;
            }
        }
        public int MaxRow
        {
            get
            {
                int maxRow = int.MinValue;
                foreach(var roomRect in _roomRects)
                {
                    maxRow = Math.Max(maxRow, roomRect.MaxRow);
                }
                return maxRow;
            }
        }
        public int MinCol
        {
            get
            {
                int minCol = int.MaxValue;
                foreach(var roomRect in _roomRects)
                {
                    minCol = Math.Min(minCol, roomRect.MinCol);
                }
                return minCol;
            }
        }
        public int MaxCol
        {
            get
            {
                int maxCol = int.MinValue;
                foreach(var roomRect in _roomRects)
                {
                    maxCol = Math.Max(maxCol, roomRect.MaxCol);
                }
                return maxCol;
            }
        }
        public RoomRect RectFilter
        {
            get
            {
                RoomRect rect;
                rect.row = MinRow;
                rect.col = MinCol;
                rect.width = MaxRow + 1;
                rect.height = MaxCol + 1;
                return rect;
            }
        }
        #endregion
        #region Properties
        public int RectCount { get { return _roomRects.Count; } }
        #endregion

        private List<RoomRect> _roomRects = null;

        public Room(int capacity)
        {
            _roomRects = new List<RoomRect>(capacity);
        }

        public void Append(RoomRect roomRect)
        {
            _roomRects.Add(roomRect);
        }

        public RoomRect GetRect(int index)
        {
            return _roomRects[index];
        }

        public bool Contains(RoomCoord roomCoord)
        {
            foreach(var roomRect in _roomRects)
            {
                if (roomRect.Contains(roomCoord) == true)
                    return true;
            }

            return false;
        }

        public bool HasIntersection(RoomRect otherRect)
        {
            if (RectFilter.HasIntersection(otherRect) == false)
            {
                return false;
            }
            
            foreach (var room in _roomRects)
            {
                if (room.HasIntersection(otherRect) == true)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasIntersection(Room other)
        {
            if (RectFilter.HasIntersection(other.RectFilter) == false)
            {
                return false;
            }

            foreach (var thisRoom in _roomRects)
            {
                foreach (var otherRoom in other._roomRects)
                {
                    if (thisRoom.HasIntersection(otherRoom) == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public static bool HasIntersection(Room a, Room b)
        {
            return a.HasIntersection(b);
        }
    }   
}