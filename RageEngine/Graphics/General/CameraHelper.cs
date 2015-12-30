using System;
using System.Collections.Generic;
using System.Text;
using RageEngine;
using SharpDX;

namespace RageEngine.Graphics {
    public class CameraHelper {
        public const float Epsilon = 0.000001f;

        /// <summary>
        /// Converts 3D point to 2D screen coordinates
        /// </summary>
        /// <param name="point">Position to check.</param>
        /// <param name="vec">Empty 2D vector to recieve coordinates</param>
        public static void Convert3DPointTo2D(Vector3 point, out Vector2 vec) {
            // Fix:
            Vector4 result4=Vector3.Transform(point, SceneManager.Camera.View*SceneManager.Camera.Projection);

            if (result4.W == 0) result4.W = Epsilon;
            Vector3 result = new Vector3(result4.X / result4.W, result4.Y / result4.W, result4.Z / result4.W);

            // Output result from 3D to 2D
            vec.X = (float)Math.Round(+result.X * Display.Width/2) + (float)Display.Width/2;
            vec.Y = (float)Math.Round(-result.Y * Display.Height/2) + (float)Display.Height/2;
        }// Convert3DPointTo2D(point,out vec)

        /// <summary>
        /// Is point in front of camera?
        /// </summary>
        /// <param name="point">Position to check.</param>
        /// <returns>Bool</returns>
        public static bool IsInFrontOfCamera(Vector3 point) {
            Vector4 result=Vector4.Transform(new Vector4(point.X, point.Y, point.Z, 1), SceneManager.Camera.ViewProjection);

            // Is result in front?
            return result.Z < result.W;// -NearPlane;
        } // IsInFrontOfCamera(point)

        /// <summary>
        /// THIS IS NOT ANY FASTER THAN FRUSTUM CONTAINS FUNCTION
        /// Helper to check if a 3d-point is visible on the screen.
        /// Will basically do the same as IsInFrontOfCamera and Convert3DPointTo2D,
        /// but requires less code and is faster. Also returns just an bool.
        /// Will return true if point is visble on screen, false otherwise.
        /// Use the offset parameter to include points into the screen that are
        /// only a couple of pixel outside of it.
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="checkOffset">Check offset in percent of total
        /// screen</param>
        /// <returns>Bool</returns>
        public static bool IsVisible(Vector3 point, float checkOffset) {
            Vector4 result=Vector3.Transform(point, SceneManager.Camera.ViewProjection);

            // Point must be in front of camera, else just skip everything.
            if (result.Z < result.W) {// - NearPlane) {
                Vector2 screenPoint = new Vector2(result.X / result.W, result.Y / result.W);

                // Change checkOffset depending on how depth we are into the scene
                // for very near objects (z < 5) pass all tests!
                // for very far objects (z >> 5) only pass if near to +- 1.0f
                float zDist = Math.Abs(result.Z);
                if (zDist < 5.0f)
                    return true;
                checkOffset = 1.0f + (checkOffset / zDist);

                return
                    screenPoint.X >= -checkOffset && screenPoint.X <= +checkOffset &&
                    screenPoint.Y >= -checkOffset && screenPoint.Y <= +checkOffset;
            } // if (result.z)

            // Point is not in front of camera, return false.
            return false;
        }

        public static Ray RayFromScreen(int x, int y) {
            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 1 is as far away as possible.
            Vector3 nearSource = new Vector3(x, y, 0f);
            Vector3 farSource = new Vector3(x, y, 1f);

            // use Viewport.Unproject to tell what those two screen space positions
            // would be in world space. we'll need the projection matrix and view
            // matrix, which we have saved as member variables. We also need a world
            // matrix, which can just be identity.

            Matrix viewProj=Matrix.Multiply(SceneManager.Camera.View, SceneManager.Camera.Projection);
            
            Vector3 ZNearPlane = Vector3.Unproject(nearSource, 0, 0, Display.Width, Display.Height, 0, 1, viewProj);
            Vector3 ZFarPlane = Vector3.Unproject(farSource, 0, 0, Display.Width, Display.Height, 0, 1, viewProj);

            Vector3 direction = ZFarPlane - ZNearPlane;
            direction.Normalize();

            return new Ray(ZNearPlane, direction);
        }

        private static bool WithinEpsilon(float a, float b) {
            float f = a - b;
            if (-1.401298E-45F <= f)
                return f <= 1.401298E-45F;
            return false;
        }

        public static float AspectRatio() {
            return (float)Display.Width / Display.Height;
        }
    }
}
