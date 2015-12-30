#define USE_OPTIMIZATIONS

using SharpDX;
using System;

namespace RageEngine
{
	//Fixed point math equivalent to Unity.Vector3
	public struct FVector3
	{
		public fint x;
		public fint y;
		public fint z;

		//
		// Constructors
		//
		public FVector3(fint x, fint y, fint z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}	
		
		
		//
		// Static Properties
		//
		public static FVector3 one = new FVector3(fint.one, fint.one, fint.one);
		
		public static FVector3 zero = new FVector3(fint.zero, fint.zero, fint.zero);
		
		public static FVector3 forward = new FVector3(fint.zero, fint.zero, fint.one);

		public static FVector3 back = new FVector3(fint.zero, fint.zero, -fint.one);

		public static FVector3 up = new FVector3(fint.zero, fint.one, fint.zero);
		
		public static FVector3 down = new FVector3(fint.zero, -fint.one, fint.zero);

		public static FVector3 right = new FVector3(fint.one, fint.zero, fint.zero);
		
		public static FVector3 left = new FVector3(-fint.one, fint.zero, fint.zero);

		//
		// Properties
		//
		public fint magnitude
		{
			get
			{
				#if USE_OPTIMIZATIONS
				long temp = (((long) x.raw) * ((long) x.raw) + ((long) y.raw) * ((long) y.raw) + ((long) z.raw) * ((long) z.raw)) >> fint.SHIFT_AMOUNT;
				return FMath.Sqrt(fint.CreateRaw((int) temp));
				#else
				return FMath.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
				#endif
			}
		}
		
		public FVector3 normalized
		{
			get
			{
				return FVector3.Normalize(this);
			}
		}
		
		public fint sqrMagnitude
		{
			get
			{
				#if USE_OPTIMIZATIONS
				long temp = (((long) x.raw) * ((long) x.raw) + ((long) y.raw) * ((long) y.raw) + ((long) z.raw) * ((long) z.raw)) >> fint.SHIFT_AMOUNT;
				return fint.CreateRaw((int) temp);
				#else
				return this.x * this.x + this.y * this.y + this.z * this.z;
				#endif
			}
		}
		
		//
		// Static Methods
		//
		public static fint Angle(FVector3 from, FVector3 to)
		{
			return FMath.Acos(FMath.Clamp(FVector3.Dot(from.normalized, to.normalized), -fint.one, fint.one)) * FMath.Rad2Deg;
		}
		
		public static FVector3 ClampMagnitude(FVector3 vector, fint maxLength)
		{
			if (vector.sqrMagnitude > maxLength * maxLength)
			{
				return vector.normalized * maxLength;
			}
			return vector;
		}
		
		public static FVector3 Cross(FVector3 lhs, FVector3 rhs)
		{
			return new FVector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}
		
		public static fint Distance(FVector3 a, FVector3 b)
		{
			FVector3 vector = new FVector3(a.x - b.x, a.y - b.y, a.z - b.z);

			return FMath.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
		}
		
		public static fint Dot(FVector3 lhs, FVector3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}
		
		public static FVector3 Exclude(FVector3 excludeThis, FVector3 fromThat)
		{
			return fromThat - FVector3.Project(fromThat, excludeThis);
		}
		
