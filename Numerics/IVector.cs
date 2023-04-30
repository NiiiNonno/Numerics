using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Nonno.Numerics;

public interface IVector<TNumber, TSelf> : IEquatable<TSelf> where TNumber : unmanaged, INumber<TNumber> where TSelf : IVector<TNumber, TSelf>
{
    int Dimension { get; }
    TNumber this[int i] { get; set; }
    TSelf Copy();
    Matrix<TNumber> Row();
    Matrix<TNumber> Column();
    static abstract TSelf operator -(TSelf self);
    static abstract TSelf operator +(TSelf a, TSelf b);
    static abstract TSelf operator -(TSelf a, TSelf b);
    static abstract TSelf operator *(TNumber a, TSelf b);
    static abstract TSelf operator *(TSelf a, TNumber b);
    static abstract TSelf operator /(TSelf a, TNumber b);
    static abstract TNumber operator *(TSelf a, TSelf b);
    static abstract bool operator ==(TSelf a, TSelf b);
    static abstract bool operator !=(TSelf a, TSelf b);
    static abstract TSelf Zero(int dimension);
    static abstract Vector<TNumber> Cast(TSelf self);
    static abstract TSelf Cast(Vector<TNumber> vector);
    static abstract implicit operator Span<TNumber>(TSelf self);
}

public unsafe readonly struct Vector<TNumber> : IVector<TNumber, Vector<TNumber>>, IUnmanagedReference, IFormattable where TNumber : unmanaged, INumber<TNumber>
{
    readonly TNumber* p;
    readonly int d;
    readonly int s;

    public TNumber this[int i]
    {
        get
        {
            if (d <= unchecked((uint)i)) ThrowHelper.ArgumentOutOfRange(i);
            return p[i];
        }
        set
        {
            if (d <= unchecked((uint)i)) ThrowHelper.ArgumentOutOfRange(i);
            p[i] = value;
        }
    }
    public int Dimension => d;
    public int Size => s;

    internal Vector(TNumber* ptr, int dimension, int size)
    {
        p = ptr;
        d = dimension;
        s = size;
    }
    public Vector(nint ptr, int dimension) : this((TNumber*)ptr, dimension) { }
    public Vector(TNumber* ptr, int dimension) : this(ptr, dimension, dimension * sizeof(TNumber)) { }
    public Vector(int dimension)
    {
        d = dimension;
        s = dimension * sizeof(TNumber);
        p = (TNumber*)IMemory.Default.Alloc(s);
    }

    public Vector<TNumber> Copy()
    {
        var ptr = IMemory.Default.Alloc(s);
        Utils.Copy(p, ptr, s);
        return new(ptr, d);
    }
    public Matrix<TNumber> Column() => new(p, (d, 1));
    public Matrix<TNumber> Row() => new(p, (1, d));
    public bool Equals(Vector<TNumber> other) => d == other.d && Utils.SequencialEquals(p, other.p, s);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Vector<TNumber> v && Equals(v);
    public override int GetHashCode() => (int)(nint)p;
    public string ToString(string? format, IFormatProvider? provider)
    {
        provider ??= Thread.CurrentThread.CurrentCulture;
        var info = provider.GetFormat<MatrixFormatInfo>() ?? MatrixFormatInfo.Default;
        return info.Format<TNumber, Matrix<TNumber>>(Row(), format, provider);
    }
    public override string ToString() => ToString(null, null);
    public void Delete(IMemory from) => from.Free((nint)p, s);

    public static Vector<TNumber> operator -(Vector<TNumber> a)
    {
        var ptr = (TNumber*)IMemory.Default.Alloc(a.s);
        var r = ptr;
        var p1 = a.p;
        for (int i = 0; i < a.d; i++)
            *ptr++ = -*p1++;
        return new(r, a.d, a.s);
    }
    public static Vector<TNumber> operator +(Vector<TNumber> a, Vector<TNumber> b)
    {
        if (a.d != b.d) ThrowHelper.InvalidBinaryOperation(a, b);
        var ptr = (TNumber*)IMemory.Default.Alloc(a.s);
        var r = ptr;
        var p1 = a.p;
        var p2 = b.p;
        for (int i = 0; i < a.d; i++)
            *ptr++ = *p1++ + *p2++;
        return new(r, a.d, a.s);
    }
    public static Vector<TNumber> operator -(Vector<TNumber> a, Vector<TNumber> b)
    {
        if (a.d != b.d) ThrowHelper.InvalidBinaryOperation(a, b);
        var ptr = (TNumber*)IMemory.Default.Alloc(a.s);
        var r = ptr;
        var p1 = a.p;
        var p2 = b.p;
        for (int i = 0; i < a.d; i++)
            *ptr++ = *p1++ - *p2++;
        return new(r, a.d, a.s);
    }
    public static Vector<TNumber> operator *(TNumber a, Vector<TNumber> b)
    {
        var ptr = (TNumber*)IMemory.Default.Alloc(b.s);
        var r = ptr;
        var p1 = b.p;
        for (int i = 0; i < b.d; i++)
            *ptr++ = a * *p1++;
        return new(r, b.d, b.s);
    }
    public static Vector<TNumber> operator *(Vector<TNumber> a, TNumber b)
    {
        var ptr = (TNumber*)IMemory.Default.Alloc(a.s);
        var r = ptr;
        var p1 = a.p;
        for (int i = 0; i < a.d; i++)
            *ptr++ = *p1++ * b;
        return new(r, a.d, a.s);
    }
    public static Vector<TNumber> operator /(Vector<TNumber> a, TNumber b)
    {
        var ptr = (TNumber*)IMemory.Default.Alloc(a.s);
        var r = ptr;
        var p1 = a.p;
        for (int i = 0; i < a.d; i++)
            *ptr++ = *p1++ / b;
        return new(r, a.d, a.s);
    }
    public static TNumber operator *(Vector<TNumber> a, Vector<TNumber> b)
    {
        var r = TNumber.Zero;
        var p1 = a.p;
        var p2 = b.p;
        for (int i = 0; i < a.d; i++)
            r += *p1++ * *p2++;
        return r;
    }
    public static bool operator ==(Vector<TNumber> a, Vector<TNumber> b) => a.Equals(b);
    public static bool operator !=(Vector<TNumber> a, Vector<TNumber> b) => !a.Equals(b);
    public static Vector<TNumber> Zero(int dimension)
    {
        var s = dimension * sizeof(TNumber);
        var ptr = (TNumber*)IMemory.Default.Alloc(s);
        new Span<TNumber>(ptr, dimension).Clear();
        return new(ptr, dimension, s);
    }
    public static Vector<TNumber> Cast(Vector<TNumber> self) => self;
    public static explicit operator TNumber*(Vector<TNumber> self) => self.p;
    public static implicit operator Span<TNumber>(Vector<TNumber> self) => new(self.p, self.d);
}

