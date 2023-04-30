// 令和弐年大暑確認済。
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Nonno.Numerics;

public readonly struct Complex<TDecimal> : IFormattable where TDecimal : IDecimal<TDecimal>
{
    public readonly TDecimal r, i;
    public TDecimal Abs => (r * r + i * i).Sqrt();
    public TDecimal Arg => TDecimal.Atan2(i, r) * ConstantValues<TDecimal>.PI_RECIPRO;
    public Complex<TDecimal> Exp
    {
        get
        {
            var v = TDecimal.Exp(r);
            return new Complex<TDecimal>(v * TDecimal.Cos(i), v * TDecimal.Sin(i));
        }
    }
    public Complex<TDecimal> Log => new(TDecimal.Abs(i), Arg);
    public bool IsReal => i == TDecimal.Zero;
    public Complex(TDecimal real, TDecimal imaginary)
    {
        r = real;
        i = imaginary;
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public bool IsLess(in Complex<TDecimal> than) => r * r + i * i < than.r * than.r + than.i * than.i;
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public bool IsMore(in Complex<TDecimal> than) => r * r + i * i > than.r * than.r + than.i * than.i;
    public override string ToString() => ToString(null, null);
    public string ToString(string? format, IFormatProvider? provider)
    {
        provider ??= Thread.CurrentThread.CurrentCulture;
        var info = provider.GetFormat<ComplexFormatInfo>() ?? ComplexFormatInfo.Default;
        return info.Format(this, format, provider);
    }
    public static Complex<TDecimal> Pow(TDecimal a, in Complex<TDecimal> b) => (b * TDecimal.Log(a)).Exp;
    public static Complex<TDecimal> Pow(in Complex<TDecimal> a, TDecimal b) => (b * a.Log).Exp;
    public static Complex<TDecimal> Pow(in Complex<TDecimal> a, in Complex<TDecimal> b) => (b * a.Log).Exp;
    public static Complex<TDecimal> operator +(in Complex<TDecimal> a, in Complex<TDecimal> b) => new(a.r + b.r, a.i + b.i);
    public static Complex<TDecimal> operator -(in Complex<TDecimal> a, in Complex<TDecimal> b) => new(a.r - b.r, a.i - b.i);
    public static Complex<TDecimal> operator *(in Complex<TDecimal> a, in Complex<TDecimal> b) => new(a.r * b.r - a.i * b.i, a.r * b.i + a.i * b.r);
    public static Complex<TDecimal> operator *(TDecimal a, in Complex<TDecimal> b) => new(a * b.r, a * b.i);
    public static Complex<TDecimal> operator *(in Complex<TDecimal> a, TDecimal b) => new(a.r * b, a.i * b);
    public static Complex<TDecimal> operator /(in Complex<TDecimal> a, TDecimal b)
    {
        var v0 = (TDecimal)1 / b;
        var v1 = a.r * v0;
        return new Complex<TDecimal>(v1, v0 * v1 * a.i);
    }
    public static Complex<TDecimal> Zero => new(TDecimal.Zero, TDecimal.Zero);
    public static Complex<TDecimal> FromPolarCoordinates(TDecimal r, TDecimal arg) => new(r * TDecimal.Cos(arg), r * TDecimal.Sin(arg));
    public static void Deconstruct(in Complex<TDecimal> p, out TDecimal r, out TDecimal i)
    {
        r = p.r;
        i = p.i;
    }
    public static implicit operator Complex<TDecimal>(TDecimal r) => new(r, TDecimal.Zero);
    public static explicit operator TDecimal(Complex<TDecimal> self) => self.r;

    //[Serializable]
    //[StructLayout(LayoutKind.Sequential)]
    //public readonly struct Recipro
    //{
    //    public readonly Dec r_r, i_r;
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public Recipro(Dec r_r, Dec i_r)
    //    {
    //        this.r_r = r_r;
    //        this.i_r = i_r;
    //    }
    //    public override string ToString() => $"{1 / r_r} + {1 / i_r}i";
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static implicit operator Recipro(Complex p)
    //    {
    //        Dec v = p.r * p.r + p.i * p.i;
    //        return new Recipro((p.r + p.i) / v, (p.r - p.i) / v);
    //    }
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static explicit operator Complex(Recipro p)
    //    {
    //        Dec v = p.r_r * p.r_r + p.i_r * p.i_r;
    //        return new Complex((p.r_r + p.i_r) / v, (p.r_r - p.i_r) / v);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static Complex operator /(in Complex a, in Recipro b) => new(a.r * b.r_r - a.i * b.i_r, a.r * b.i_r + a.i * b.r_r);
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static Complex operator /(Dec a, in Recipro b) => new(a * b.r_r, a * b.i_r);
    //}
}
