using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;

public unsafe readonly struct Vector<TNumber> : IVector<TNumber, Vector<TNumber>>, IDisposable, IEquatable<Vector<TNumber>> where TNumber : unmanaged, INumber<TNumber>
{
    public readonly TNumber* p;
    readonly int d;

    public int Dimension => d;
    public int Size { get; }
    public TNumber this[int i]
    {
        get => p[i];
        set => p[i] = value;
    }

    public Vector(TNumber* p, int d)
    {
        this.p = p;
        this.d = d;

        Size = d * sizeof(TNumber);
    }
    public Vector(int dimension)
    {
        var l = dimension * sizeof(TNumber);
        p = (TNumber*)Memory.Alloc(l);
        d = dimension;

        Size = l;
    }

    public TNumber Abs() => (this * this).Sqrt();

    public void Dispose()
    {
        Memory.Free((nint)p, Size);
    }

    public override bool Equals(object? obj) => obj is Vector<TNumber> vector && Equals(vector);
    public bool Equals(Vector<TNumber> other) => other.d == d && Utils.SequencialEquals(other.p, p, Size);
    public override int GetHashCode() => Utils.GetHashCode(p, Size);

    public static TNumber operator *(Vector<TNumber> a, Vector<TNumber> b)
    {
        var r = TNumber.Zero;
        for (int i = 0; i < a.d; i++)
        {
            r += a[i] * b[i];
        }
        return r;
    }
    public static Vector<TNumber> operator *(TNumber a, Vector<TNumber> b)
    {
        var r = new Vector<TNumber>(b.d);
        for (int i = 0; i < b.d; i++)
        {
            r[i] = a * b[i];
        }
        return r;
    }
    public static Vector<TNumber> operator +(Vector<TNumber> a, Vector<TNumber> b)
    {
        var r = new Vector<TNumber>(a.d);
        for (int i = 0; i < a.d; i++)
        {
            r[i] = a[i] + b[i];
        }
        return r;
    }
    public static Vector<TNumber> operator -(Vector<TNumber> a, Vector<TNumber> b)
    {
        var r = new Vector<TNumber>(a.d);
        for (int i = 0; i < a.d; i++)
        {
            r[i] = a[i] + b[i];
        }
        return r;
    }
    public static bool operator ==(Vector<TNumber> a, Vector<TNumber> b) => a.Equals(b);
    public static bool operator !=(Vector<TNumber> a, Vector<TNumber> b) => !a.Equals(b);
    public static Vector<TNumber> operator -(Vector<TNumber> self)
    {
        var r = new Vector<TNumber>(self.d);
        for (int i = 0; i < self.d; i++)
        {
            r[i] = -self[i];
        }
        return r;
    }
    public static Matrix<TNumber> Direct(Vector<TNumber> a, Vector<TNumber> b)
    {
        var r = new Matrix<TNumber>(a.Size, b.Size);
        var p = r.p;
        for (int i = 0; i < a.d; i++)
        {
            for (int j = 0; j < b.d; j++)
            {
                *p = a[i] * b[j];
                p++;
            }
        }
        return r;
    }
}
