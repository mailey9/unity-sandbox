using System;
using System.Collections.Generic;

namespace minorlife
{
    public class Room : IEquatable<Room>
    {
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
            //_rects.Sort(Rect.RowColumnWidthHeightComparison);
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

        public int CalculateRoughManhattanDistance(Room other)
        {
            Coord diff = RectFilter.Center - other.RectFilter.Center;
            return Math.Abs(diff.row) + Math.Abs(diff.col);
        }
        public static int CalculateRoughManhattanDistance(Room a, Room b)
        {
            return a.CalculateRoughManhattanDistance(b);
        }
        public static bool CalculateDetailManhattanDistance(List<Room> rooms, int srcRoomId, int destRoomId, ref Rect srcRect, ref Rect destRect, out Coord diff)
        {
            diff.row = diff.col = int.MaxValue / 2;
            //srcRect.row = srcRect.col = srcRect.width = srcRect.height = int.MaxValue;
            //destRect.row = destRect.col = destRect.width = destRect.height = int.MaxValue;

            Room srcRoom = FindRoom(rooms, srcRoomId);
            Room destRoom= FindRoom(rooms, destRoomId);

            foreach (var a in srcRoom._rects)
            {
                foreach (var b in destRoom._rects)
                {
                    Coord result = b.Center - a.Center;

                    int sumOfDiff = Math.Abs(diff.row) + Math.Abs(diff.col);
                    int sumOfResult = Math.Abs(result.row) + Math.Abs(result.col);
                    int minSum = Math.Min(sumOfDiff, sumOfResult);

                    if (minSum == sumOfResult)
                    {
                        diff.row = Math.Abs(result.row);
                        diff.col = Math.Abs(result.col);
                        srcRect = a;
                        destRect = b;
                    }
                }
            }

            if (diff.row == int.MaxValue/2 || diff.col == int.MaxValue/2)
                return false;
            else
                return true;
        }
        public static bool CanMerge(Room a, Room b)
        {
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
                    return true;
                }
            }

            int bottomRow = a.RowMax + 1;
            int[] bottomColumns = a.GetColumns(a.RowMax);
            for (int i = 0; i < bottomColumns.Length; ++i)
            {
                if (b.Contains(bottomRow, bottomColumns[i]) == true)
                {
                    return true;
                }
            }

            int leftColumn = (a.ColMin == 0) ? a.ColMin : a.ColMin - 1;
            int[] leftRows = a.GetRows(a.ColMin);
            for (int i = 0; i < leftRows.Length; ++i)
            {
                if (b.Contains(leftRows[i], leftColumn) == true)
                {
                    return true;
                }
            }

            int rightColumn = a.ColMax + 1;
            int[] rightRows = a.GetRows(a.ColMax);
            for (int i = 0; i < rightRows.Length; ++i)
            {
                if (b.Contains(rightRows[i], rightColumn) == true)
                {
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

            int tilemapRowMax = tileMap.GetLength(0);
            int tilemapColumnMax = tileMap.GetLength(1);
            foreach (var sample in samplingCoordsSet)
            {
                if (sample.row == 0 || sample.col == 0 || sample.row == tilemapRowMax || sample.col == tilemapColumnMax)
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
                            hullCoords.Add(sample);
                            break;
                        }
                    }
                }//if (samplePt is boundary) or (not)
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
        public bool CalculateDoorCandidates(Map.Tile[,] tileMap, int reservedBreadth, out List<Coord> columnCandidates, out List<Coord> rowCandidates)
        {
            List<Coord> hulls = GetHullCoords(tileMap);

            columnCandidates = new List<Coord>(hulls.Count);
            rowCandidates = new List<Coord>(hulls.Count);

            //NOTE(용택): w, h 만큼의 RoomWall 이 존재하는지 Sample 한다.
            bool SampleColumns(Coord coord, int w)
            {
                int positiveOffset = w / 2;
                int negativeOffset = -1 * (w - positiveOffset - 1);
                for (int i = negativeOffset; i <= positiveOffset; ++i)
                {
                    //TODO(용택): narrow height 를 가진 틈은 candidate 가 되어서는 안된다.
                    if (coord.col + i < 0)
                        return false;
                    if (coord.col + i >= tileMap.GetLength(1))
                        return false;
                    if (tileMap[coord.row, coord.col + i] != Map.Tile.RoomWall)
                        return false;
                }
                return true;
            }
            bool SampleRows(Coord coord, int h)
            {
                int positiveOffset = h / 2;
                int negativeOffset = -1 * (h - positiveOffset - 1);
                for (int i = negativeOffset; i <= positiveOffset; ++i)
                {
                    //TODO(용택): narrow width 를 가진 틈은 candidate 가 되어서는 안된다.
                    if (coord.row + i < 0)
                        return false;
                    if (coord.row + i >= tileMap.GetLength(0))
                        return false;
                    if (tileMap[coord.row + i, coord.col] != Map.Tile.RoomWall)
                        return false;
                }
                return true;
            }

            foreach (Coord coord in hulls)
            {
                if (SampleColumns(coord, reservedBreadth) == true)
                    columnCandidates.Add(coord);
                if (SampleRows(coord, reservedBreadth) == true)
                    rowCandidates.Add(coord);
            }

            if (columnCandidates.Count == 0 && rowCandidates.Count == 0)
                return false;
            else
                return true;
        }
    }
}