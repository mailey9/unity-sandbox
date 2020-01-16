using System;

namespace minorlife
{
    public class PathCandidate : IEquatable<PathCandidate>
    {
        #region Field & Access Properties
        public Point A { get; private set; }//NOTE(용택):Cached Rect.Center
        public Point B { get; private set; }
        public Rect RectA { get; private set; }
        public Rect RectB { get; private set; }
        public Room RoomA { get; private set; }
        public Room RoomB { get; private set; }
        public ulong IdA { get { return RoomA.Id; } }
        public ulong IdB { get { return RoomB.Id; } }
        #endregion

        public int ManhattanDistance { get { return Math.Abs(A.x - B.x) + Math.Abs(A.y - B.y); } }

        public PathCandidate(Room a, Room b)
        {
            RoomA = a;
            RoomB = b;

            //NOTE(용택): Room 들이 포함한 Rect 를 비교해 가장 가까운 Rect쌍 중점을 찾는다.
            int diff = int.MaxValue;

            foreach (Rect rcA in a.GetRects())
            {
                foreach (Rect rcB in b.GetRects())
                {
                    if (rcA == rcB) continue;

                    Point comparingPoint = rcA.Center - rcB.Center;
                    int comparingDiff = Math.Abs(comparingPoint.x) + Math.Abs(comparingPoint.y);

                    if (comparingDiff == Math.Min(diff, comparingDiff))
                    {
                        diff = comparingDiff;
                        RectA = rcA;
                        RectB = rcB;
                        A = rcA.Center;
                        B = rcB.Center;
                    }
                }
            }

            System.Diagnostics.Debug.Assert(diff != int.MaxValue);
        }

        public Point[] GetSamplingVectorPoints()
        {
            Point[] samplingVectors = new Point[ManhattanDistance];

            Point diff = B - A;
            diff.x = Math.Abs(diff.x);
            diff.y = Math.Abs(diff.y);

            Point vectorStep = B - A;
            vectorStep.x = (vectorStep.x == 0) ? 0 : vectorStep.x / Math.Abs(vectorStep.x);
            vectorStep.y = (vectorStep.y == 0) ? 0 : vectorStep.y / Math.Abs(vectorStep.y);

            if (diff.x == Math.Min(diff.x, diff.y))
            {
                int i = 0;
                for (int x = 0; x < diff.x; ++x)
                    samplingVectors[i++] = new Point(vectorStep.x, 0);
                for (int y = 0; y < diff.y; ++y)
                    samplingVectors[i++] = new Point(0, vectorStep.y);
            }
            else
            {
                int i = 0;
                for (int y = 0; y < diff.y; ++y)
                    samplingVectors[i++] = new Point(0, vectorStep.y);
                for (int x = 0; x < diff.x; ++x)
                    samplingVectors[i++] = new Point(vectorStep.x, 0);
            }

            return samplingVectors;
        }
        public Point[] GetSamplingPoints()
        {
            Point[] samplingPoints = new Point[ManhattanDistance + 1];
            Point[] samplingVectors = GetSamplingVectorPoints();

            //NOTE(용택): 직선연결만 고려하기 때문에, 짧은 쪽을 먼저 접근한다. ( diff.X < diff.Y 면 X 를 먼저채운다.)
            Point current = A;
            samplingPoints[0] = current;

            for (int i = 0; i < samplingVectors.Length; ++i)
            {
                current += samplingVectors[i];
                samplingPoints[i + 1] = current;
            }

            return samplingPoints;
        }
        public override int GetHashCode()
        {
            return IdA.GetHashCode() ^ IdB.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as PathCandidate);
        }
        public bool Equals(PathCandidate other)
        {
            if (other == null)
                return false;
            else if (IdA == other.IdA && IdB == other.IdB)
                return true;
            else if (IdA == other.IdB && IdB == other.IdA)
                return true;
            else
                return false;
        }

        public static Comparison<PathCandidate> ManhattanDistanceComparison = delegate (PathCandidate a, PathCandidate b)
        {
            return a.ManhattanDistance.CompareTo(b.ManhattanDistance);
        };

        public override string ToString()
        {
            return IdA + "  (" + A + ") to " + IdB + "  (" + B + "), " + ManhattanDistance;
        }
    }
}