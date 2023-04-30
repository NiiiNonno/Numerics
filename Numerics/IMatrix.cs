using System.Diagnostics.CodeAnalysis;

namespace Nonno.Numerics;

public interface IMatrix<TNumber, TSelf> : IEquatable<TSelf> where TNumber : unmanaged, INumber<TNumber> where TSelf : IMatrix<TNumber, TSelf>
{
    (int, int) Type { get; }
    TNumber this[int i, int j] { get; set; }
    TSelf Copy();
    Matrix<TNumber> Transpose();
    static abstract TSelf operator -(TSelf self);
    static abstract TSelf operator +(TSelf a, TSelf b);
    static abstract TSelf operator -(TSelf a, TSelf b);
    static abstract TSelf operator *(TNumber a, TSelf b);
    static abstract TSelf operator *(TSelf a, TNumber b);
    static abstract TSelf operator /(TSelf a, TNumber b);
    static abstract Vector<TNumber> operator *(TSelf a, Vector<TNumber> b);
    static abstract Vector<TNumber> operator *(Vector<TNumber> a, TSelf b);
    static abstract Matrix<TNumber> operator *(TSelf a, Matrix<TNumber> b);
    static abstract Matrix<TNumber> operator *(Matrix<TNumber> a, TSelf b);
    static abstract bool operator ==(TSelf a, TSelf b);
    static abstract bool operator !=(TSelf a, TSelf b);
    static abstract TSelf Zero((int, int) type);
    static abstract Matrix<TNumber> Cast(TSelf self);
    static abstract TSelf Cast(Matrix<TNumber> matrix);
    static abstract implicit operator Span<TNumber>(TSelf self);
    static abstract implicit operator TSelf((Vector<TNumber>, Vector<TNumber>) pair);
}

