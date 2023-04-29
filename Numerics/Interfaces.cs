using System.Runtime.CompilerServices;

namespace Nonno.Numerics;

public interface INumber<TSelf> : IEquatable<TSelf> where TSelf : INumber<TSelf>
{
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

public interface IVector<TNumber, TSelf> where TNumber : INumber<TNumber> where TSelf : IVector<TNumber, TSelf>
{
    int Dimension { get; }
    TNumber this[int i] { get; set; }
    static abstract TNumber operator *(TSelf a, TSelf b);
    static abstract TSelf operator *(TNumber a, TSelf b);
    static abstract TSelf operator +(TSelf a, TSelf b);
    static abstract TSelf operator -(TSelf a, TSelf b);
    static abstract bool operator ==(TSelf a, TSelf b);
    static abstract bool operator !=(TSelf a, TSelf b);
    static abstract TSelf operator -(TSelf self);
    static abstract TSelf Zero(int dimension);
}

public interface IMatrix<TNumber, TVector, TSelf> where TNumber : INumber<TNumber> where TVector : IVector<TNumber, TVector> where TSelf : IMatrix<TNumber, TVector, TSelf>
{
    (int, int) Type { get; }
    TVector this[int i] { get; set; }
    TNumber this[int i, int j] { get; set; }
    static abstract TVector operator *(TSelf a, TVector b);
    static abstract TSelf operator *(TNumber a, TSelf b);
    static abstract TSelf operator +(TSelf a, TSelf b);
    static abstract TSelf operator -(TSelf a, TSelf b);
    static abstract TSelf operator -(TSelf self);
    static abstract TSelf Zero((int, int) type);
}