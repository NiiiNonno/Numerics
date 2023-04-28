using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
public unsafe readonly struct Matrix<TNumber> : IMatrix<TNumber, Vector<TNumber>, Matrix<TNumber>>, IDisposable where TNumber : unmanaged, INumber<TNumber>
{
    public readonly TNumber* p;
    readonly int m;
    readonly int n;

    public (int, int) Type => (m, n);
    public int Size { get; }
    public Vector<TNumber> this[int i]
    {
        get => new(p + i * sizeof(TNumber), n);
        set
        {
            if (n != value.Dimension) ThrowHelper.InvalidArgument(value);
            Utils.Copy(p + i * sizeof(TNumber), value.p, n);
        }
    }
    public TNumber this[int i, int j]
    {
        get => p[i * sizeof(TNumber) + j];
        set => p[i * sizeof(TNumber) + j] = value;
    }

    public Matrix(TNumber* p, int m, int n)
    {
        this.p = p;
        this.m = m;
        this.n = n;

        Size = m * n * sizeof(TNumber);
    }
    public Matrix(int m, int n)
    {
        var s = m * n * sizeof(TNumber);
        this.p = (TNumber*)Memory.Alloc(s);
        this.m = m;
        this.n = n;

        Size = s;
    }

    public Matrix<TNumber> Transpose()
    {
        var r = new Matrix<TNumber>(m:n, n:m);
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

    public void Dispose()
    {
        Memory.Free((nint)p, Size);
    }

    public static Vector<TNumber> operator *(Matrix<TNumber> a, Vector<TNumber> b)
    {
        if (a.n != b.Dimension) ThrowHelper.InvalidArgument(b);

        var p = (TNumber*)Memory.Alloc(a.m * sizeof(TNumber));
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
}