public unsafe readonly struct Matrix<TNumber> : IMatrix<TNumber, Matrix<TNumber>>, IUnmanagedReference, IFormattable where TNumber : unmanaged, INumber<TNumber>
{
    readonly TNumber* p;
    readonly int m, n;

    public (int, int) Type => (m, n);
    public TNumber this[int i, int j]
    {
        get
        {
            if (m <= unchecked((uint)i)) ThrowHelper.ArgumentOutOfRange(i);
            if (n <= unchecked((uint)j)) ThrowHelper.ArgumentOutOfRange(j);
            return p[n * i + j];
        }
        set
        {
            if (m <= unchecked((uint)i)) ThrowHelper.ArgumentOutOfRange(i);
            if (n <= unchecked((uint)j)) ThrowHelper.ArgumentOutOfRange(j);
            p[n * i + j] = value;
        }
    }

    public Matrix(TNumber* ptr, (int, int) type)
    {
        p = ptr;
        m = type.Item1;
        n = type.Item2;
    }
    public Matrix(nint ptr, (int, int) type) : this((TNumber*)ptr, type) { }
    public Matrix((int, int) type)
    {
        m = type.Item1;
        n = type.Item2;
        p = (TNumber*)IMemory.Default.Alloc(m * n * sizeof(TNumber));
    }

    public Matrix<TNumber> Transpose()
    {
        var s = m * n * sizeof(TNumber);
        var ptr = (TNumber*)IMemory.Default.Alloc(s);
        var r = ptr;
        var ps = stackalloc TNumber*[n];

        var p1 = this.p;
        for (int j = 0; j < n; j++)
        {
            *ptr = *p1++;
            ps[j] = ptr;
            ptr += m;
        }

        for (int i = 1; i < m; i++)
            for (int j = 0; j < n; j++)
                *ps[j]++ = *p1++;
        return new(r, (n, m));
    }

    public Matrix<TNumber> Copy()
    {
        var s = m * n * sizeof(TNumber);
        var ptr = IMemory.Default.Alloc(s);
        Utils.Copy(p, ptr, s);
        return new(ptr, (m, n));
    }
    public bool Equals(Matrix<TNumber> other) => m == other.m && n == other.n && Utils.SequencialEquals(p, other.p, m * n * sizeof(TNumber));
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Matrix<TNumber> m && Equals(m);
    public override int GetHashCode() => (int)(nint)p;
    public string ToString(string? format, IFormatProvider? provider)
    {
        provider ??= Thread.CurrentThread.CurrentCulture;
        var info = provider.GetFormat<MatrixFormatInfo>() ?? MatrixFormatInfo.Default;
        return info.Format<TNumber, Matrix<TNumber>>(this, format, provider);
    }
    public override string ToString() => ToString(null, null);
    public void Delete(IMemory from)
    {
        from.Free((nint)p, n * m * sizeof(TNumber));
    }

    public static Matrix<TNumber> operator -(Matrix<TNumber> self)
    {
        var s = self.m * self.n * sizeof(TNumber);
        var ptr = (TNumber*)IMemory.Default.Alloc(s);
        var r = ptr;
        var p1 = self.p;
        for (int i = 0; i < self.m; i++)
            for (int j = 0; j < self.n; j++)
                *ptr++ = -*p1++;
        return new(r, (self.m, self.n));
    }
    public static Matrix<TNumber> operator +(Matrix<TNumber> a, Matrix<TNumber> b)
    {
        if (a.m != b.m || a.n != b.n) ThrowHelper.InvalidBinaryOperation(a, b);
        var s = a.m * a.n * sizeof(TNumber);
        var ptr = (TNumber*)IMemory.Default.Alloc(s);
        var r = ptr;
        var p1 = a.p;
        var p2 = b.p;
        for (int i = 0; i < a.m; i++)
            for (int j = 0; j < a.n; j++)
                *ptr++ = *p1++ + *p2++;
        return new(r, (a.m, a.n));
    }
    public static Matrix<TNumber> operator -(Matrix<TNumber> a, Matrix<TNumber> b)
    {
        if (a.m != b.m || a.n != b.n) ThrowHelper.InvalidBinaryOperation(a, b);
        var s = a.m * a.n * sizeof(TNumber);
        var ptr = (TNumber*)IMemory.Default.Alloc(s);
        var r = ptr;
        var p1 = a.p;
        var p2 = b.p;
        for (int i = 0; i < a.m; i++)
            for (int j = 0; j < a.n; j++)
                *ptr++ = *p1++ - *p2++;
        return new(r, (a.m, a.n));
    }
    public static Matrix<TNumber> operator *(TNumber a, Matrix<TNumber> b)
    {
        var s = b.m * b.n * sizeof(TNumber);
        var ptr = (TNumber*)IMemory.Default.Alloc(s);
        var r = ptr;
        var p1 = b.p;
        for (int i = 0; i < b.m; i++)
            for (int j = 0; j < b.n; j++)
                *ptr++ = a * *p1++;
        return new(r, (b.m, b.n));
    }
    public static Matrix<TNumber> operator *(Matrix<TNumber> a, TNumber b)
    {
        var s = a.m * a.n * sizeof(TNumber);
        var ptr = (TNumber*)IMemory.Default.Alloc(s);
        var r = ptr;
        var p1 = a.p;
        for (int i = 0; i < a.m; i++)
            for (int j = 0; j < a.n; j++)
                *ptr++ = *p1++ * b;
        return new(r, (a.m, a.n));
    }
    public static Matrix<TNumber> operator /(Matrix<TNumber> a, TNumber b)
    {
        var s = a.m * a.n * sizeof(TNumber);
        var ptr = (TNumber*)IMemory.Default.Alloc(s);
        var r = ptr;
        var p1 = a.p;
        for (int i = 0; i < a.m; i++)
            for (int j = 0; j < a.n; j++)
                *ptr++ = *p1++ / b;
        return new(r, (a.m, a.n));
    }
    public static Vector<TNumber> operator *(Matrix<TNumber> a, Vector<TNumber> b)
    {
        if (a.n != b.Dimension) ThrowHelper.InvalidBinaryOperation(a, b);
        var ptr = (TNumber*)IMemory.Default.Alloc(a.m * sizeof(TNumber));
        var r = ptr;
        var p1 = a.p;
        for (int i = 0; i < a.m; i++)
        {
            var p2 = (TNumber*)b;
            *ptr = TNumber.Zero;
            for (int j = 0; j < a.n; j++)
            {
                *ptr += *p1++ * *p2++;
            }
            ptr++;
        }
        return new(r, a.n);
    }
    public static Vector<TNumber> operator *(Vector<TNumber> a, Matrix<TNumber> b)
    {
        if (a.Dimension != b.m) ThrowHelper.InvalidBinaryOperation(a, b);
        var l = a.Dimension * b.n;
        var ptr = (TNumber*)IMemory.Default.Alloc(l * sizeof(TNumber));
        var r = ptr;
        new Span<TNumber>(ptr, l).Clear();
        var p1 = (TNumber*)a;
        var p2 = b.p;
        for (int j = 0; j < b.m; j++)
        {
            ptr = r;
            for (int k = 0; k < b.n; k++)
            {
                *ptr++ += *p1 * *p2++;
            }
            p1++;
        }
        return new(r, b.n);
    }
    public static Matrix<TNumber> operator *(Matrix<TNumber> a, Matrix<TNumber> b)
    {
        if (a.n != b.m) ThrowHelper.InvalidBinaryOperation(a, b);
        var l = a.m * b.n;
        var ptr = (TNumber*)IMemory.Default.Alloc(l * sizeof(TNumber));
        var r = ptr;
        new Span<TNumber>(ptr, l).Clear();
        var p1 = a.p;
        for (int i = 0; i < a.m; i++)
        {
            var ptr_r = ptr;
            var p2 = b.p;
            for (int j = 0; j < a.n; j++)
            {
                ptr = ptr_r;
                for (int k = 0; k < b.n; k++)
                {
                    *ptr++ += *p1 * *p2++;
                }
                p1++;
            }
        }
        return new(r, (a.m, b.n));
    }
    public static bool operator ==(Matrix<TNumber> a, Matrix<TNumber> b) => a.Equals(b);
    public static bool operator !=(Matrix<TNumber> a, Matrix<TNumber> b) => !a.Equals(b);
    public static Matrix<TNumber> Cast(Matrix<TNumber> self) => self;
    public static Matrix<TNumber> Zero((int, int) type)
    {
        var m = type.Item1;
        var n = type.Item2;
        var l = m * n;
        var s = l * sizeof(TNumber);
        var ptr = (TNumber*)IMemory.Default.Alloc(s);
        new Span<TNumber>(ptr, l).Clear();
        return new(ptr, (m, n));
    }
    public static explicit operator TNumber*(Matrix<TNumber> self) => self.p;
    public static implicit operator Span<TNumber>(Matrix<TNumber> self) => new(self.p, self.m * self.n);
    public static implicit operator Matrix<TNumber>((Vector<TNumber>, Vector<TNumber>) pair)
    {
        var m = pair.Item1.Dimension;
        var n = pair.Item2.Dimension;
        var s = m * n * sizeof(TNumber);
        var ptr = (TNumber*)IMemory.Default.Alloc(s);
        var r = ptr;
        var p1 = (TNumber*)pair.Item1;
        for (int i = 0; i < m; i++)
        {
            var p2 = (TNumber*)pair.Item2;
            for (int j = 0; j < n; j++)
            {
                *ptr++ = *p1 * *p2++;
            }
            p1++;
        }
        return new(r, (m, n));
    }
}