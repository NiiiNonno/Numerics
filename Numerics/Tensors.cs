//using System.ComponentModel.DataAnnotations;
//using System.Runtime.CompilerServices;

//namespace Nonno.Numerics.TensorAlgebra;

//public interface IVector<T, D, N> where T : unmanaged, INumber<T>
//    where D : IVector<T, D, N>
//    where N : IVector<T, N, N>
//{
//    int SpaceIdentifier { get; }
//    int Dimension { get; }
//    T this[int i] { get; init; }

//    bool SequenceEqual<TVector>(in TVector other) where TVector : IVector<T, D, N>;
//    bool SequenceEqual(in N other);
//    ref T GetPinnableReference();
//    Span<T> AsSpan();
//    bool TryGetInnerProduct<TVector>(in TVector other, out T result) where TVector : IVector<T, D, N>;

//    protected static abstract N ToN(in D d);
//    protected static abstract D FromN(in N n);
//    public static abstract D operator -(in D a);
//    public static abstract D operator +(in D a, in D b);
//    public static abstract D operator -(in D a, in D b);
//    public static abstract D operator *(T a, in D b);
//    public static virtual D operator *(in D a, T b) => b * a;
//    public static virtual D operator /(in D a, T b) => ((T)1 / b) * a;
//    public static virtual bool operator ==(in D a, in D b) => a.SequenceEqual(b);
//    public static virtual bool operator !=(in D a, in D b) => !a.SequenceEqual(b);
//    public static virtual bool operator ==(in D a, in N b) => a.SequenceEqual(b);
//    public static virtual bool operator !=(in D a, in N b) => !a.SequenceEqual(b);
//    public static virtual bool operator ==(in N a, in D b) => b.SequenceEqual(a);
//    public static virtual bool operator !=(in N a, in D b) => !b.SequenceEqual(a);
//    public static virtual implicit operator N(in D a) => D.ToN(a);
//    public static virtual explicit operator D(in N a) => D.FromN(a);
//}

//public readonly unsafe partial struct VectorN<T> : IVector<T, VectorN<T>, VectorN<T>> where T : unmanaged, INumber<T>
//{
//    readonly T* p;
//    readonly int d;
//    readonly int id;

//    public T this[int i]
//    {
//        get
//        {
//            if (unchecked((uint)i) >= d) ThrowHelper.ArgumentOutOfRange(i);
//            return p[i];
//        }
//        init
//        {
//            if (unchecked((uint)i) >= d) ThrowHelper.ArgumentOutOfRange(i);
//            p[i] = value;
//        }
//    }
//    internal T* Pointer { get => p; }
//    public int SpaceIdentifier => id;
//    public int Dimension => d;

//    public VectorN(T* pointer, int dimension)
//    {
//        p = pointer;
//        d = dimension;
//    }

//    public bool TryGetInnerProduct<TVector>(in TVector other, out T result) where TVector : IVector<T, VectorN<T>, VectorN<T>>
//    {
//        if (other.SpaceIdentifier == id || other.SpaceIdentifier == -id)
//        {
//            result = (T)1;
//            return true;
//        }
//        result = default;
//        return false;
//    }
//    static VectorN<T> IVector<T, VectorN<T>, VectorN<T>>.FromN(in VectorN<T> n) => n;
//    static VectorN<T> IVector<T, VectorN<T>, VectorN<T>>.ToN(in VectorN<T> d) => d;
//    public ref T GetPinnableReference() => ref Unsafe.AsRef<T>(p);
//    public Span<T> AsSpan() => new(p, sizeof(T) * d);
//    public bool SequenceEqual<TVector>(in TVector other) where TVector : IVector<T, VectorN<T>, VectorN<T>> => AsSpan().SequenceEqual(other.AsSpan());
//    public bool SequenceEqual(in VectorN<T> other) => AsSpan().SequenceEqual(other.AsSpan());

