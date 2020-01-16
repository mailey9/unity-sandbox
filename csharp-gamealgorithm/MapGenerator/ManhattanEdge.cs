using System;

namespace minorlife
{
    public class ManhattanEdge : IEquatable<ManhattanEdge>
    {
        public ulong NodeA { get; private set; }
        public ulong NodeB { get; private set; }
        public int ManhattanDistance { get; private set; }

        public ManhattanEdge(ulong a, ulong b, int d)
        {
            NodeA = a;
            NodeB = b;
            ManhattanDistance = d;
        }

        public override int GetHashCode()
        {
            return (NodeA ^ NodeB).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            ManhattanEdge other = obj as ManhattanEdge;
            if (other == null) return false;

            return Equals(other);
        }
        public bool Equals(ManhattanEdge other)
        {
            if (NodeA == other.NodeA && NodeB == other.NodeB)
                return true;
            else if (NodeA == other.NodeB && NodeB == other.NodeA)
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return "(" + NodeA + "," + NodeB + ") ==> " + ManhattanDistance;
        }
        public static Comparison<ManhattanEdge> DistanceComparison = delegate (ManhattanEdge a, ManhattanEdge b)
        {
            return a.ManhattanDistance.CompareTo(b.ManhattanDistance);
        };
    }
}