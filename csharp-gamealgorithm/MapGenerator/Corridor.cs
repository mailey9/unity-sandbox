using System;
using System.Collections.Generic;

namespace minorlife
{
    public class Corridor : IEquatable<Corridor>
    {
        //TODO(용택): (오브젝트 ID 발급) 이건 좀 제네릭하게 풀어갈 필요가 있을 것 같다. int면 충분하기도 하고.
        private static ulong _id = 0;

        public ulong Id { get; private set; }
        public Room RoomA { get; private set; }
        public Room RoomB { get; private set; }
        public Point DoorPointA { get; private set; }
        public Point DoorPointB { get; private set; }
        public int CountTotalPoints { get { return 1 + 1 + _corridorPoints.Count + _wallPoints.Count; } }

        private List<Point> _corridorPoints = null;
        private List<Point> _wallPoints = null;

        //NOTE(용택): 아예 만들 때 전부 가공해서 넘겨주도록 요구한다.
        public Corridor(Room a, Room b, Point doorA, Point doorB, List<Point> corridors, List<Point> walls)
        {
            Id = _id++;

            RoomA = a;
            RoomB = b;

            DoorPointA = doorA;
            DoorPointB = doorB;

            _corridorPoints = corridors;
            _corridorPoints.TrimExcess();
            _wallPoints = walls;
            _wallPoints.TrimExcess();
        }

        public IReadOnlyList<Point> GetCorridorPoints() { return _corridorPoints; }
        public IReadOnlyList<Point> GetWallPoints() { return _wallPoints; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Corridor);
        }
        public bool Equals(Corridor other)
        {
            if (other == null) return false;
            if (Id == other.Id) return true;

            if (RoomA.Id == other.RoomA.Id && RoomB.Id == other.RoomB.Id)
                return true;
            else if (RoomA.Id == other.RoomB.Id && RoomB.Id == other.RoomA.Id)
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return String.Format("Corridor {0} : {1} to {2}", Id, RoomA.Id, RoomB.Id);
        }
    }
}