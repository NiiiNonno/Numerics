//using System.Diagnostics.CodeAnalysis;
//using System.Drawing;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;

//namespace Nonno.Numerics;

//public interface IVector<T> where T : INumber<T>
//{
//    int Dimension { get; }
//    T this[int i] { get; init; }
//    bool Equals<TVector>(TVector other) where TVector : IVector<T>;
//    ref T GetPinnableReference();
//}

//public interface IVector<T, D, N> : ITensor<D>
//    where T : unmanaged, INumber<T> 
//    where D : IVector<T, D, N> 
//    where N : IVector<T, N, N>
//{
//    D Copy();
//    void Copy(ref D to);
//    protected N AsN();

//    static abstract D From(N vector);
//    static abstract T operator -(D self);
//    static abstract T operator +(D a, D b);
//    static abstract T operator -(D a, D b);
//    static abstract T operator *(T a, D b);
//    static virtual T operator *(D a, T b) => b * a;
//    static virtual T operator /(D a, T b) => (T)1 / b * a;
//    static virtual bool operator ==(D a, D b) => a.Equals(b);
//    static virtual bool operator !=(D a, D b) => !a.Equals(b);
//    static virtual bool operator ==(D a, N b) => a.Equals(b);
//    static virtual bool operator !=(D a, N b) => !a.Equals(b);
//    static virtual bool operator ==(N a, D b) => a.Equals(b);
//    static virtual bool operator !=(N a, D b) => !a.Equals(b);
//    static virtual implicit operator N(D self) => self.AsN();
//    static virtual explicit operator D(N v) => D.From(v);
//}

