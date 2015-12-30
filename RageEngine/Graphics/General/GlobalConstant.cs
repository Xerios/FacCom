using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace RageEngine.Graphics {

    [StructLayout(LayoutKind.Sequential)]
    internal struct GlobalConstant {
        public Matrix  ViewProjection;
        public Vector4 CameraPosition;
        public Vector4 AmbientColor;
        public Vector3 SunColor;
        public float   Time;
        public Vector3 SunDirection;
        public float   EyeHeight;
    }

    public static class GlobalConstantVars {
        public static float Time;
        public static float EyeHeight;
        public static Vector3 SunDirection;
        public static Vector3 SunColor;
        public static Vector4 AmbientColor;
    }
}
