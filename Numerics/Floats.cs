using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
readonly struct Float32 : IDecimal<Float32>, IFormattable
{
    readonly float v;

    public bool IsPositiveInfinity => Single.IsPositiveInfinity(v);
    public bool IsNegativeInfinity => Single.IsNegativeInfinity(v);
    public bool IsNaN => Single.IsNaN(v);

    public Float32(float v) => this.v = v;

    public Float32 Copy() => new(v);

    public Float32 Sqrt() => new(MathF.Sqrt(v));

    public bool Equals(Float32 other) => other.v == v;
    public override bool Equals([NotNullWhen(true)] object? obj) => v.Equals(obj);
    public override int GetHashCode() => v.GetHashCode();
    public string ToString(string? format, IFormatProvider? provider) => v.ToString(format, provider);
    public override string ToString() => ToString(null, null);

    public static Float32 Zero => new(0);
    public static Float32 Unit => new(1);

    public static Float32 operator +(Float32 a, Float32 b) => new(a.v + b.v);
    public static Float32 operator -(Float32 self) => new(-self.v);
    public static Float32 operator -(Float32 a, Float32 b) => new(a.v - b.v);
    public static Float32 operator *(Float32 a, Float32 b) => new(a.v * b.v);
    public static Float32 operator /(Float32 a, Float32 b) => new(a.v / b.v);
    public unsafe static Float32 operator <<(Float32 a, int b)
    {
        if (a == 0) return a;
        var v = *(uint*)&a;
        v += (uint)b << 23;
        return *(Float32*)&v;
    }
    public unsafe static Float32 operator >>(Float32 a, int b)
    {
        if (a == 0) return a;
        var v = *(uint*)&a;
        v -= (uint)b << 23;
        return *(Float32*)&v;
    }
    public static bool operator ==(Float32 a, Float32 b) => a.v == b.v;
    public static bool operator !=(Float32 a, Float32 b) => a.v != b.v;
    public static Float32 Sin(Float32 rad) => new(MathF.Sin(rad.v));
    public static Float32 Cos(Float32 rad) => new(MathF.Cos(rad.v));
    public static Float32 Tan(Float32 rad) => new(MathF.Tan(rad.v));
    public static Float32 Exp(Float32 x) => new(MathF.Exp(x.v));
    public static Float32 Abs(Float32 x) => new(MathF.Abs(x.v));
    public static Float32 Log(Float32 x) => new(MathF.Log(x.v));
    public static Float32 Atan2(Float32 a, Float32 b) => new(MathF.Atan2(a.v, b.v));
    public static explicit operator Float32(long v) => new(v);
    public static implicit operator Float32(int v) => new(v);
    public static implicit operator Float32(short v) => new(v);
    public static implicit operator Float32(byte v) => new(v);
    public static explicit operator Float32(ulong v) => new(v);
    public static implicit operator Float32(uint v) => new(v);
    public static implicit operator Float32(ushort v) => new(v);
    public static implicit operator Float32(sbyte v) => new(v);
    public static explicit operator Float32(double v) => new((float)v);
    public static implicit operator Float32(float v) => new((float)v);
    public static implicit operator Float32(Half v) => new((float)v);
    public static explicit operator Float32(decimal v) => new((float)v);
    public static implicit operator float(Float32 v) => v.v;
}

readonly struct Float64 : IDecimal<Float64>, IFormattable
{
    readonly double v;

    public Float64(double v) => this.v = v;

    public Float64 Copy() => new(v);

    public Float64 Sqrt() => new(Math.Sqrt(v));

    public bool Equals(Float64 other) => other.v == v;
    public override bool Equals([NotNullWhen(true)] object? obj) => v.Equals(obj);
    public override int GetHashCode() => v.GetHashCode();
    public string ToString(string? format, IFormatProvider? provider) => v.ToString(format, provider);
    public override string ToString() => ToString(null, null);

    public static Float64 Zero => new(0);
    public static Float64 Unit => new(1);

    public static Float64 operator +(Float64 a, Float64 b) => new(a.v + b.v);
    public static Float64 operator -(Float64 self) => new(-self.v);
    public static Float64 operator -(Float64 a, Float64 b) => new(a.v - b.v);
    public static Float64 operator *(Float64 a, Float64 b) => new(a.v * b.v);
    public static Float64 operator /(Float64 a, Float64 b) => new(a.v / b.v);
    public unsafe static Float64 operator <<(Float64 a, int b)
    {
        if (a == 0) return a;
        var v = *(ulong*)&a;
        v += (ulong)b << 52;
        return *(Float64*)&v;
    }
    public unsafe static Float64 operator >>(Float64 a, int b)
    {
        if (a == 0) return a;
        var v = *(ulong*)&a;
        v -= (ulong)b << 52;
        return *(Float64*)&v;
    }
    public static bool operator ==(Float64 a, Float64 b) => a.v == b.v;
    public static bool operator !=(Float64 a, Float64 b) => a.v != b.v;
    public static explicit operator Float64(long v) => new(v);
    public static implicit operator Float64(int v) => new(v);
    public static implicit operator Float64(short v) => new(v);
    public static implicit operator Float64(byte v) => new(v);
    public static explicit operator Float64(ulong v) => new(v);
    public static implicit operator Float64(uint v) => new(v);
    public static implicit operator Float64(ushort v) => new(v);
    public static implicit operator Float64(sbyte v) => new(v);
    public static explicit operator Float64(double v) => new((float)v);
    public static implicit operator Float64(float v) => new((float)v);
    public static implicit operator Float64(Half v) => new((float)v);
    public static explicit operator Float64(decimal v) => new((float)v);
    public static implicit operator double(Float64 v) => v.v;
}
