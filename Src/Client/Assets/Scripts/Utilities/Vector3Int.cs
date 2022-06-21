
using System;
using System.Runtime.CompilerServices;
using Protocol;
using UnityEngine;

public class Vector3Int : IEquatable<Vector3Int>
{

    #region Private Fields

    private int m_X;

    private int m_Y;

    private int m_Z;

    private static readonly Vector3Int s_Zero = new Vector3Int(0, 0, 0);

    private static readonly Vector3Int s_One = new Vector3Int(1, 1, 1);

    private static readonly Vector3Int s_Up = new Vector3Int(0, 1, 0);

    private static readonly Vector3Int s_Down = new Vector3Int(0, -1, 0);

    private static readonly Vector3Int s_Left = new Vector3Int(-1, 0, 0);

    private static readonly Vector3Int s_Right = new Vector3Int(1, 0, 0);

    #endregion

    #region Public Properties

    /// <summary>
    ///   <para>X component of the vector.</para>
    /// </summary>
    public int x
    {
        get
        {
            return m_X;
        }
        set
        {
            m_X = value;
        }
    }

    /// <summary>
    ///   <para>Y component of the vector.</para>
    /// </summary>
    public int y
    {
        get
        {
            return m_Y;
        }
        set
        {
            m_Y = value;
        }
    }

    /// <summary>
    ///   <para>Z component of the vector.</para>
    /// </summary>
    public int z
    {
        get
        {
            return m_Z;
        }
        set
        {
            m_Z = value;
        }
    }

    /// <summary>
    ///   <para>Returns the length of this vector (Read Only).</para>
    /// </summary>
    public float Magnitude => (float)Math.Sqrt(x * x + y * y + z * z);

    /// <summary>
    ///   <para>Returns the squared length of this vector (Read Only).</para>
    /// </summary>
    public int SqrMagnitude => x * x + y * y + z * z;

    /// <summary>
    ///   <para>Shorthand for writing Vector3Int (0, 0, 0).</para>
    /// </summary>
    public static Vector3Int Zero => s_Zero;

    /// <summary>
    ///   <para>Shorthand for writing Vector3Int (1, 1, 1).</para>
    /// </summary>
    public static Vector3Int One => s_One;

    /// <summary>
    ///   <para>Shorthand for writing Vector3Int (0, 1, 0).</para>
    /// </summary>
    public static Vector3Int Up => s_Up;

    /// <summary>
    ///   <para>Shorthand for writing Vector3Int (0, -1, 0).</para>
    /// </summary>
    public static Vector3Int Down => s_Down;

    /// <summary>
    ///   <para>Shorthand for writing Vector3Int (-1, 0, 0).</para>
    /// </summary>
    public static Vector3Int Left => s_Left;

    /// <summary>
    ///   <para>Shorthand for writing Vector3Int (1, 0, 0).</para>
    /// </summary>
    public static Vector3Int Right => s_Right;
    #endregion

    #region Constructor
    public Vector3Int()
    {
        m_X = 0;
        m_Y = 0;
        m_Z = 0;
    }
    public Vector3Int(int x, int y, int z)
    {
        m_X = x;
        m_Y = y;
        m_Z = z;
    }

    #endregion

    #region Indexer

    public int this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return x;
                case 1:
                    return y;
                case 2:
                    return z;
                default:
                    throw new IndexOutOfRangeException(string.Format("Invalid Vector3Int index addressed: {0}!", index));
            }
        }
        set
        {
            switch (index)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                default:
                    throw new IndexOutOfRangeException(string.Format("Invalid Vector3Int index addressed: {0}!", index));
            }
        }
    }

    #endregion

    #region Static Methods

    /// <summary>
    ///   <para>Returns the distance between a and b.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static float Distance(Vector3Int a, Vector3Int b)
    {
        return (a - b).Magnitude;
    }

    /// <summary>
    ///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static Vector3Int Min(Vector3Int lhs, Vector3Int rhs)
    {
        return new Vector3Int(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
    }

    /// <summary>
    ///   <para>Returns a vector that is made from the largest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static Vector3Int Max(Vector3Int lhs, Vector3Int rhs)
    {
        return new Vector3Int(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
    }

    /// <summary>
    ///   <para>Multiplies two vectors component-wise.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static Vector3Int Scale(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    #endregion

    #region Public Methods

    /// <summary>
    ///   <para>Multiplies every component of this vector by the same component of scale.</para>
    /// </summary>
    /// <param name="scale"></param>
    public void Scale(Vector3Int scale)
    {
        x *= scale.x;
        y *= scale.y;
        z *= scale.z;
    }

    /// <summary>
    ///   <para>Clamps the Vector3Int to the bounds given by min and max.</para>
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void Clamp(Vector3Int min, Vector3Int max)
    {
        x = Math.Max(min.x, x);
        x = Math.Min(max.x, x);
        y = Math.Max(min.y, y);
        y = Math.Min(max.y, y);
        z = Math.Max(min.z, z);
        z = Math.Min(max.z, z);
    }
    
    /// <summary>
    ///   <para>Gets the hash code for the Vector3Int.</para>
    /// </summary>
    /// <returns>
    ///   <para>The hash code of the Vector3Int.</para>
    /// </returns>
    public override int GetHashCode()
    {
        return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
    }

    /// <summary>
    ///   <para>Returns a nicely formatted string for this vector.</para>
    /// </summary>
    /// <param name="format"></param>
    public override string ToString()
    {
        return string.Format("({0}, {1}, {2})", x, y, z);
    }

    /// <summary>
    ///   <para>Returns a nicely formatted string for this vector.</para>
    /// </summary>
    /// <param name="format"></param>
    public string ToString(string format)
    {
        return string.Format("({0}, {1}, {2})", x.ToString(format), y.ToString(format), z.ToString(format));
    }
    #endregion

    #region Operator Overloading 

    public static implicit operator NVector3(Vector3Int v)
    {
        return new NVector3() { X = v.x, Y = v.y, Z = v.z };
    }

    public static implicit operator Vector3Int(NVector3 v)
    {
        return new Vector3Int(v.X, v.Y, v.Z);
    }

    public static implicit operator Vector3(Vector3Int v)
    {
        return new Vector3(){
            x = v.x / 100f,
            y = v.y / 100f,
            z = v.z / 100f,
        };
    }

    public static implicit operator Vector3Int(Vector3 v)
    {
        return new Vector3Int(){
            x = Mathf.RoundToInt(v.x) * 100,
            y = Mathf.RoundToInt(v.y) * 100,
            z = Mathf.RoundToInt(v.z) * 100,

        };
    }
    
    public static Vector3Int operator +(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vector3Int operator -(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vector3Int operator *(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector3Int operator *(Vector3Int a, int b)
    {
        return new Vector3Int(a.x * b, a.y * b, a.z * b);
    }

    public static bool operator ==(Vector3Int lhs, Vector3Int rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }

    public static bool operator !=(Vector3Int lhs, Vector3Int rhs)
    {
        return !(lhs == rhs);
    }
    #endregion

    #region Interface

    /// <summary>
    ///   <para>Returns true if the objects are equal.</para>
    /// </summary>
    /// <param name="other"></param>
    public override bool Equals(object other)
    {
        if (other is Vector3Int)
        {
            return Equals((Vector3Int)other);
        }
        return false;
    }

    public bool Equals(Vector3Int other)
    {
        return this == other;
    }

    /// <summary>
    ///   <para>Gets the hash code for the Vector3Int.</para>
    /// </summary>
    /// <returns>
    ///   <para>The hash code of the Vector3Int.</para>
    /// </returns>

    #endregion
}

