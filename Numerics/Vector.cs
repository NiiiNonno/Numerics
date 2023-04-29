using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;

public unsafe readonly struct Vector<TNumber> : IVector<TNumber, Vector<TNumber>>, IUnmanagedReference, IEquatable<Vector<TNumber>>, IFormattable where TNumber : unmanaged, INumber<TNumber>
{
    readonly TNumber* p;
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
    public Vector(nint p, int d) : this((TNumber*)p, d) { }
    public Vector(IMemory memory, int dimension)
    {
        var l = dimension * sizeof(TNumber);
        p = (TNumber*)memory.Alloc(l);
        d = dimension;

        Size = l;
    }
    public Vector(int dimension) : this(IMemory.Default, dimension) { }

    public TNumber Abs() => (this * this).Sqrt();

    public Matrix<TNumber> AsColumn() => new(p, d, 1);
    public Matrix<TNumber> AsRow() => new(p, 1, d);

    public Vector<TNumber> Copy()
    {
        var p = IMemory.Default.Alloc(Size);
        Utils.Copy(this.p, (void*)p, Size);
        return new((TNumber*)p, d);
    }

    public void Delete(IMemory from)
    {
        from.Free((nint)p, Size);
    }

    public override bool Equals(object? obj) => obj is Vector<TNumber> vector && Equals(vector);
    public bool Equals(Vector<TNumber> other) => other.d == d && Utils.SequencialEquals(other.p, p, Size);
    public override int GetHashCode() => Utils.GetHashCode(p, Size);

    public string ToString(string? format, IFormatProvider? provider)
    {
        provider ??= Thread.CurrentThread.CurrentCulture;
        var info = provider.GetFormat<MatrixFormatInfo>();
        if (info is null) return ToString();
        else return info.Format<TNumber, Vector<TNumber>, Matrix<TNumber>>(format, AsRow(), provider);
    }
    public override string ToString()
    {
        StringBuilder r = new();
        for (int i = 0; i < d; i++)
        {
            _ = r.Append(this[i].ToString());
            _ = r.Append('\t');
        }
        return r.ToString();
    }

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
        var r = new Vector<TNumber>(IMemory.Default, b.d);
        for (int i = 0; i < b.d; i++)
        {
            r[i] = a * b[i];
        }
        return r;
    }
    public static Vector<TNumber> operator +(Vector<TNumber> a, Vector<TNumber> b)
    {
        var r = new Vector<TNumber>(IMemory.Default, a.d);
        for (int i = 0; i < a.d; i++)
        {
            r[i] = a[i] + b[i];
        }
        return r;
    }
    public static Vector<TNumber> operator -(Vector<TNumber> a, Vector<TNumber> b)
    {
        var r = new Vector<TNumber>(IMemory.Default, a.d);
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
        var r = new Vector<TNumber>(IMemory.Default, self.d);
        for (int i = 0; i < self.d; i++)
        {
            r[i] = -self[i];
        }
        return r;
    }
    public static Vector<TNumber> Zero(int dimension) 
    {
        var r = new Vector<TNumber>(IMemory.Default, dimension);
        for (int i = 0; i < dimension; i++)
        {
            r.p[i] = default;
        }
        return r;
    }
    public static Matrix<TNumber> Direct(Vector<TNumber> a, Vector<TNumber> b)
    {
        var r = new Matrix<TNumber>(a.d, b.d);
        var p = (TNumber*)r;
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
    public static explicit operator TNumber*(Vector<TNumber> self) => self.p;
}
