using System;
using System.Collections.Generic;
using System.Text;
using RageEngine;
using SharpDX;

namespace RageEngine.Utils {
    public class GameUtils {
            public static Random random = new Random();

            public const float E = 2.71828f;
            //
            public const float Log10E = 0.434294f;
            public const float Log2E = 1.4427f;

            public const float Pi = 3.14159265f;
            public const float PiOver2 = 1.57079633f;
            public const float PiOver4 = 0.785398163f;
            public const float TwoPi = 6.28318531f;

            public const float Epsilon = 0.00001f;

            public static float Barycentric(float value1, float value2, float value3, float amount1, float amount2) {
                return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
            }

            public static float CatmullRom(float value1, float value2, float value3, float value4, float amount) {
                // Using formula from http://www.mvps.org/directx/articles/catmull/
                // Internally using doubles not to lose precission
                double amountSquared = amount * amount;
                double amountCubed = amountSquared * amount;
                return (float)(0.5 * (2.0 * value2 +
                    (value3 - value1) * amount +
                    (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                    (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));
            }

            public static float Clamp(float value, float min, float max) {
                value = (value > max) ? max : value;
                value = (value < min) ? min : value;
                return value;
            }

            public static int Clamp(int value, int min, int max) {
                value = (value > max) ? max : value;
                value = (value < min) ? min : value;
                return value;
            }

            public static fint Clamp(fint value, fint min, fint max)
            {
                value = (value > max) ? max : value;
                value = (value < min) ? min : value;
                return value;
            }

            public static fint Clamp(fint value, int min, int max)
            {
                value = (value > (fint)max) ? (fint)max : value;
                value = (value < (fint)min) ? (fint)min : value;
                return value;
            }
            public static float Det(Vector2 v1, Vector2 v2) {
                return v1.X * v2.Y - v1.Y * v2.X;
            }

            public static float Sqr(float value1) {
                return value1 * value1;
            }

            public static float Sqr(Vector2 value) {
                return value.X * value.Y;
            }

            public static float Sqrt(float a) {
                return (float)Math.Sqrt(a);
            }

            public static float Distance(float value1, float value2) {
                return Math.Abs(value1 - value2);
            }

            public static double Distance2D(int x,int y, int x2, int y2) {
                int xDist = x - x2;
                int yDist = y - y2;
                return Math.Sqrt(xDist * xDist + yDist * yDist);
            }

            public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount) {
                // All transformed to double not to lose precission
                // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
                double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
                double sCubed = s * s * s;
                double sSquared = s * s;

                if (amount == 0f)
                    result = value1;
                else if (amount == 1f)
                    result = value2;
                else
                    result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                        (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                        t1 * s +
                        v1;
                return (float)result;
            }

            public static float Log(float a, int nbase) {
                return (float)Math.Log(a, nbase);
            }

            public static float Lerp(float value1, float value2, float amount) {
                return value1 + (value2 - value1) * amount;
            }

            public static byte Lerp(int value1, int value2, float amount) {
                return (byte)GameUtils.Clamp(value1 + (value2 - value1) * amount,0,255);
            }

            public static int Max(int value1, int value2) {
                return Math.Max(value1, value2);
            }

            public static float Max(float value1, float value2) {
                return Math.Max(value1, value2);
            }

            public static float Min(float value1, float value2) {
                return Math.Min(value1, value2);
            }

            public static float RandomRotation() {
                return (float)random.NextDouble() * Pi * 2;
            }

            public static float ToDegrees(float radians) {
                return radians * 57.29578F;
            }

            public static float ToRadians(float degrees) {
                return degrees * 0.01745329F;
            }

            public static float WrapAngle(float angle) {
                angle = (float)Math.IEEERemainder((double)angle, 6.28318548202515);
                if (angle <= -3.141593F)
                    angle += 6.283185F;
                else if (angle > 3.141593F)
                    angle -= 6.283185F;
                return angle;
            }

            public static float AbsoluteAngle(float angle) {
                if (angle < 0) angle += 6.283185F;
                if (angle > 6.283185F) angle -= 6.283185F;
                return angle;
            }

            public static float AngleStep(float angle, float angleTo, float step) {
                if (angleTo > angle) {
                    if (angleTo - angle > Pi) angleTo -= TwoPi; // its usally while <-- more accurate
                } else {
                    if (angleTo - angle < -Pi) angleTo += TwoPi;
                }

                float diffAngle = angle - angleTo;

                if (diffAngle > 0) {
                    angle -= step;
                    if (angle < angleTo) angle = angleTo;
                } else {
                    angle += step;
                    if (angle > angleTo) angle = angleTo;
                }
                return angle;
            }

            public static float AngleLerp(float angle, float angleTo, float alpha) {
                if (alpha == 1) return angleTo;

                if (angleTo > angle) {
                    if (angleTo - angle > Pi)  angleTo -= TwoPi; // its usally while <-- more accurate
                } else {
                    if (angleTo - angle < -Pi) angleTo += TwoPi;
                }

                return angle + (angleTo - angle) * alpha;
            }


            public static float AngleDifference(Vector2 dir, float angle2) {
                Vector2 diffAngle = new Vector2(dir.X - (float)Math.Cos(angle2),dir.Y - (float)Math.Sin(angle2));
                return (float)Math.Atan2(diffAngle.Y, diffAngle.X); ;
            }

            public static float AngleDifference(float angle, float angle2) {
                Vector2 diffAngle = new Vector2((float)Math.Cos(angle-angle2),((float)Math.Sin(angle-angle2)));
                return (float)Math.Atan2(diffAngle.Y, diffAngle.X);
            }

            public static float AngleDifference2(float angle, float angle2) {
                angle2 = angle2 - PiOver2 + Pi;
                if (angle2 > TwoPi) angle2 -= TwoPi;
                if (angle2 < 0) angle2 += TwoPi;
                float diffAngle = angle + Pi - angle2;

                if (diffAngle > Pi) diffAngle -= TwoPi;
                if (diffAngle < -Pi) diffAngle += TwoPi;
                return diffAngle;
            }

            public static float AngleNormalize(float angle) {
                float newAngle = angle;
                while (newAngle <= -Pi) newAngle += TwoPi;
                while (newAngle > Pi) newAngle -= TwoPi;
                return newAngle;
            }

            public static bool InsideQuad(float px, float py,float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3){
	            if (dot(x0, y0, x1, y1, px, py) > 0 )
		            if (dot(x1,y1,x2,y2,px,py)>0 )
			            if (dot(x2,y2,x3,y3,px,py)>0 )
				            if (dot(x3,y3,x0,y0,px,py)>0 )
					            return true;
            	return false;
            }

            public static float dot(float x0,float y0,float x1,float y1,float x2,float y2) {
	            return ((x1-x0)*(y2-y1))-((x2-x1)*(y1-y0));
            } 

            public static bool InsideATriangle(int x0, int y0, int x1, int y1, int x2, int y2, int x3, int y3) {
                int AB = (y0 - y1) * (x2 - x1) - (x0 - x1) * (y2 - y1);
                int BC = (y0 - y2) * (x3 - x2) - (x0 - x2) * (y3 - y2);
                int CA = (y0 - y3) * (x1 - x3) - (x0 - x3) * (y1 - y3);
                return ((AB * BC) > 0 && (BC * CA) > 0);
            }

            public static bool PointInQuadix(int x, int y, int x0, int y0, int x1, int y1, int x2, int y2, int x3, int y3) {
                if (InsideATriangle(x, y, x0, y0, x1, y1, x2, y2)) return true;
                if (InsideATriangle(x, y, x3, y3, x1, y1, x2, y2)) return true;
                return false;
            }

            public static bool PointInRectangle(int x, int y, int x0, int y0, int width, int height) {
                return (((x0) <= x) && ((x0 + width) >= x) && ((y0) <= y) && ((y0 + height) >= y));
            }


            /// <summary>Returns a vector that is perpendicular to the input vector</summary>
            /// <param name="vector">Vector to which a perpendicular vector will be found</param>
            /// <param name="perpendicular">
            ///   Output parameter that receives a vector perpendicular to the provided vector
            /// </param>
            /// <remarks>
            ///   <para>
            ///     This method does not care for the orientation of the resulting vector, so it
            ///     shouldn't be used for billboards or to orient a view matrix. On the other hand,
            ///     if you don't care for the orientation of the resulting vector, only that it is
            ///     perpendicular, this method can provide better numerical stability and
            ///     performance than a generic LookAt() method.
            ///   </para>
            /// </remarks>
            public static void GetPerpendicularVector(ref Vector3 vector, out Vector3 perpendicular) {
                float absX=Math.Abs(vector.X);
                float absY=Math.Abs(vector.Y);
                float absZ=Math.Abs(vector.Z);

                if (absX<absY) {
                    if (absZ<absX) {
                        perpendicular=new Vector3(vector.Y, -vector.X, 0.0f);
                    } else {
                        perpendicular=new Vector3(0.0f, vector.Z, -vector.Y);
                    }
                } else {
                    if (absZ<absY) {
                        perpendicular=new Vector3(vector.Y, -vector.X, 0.0f);
                    } else {
                        perpendicular=new Vector3(vector.Z, 0.0f, -vector.X);
                    }
                }
            }

    }
}
