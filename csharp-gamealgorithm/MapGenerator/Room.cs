using System;
using System.Collections.Generic;

namespace minorlife
{
    public class Room : IEquatable<Room>
    {
        public enum MergeDirection
        {
            None, Top, Bottom, Left, Right
        }

        #region Properties: Coord Utilities
        public int RowMin
        {
            get
            {
                int minRow = int.MaxValue;
                foreach (var rect in _rects)
                {
                    minRow = Math.Min(minRow, rect.RowMin);
                }
                return minRow;
            }
        }
        public int RowMax
        {
            get
            {
                int maxRow = int.MinValue;
                foreach (var rect in _rects)
                {
                    maxRow = Math.Max(maxRow, rect.RowMax);
                }
                return maxRow;
            }
        }
        public int ColMin
        {
            get
            {
                int minCol = int.MaxValue;
                foreach (var rect in _rects)
                {
                    minCol = Math.Min(minCol, rect.ColMin);
                }
                return minCol;
            }
        }
        public int ColMax
        {
            get
            {
                int maxCol = int.MinValue;
                foreach (var rect in _rects)
                {
                    maxCol = Math.Max(maxCol, rect.ColMax);
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
                rect.row = RowMin;
                rect.col = ColMin;
                rect.width = ColMax - ColMin + 1;
                rect.height = RowMax - RowMin + 1;
                return rect;
            }
        }
        #endregion

        public int RectCount { get { return _rects.Count; } }
        public int Id { get; private set; }

        private static int _id = 0;
        private List<Rect> _rects = null;

        public Room(int capacity = 0)
        {
            Id = _id++;
            _rects = new List<Rect>(capacity);
        }
        public static Room FindRoom(List<Room> rooms, int id)
        {
            //NOTE(용택): (FindRoom) LinearSearch
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
            _rects.Sort(Rect.RowColumnWidthHeightComparison);

            //TODO(용택): (Concave Hull) Rect 가 추가될 때마다 좌표를 계산해두도록 한다.
            //NOTE(용택): (Concave Hull) 다소 무겁지만 미리해둔다.
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
        public Rect GetRandomRect()
        {
            int random = Rand.Range(0, _rects.Count);
            return GetRect(random);
        }
        public int[] GetColumns(int row)
        {
            //NOTE(용택): HashSet 생성자에 capacity 를 받지 않는다.
            //NOTE(용택): .NET Standard 2.0 or Unity only?
            var columnSet = new HashSet<int>();

            foreach (Rect rect in _rects)
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
            var rowSet = new HashSet<int>();

            foreach (Rect rect in _rects)
            {
                int[] resultRows = rect.GetRows(column);
                if (resultRows != null)
                    rowSet.UnionWith(resultRows);
            }

            int[] rows = new int[rowSet.Count];
            rowSet.CopyTo(rows);//NOTE(용택): 코드스멜인가..?
            return rows;
        }
        public bool Contains(int r, int c)
        {
            foreach (var rect in _rects)
            {
                if (rect.Contains(r, c) == true)
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
            Coord diff = RectFilter.Center - other.RectFilter.Center;
            return Math.Abs(diff.row) + Math.Abs(diff.col);
        }
        public static int CalculateManhattanDistance(Room a, Room b)
        {
            return a.CalculateManhattanDistance(b);
        }
        public static bool CanMerge(Room a, Room b, ref MergeDirection mergeDir)
        {
            mergeDir = MergeDirection.None;

            Rect filter = a.RectFilter;
            filter.row = (a.RowMin == 0) ? 0 : filter.row - 1;
            filter.col = (a.ColMin == 0) ? 0 : filter.col - 1;
            filter.width = (a.RowMin == 0) ? filter.width + 1 : filter.width + 2; ;
            filter.height = (a.ColMin == 0) ? filter.height + 1 : filter.height + 2;

            //NOTE(용택): 한 칸을 키운 RectFilter 와 대상이 교차하는지 살펴본다. 이 과정에 실패하면 이미 걸러진다.
            if (filter.HasIntersection(b.RectFilter) == false)
                return false;

            //NOTE(용택): 한 칸을 키운 좌표들과 대상이 교차하는지 살펴본다. 4방향 전수좌표를 검사한다.
            int topRow = (a.RowMin == 0) ? a.RowMin : a.RowMin - 1;
            int[] topColumns = a.GetColumns(a.RowMin);
            for (int i = 0; i < topColumns.Length; ++i)
            {
                if (b.Contains(topRow, topColumns[i]) == true)
                {
                    mergeDir = MergeDirection.Top;
                    return true;
                }
            }

            int bottomRow = a.RowMax + 1;
            int[] bottomColumns = a.GetColumns(a.RowMax);
            for (int i = 0; i < bottomColumns.Length; ++i)
            {
                if (b.Contains(bottomRow, bottomColumns[i]) == true)
                {
                    mergeDir = MergeDirection.Bottom;
                    return true;
                }
            }

            int leftColumn = (a.ColMin == 0) ? a.ColMin : a.ColMin - 1;
            int[] leftRows = a.GetRows(a.ColMin);
            for (int i = 0; i < leftRows.Length; ++i)
            {
                if (b.Contains(leftRows[i], leftColumn) == true)
                {
                    mergeDir = MergeDirection.Left;
                    return true;
                }
            }

            int rightColumn = a.ColMax + 1;
            int[] rightRows = a.GetRows(a.ColMax);
            for (int i = 0; i < rightRows.Length; ++i)
            {
                if (b.Contains(rightRows[i], rightColumn) == true)
                {
                    mergeDir = MergeDirection.Right;
                    return true;
                }
            }

            return false;
        }

        public static Comparison<Room> IdComparison = delegate (Room a, Room b)
        {
            return a.Id.CompareTo(b.Id);
        };

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Room other = obj as Room;
            return other == null ? false : Equals(other);
        }
        public bool Equals(Room other)
        {
            return Id == other.Id;
        }

        public List<Coord> GetHullCoords(Map.Tile[,] tileMap)
        {
            HashSet<Coord> samplingCoordsSet = new HashSet<Coord>();
            samplingCoordsSet.UnionWith(GetTopCoords());
            samplingCoordsSet.UnionWith(GetBottomCoords());
            samplingCoordsSet.UnionWith(GetLeftCoords());
            samplingCoordsSet.UnionWith(GetRightCoords());

            List<Coord> hullCoords = new List<Coord>(samplingCoordsSet.Count);

            //  HashSet 중 edge 에 해당하는 좌표만 모은다.
            //      edge 에 해당하는지는 어떻게 아는가?
            //      row == 0 이거나 col == 0 인 경우 edge 이다.
            //      vectorTable 에 돌린 경우 empty 에 인접하는 좌표면 edge 이다.
            //  모아서 리턴

            Coord[] sampleVectors = new Coord[8];
            //  0   1   2
            //  3 smplP 4
            //  5   6   7
            sampleVectors[0].row = -1; sampleVectors[0].col = -1;
            sampleVectors[1].row = -1; sampleVectors[1].col = 0;
            sampleVectors[2].row = -1; sampleVectors[2].col = +1;
            sampleVectors[3].row = 0; sampleVectors[3].col = -1;
            sampleVectors[4].row = 0; sampleVectors[4].col = +1;
            sampleVectors[5].row = +1; sampleVectors[5].col = -1;
            sampleVectors[6].row = +1; sampleVectors[6].col = 0;
            sampleVectors[7].row = +1; sampleVectors[7].col = +1;

            System.Console.WriteLine(tileMap[7, 18]);

            foreach (var sample in samplingCoordsSet)
            {
                if (sample.row == 8 && sample.col == 19)
                {
                    System.Console.WriteLine("break;");
                }
                if (sample.row == 0 || sample.col == 0)
                {
                    hullCoords.Add(sample);
                }
                else
                {
                    foreach (var v in sampleVectors)
                    {
                        Coord samplePoint = sample;
                        samplePoint.row += v.row;
                        samplePoint.col += v.col;

                        if (tileMap[samplePoint.row, samplePoint.col] == Map.Tile.Empty)
                        {
                            var tile_v = tileMap[samplePoint.row, samplePoint.col];
                            //System.Console.WriteLine(tile_v);
                            hullCoords.Add(sample);
                            break;
                        }
                    }
                }
            }

            return hullCoords;
        }

        private HashSet<Coord> GetTopCoords()
        {
            var topCoordsSet = new HashSet<Coord>();
            for (int i = 0; i < _rects.Count; ++i)
            {
                int minRow = _rects[i].RowMin;
                int[] columns = _rects[i].GetColumns(minRow);
                for (int j = 0; j < columns.Length; ++j)
                {
                    Coord coord;
                    coord.row = minRow;
                    coord.col = columns[j];
                    topCoordsSet.Add(coord);
                }
            }
            return topCoordsSet;
        }
        private HashSet<Coord> GetBottomCoords()
        {
            var bottomCoordsSet = new HashSet<Coord>();
            for (int i = 0; i < _rects.Count; ++i)
            {
                int maxRow = _rects[i].RowMax;
                int[] bottomColumns = _rects[i].GetColumns(maxRow);
                for (int j = 0; j < bottomColumns.Length; ++j)
                {
                    Coord coord;
                    coord.row = maxRow;
                    coord.col = bottomColumns[j];
                    bottomCoordsSet.Add(coord);
                }
            }
            return bottomCoordsSet;
        }
        private HashSet<Coord> GetLeftCoords()
        {
            var leftCoordsSet = new HashSet<Coord>();
            for (int i = 0; i < _rects.Count; ++i)
            {
                int minCol = _rects[i].ColMin;
                int[] leftRows = _rects[i].GetRows(minCol);
                for (int j = 0; j < leftRows.Length; ++j)
                {
                    Coord coord;
                    coord.row = leftRows[j];
                    coord.col = minCol;
                    leftCoordsSet.Add(coord);
                }
            }
            return leftCoordsSet;
        }
        private HashSet<Coord> GetRightCoords()
        {
            var rightCoordsSet = new HashSet<Coord>();
            for (int i = 0; i < _rects.Count; ++i)
            {
                int maxCol = _rects[i].ColMax;
                int[] rightRows = _rects[i].GetRows(maxCol);
                for (int j = 0; j < rightRows.Length; ++j)
                {
                    Coord coord;
                    coord.row = rightRows[j];
                    coord.col = maxCol;
                    rightCoordsSet.Add(coord);
                }
            }
            return rightCoordsSet;
        }
    }
}