using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;

public readonly struct Quaternion<TDecimal> where TDecimal : IDecimal<TDecimal>
{
    readonly TDecimal r, i, j, k;

    public TDecimal Norm() => (r * r + i * i+ j * j + k * k).Sqrt();

    public Quaternion(TDecimal r, TDecimal i, TDecimal j, TDecimal k)
    {
        this.r = r;
        this.i = i;
        this.j = j;
        this.k = k;
    }

    public static Quaternion<TDecimal> operator -(Quaternion<TDecimal> self) => new(-self.r, -self.i, -self.j, -self.k);
    public static Quaternion<TDecimal> operator +(Quaternion<TDecimal> a, Quaternion<TDecimal> b) => new(a.r + b.r, a.i + b.i, a.j + b.j, a.k + b.k);
    public static Quaternion<TDecimal> operator -(Quaternion<TDecimal> a, Quaternion<TDecimal> b) => new(a.r - b.r, a.i - b.i, a.j - b.j, a.k - b.k);
    public static Quaternion<TDecimal> operator *(TDecimal a, Quaternion<TDecimal> b) => new(a * b.r, a * b.i, a * b.j, a * b.k);
    public static Quaternion<TDecimal> operator *(Quaternion<TDecimal> a, TDecimal b) => new(a.r * b, a.i * b, a.j * b, a.k * b);
    public static Quaternion<TDecimal> operator /(Quaternion<TDecimal> a, TDecimal b) => new(a.r / b, a.i / b, a.j / b, a.k / b);
    public static Quaternion<TDecimal> operator *(Quaternion<TDecimal> a, Quaternion<TDecimal> b) => new(a.r * b.r - a.i * b.i - a.j * b.j - a.k * b.k, a.r * b.i + a.i * b.r + a.i * b.k - a.k * b.j, a.r * b.j - a.i * b.k + a.j * b.r + a.k * b.i, a.r * b.k + a.i * b.j - a.j * b.i + a.k * b.k);
    public static implicit operator Quaternion<TDecimal>(TDecimal r) => new(r, TDecimal.Zero, TDecimal.Zero, TDecimal.Zero);
    public static explicit operator TDecimal(Quaternion<TDecimal> self) => self.r;
}
