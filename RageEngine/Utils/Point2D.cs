using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageEngine {
    public struct Point2D {
        public int X;
        public int Y;

        public Point2D(int X, int Y) {
            this.X = X;
            this.Y = Y;
        }

        public Point2D(float X, float Y) {
            this.X = (int)X;
            this.Y = (int)Y;
        }

        public double Distance(Point2D p2) {
            int xDist = X - p2.X;
            int yDist = Y - p2.Y;
            return Math.Sqrt(xDist * xDist + yDist * yDist);
        }

        public long GetDistanceSquared(Point2D point) {
            long dx = this.X - point.X;
            long dy = this.Y - point.Y;
            return (dx * dx) + (dy * dy);
        }

        public int LengthSquared() {
            return (this.X * this.X) + (this.Y * this.Y);
        }

        public override bool Equals(object obj) {
            if (obj is Point2D) {
                Point2D p3d = (Point2D)obj;
                return p3d.X == this.X && p3d.Y == this.Y;
            }
            return false;
        }

        public override int GetHashCode() {

            return (X + " " + Y).GetHashCode();
        }

        public override string ToString() {
            return X + ", " + Y;
        }

        public static bool operator ==(Point2D one, Point2D two) {
            return (one.X == two.X && one.Y == two.Y);
        }

        public static bool operator !=(Point2D one, Point2D two) {
            return (one.X != two.X || one.Y != two.Y);
        }

        public static Point2D operator +(Point2D one, Point2D two) {
            return new Point2D(one.X + two.X, one.Y + two.Y);
        }

        public static Point2D operator -(Point2D one, Point2D two) {
            return new Point2D(one.X - two.X, one.Y - two.Y);
        }

        public static Point2D Zero = new Point2D(0, 0);
    }
}
