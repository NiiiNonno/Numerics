using System.Runtime.CompilerServices;

namespace Nonno.Numerics;

public interface INumber<TSelf> : IEquatable<TSelf> where TSelf : INumber<TSelf>
{
    TSelf Copy();
    TSelf Sqrt();
    static abstract TSelf Zero { get; }
    static abstract TSelf Unit { get; }
    static abstract TSelf operator *(TSelf a, TSelf b);
    static abstract TSelf operator /(TSelf a, TSelf b);
    static abstract TSelf operator +(TSelf a, TSelf b);
    static abstract TSelf operator -(TSelf a, TSelf b);
    static abstract bool operator ==(TSelf a, TSelf b);
    static abstract bool operator !=(TSelf a, TSelf b);
    static abstract TSelf operator -(TSelf self);
    static abstract explicit operator TSelf(long v);
    static abstract explicit operator TSelf(double v);
}

public interface IDecimal<TSelf> : INumber<TSelf> where TSelf : IDecimal<TSelf>
{
    bool IsPositiveInfinity { get; }
    bool IsNegativeInfinity { get; }
    bool IsNaN { get; }
    static abstract TSelf Sin(TSelf rad);
    static abstract TSelf Cos(TSelf rad);
    static abstract TSelf Tan(TSelf rad);
    static abstract TSelf Exp(TSelf x);
    static abstract TSelf Abs(TSelf x);
    static abstract TSelf Log(TSelf x);
    static abstract TSelf Atan2(TSelf a, TSelf b);
}
