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

        public static float CalculateRoughEuclideanDistanceSqaure(Room roomFrom, Room roomTo)
        {
            Point roughDiff = roomTo.Filter.Center - roomFrom.Filter.Center;
            return MathF.Pow(Math.Abs(roughDiff.x), 2.0f) + MathF.Pow(Math.Abs(roughDiff.y), 2.0f);
        }
    }
}