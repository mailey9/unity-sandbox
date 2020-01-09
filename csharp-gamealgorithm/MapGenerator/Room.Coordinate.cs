using System;
using System.Collections.Generic;

namespace minorlife
{
    public partial class Room : IEquatable<Room>
    {
        public int xMin
        {
            get
            {
                int minX = int.MaxValue;
                foreach (var rect in _rects)
                    minX = Math.Min(minX, rect.xMin);
                return minX;
            }
        }
        public int xMax
        {
            get
            {
                int maxX = int.MinValue;
                foreach (var rect in _rects)
                    maxX = Math.Max(maxX, rect.xMax);
                return maxX;
            }
        }
        public int yMin
        {
            get
            {
                int minY = int.MaxValue;
                foreach (var rect in _rects)
                    minY = Math.Min(minY, rect.yMin);
                return minY;
            }
        }
        public int yMax
        {
            get
            {
                int maxY = int.MinValue;
                foreach (var rect in _rects)
                    maxY = Math.Max(maxY, rect.yMax);
                return maxY;
            }
        }

        public Rect Filter
        {
            //NOTE(용택): Room의 대략적인 Rect만 반환한다.
            //      RRRRRR          FFFFFFFFFFF
            //      RRRRRRRRRRR     F         F
            //         RRRRRRRR     F         F
            //         RRRRRRRR     FFFFFFFFFFF //이런느낌
            get
            {
                return new Rect(xMin, yMin, xMax - xMin + 1, yMax - yMin + 1);
            }
        }

        public List<Point> GetHullPoints(Map.Tile[,] roomtileAppliedMap)
        {
            //NOTE(용택): map 에 이미 Map.Tile.Room 들이 반영되어있다고 가정한다.

            HashSet<Point> candidates = new HashSet<Point>();
            candidates.UnionWith(GetTopPoints());
            candidates.UnionWith(GetBottomPoints());
            candidates.UnionWith(GetLeftPoints());
            candidates.UnionWith(GetRightPoints());

            Point[] lookupTable = new Point[8];
            //  0   1   2
            //  3 sampl 4
            //  5   6   7
            lookupTable[0] = new Point(-1, -1);
            lookupTable[1] = new Point( 0, -1);
            lookupTable[2] = new Point(+1, -1);
            lookupTable[3] = new Point(-1,  0);
            lookupTable[4] = new Point(+1,  0);
            lookupTable[5] = new Point(-1, +1);
            lookupTable[6] = new Point( 0, +1);
            lookupTable[7] = new Point(+1, +1);

            int map_maxRow = roomtileAppliedMap.GetLength(0) - 1;
            int map_maxColumn = roomtileAppliedMap.GetLength(1) - 1;

            List<Point> hullPoints = new List<Point>(candidates.Count);
            foreach (Point pt in candidates)
            {
                if (pt.x == 0 || pt.y == 0 || pt.x == map_maxColumn || pt.y == map_maxRow)
                {
                    hullPoints.Add(pt);
                }
                else
                {
                    foreach(Point lookupPt in lookupTable)
                    {
                        Point samplePt = pt + lookupPt;
                        if (roomtileAppliedMap[samplePt.y, samplePt.x] == Map.Tile.Empty)
                        {
                            hullPoints.Add(pt);
                            break;
                        }
                    }
                }//if (pt at boundary) or (not)
            }

            return hullPoints;
        }

        private HashSet<Point> GetTopPoints()
        {
            HashSet<Point> topPoints = new HashSet<Point>();
            foreach (Rect rc in _rects)
                topPoints.UnionWith(rc.GetXPoints(rc.yMin));
            return topPoints;
        }
        private HashSet<Point> GetBottomPoints()
        {
            HashSet<Point> bottomPoints = new HashSet<Point>();
            foreach (Rect rc in _rects)
                bottomPoints.UnionWith(rc.GetXPoints(rc.yMax));
            return bottomPoints;
        }
        private HashSet<Point> GetLeftPoints()
        {
            HashSet<Point> leftPoints = new HashSet<Point>();
            foreach (Rect rc in _rects)
                leftPoints.UnionWith(rc.GetYPoints(rc.xMin));
            return leftPoints;
        }
        private HashSet<Point> GetRightPoints()
        {
            HashSet<Point> rightPoints = new HashSet<Point>();
            foreach (Rect rc in _rects)
                rightPoints.UnionWith(rc.GetYPoints(rc.xMax));
            return rightPoints;
        }
    }
}