public unsafe readonly struct Vector3<TNumber> : IVector<TNumber, Vector3<TNumber>> where TNumber : unmanaged, INumber<TNumber>
{
    const int DIMENSION = 3;

    readonly TNumber* p;

    public TNumber this[int i]
    {
        get
        {
            if (DIMENSION <= unchecked((uint)i)) ThrowHelper.ArgumentOutOfRange(i);
            return p[i];
        }
        set
        {
            if (DIMENSION <= unchecked((uint)i)) ThrowHelper.ArgumentOutOfRange(i);
            p[i] = value;
        }
    }
    public int Dimension => DIMENSION;

    public Vector3(TNumber* ptr)
    {
        p = ptr;
    }
    public Vector3(nint ptr) : this((TNumber*)ptr) { }
    public Vector3()
    {
        p = (TNumber*)IMemory.Default.Alloc(3 * sizeof(TNumber));
    }

    public Matrix<TNumber> Column()
    {
        throw new NotImplementedException();
    }
    public Matrix<TNumber> Row()
    {
        throw new NotImplementedException();
    }
    public Vector3<TNumber> Copy()
    {
        throw new NotImplementedException();
    }
    public bool Equals(Vector3<TNumber> other) => p[0] == other.p[0] && p[1] == other.p[1] && p[2] == other.p[2];
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Vector3<TNumber> v && Equals(v);
    public override int GetHashCode() => (int)(nint)p;

    public static Vector3<TNumber> operator -(Vector3<TNumber> self)
    {
        var ptr = (TNumber*)IMemory.Default.Alloc(3 * sizeof(TNumber));
        ptr[0] = -self.p[0];
        ptr[1] = -self.p[1];
        ptr[2] = -self.p[2];
        return new(ptr);
    }
    public static Vector3<TNumber> operator +(Vector3<TNumber> a, Vector3<TNumber> b)
    {
        var ptr = (TNumber*)IMemory.Default.Alloc(3 * sizeof(TNumber));
        ptr[0] = a.p[0] + b.p[0];
        ptr[1] = a.p[1] + b.p[1];
        ptr[2] = a.p[2] + b.p[2];
        return new(ptr);
    }
    public static Vector3<TNumber> operator -(Vector3<TNumber> a, Vector3<TNumber> b)
    {
        var ptr = (TNumber*)IMemory.Default.Alloc(3 * sizeof(TNumber));
        ptr[0] = a.p[0] - b.p[0];
        ptr[1] = a.p[1] - b.p[1];
        ptr[2] = a.p[2] - b.p[2];
        return new(ptr);
    }
    public static Vector3<TNumber> operator *(TNumber a, Vector3<TNumber> b)
    {
        var ptr = (TNumber*)IMemory.Default.Alloc(3 * sizeof(TNumber));
        ptr[0] = a * b.p[0];
        ptr[1] = a * b.p[1];
        ptr[2] = a * b.p[2];
        return new(ptr);
    }
    public static Vector3<TNumber> operator *(Vector3<TNumber> a, TNumber b)
    {
        var ptr = (TNumber*)IMemory.Default.Alloc(3 * sizeof(TNumber));
        ptr[0] = a.p[0] * b;
        ptr[1] = a.p[1] * b;
        ptr[2] = a.p[2] * b;
        return new(ptr);
    }
    public static Vector3<TNumber> operator /(Vector3<TNumber> a, TNumber b)
    {
        var ptr = (TNumber*)IMemory.Default.Alloc(3 * sizeof(TNumber));
        ptr[0] = a.p[0] / b;
        ptr[1] = a.p[1] / b;
        ptr[2] = a.p[2] / b;
        return new(ptr);
    }
    public static TNumber operator *(Vector3<TNumber> a, Vector3<TNumber> b) => a.p[0] * b.p[0] + a.p[1] * b.p[1] + a.p[2] * b.p[2];
    public static bool operator ==(Vector3<TNumber> a, Vector3<TNumber> b) => a.Equals(b);
    public static bool operator !=(Vector3<TNumber> a, Vector3<TNumber> b) => !a.Equals(b);
    public static Vector<TNumber> Cast(Vector3<TNumber> self) => new(self.p, 3);
    public static Vector3<TNumber> Cast(Vector<TNumber> vector)
    {
        if (DIMENSION < vector.Dimension) ThrowHelper.InvalidArgument(vector);
        return new((TNumber*)vector);
    }
#warning 如何せん。
    public static Vector3<TNumber> Zero(int dimension)
    {
        throw new NotImplementedException();
    }
    public static implicit operator Span<TNumber>(Vector3<TNumber> self) => new(self.p, DIMENSION);
}
