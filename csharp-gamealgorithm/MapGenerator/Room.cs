using System;
using System.Collections.Generic;

namespace minorlife
{
    public partial class Room : IEquatable<Room>
    {
        public int RectCount { get { return _rects.Count; } }
        public UInt64 Id { get; private set; }

        private static UInt64 _id = 0;
        private List<Rect> _rects = null;

        public Room(int capacity = 0)
        {
            Id = _id++;
            _rects = new List<Rect>(capacity);
        }

        public void Append(Rect rect)
        {
            _rects.Add(rect);
        }
        public void Append(Room room)
        {
            foreach (Rect rc in room._rects)
                Append(rc);
        }

        public Rect GetRect(int index)
        {
            return _rects[index];
        }

        public bool Contains(int x, int y)
        {
            foreach (var rect in _rects)
            {
                if (rect.Contains(x, y) == true)
                    return true;
            }
            return false;
        }

        public bool HasIntersection(Rect otherRect)
        {
            if (Filter.HasIntersection(otherRect) == false)
                return false;

            foreach (Rect rc in _rects)
            {
                if (rc.HasIntersection(otherRect) == true)
                    return true;
            }

            return false;
        }

        public static Room FindRoom(List<Room> rooms, UInt64 id)
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
        public static bool CanMerge(Room a, Room b)
        {
            Rect expandedFilter = a.Filter;
            expandedFilter.x      = (a.Filter.x == 0) ? 0                   : a.Filter.x - 1;
            expandedFilter.y      = (a.Filter.y == 0) ? 0                   : a.Filter.y - 1;
            expandedFilter.width  = (a.Filter.x == 0) ? a.Filter.width + 1  : a.Filter.width + 2; ;
            expandedFilter.height = (a.Filter.y == 0) ? a.Filter.height + 1 : a.Filter.height + 2;

            //NOTE(용택): 한 칸을 키운 Filter 와 대상이 교차하는지 살펴본다. 이 과정에 실패하면 이미 걸러진다.
            if (expandedFilter.HasIntersection(b.Filter) == false)
                return false;

            int numContact = 0;
            //NOTE(용택): 외곽은 벽이 될 것이기 때문에 최소한 3포인트는 포함 할 때만 합친다.
            //          따라서 Early Return 하지 않고 전수 검사한다.

            //Top of Room A
            numContact = 0;
            HashSet<Point> topPoints = a.GetTopPoints();
            foreach(Point top in topPoints)
            {
                if (b.Contains(top.x, top.y - 1) == true)
                    numContact += 1;
            }
            if (numContact > 2)
            {
                return true;
            }

            //Bottom of Room A
            numContact = 0;
            HashSet<Point> bottomPoints = a.GetBottomPoints();
            foreach (Point bottom in bottomPoints)
            {
                if (b.Contains(bottom.x, bottom.y + 1) == true)
                    numContact += 1;
            }
            if (numContact > 2)
            {
                return true;
            }

            //Left of Room A
            numContact = 0;
            HashSet<Point> leftPoints = a.GetLeftPoints();
            foreach (Point left in leftPoints)
            {
                if (b.Contains(left.x - 1, left.y) == true)
                    numContact += 1;
            }
            if (numContact > 2)
            {
                return true;
            }

            //Right of Room A
            numContact = 0;
            HashSet<Point> rightPoints = a.GetRightPoints();
            foreach (Point right in rightPoints)
            {
                if (b.Contains(right.x + 1, right.y) == true)
                    numContact += 1;
            }
            if (numContact > 2)
            {
                return true;
            }

            return false;
        }

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
        public override string ToString()
        {
            return "id:" + Id;
        }

        public static Comparison<Room> IdComparison = delegate (Room a, Room b)
        {
            return a.Id.CompareTo(b.Id);
        };
        
        //  HullPoints 중 상하 - 좌우 로 샘플링해본다.
        //  pos = w/2; neg = -1 * (w-pos-1);    //좌우
        //public bool CalculateDoorCandidates(Map.Tile[,] tileMap, int reservedBreadth, out List<Point> columnCandidates, out List<Point> rowCandidates)
    }
}