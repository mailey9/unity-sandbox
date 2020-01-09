using System;

namespace minorlife
{
    public struct Rect : IEquatable<Rect>
    {
        public int x;
        public int y;
        public int width;
        public int height;

        #region Point Utility Properties
        public int xMin { get { return x; } }
        public int xMax { get { return x + width - 1; } }
        public int yMin { get { return y; } }
        public int yMax { get { return y + height - 1; } }
        public Point TopLeft { get { return new Point(xMin, yMin); } }
        public Point TopRight { get { return new Point(xMax, yMin); } }
        public Point BottomRight { get { return new Point(xMax, yMax); } }
        public Point BottomLeft { get { return new Point(xMin, yMax); } }
        public Point Center { get { return 0.5f * new Point(xMin + xMax, yMin + yMax); } }
        #endregion//Point Utility Properties

        public Rect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        
        public Point[] GetYPoints(int x)
        {
            if (x < xMin || x > xMax) return null;

            Point[] yPoints = new Point[height];
            for (int i = 0; i < height; ++i)
                yPoints[i] = new Point(x, yMin + i);
            return yPoints;
        }
        public Point[] GetXPoints(int y)
        {
            if (y < yMin || y > yMax) return null;

            Point[] xPoints = new Point[width];
            for (int i = 0; i < width; ++i)
                xPoints[i] = new Point(xMin + i, y);
            return xPoints;
        }

        public bool Contains(int x, int y)
        {
            if (x < this.x || y < this.y || x > this.xMax || y > this.yMax)
                return false;
            else
                return true;
        }
        public bool HasIntersection(Rect other)
        {
            //REF(용택): https://stackoverflow.com/questions/2752349/fast-rectangle-to-rectangle-intersection
            //return !(other.ColMin > ColMax || other.ColMax < ColMin || other.RowMin > RowMax || other.RowMax < RowMin);
            if (other.y < y + height && other.x < x + width &&
                y < other.y + other.height && x < other.x + other.width)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Rect Intersect(Rect a, Rect b)
        {
            Rect intersectArea;
                 intersectArea.x = Math.Max(a.x, b.x);
                 intersectArea.y = Math.Max(a.y, b.y);
                 intersectArea.width = Math.Min(a.xMax, b.xMax) + 1 - intersectArea.x;
                 intersectArea.height = Math.Min(a.yMax, b.yMax) + 1 - intersectArea.y;
            return intersectArea;
        }

        public override string ToString()
        {
            return "{x:" + x + ", y:" + y + ", w:" + width + ", h:" + height + "}";
        }

        public override int GetHashCode()
        {
            return (x^y+width^height).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Rect? rect = obj as Rect?;
            if (rect == null) return false;
            return Equals(rect);
        }
        public bool Equals(Rect other)
        {
            if (x != other.x || y != other.y || width != other.width || height != other.height)
                return false;
            else
                return true;
        }
        public static bool operator==(Rect a, Rect b)
        {
            return a.Equals(b);
        }
        public static bool operator!=(Rect a, Rect b)
        {
            return !a.Equals(b);
        }
    }
}