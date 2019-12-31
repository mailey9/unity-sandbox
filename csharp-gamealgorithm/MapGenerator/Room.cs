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
            //NOTE(용택): Room의 대략적인 Rect만 반환한다.
            //      RRRRRR          FFFFFFFFFFF
            //      RRRRRRRRRRR     F         F
            //         RRRRRRRR     F         F
            //         RRRRRRRR     FFFFFFFFFFF //이런느낌
            get
            {
                RoomRect rect;
                rect.row = MinRow;
                rect.col = MinCol;
                rect.width = MaxCol + 1;
                rect.height = MaxRow + 1;
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
        public static Room FindRoom(int id, List<Room> rooms)
        {
            //NOTE(용택): O(n) 으로 리스트에서 ID 를 찾아본다.
            //TODO(용택): Id 로 Sort 해두고, BinSearch 로 찾을 수 있도록 한다.
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
        public void Append(RoomRect roomRect)
        {
            _roomRects.Add(roomRect);
        }

        public void Append(Room room)
        {
            //TODO(용택): 찾아보고 고려, Enumerable 인터페이스가 필요한 듯.. foreach 등에도..
            //_roomRects.AddRange(room);

            for (int i = 0; i < room.RectCount; ++i)
            {
                _roomRects.Add(room._roomRects[i]);
            }
        }
        public RoomRect GetRect(int index)
        {
            return _roomRects[index];
        }

        public int[] GetColumns(int row)
        {
            var columnSet = new HashSet<int>(MaxCol - MinCol + 1);

            foreach(RoomRect rect in _roomRects)
            {
                columnSet.UnionWith(rect.GetColumns(row));
            }
            
            int[] columns = new int[columnSet.Count];
            columnSet.CopyTo(columns);
            return columns;
        }
        public int[] GetRows(int column)
        {
            var rowSet = new HashSet<int>(MaxRow - MinRow + 1);

            foreach(RoomRect rect in _roomRects)
            {
                rowSet.UnionWith(rect.GetRows(column));
            }

            int[] rows = new int[rowSet.Count];
            rowSet.CopyTo(rows);
            return rows;
        }

        public RoomCoord[] GetTopCoords()
        {
            int minRow = MinRow;
            int[] topColumns = GetColumns(minRow);

            RoomCoord[] topCoords = new RoomCoord[topColumns.Length];
            for(int i = 0; i < topCoords.Length; ++i)
            {
                topCoords[i].row = minRow;
                topCoords[i].col = topColumns[i];
            }
            return topCoords;
        }
        public RoomCoord[] GetBottomCoords()
        {
            int maxRow = MaxRow;
            int[] bottomColumns = GetColumns(maxRow);

            RoomCoord[] bottomCoords = new RoomCoord[bottomColumns.Length];
            for (int i = 0; i < bottomCoords.Length; ++i)
            {
                bottomCoords[i].row = maxRow;
                bottomCoords[i].col = bottomColumns[i];
            }
            return bottomCoords;
        }
        public RoomCoord[] GetLeftCoords()
        {
            int minCol = MinCol;
            int[] leftRows = GetRows(minCol);

            RoomCoord[] leftCoords = new RoomCoord[leftRows.Length];
            for (int i = 0; i < leftCoords.Length; ++i)
            {
                leftCoords[i].row = leftRows[i];
                leftCoords[i].col = MinCol;
            }
            return leftCoords;
        }
        public RoomCoord[] GetRightCoords()
        {
            int maxCol = MaxCol;
            int[] rightRows = GetRows(maxCol);

            RoomCoord[] rightCoords = new RoomCoord[rightRows.Length];
            for (int i = 0; i < rightCoords.Length; ++i)
            {
                rightCoords[i].row = rightRows[i];
                rightCoords[i].col = MaxCol;
            }
            return rightCoords;
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

        public static bool canMerge(Room a, Room b)
        {
            RoomRect filter = a.RectFilter;
            filter.row = (filter.row == 0) ? filter.row : filter.row - 1;
            filter.col = (filter.col == 0) ? filter.col : filter.col - 1;
            filter.width += 1;
            filter.height += 1;

            //NOTE(용택): 한 칸을 키운 RectFilter 와 대상이 교차하는지 살펴본다. 이 과정에 실패하면 이미 걸러진다.
            if (filter.HasIntersection(b.RectFilter) == false)
                return false;

            //TODO(용택): 오버헤드가 큰 지 측정 필요.
            RoomCoord[] aTop = a.GetTopCoords();
            for(int i = 0; i < aTop.Length; ++i)
            {
                aTop[i].row = (aTop[i].row == 0) ? aTop[i].row : aTop[i].row - 1;
                if (b.Contains(aTop[i]) == true) return true;
            }

            RoomCoord[] aBottom = a.GetBottomCoords();
            for(int i = 0; i < aBottom.Length; ++i)
            {
                aBottom[i].row += 1;
                if (b.Contains(aBottom[i]) == true) return true;
            }

            RoomCoord[] aLeft = a.GetLeftCoords();
            for(int i = 0; i < aLeft.Length; ++i)
            {
                aLeft[i].col = (aLeft[i].col == 0) ? aLeft[i].col : aLeft[i].col - 1;
                if (b.Contains(aLeft[i]) == true) return true;
            }

            RoomCoord[] aRight = a.GetRightCoords();
            for(int i = 0; i < aRight.Length; ++i)
            {
                aRight[i].col += 1;
                if (b.Contains(aRight[i]) == true) return true;
            }

            return false;
        }

        public static Comparison<Room> IdComparison = delegate (Room a, Room b)
        {
            return a.Id.CompareTo(b.Id);
        };
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