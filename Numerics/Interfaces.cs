using System.Runtime.CompilerServices;

namespace Nonno.Numerics;

public interface INumber<TSelf> : IEquatable<TSelf> where TSelf : INumber<TSelf>
{
    TSelf Sqrt();
    TSelf Copy();
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
