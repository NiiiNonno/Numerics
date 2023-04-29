using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
readonly struct Float32 : INumber<Float32>, IFormattable
{
    readonly float v;

    public Float32(float v) => this.v = v;

    public bool Equals(Float32 other) => other.v == v;

    public Float32 Copy() => new(v);

    public Float32 Sqrt() => new(MathF.Sqrt(v));

    public string ToString(string? format, IFormatProvider? provider) => v.ToString(format, provider);
    public override string ToString() => v.ToString("f5");

    public static Float32 Zero => new(0);
    public static Float32 Unit => new(1);

    public static Float32 operator +(Float32 a, Float32 b) => new(a.v + b.v);
    public static Float32 operator -(Float32 self) => new(-self.v);
    public static Float32 operator -(Float32 a, Float32 b) => new(a.v - b.v);
    public static Float32 operator *(Float32 a, Float32 b) => new(a.v * b.v);
    public static Float32 operator /(Float32 a, Float32 b) => new(a.v / b.v);
    public static bool operator ==(Float32 a, Float32 b) => a.v == b.v;
    public static bool operator !=(Float32 a, Float32 b) => a.v != b.v;
    public static explicit operator Float32(long v) => new(v);
    public static explicit operator Float32(double v) => new((float)v);
}
