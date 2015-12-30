using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageEngine {
    public struct Point3D {
        public int X,Y,Z;

        public Point3D(int X, int Y, int Z) {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public int LengthSquared() {
            return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
        }

        public override bool Equals(object obj) {
            if (obj is Point3D) {
                Point3D p3d = (Point3D)obj;
                return p3d.X == this.X && p3d.Y == this.Y && p3d.Z == this.Z;
            }
            return false;
        }

        public override int GetHashCode() {
            return (X + " " + Y + " " + Z).GetHashCode();
        }

        public override string ToString() {
            return X + ", " + Y + ", " + Z;
        }

        public static bool operator ==(Point3D one, Point3D two) {
            return (one.X == two.X && one.Y == two.Y && one.Z == two.Z);
        }

        public static bool operator !=(Point3D one, Point3D two) {
            return (one.X != two.X || one.Y != two.Y || one.Z != two.Z);
        }

        public static Point3D operator +(Point3D one, Point3D two) {
            return new Point3D(one.X + two.X, one.Y + two.Y, one.Z + two.Z);
        }

        public static Point3D operator -(Point3D one, Point3D two) {
            return new Point3D(one.X - two.X, one.Y - two.Y, one.Z - two.Z);
        }

        public static Point3D Zero = new Point3D(0, 0, 0);
    }
}