//    public static VectorN<T> operator +(in VectorN<T> a, in VectorN<T> b)
//    {
//        if (a.d != b.d) ThrowHelper.InvalidBinaryOperation(a, b);
//        var p = (T*)Stack.Alloc(sizeof(T) * a.d);
//        for (int i = 0; i < a.d; i++)
//        {
//            p[i] = a.p[i] + b.p[i];
//        }
//        return new(p, a.d);
//    }
//    public static VectorN<T> operator -(in VectorN<T> a)
//    {
//        var p = (T*)Stack.Alloc(sizeof(T) * a.d);
//        for (int i = 0; i < a.d; i++)
//        {
//            p[i] = -a.p[i];
//        }
//        return new(p, a.d);
//    }
//    public static VectorN<T> operator -(in VectorN<T> a, in VectorN<T> b)
//    {
//        if (a.d != b.d) ThrowHelper.InvalidBinaryOperation(a, b);
//        var p = (T*)Stack.Alloc(sizeof(T) * a.d);
//        for (int i = 0; i < a.d; i++)
//        {
//            p[i] = a.p[i] - b.p[i];
//        }
//        return new(p, a.d);
//    }
//    public static VectorN<T> operator *(T a, in VectorN<T> b)
//    {
//        var p = (T*)Stack.Alloc(sizeof(T) * b.d);
//        for (int i = 0; i < b.d; i++)
//        {
//            p[i] = a * b.p[i];
//        }
//        return new(p, b.d);
//    }
//}

//[Tensor(2)]
//public readonly unsafe partial struct Vector2<T> : IVector<T, Vector2<T>, VectorN<T>> where T : unmanaged, INumber<T>
//{
//    public const int IDENTIFIER = 2;

//    readonly T v0, v1;

//    public T this[int i]
//    {
//        get
//        {
//            switch (i)
//            {
//            case 0: return v0;
//            case 1: return v1;
//            default: ThrowHelper.ArgumentOutOfRange(i); return default;
//            }
//        }
//        init
//        {
//            switch (i)
//            {
//            case 0: v0 = value; break;
//            case 1: v1 = value; break;
//            default: ThrowHelper.ArgumentOutOfRange(i); break;
//            }
//        }
//    }
//    internal T* Pointer { get { fixed (Vector2<T>* p = &this) return (T*)p; } }
//    public int SpaceIdentifier => IDENTIFIER;
//    public int Dimension => 2;

//    public Vector2(T v0, T v1)
//    {
//        this.v0 = v0;
//        this.v1 = v1;
//    }

//    public bool TryGetInnerProduct<TVector>(in TVector other, out T result) where TVector : IVector<T, Vector2<T>, VectorN<T>>
//    {
//        if (other.SpaceIdentifier == IDENTIFIER || other.SpaceIdentifier == -IDENTIFIER)
//        {
//            result = (T)1;
//            return true;
//        }
//        result = default;
//        return false;
//    }
//    static Vector2<T> IVector<T, Vector2<T>, VectorN<T>>.FromN(in VectorN<T> n)
//    {
//        if (n.SpaceIdentifier != IDENTIFIER) throw new InvalidOperationException();
//        return *(Vector2<T>*)n.Pointer;
//    }
//    static VectorN<T> IVector<T, Vector2<T>, VectorN<T>>.ToN(in Vector2<T> d)
//    {
//        return new(d.Pointer, 2);
//    }
//    public ref T GetPinnableReference() => ref Unsafe.AsRef<T>(Pointer);
//    public Span<T> AsSpan() => new(Pointer, sizeof(T) * 2);
//    public bool SequenceEqual<TVector>(in TVector other) where TVector : IVector<T, Vector2<T>, VectorN<T>> => AsSpan().SequenceEqual(other.AsSpan());
//    public bool SequenceEqual(in VectorN<T> other) => AsSpan().SequenceEqual(other.AsSpan());

//    public static Vector2<T> operator +(in Vector2<T> a, in Vector2<T> b)
//    {
//        return new(a.v0 + b.v0, a.v1 + b.v1);
//    }
//    public static Vector2<T> operator -(in Vector2<T> a)
//    {
//        return new(-a.v0, -a.v1);
//    }
//    public static Vector2<T> operator -(in Vector2<T> a, in Vector2<T> b)
//    {
//        return new(a.v0 - b.v0, a.v1 - b.v1);
//    }
//    public static Vector2<T> operator *(T a, in Vector2<T> b)
//    {
//        return new(a * b.v0, a * b.v1);
//    }
//}

