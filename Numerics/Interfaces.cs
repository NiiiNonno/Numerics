using System.Runtime.CompilerServices;

namespace Nonno.Numerics;

////珺の元
//public interface IMember<TSelf> : IEquatable<TSelf> where TSelf : IMember<TSelf>
//{
//    public TSelf Zero { get; }

//    public static abstract TSelf operator +(in TSelf a);
//    public static abstract TSelf operator -(in TSelf a);
//    public static abstract TSelf operator +(in TSelf a, in TSelf b);
//    public static abstract TSelf operator -(in TSelf a, in TSelf b);
//    public static virtual bool operator ==(in TSelf a, in TSelf b) => a.Equals(b);
//    public static virtual bool operator !=(in TSelf a, in TSelf b) => !a.Equals(b);
//}

//public interface IRightOperator<TSelf, TFrom, TTo> where TSelf : IRightOperator<TSelf, TFrom, TTo>
//{
//    public static abstract TTo operator *(in TSelf a, in TFrom b);
//}

//public interface ILeftOperator<TSelf, TFrom, TTo> where TSelf : ILeftOperator<TSelf, TFrom, TTo>
//{
//    public static abstract TTo operator *(in TFrom a, in TSelf b);
//}

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
    static abstract explicit operator TSelf(double v);
    static abstract implicit operator double(TSelf v);
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

public interface IOrdered<TSelf> : IComparable<TSelf> where TSelf : struct, IOrdered<TSelf>
{
    int IComparable<TSelf>.CompareTo(TSelf other) => (TSelf)this < other ? -1 : (TSelf)this == other ? 0 : 1;
    static abstract bool operator ==(TSelf a, TSelf b);
    static abstract bool operator !=(TSelf a, TSelf b);
    static abstract bool operator <(TSelf a, TSelf b);
    static abstract bool operator >(TSelf a, TSelf b);
}
