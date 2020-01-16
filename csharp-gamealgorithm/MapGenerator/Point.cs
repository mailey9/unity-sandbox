using System;
using System.Collections.Generic;

namespace minorlife
{
    public struct Point : IEquatable<Point>, IComparer<Point>
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Point(Point other)
        {
            this.x = other.x;
            this.y = other.y;
        }

        public static Point operator+(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }
        public static Point operator-(Point a, Point b)
        {
            return new Point(a.x - b.x, a.y - b.y);
        }
        public static Point operator*(Point a, float f)
        {
            return new Point((int)(a.x * f), (int)(a.y * f));
        }
        public static Point operator*(float f, Point a)
        {
            return new Point((int)(a.x * f), (int)(a.y * f));
        }

        public override int GetHashCode()
        {
            return (x^y).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Point? coord = obj as Point?;
            return coord == null ? false : Equals(coord);
        }
        public bool Equals(Point other)
        {
            return (x == other.x && y == other.y);
        }
        public static bool operator==(Point a, Point b)
        {
            return a.Equals(b);
        }
        public static bool operator!=(Point a, Point b)
        {
            return !a.Equals(b);
        }
        public override string ToString()
        {
            return "x:" + x + ", y:" + y;
        }
        public int Compare(Point a, Point b)
        {
            int yCompare = a.y.CompareTo(b.y);
            return (yCompare != 0) ? yCompare : a.x.CompareTo(b.x);
        }

        public static Comparison<Point> YXComparison = delegate (Point a, Point b)
        {
            int yCompare = a.y.CompareTo(b.y);
            return (yCompare != 0) ? yCompare : a.x.CompareTo(b.x);
        };
        public static Comparison<Point>XYComparison = delegate (Point a, Point b)
        {
            int xCompare = a.x.CompareTo(b.x);
            return (xCompare != 0) ? xCompare : a.y.CompareTo(b.y);
        };
    }
}