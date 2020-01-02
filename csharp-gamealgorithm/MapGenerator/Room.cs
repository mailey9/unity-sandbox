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
                foreach(var rect in _rects)
                {
                    minRow = Math.Min(minRow, rect.MinRow);
                }
                return minRow;
            }
        }
        public int MaxRow
        {
            get
            {
                int maxRow = int.MinValue;
                foreach(var rect in _rects)
                {
                    maxRow = Math.Max(maxRow, rect.MaxRow);
                }
                return maxRow;
            }
        }
        public int MinCol
        {
            get
            {
                int minCol = int.MaxValue;
                foreach(var rect in _rects)
                {
                    minCol = Math.Min(minCol, rect.MinCol);
                }
                return minCol;
            }
        }
        public int MaxCol
        {
            get
            {
                int maxCol = int.MinValue;
                foreach(var rect in _rects)
                {
                    maxCol = Math.Max(maxCol, rect.MaxCol);
                }
                return maxCol;
            }
        }
        public Rect RectFilter
        {
            //NOTE(용택): Room의 대략적인 Rect만 반환한다.
            //      RRRRRR          FFFFFFFFFFF
            //      RRRRRRRRRRR     F         F
            //         RRRRRRRR     F         F
            //         RRRRRRRR     FFFFFFFFFFF //이런느낌
            get
            {
                Rect rect;
                rect.row = MinRow;
                rect.col = MinCol;
                rect.width = MaxCol + 1;
                rect.height = MaxRow + 1;
                return rect;
            }
        }

        public Coord RectFilterCentroid
        {
            get
            {
                Rect rectFilter = RectFilter;
                Coord centroid = (rectFilter.BottomRight - rectFilter.TopLeft) * 0.5f;
                return centroid;
            }
        }
        #endregion
        
        public int RectCount { get { return _rects.Count; } }
        public int Id { get; private set; }
        
        private static int _id = 0;
        private List<Rect> _rects = null;

        public Room(int capacity=0)
        {
            Id = _id++;
            _rects = new List<Rect>(capacity);
        }
        public static Room FindRoom(int id, List<Room> rooms)
        {
            //NOTE(용택): O(n) 으로 리스트에서 ID 를 찾아본다.
            //TODO(용택): Id 로 Sort 해두고, BinSearch 로 찾을 수 있도록 한다.
            //  rooms.Sort(IdComparison);
            //  rooms.BinarySearch(/*IComparer<T>*/);
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
        public void Append(Rect rect)
        {
            _rects.Add(rect);
        }

        public void Append(Room room)
        {
            for (int i = 0; i < room.RectCount; ++i)
            {
                _rects.Add(room._rects[i]);
            }
        }
        public Rect GetRect(int index)
        {
            return _rects[index];
        }

        public int[] GetColumns(int row)
        {
            var columnSet = new HashSet<int>(MaxCol - MinCol + 1);

            foreach(Rect rect in _rects)
            {
                int[] resultColumns = rect.GetColumns(row);
                if (resultColumns != null)
                    columnSet.UnionWith(resultColumns);
            }
            
            int[] columns = new int[columnSet.Count];
            columnSet.CopyTo(columns);//NOTE(용택): 코드스멜인가..?
            return columns;
        }
        public int[] GetRows(int column)
        {
            var rowSet = new HashSet<int>(MaxRow - MinRow + 1);

            foreach(Rect rect in _rects)
            {
                int[] resultRows = rect.GetRows(column);
                if (resultRows != null)
                    rowSet.UnionWith(resultRows);
            }

            int[] rows = new int[rowSet.Count];
            rowSet.CopyTo(rows);//NOTE(용택): 코드스멜인가..?
            return rows;
        }

        public Coord[] GetTopCoords()
        {
            //TODO(용택): HashSet 에 모든 Rect 의 Top 을 적재한 뒤 반환
            int minRow = MinRow;
            int[] topColumns = GetColumns(minRow);

            Coord[] topCoords = new Coord[topColumns.Length];
            for(int i = 0; i < topCoords.Length; ++i)
            {
                topCoords[i].row = minRow;
                topCoords[i].col = topColumns[i];
            }
            return topCoords;
        }
        public Coord[] GetBottomCoords()
        {
            //TODO(용택): HashSet 에 모든 Rect 의 Bottom 을 적재한 뒤 반환
            int maxRow = MaxRow;
            int[] bottomColumns = GetColumns(maxRow);

            Coord[] bottomCoords = new Coord[bottomColumns.Length];
            for (int i = 0; i < bottomCoords.Length; ++i)
            {
                bottomCoords[i].row = maxRow;
                bottomCoords[i].col = bottomColumns[i];
            }
            return bottomCoords;
        }
        public Coord[] GetLeftCoords()
        {
            //TODO(용택): HashSet 에 모든 Rect 의 Left 를 적재한 뒤 반환
            int minCol = MinCol;
            int[] leftRows = GetRows(minCol);

            Coord[] leftCoords = new Coord[leftRows.Length];
            for (int i = 0; i < leftCoords.Length; ++i)
            {
                leftCoords[i].row = leftRows[i];
                leftCoords[i].col = MinCol;
            }
            return leftCoords;
        }
        public Coord[] GetRightCoords()
        {
            //TODO(용택): HashSet 에 모든 Rect 의 Right 를 적재한 뒤 반환
            int maxCol = MaxCol;
            int[] rightRows = GetRows(maxCol);

            Coord[] rightCoords = new Coord[rightRows.Length];
            for (int i = 0; i < rightCoords.Length; ++i)
            {
                rightCoords[i].row = rightRows[i];
                rightCoords[i].col = MaxCol;
            }
            return rightCoords;
        }

        public bool Contains(int r, int c)
        {
            foreach(var rect in _rects)
            {
                if (rect.Contains(r,c) == true)
                    return true;
            }
            return false;
        }

        public bool HasIntersection(Rect otherRect)
        {
            if (RectFilter.HasIntersection(otherRect) == false)
            {
                return false;
            }
            
            foreach (var room in _rects)
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

            foreach (var thisRoom in _rects)
            {
                foreach (var otherRoom in other._rects)
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

        public int CalculateManhattanDistance(Room other)
        {
            Coord diff = RectFilterCentroid - other.RectFilterCentroid;
            return Math.Abs(diff.row) + Math.Abs(diff.col);
        }
        public static int CalculateManhattanDistance(Room a, Room b)
        {
            return a.CalculateManhattanDistance(b);
        }
        public float GetEuclidDistanceSq(Room other)
        {
            Coord diffCentroid = RectFilterCentroid - other.RectFilterCentroid;
            return MathF.Pow(diffCentroid.row, 2.0f) + MathF.Pow(diffCentroid.col, 2.0f);
        }
        public static float GetEuclidDistanceSq(Room a, Room b)
        {
            return a.GetEuclidDistanceSq(b);
        }

        //TODO(용택): Obsolote 시킨다. Matrix 계산할 때 구할 것이고, Manhattan Distance 를 기본으로 한다.
        [ObsoleteAttribute()]
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
            Rect filter = a.RectFilter;
            filter.row = (filter.row == 0) ? filter.row : filter.row - 1;
            filter.col = (filter.col == 0) ? filter.col : filter.col - 1;
            filter.width += 1;
            filter.height += 1;

            //NOTE(용택): 한 칸을 키운 RectFilter 와 대상이 교차하는지 살펴본다. 이 과정에 실패하면 이미 걸러진다.
            if (filter.HasIntersection(b.RectFilter) == false)
                return false;

            //NOTE(용택): 한 칸을 키운 좌표들과 대상이 교차하는지 살펴본다. 4방향 전수좌표를 검사한다.
            //TODO(용택): 오버헤드가 큰 지 측정 필요.
            int   topRow     = (a.MinRow == 0) ? a.MinRow : a.MinRow - 1;
            int[] topColumns = a.GetColumns(a.MinRow);
            for(int i = 0; i < topColumns.Length; ++i)
            {
                if (b.Contains(topRow, topColumns[i]) == true) return true;
            }

            int   bottomRow     = a.MaxRow + 1;
            int[] bottomColumns = a.GetColumns(a.MaxRow);
            for(int i = 0; i < bottomColumns.Length; ++i)
            {
                if (b.Contains(bottomRow, bottomColumns[i]) == true) return true;
            }

            int   leftColumn = (a.MinCol == 0) ? a.MinCol : a.MinCol - 1;
            int[] leftRows   = a.GetRows(a.MinCol);
            for(int i = 0; i < leftRows.Length; ++i)
            {
                if (b.Contains(leftRows[i], leftColumn) == true) return true;
            }

            int   rightColumn = a.MaxCol + 1;
            int[] rightRows   = a.GetRows(a.MaxCol);
            for(int i = 0; i < rightRows.Length; ++i)
            {
                if (b.Contains(rightRows[i], rightColumn) == true) return true;
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