using System.Collections.Generic;
using System;
using System.IO;
using SharpDX;

namespace RageEngine
{
	public struct IntVector3 : IEquatable<IntVector3>
	{
	    public int x, y, z;

	    public int this[int index]
	    {
	        set { if (index == 0) x = value; else if (index == 1) y = value; else if (index == 2) z = value; }
	        get { if (index == 0) return x; else if (index == 1) return y; else if (index == 2) return z; return 0; }
	    }

	    public IntVector3(int x, int y, int z)
	    {
	        this.x = x;
	        this.y = y;
	        this.z = z;
	    }

	    public int magnitude
	    {
			get
			{
		        return (int) Math.Sqrt(x * x + y * y + z * z);
			}
	    }

	    public int sqrMagnitude
	    {
			get
			{
		        return x * x + y * y + z * z;
			}
	    }

	    public int sumComponents
	    {
			get
			{
		        return x + y + z;
			}
	    }

	    public override bool Equals(object obj)
	    {
	        return obj is IntVector3 && this == (IntVector3)obj;
	    }

	    public override int GetHashCode()
	    {
	        return x | (y << 8) | (z << 16);
	    }

	    public static IntVector3 operator +(IntVector3 left, IntVector3 right)
	    {
	        return new IntVector3(left.x + right.x, left.y + right.y, left.z + right.z);
	    }

	    public static IntVector3 operator -(IntVector3 left, IntVector3 right)
	    {
	        return new IntVector3(left.x - right.x, left.y - right.y, left.z - right.z);
	    }

	    public static IntVector3 operator *(IntVector3 left, int v)
	    {
	        return new IntVector3(left.x * v, left.y * v, left.z * v);
	    }

	    public static IntVector3 operator *(IntVector3 left, IntVector3 right)
	    {
	        return new IntVector3(left.x * right.x, left.y * right.y, left.z * right.z);
	    }

	    public static bool operator !=(IntVector3 left, IntVector3 right)
	    {
	        return left.x != right.x || left.y != right.y || left.z != right.z;
	    }

	    public static bool operator ==(IntVector3 left, IntVector3 right)
	    {
	        return left.x == right.x && left.y == right.y && left.z == right.z;
	    }

	    public bool Equals(IntVector3 other)
	    {
	        return other.x == x && other.y == y && other.z == z;
	    }

	    public override string ToString()
	    {
	        return x + "," + y + "," + z;
	    }

		public Vector3 ToVector3()
		{
			return new Vector3(x, y, z);
		}

		public FVector3 ToFVector3()
		{
			return new FVector3(fint.CreateFromInt(x), fint.CreateFromInt(y), fint.CreateFromInt(z));
		}
	}
}

