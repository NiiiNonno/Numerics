using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Nonno.Numerics;

//public interface ITensor<T, C, R, N, CR, CN, MR, RC, RN, MC, MN> : ITensor<CR>
//    where T : unmanaged, INumber<T>
//    where C : IVector<T, C, N>
//    where R : IVector<T, R, N>
//    where N : IVector<T, N, N>
//    where CR : ITensor<T, C, R, N, CR, CN, MR, RC, RN, MC, MN>
//    where CN : ITensor<T, C, N, N, CN, CN, MN, MC, MN, MC, MN>
//    where MR : ITensor<T, N, R, N, MR, MN, MR, RN, RN, MN, MN>
//    where RC : ITensor<T, R, C, N, RC, RN, MC, CR, CN, MR, MN>
//    where RN : ITensor<T, R, N, N, RN, RN, MN, MR, MN, MR, MN>
//    where MC : ITensor<T, N, C, N, MC, MN, MC, CN, CN, MN, MN>
//    where MN : ITensor<T, N, N, N, MN, MN, MN, MN, MN, MN, MN>
//{
//    CR Copy();
//    void Copy(ref CR to);
//    protected MR AsMR();
//    protected CN AsCN();

//    static abstract CR From(MR matrix);
//    static abstract CR From(CN matrix);
//    static abstract CR operator -(CR self);
//    static abstract CR operator +(CR a, CR b);
//    static abstract CR operator -(CR a, CR b);
//    static abstract CR operator *(T a, CR b);
//    static virtual CR operator *(CR a, T b) => b * a;
//    static virtual CR operator /(CR a, T b) => (T)1 / b * a;
//    static abstract C operator *(in C a, in CR b);
//    static abstract R operator *(in CR a, in R b);
//    static abstract CN operator *(in CR a, in RN b);
//    static abstract MR operator *(in MC a, in CR b);
//    static abstract RC operator ~(in CR a);
//    static virtual bool operator ==(in CR a, in CR b) => a.Equals(b);
//    static virtual bool operator !=(in CR a, in CR b) => !a.Equals(b);
//    static virtual bool operator ==(in CR a, in MR b) => a.Equals(b);
//    static virtual bool operator !=(in CR a, in MR b) => !a.Equals(b);
//    static virtual bool operator ==(in MR a, in CR b) => a.Equals(b);
//    static virtual bool operator !=(in MR a, in CR b) => !a.Equals(b);
//    static virtual bool operator ==(in CR a, in CN b) => a.Equals(b);
//    static virtual bool operator !=(in CR a, in CN b) => !a.Equals(b);
//    static virtual bool operator ==(in CN a, in CR b) => a.Equals(b);
//    static virtual bool operator !=(in CN a, in CR b) => !a.Equals(b);
//    static virtual bool operator ==(in CR a, in MN b) => a.Equals(b);
//    static virtual bool operator !=(in CR a, in MN b) => !a.Equals(b);
//    static virtual bool operator ==(in MN a, in CR b) => a.Equals(b);
//    static virtual bool operator !=(in MN a, in CR b) => !a.Equals(b);
//    static virtual implicit operator MR(in CR self) => self.AsMR();
//    static virtual implicit operator CN(in CR self) => self.AsCN();
//    static virtual implicit operator MN(in CR self) => self.AsMR().AsCN();
//    static virtual explicit operator CR(in MR v) => CR.From(v);
//    static virtual explicit operator CR(in CN v) => CR.From(v);
//    static virtual explicit operator CR(in MN v) => CR.From(MR.From(v));
//}

//public unsafe readonly struct MatrixMN<T> : ITensor<T, VectorN<T>, VectorN<T>, VectorN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>> where T : unmanaged, INumber<T>
//{
//    readonly int m;
//    readonly int n;
//    readonly int s;
//    readonly bool f_tp;
//    readonly bool f_sf;
//    readonly byte sf;
//    readonly T[]? a;
//    readonly T* p;

