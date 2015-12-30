#define USE_OPTIMIZATIONS

using SharpDX;
using System;

namespace RageEngine
{
	//Fixed point math equivalent to Unity.Vector2
	public struct FVector2
	{
		public fint X;
		public fint Y;

		//
		// Constructors
        //
		public FVector2(fint x, fint y)
		{
			this.X = x;
			this.Y = y;
		}	


		//
		// Static Properties
		//
		public static FVector2 One = new FVector2(fint.one, fint.one);
		
		public static FVector2 Zero = new FVector2(fint.zero, fint.zero);

		public static FVector2 Right = new FVector2(fint.one, fint.zero);
		
		public static FVector2 Up = new FVector2(fint.zero, fint.one);

		//
		// Properties
		//
		public fint magnitude
		{
			get
			{
				#if USE_OPTIMIZATIONS
				long temp = (((long) X.raw) * ((long) X.raw) + ((long) Y.raw) * ((long) Y.raw)) >> fint.SHIFT_AMOUNT;
				return FMath.Sqrt(fint.CreateRaw((int) temp));
				#else
				return FMath.Sqrt(this.x * this.x + this.y * this.y);
				#endif
			}
		}
		
		public FVector2 normalized
		{
			get
			{
				return FVector2.Normalize(this);
			}
		}
		
		public fint sqrMagnitude
		{
			get
			{
				#if USE_OPTIMIZATIONS
				long temp = (((long) X.raw) * ((long) X.raw) + ((long) Y.raw) * ((long) Y.raw)) >> fint.SHIFT_AMOUNT;
				return fint.CreateRaw((int) temp);
				#else
				return this.x * this.x + this.y * this.y;
				#endif
			}
		}
		
		//
		// Static Methods
		//
		public static fint Angle(FVector2 from, FVector2 to)
		{
			return FMath.Acos(FMath.Clamp(FVector2.Dot(from.normalized, to.normalized), -fint.one, fint.one)) * FMath.Rad2Deg;
		}
		
		public static FVector2 ClampMagnitude(FVector2 vector, fint maxLength)
		{
			if (vector.sqrMagnitude > maxLength * maxLength)
			{
				return vector.normalized * maxLength;
			}
			return vector;
		}
		
		public static fint Distance(FVector2 a, FVector2 b)
		{
			return (a - b).magnitude;
		}
		
		public static fint Dot(FVector2 lhs, FVector2 rhs)
		{
			return lhs.X * rhs.X + lhs.Y * rhs.Y;
		}
		
		public static FVector2 Lerp(FVector2 from, FVector2 to, fint t)
		{
			t = FMath.Clamp01(t);
			return new FVector2(from.X + (to.X - from.X) * t, from.Y + (to.Y - from.Y) * t);
		}
		
		public static fint Magnitude(FVector2 a)
		{
			#if USE_OPTIMIZATIONS
			long temp = (((long) a.X.raw) * ((long) a.X.raw) + ((long) a.Y.raw) * ((long) a.Y.raw)) >> fint.SHIFT_AMOUNT;
			return FMath.Sqrt(fint.CreateRaw((int) temp));
			#else
			return FMath.Sqrt(a.x * a.x + a.y * a.y);
			#endif
		}

		public static FVector2 Max(FVector2 lhs, FVector2 rhs)
		{
			return new FVector2(FMath.Max(lhs.X, rhs.X), FMath.Max(lhs.Y, rhs.Y));
		}
		
		public static FVector2 Min(FVector2 lhs, FVector2 rhs)
		{
			return new FVector2(FMath.Min(lhs.X, rhs.X), FMath.Min(lhs.Y, rhs.Y));
		}
		
		public static FVector2 MoveTowards(FVector2 current, FVector2 target, fint maxDistanceDelta)
		{
			FVector2 a = target - current;
			fint magnitude = a.magnitude;
			if (magnitude <= maxDistanceDelta || magnitude == fint.zero)
			{
				return target;
			}
			return current + a / magnitude * maxDistanceDelta;
		}

		public static FVector2 Normalize(FVector2 value)
		{
			fint magnitude = FVector2.Magnitude(value);
			if (magnitude.raw > 1)
			{
				return value / magnitude;
			}
			return FVector3.zero;
		}

		public static FVector2 Scale(FVector2 a, FVector2 b)
		{
			return new FVector2(a.X * b.X, a.Y * b.Y);
		}
		
		public static fint SqrMagnitude(FVector2 a)
		{
			return a.X * a.X + a.Y * a.Y;
		}
		
		//
		// Methods
		//
		public override bool Equals(object other)
		{
			if (!(other is FVector2))
				return false;

			FVector2 vector = (FVector2)other;

			return this.X.raw == vector.X.raw && this.Y.raw == vector.Y.raw;
		}
		
		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2;
		}
		
		public void Normalize()
		{
			fint magnitude = this.magnitude;
			if (magnitude.raw > 1)
			{
				this /= magnitude;
			}
			else
			{
				this = FVector2.Zero;
			}
		}
		
		public void Scale(FVector2 scale)
		{
			this.X *= scale.X;
			this.Y *= scale.Y;
		}
		
		public void Set(fint new_x, fint new_y)
		{
			this.X = new_x;
			this.Y = new_y;
		}
		
		public fint SqrMagnitude()
		{
			return this.X * this.X + this.Y * this.Y;
		}
		
		public override string ToString()
		{
			return string.Format("({0}, {1})", this.X.ToFloat(), this.Y.ToFloat());
		}

		public Vector2 ToVector2()
		{
			return new Vector2(X.ToFloat(), Y.ToFloat());
		}

        public Point2D ToPoint2D()
        {
            return new Point2D(X.ToInt(), Y.ToInt());
        }

		//
		// Operators
		//
		public static FVector2 operator +(FVector2 a, FVector2 b)
		{
			return new FVector2(a.X + b.X, a.Y + b.Y);
		}
		
		public static FVector2 operator /(FVector2 a, fint d)
		{
			return new FVector2(a.X / d, a.Y / d);
		}
		
		public static bool operator ==(FVector2 lhs, FVector2 rhs)
		{
			return lhs.X.raw == rhs.X.raw && lhs.Y.raw == rhs.Y.raw;
		}

		public static implicit operator FVector2(FVector3 v)
		{
			return new FVector2(v.x, v.y);
		}
		
		public static implicit operator FVector3(FVector2 v)
		{
			return new FVector3(v.X, v.Y, fint.zero);
		}

		public static bool operator !=(FVector2 lhs, FVector2 rhs)
		{
			return lhs.X.raw != rhs.X.raw || lhs.Y.raw == rhs.Y.raw;
		}
		
		public static FVector2 operator *(FVector2 a, fint d)
		{
			return new FVector2(a.X * d, a.Y * d);
		}
		
		public static FVector2 operator *(fint d, FVector2 a)
		{
			return new FVector2(a.X * d, a.Y * d);
		}
		
		public static FVector2 operator -(FVector2 a, FVector2 b)
		{
			return new FVector2(a.X - b.X, a.Y - b.Y);
		}
		
		public static FVector2 operator -(FVector2 a)
		{
			return new FVector2(-a.X, -a.Y);
		}
	}
}

