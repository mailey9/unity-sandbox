using System;
using System.Collections.Generic;

namespace minorlife
{
    //TODO(용택): 어떻게 채워 넣을 것이냐..?
    //NOTE(용택): BFS 를 응용하는 방법이 있겠다. ... 구체적인 건 떠올려보자.

    public class Room
    {
        #region Properties: Coord Utilities
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

        public RoomCoord RectFilterCentroid
        {
            get
            {
                RoomRect rectFilter = RectFilter;
                RoomCoord centroid = (rectFilter.BottomRight - rectFilter.TopLeft) * 0.5f;
                return centroid;
            }
        }
        #endregion
        
        public int RectCount { get { return _roomRects.Count; } }
        public int Id { get; private set; }
        
        private static int _id = 0;
        private List<RoomRect> _roomRects = null;

        public Room(int capacity=0)
        {
            Id = _id++;
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

        public float GetEuclidDistanceSq(Room other)
        {
            RoomCoord diffCentroid = RectFilterCentroid - other.RectFilterCentroid;
            return MathF.Pow(diffCentroid.row, 2.0f) + MathF.Pow(diffCentroid.col, 2.0f);
        }
        public static float GetEuclidDistanceSq(Room a, Room b)
        {
            return a.GetEuclidDistanceSq(b);
        }

        public static Room FindRoom(int id, List<Room> rooms)
        {
            //NOTE(용택): O(n) 으로 리스트에서 ID 를 찾아본다.
            Room found = null;
            foreach (Room room in rooms)
            {
                if (room.Id == id)
                {
                    found = room;
                    break;
                }
            }
            return found;
        }

        public static SortedDictionary<float, int> GetEuclidDistanceIdPairs(Room comparison, List<Room> roomsCompareTo)
        {
            //NOTE(용택): 대상 comparison 에 대한 <거리,ID> pair 를 리턴.
            bool containsRoomComparer = false;
            
            SortedDictionary<float, int> distanceIdPairs = new SortedDictionary<float, int>();
            foreach (Room room in roomsCompareTo)
            {
                if (room.Id == comparison.Id)
                {
                    containsRoomComparer = true;
                    continue;
                }

                float roughEuclidDistanceSq = comparison.GetEuclidDistanceSq(room);
                distanceIdPairs.Add(roughEuclidDistanceSq, room.Id);
            }
            
            if( containsRoomComparer == false ) return null;//NOTE(용택): 에러, 사용이 잘못되었다. roomsCompareTo 에 comparison 가 포함되어있지 않다.
            return distanceIdPairs;
        }

        public static Comparison<Room> RowMajorComparison = delegate (Room a, Room b)
        {
            return a.MinRow.CompareTo(b.MinRow);
        };
        public static Comparison<Room> ColumnMajorComparison = delegate (Room a, Room b)
        {
            return a.MinCol.CompareTo(b.MinCol);
        };
    }   
}