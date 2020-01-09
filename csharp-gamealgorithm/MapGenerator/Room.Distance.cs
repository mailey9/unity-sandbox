using System;
using System.Collections.Generic;

namespace minorlife
{
    public partial class Room : IEquatable<Room>
    {
        public static int CalculateRoughManhattanDistance(Room from, Room to)
        {
            Point roughDiff = to.Filter.Center - from.Filter.Center;
            return Math.Abs(roughDiff.y) + Math.Abs(roughDiff.x);
        }

        [Obsolete]
        public static bool CalculateDetailManhattanDistance(Room roomFrom, Room roomTo, out Rect rectFrom, out Rect rectTo, out Point actualDiff)
        {
            rectFrom = new Rect();
            rectTo = new Rect();
            actualDiff = new Point(int.MaxValue / 2, int.MaxValue / 2);

            foreach (Rect i_rcFrom in roomFrom._rects)
            {
                foreach (Rect i_rcTo in roomTo._rects)
                {
                    Point result = i_rcTo.Center - i_rcFrom.Center;

                    int sumOfDiff = Math.Abs(actualDiff.x) + Math.Abs(actualDiff.y);
                    int sumOfResult = Math.Abs(result.x) + Math.Abs(result.y);
                    
                    if (Math.Min(sumOfDiff, sumOfResult) == sumOfResult)
                    {
                        actualDiff.y = Math.Abs(result.y);
                        actualDiff.x = Math.Abs(result.x);

                        rectFrom = i_rcFrom;
                        rectTo = i_rcTo;
                    }
                }
            }

            if (actualDiff.y == int.MaxValue / 2 || actualDiff.x == int.MaxValue / 2)
                return false;
            else
                return true;
        }

        public static float CalculateRoughEuclideanDistanceSqaure(Room roomFrom, Room roomTo)
        {
            Point roughDiff = roomTo.Filter.Center - roomFrom.Filter.Center;
            return MathF.Pow(Math.Abs(roughDiff.x), 2.0f) + MathF.Pow(Math.Abs(roughDiff.y), 2.0f);
        }
    }
}