		public static FVector3 Lerp(FVector3 from, FVector3 to, fint t)
		{
			t = FMath.Clamp01(t);

			return new FVector3(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
		}
		
		public static fint Magnitude(FVector3 a)
		{
			#if USE_OPTIMIZATIONS
			long temp = (((long) a.x.raw) * ((long) a.x.raw) + ((long) a.y.raw) * ((long) a.y.raw) + ((long) a.z.raw) * ((long) a.z.raw)) >> fint.SHIFT_AMOUNT;
			return FMath.Sqrt(fint.CreateRaw((int) temp));
			#else
			return FMath.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
			#endif
		}
		
		public static FVector3 Max(FVector3 lhs, FVector3 rhs)
		{
			return new FVector3(FMath.Max(lhs.x, rhs.x), FMath.Max(lhs.y, rhs.y), FMath.Max(lhs.z, rhs.z));
		}
		
		public static FVector3 Min(FVector3 lhs, FVector3 rhs)
		{
			return new FVector3(FMath.Min(lhs.x, rhs.x), FMath.Min(lhs.y, rhs.y), FMath.Min(lhs.z, rhs.z));
		}
		
		public static FVector3 MoveTowards(FVector3 current, FVector3 target, fint maxDistanceDelta)
		{
			FVector3 a = target - current;
			fint magnitude = a.magnitude;
			if (magnitude <= maxDistanceDelta || magnitude == fint.zero)
			{
				return target;
			}
			return current + a / magnitude * maxDistanceDelta;
		}
		
		public static FVector3 Normalize(FVector3 value)
		{
			fint magnitude = FVector3.Magnitude(value);
			if (magnitude.raw > 1)
			{
				return value / magnitude;
			}
			return FVector3.zero;
		}
		
		public static FVector3 Project(FVector3 vector, FVector3 onNormal)
		{
			fint num = FVector3.Dot(onNormal, onNormal);
			if (num.raw < 1)
			{
				return FVector3.zero;
			}
			return onNormal * FVector3.Dot(vector, onNormal) / num;
		}
		
		public static FVector3 Reflect(FVector3 inDirection, FVector3 inNormal)
		{
			return fint.CreateFromInt(2) * FVector3.Dot(inNormal, inDirection) * inNormal + inDirection;
		}
				
		public static FVector3 Scale(FVector3 a, FVector3 b)
		{
			return new FVector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}
		
		public static fint SqrMagnitude(FVector3 a)
		{
			#if USE_OPTIMIZATIONS
			long temp = (((long) a.x.raw) * ((long) a.x.raw) + ((long) a.y.raw) * ((long) a.y.raw) + ((long) a.z.raw) * ((long) a.z.raw)) >> fint.SHIFT_AMOUNT;
			return fint.CreateRaw((int) temp);
			#else
			return a.x * a.x + a.y * a.y + a.z * a.z;
			#endif
		}
		
		//
		// Methods
		//
		public override bool Equals(object other)
		{
			if (!(other is FVector3))
				return false;

			FVector3 vector = (FVector3)other;

			return this.x.raw == vector.x.raw && this.y.raw == vector.y.raw && this.z.raw == vector.z.raw; 
		}
		
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
		}
		
		public void Normalize()
		{
			fint num = FVector3.Magnitude(this);
			if (num.raw > 1)
			{
				this /= num;
			}
			else
			{
				this = FVector3.zero;
			}
		}
		
		public void Scale(FVector3 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}
		
		public void Set(fint new_x, fint new_y, fint new_z)
		{
			this.x = new_x;
			this.y = new_y;
			this.z = new_z;
		}
		
		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", this.x.ToFloat(), this.y.ToFloat(), this.z.ToFloat());
		}

		public Vector3 ToVector3()
		{
			return new Vector3(x.ToFloat(), y.ToFloat(), z.ToFloat());
		}
		
		public IntVector3 ToIntVector3()
		{
			return new IntVector3(x.ToInt(), y.ToInt(), z.ToInt());
		}

		//
		// Operators
		//
		public static FVector3 operator +(FVector3 a, FVector3 b)
		{
			return new FVector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}
		
		public static FVector3 operator /(FVector3 a, fint d)
		{
			return new FVector3(a.x / d, a.y / d, a.z / d);
		}
		
		public static bool operator ==(FVector3 lhs, FVector3 rhs)
		{
			return lhs.x.raw == rhs.x.raw && lhs.y.raw == rhs.y.raw && lhs.z.raw == rhs.z.raw; 
		}
		
		public static bool operator !=(FVector3 lhs, FVector3 rhs)
		{
			return lhs.x.raw != rhs.x.raw || lhs.y.raw != rhs.y.raw || lhs.z.raw != rhs.z.raw; 
		}
		
		public static FVector3 operator *(FVector3 a, fint d)
		{
			return new FVector3(a.x * d, a.y * d, a.z * d);
		}
		
		public static FVector3 operator *(fint d, FVector3 a)
		{
			return new FVector3(a.x * d, a.y * d, a.z * d);
		}
		
		public static FVector3 operator -(FVector3 a, FVector3 b)
		{
			return new FVector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}
		
		public static FVector3 operator -(FVector3 a)
		{
			return new FVector3(-a.x, -a.y, -a.z);
		}
	}
}