//    public bool IsTransposed => f_tp;
//    private T* Pointer { get => p; init => p = value; }
//    public (int, int) Type => (m, n);
//    public T this[int i, int j]
//    {
//        get
//        {
//            if (a is not null) fixed(T* p_ = a) { }
//            if ((uint)m <= unchecked((uint)i)) ThrowHelper.ArgumentOutOfRange(i);
//            switch (f_tp, f_sf)
//            {
//            case (false, false):
//                return p[i * n + j];
//            case (false, true):
//                return p[(i << sf) + j];
//            case (true, false):
//                return p[j * m + i];
//            case (true, true):
//                return p[(j << sf) + i];
//            }   
//        }
//    }

//    public MatrixMN(nint pointer, (int, int) type, bool isTransposed = false, bool isManaged = true) : this((T*)pointer, type, isTransposed, isManaged) { }
//    public MatrixMN(T* pointer, (int, int) type, bool isTransposed = false, bool isManaged = true)
//    {
//        p = pointer;
//        m = (uint)type.Item1;
//        n = (uint)type.Item2;
//        s = type.Item1 * type.Item2 * sizeof(T);
//        f_tp = isTransposed;
//        f_m = isManaged;
//        f_sf = false;
//    }
//    internal MatrixMN(T* pointer, uint type_m, uint type_n, int size, bool isTransposed = false, bool isManaged = true)
//    {
//        p = pointer;
//        m = type_m;
//        n = type_n;
//        s = size;
//        f_tp = isTransposed;
//        f_m = isManaged;
//        f_sf = false;
//    }
//    internal MatrixMN(T* pointer, uint type_m, uint type_n, int size, byte shift, bool isTransposed = false, bool isManaged = true)
//    {
//        p = pointer;
//        m = type_m;
//        n = type_n;
//        s = size;
//        f_tp = isTransposed;
//        f_m = isManaged;
//        f_sf = true;
//        sf = shift;
//    }
//    private MatrixMN(MatrixMN<T> original)
//    {
//        fixed (MatrixMN<T>* p = &this) *p = original;
//    }

//    public Span<T> AsSpan() => new(p, s);
//    public MatrixMN<T> Copy()
//    {
//        var ptr = (T*)IMemory.Default.Alloc(s);
//        AsSpan().CopyTo(new Span<T>(ptr, s));
//        return this with { Pointer = ptr };
//    }
//    public bool Equals(MatrixMN<T> other)
//    {
//        switch (f_sf, other.f_sf, f_tp, other.f_tp)
//        {
//        default:
//            break;
//        }
//    }
//    public VectorN<T> GetColumn(int j) => throw new NotImplementedException();
//    public VectorN<T> GetRow(int i) => throw new NotImplementedException();

//    MatrixMN<T> ITensor<T, VectorN<T>, VectorN<T>, VectorN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>>.AsCN() => this;
//    MatrixMN<T> ITensor<T, VectorN<T>, VectorN<T>, VectorN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>, MatrixMN<T>>.AsMR() => this;
//    public ref T GetPinnableReference() => ref Unsafe.AsRef<T>(p);

//    public static MatrixMN<T> operator ~(in MatrixMN<T> a)
//    {

//    }
//    public static VectorN<T> operator *(in VectorN<T> a, in MatrixMN<T> b) => throw new NotImplementedException();
//    public static VectorN<T> operator *(in MatrixMN<T> a, in VectorN<T> b) => throw new NotImplementedException();
//    public static MatrixMN<T> operator *(in MatrixMN<T> a, in MatrixMN<T> b) => throw new NotImplementedException();

//    [Flags]
//    enum Flags : byte
//    {
//        IsTransposed = 0b0000_0000_0000_0000_0000_0000_0000_0001,
//        IsManaged = 0b0000_0000_0000_0000_0000_0000_0000_0010,
//    }
//}