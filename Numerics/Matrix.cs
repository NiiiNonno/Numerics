using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Nonno.Numerics;
public unsafe readonly struct Matrix<TNumber> : IMatrix<TNumber, Vector<TNumber>, Matrix<TNumber>>, IUnmanagedReference, IFormattable where TNumber : unmanaged, INumber<TNumber>
{
    readonly TNumber* p;
    readonly int m;
    readonly int n;

    public (int, int) Type => (m, n);
    public int Size { get; }
    public Vector<TNumber> this[int i]
    {
        get => new(p + i * n, n);
        set
        {
            if (n != value.Dimension) ThrowHelper.InvalidArgument(value);
            Utils.Copy(p + i * n, (TNumber*)value, n);
        }
    }
    public TNumber this[int i, int j]
    {
        get => p[i * n + j];
        set => p[i * n + j] = value;
    }

    public Matrix(TNumber* p, int m, int n)
    {
        this.p = p;
        this.m = m;
        this.n = n;

        Size = m * n * sizeof(TNumber);
    }
    public Matrix(IMemory memory, int m, int n)
    {
        var s = m * n * sizeof(TNumber);
        this.p = (TNumber*)memory.Alloc(s);
        this.m = m;
        this.n = n;

        Size = s;
    }
    public Matrix(int m, int n) : this(IMemory.Default, m, n) { }

    public Matrix<TNumber> Transpose()
    {
        var r = new Matrix<TNumber>(m: n, n: m);
        var ps = stackalloc TNumber*[n];

        int i = 0;
        var c = this.p;
        var p = r.p + 1;
        for (int j = 0; j < n; j++)
        {
            *p = *c;
            c++;

            ps[j] = p;
            p += m;
        }

        for (i = 1; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                *ps[j] = *c;
                ps[j]++;
                c++;
            }
        }

        return r;
    }

    public void Delete(IMemory from)
    {
        from.Free((nint)p, Size);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        formatProvider ??= Thread.CurrentThread.CurrentCulture;
        var info = formatProvider.GetFormat<MatrixFormatInfo>();
        if (info is null) return ToString();
        else return info.Format<TNumber, Vector<TNumber>, Matrix<TNumber>>(format, this, formatProvider);
    }
    public override string ToString()
    {
        StringBuilder b = new();
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                _ = b.Append(this[i, j].ToString());
                _ = b.Append('\t');
            }
            _ = b.AppendLine();
        }
        return b.ToString();
    }

    public static Vector<TNumber> operator *(Matrix<TNumber> a, Vector<TNumber> b)
    {
        if (a.n != b.Dimension) ThrowHelper.InvalidArgument(b);

        var p = (TNumber*)IMemory.Default.Alloc(a.m * sizeof(TNumber));
        for (int i = 0; i < a.m; i++)
        {
            p[i] = a[i] * b;
        }
        return new(p, a.m);
    }
    public static Matrix<TNumber> operator *(TNumber a, Matrix<TNumber> b)
    {
        var r = new Matrix<TNumber>(m: b.m, n: b.n);
        var l = b.Size;
        for (int i = 0; i < l; i++)
        {
            r.p[i] = a * b.p[i];
        }
        return r;
    }
    public static Matrix<TNumber> operator *(Matrix<TNumber> a, Matrix<TNumber> b)
    {
        if (a.n != b.m) ThrowHelper.ArgumentOutOfRange(b);

        var r = new Matrix<TNumber>(a.m, b.n);
        for (int i = 0; i < a.m; i++)
        {
            for (int j = 0; j < b.n; j++)
            {
                TNumber n = TNumber.Zero;
                for (int k = 0; k < a.n; k++)
                {
                    n += a[i, k] * b[k, j];
                }
                r[i, j] = n;
            }
        }
        return r;
    }
    public static Matrix<TNumber> operator /(Matrix<TNumber> a, TNumber b)
    {
        var r = new Matrix<TNumber>(m: a.m, n: a.n);
        var l = a.Size;
        for (int i = 0; i < l; i++)
        {
            r.p[i] = a.p[i] / b;
        }
        return r;
    }
    public static Matrix<TNumber> operator +(Matrix<TNumber> a, Matrix<TNumber> b)
    {
        if (a.m != b.m || a.n != b.n) ThrowHelper.InvalidArgument(b);

        var r = new Matrix<TNumber>(m: b.m, n: b.n);
        var l = b.Size;
        for (int i = 0; i < l; i++)
        {
            r.p[i] = a.p[i] + b.p[i];
        }
        return r;
    }
    public static Matrix<TNumber> operator -(Matrix<TNumber> a, Matrix<TNumber> b)
    {
        if (a.m != b.m || a.n != b.n) ThrowHelper.InvalidArgument(b);

        var r = new Matrix<TNumber>(m: b.m, n: b.n);
        var l = b.Size;
        for (int i = 0; i < l; i++)
        {
            r.p[i] = a.p[i] - b.p[i];
        }
        return r;
    }
    public static Matrix<TNumber> operator -(Matrix<TNumber> self)
    {
        var r = new Matrix<TNumber>(m: self.m, n: self.n);
        var l = self.Size;
        for (int i = 0; i < l; i++)
        {
            r.p[i] = -self.p[i];
        }
        return r;
    }
    public static explicit operator TNumber*(Matrix<TNumber> self) => self.p;
    public static Matrix<TNumber> Zero((int, int) type)
    {
        var r = new Matrix<TNumber>(m: type.Item1, n: type.Item2);
        var l = r.m * r.n;
        for (int i = 0; i < l; i++)
        {
            r.p[i] = default;
        }
        return r;
    }
}