using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageEngine {
    public static class Extensions {
        public static Vector2 ToXY(this Vector3 vec){
            return new Vector2(vec.X, vec.Y);
        }
        public static Vector2 ToXY(this Vector4 vec) {
            return new Vector2(vec.X, vec.Y);
        }
        public static Vector3 ToXYZ(this Vector4 vec) {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }
        public static Vector2 ToXZ(this Vector3 vec) {
            return new Vector2(vec.X, vec.Z);
        }
        public static Vector3 ToX0Z(this Vector3 vec) {
            return new Vector3(vec.X,0, vec.Z);
        }
        public static T[] RemoveAt<T>(this T[] source, int index) {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }
    }
}
