using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
public static partial class Utils
{
    [MI(MIO.AggressiveInlining | MIO.AggressiveOptimization)]
    public static unsafe bool SequencialEquals(void* a, void* b, int byteLength)
    {
        var a1 = (Vector256<byte>*)a;
        var b1 = (Vector256<byte>*)b;
        while (true)
        {
            int r = byteLength - 0x20;
            if (r < 0) break;
            
            byteLength = r;
            if (*a1 != *b1) return false;
            a1++;
            b1++;
        }

        var a2 = (Vector128<byte>*)a1;
        var b2 = (Vector128<byte>*)b1;
        if ((byteLength & 0x10) != 0)
        {
            if (*a2 != *b2) return false;
            a2++;
            b2++;
        }

        var a3 = (ulong*)a2;
        var b3 = (ulong*)b2;
        if ((byteLength & 0x08) != 0)
        {
            if (*a3 != *b3) return false;
            a3++;
            b3++;
        }

        var a4 = (uint*)a3;
        var b4 = (uint*)b3;
        if ((byteLength & 0x04) != 0)
        {
            if (*a4 != *b4) return false;
            a4++;
            b4++;
        }

        var a5 = (ushort*)a4;
        var b5 = (ushort*)b4;
        if ((byteLength & 0x02) != 0)
        {
            if (*a5 != *b5) return false;
            a5++;
            b5++;
        }

        var a6 = (byte*)a5;
        var b6 = (byte*)b5;
        if ((byteLength & 0x01) != 0)
        {
            if (*a6 != *b6) return false;
        }

        return true;
    }

    [MI(MIO.AggressiveInlining | MIO.AggressiveOptimization)]
    public static unsafe void Copy(nint source, void* to, int byteLength) => Copy((void*)source, to, byteLength);
    [MI(MIO.AggressiveInlining | MIO.AggressiveOptimization)]
    public static unsafe void Copy(void* source, nint to, int byteLength) => Copy(source, (void*)to, byteLength);
    [MI(MIO.AggressiveInlining | MIO.AggressiveOptimization)]
    public static unsafe void Copy(nint source, nint to, int byteLength) => Copy((void*)source, (void*)to, byteLength);
    [MI(MIO.AggressiveInlining | MIO.AggressiveOptimization)]
    public static unsafe void Copy(void* source, void* to, int byteLength)
    {
        var a1 = (Vector256<byte>*)to;
        var b1 = (Vector256<byte>*)source;
        while (true)
        {
            int r = byteLength - 0x20;
            if (r < 0) break;

            byteLength = r;
            *a1 = *b1;
            a1++;
            b1++;
        }

        var a2 = (Vector128<byte>*)a1;
        var b2 = (Vector128<byte>*)b1;
        if ((byteLength & 0x10) != 0)
        {
            *a2 = *b2;
            a2++;
            b2++;
        }

        var a3 = (ulong*)a2;
        var b3 = (ulong*)b2;
        if ((byteLength & 0x08) != 0)
        {
            *a3 = *b3;
            a3++;
            b3++;
        }

        var a4 = (uint*)a3;
        var b4 = (uint*)b3;
        if ((byteLength & 0x04) != 0)
        {
            *a4 = *b4;
            a4++;
            b4++;
        }

        var a5 = (ushort*)a4;
        var b5 = (ushort*)b4;
        if ((byteLength & 0x02) != 0)
        {
            *a5 = *b5;
            a5++;
            b5++;
        }

        var a6 = (byte*)a5;
        var b6 = (byte*)b5;
        if ((byteLength & 0x01) != 0)
        {
            *a6 = *b6;
        }
    }

    public static unsafe int GetHashCode(void* a, int byteLength)
    {
        int h = 0;

        var a1 = (Vector256<byte>*)a;
        while (true)
        {
            int r = byteLength - 0x20;
            if (r < 0) break;

            byteLength = r;
            h ^= a1->GetHashCode();
            a1++;
        }

        var a2 = (Vector128<byte>*)a1;
        if ((byteLength & 0x10) != 0)
        {
            h ^= a2->GetHashCode();
            a2++;
        }

        var a3 = (ulong*)a2;
        if ((byteLength & 0x08) != 0)
        {
            h ^= a3->GetHashCode();
            a3++;
        }

        var a4 = (uint*)a3;
        if ((byteLength & 0x04) != 0)
        {
            h ^= a4->GetHashCode();
            a4++;
        }

        var a5 = (ushort*)a4;
        if ((byteLength & 0x02) != 0)
        {
            h ^= a5->GetHashCode();
            a5++;
        }

        var a6 = (byte*)a5;
        if ((byteLength & 0x01) != 0)
        {
            h ^= a6->GetHashCode();
        }

        return h;
    }

    public static TFormatInfo? GetFormat<TFormatInfo>(this IFormatProvider @this) where TFormatInfo : class => @this.GetFormat(typeof(TFormatInfo)) as TFormatInfo;

    public static TVector Solve<TNumber, TVector, TMatrix>(TMatrix leftSide, TVector rightSide) where TNumber : unmanaged, INumber<TNumber> where TVector : IVector<TNumber, TVector> where TMatrix : IMatrix<TNumber, TMatrix>
    {
        var I = Matrix<TNumber>.Zero((3, 3));
        I[0, 0] = I[1, 1] = I[2, 2] = (TNumber)1;

        for (int i = 0; i < 2; i++)
        {
            var e = TVector.Zero(3);
            e[i] = (TNumber)1;
            var a = TVector.Cast(leftSide * TVector.Cast(e));
            for (int j = 0; j < i; j++)
                a[j] = TNumber.Zero;
            var a_d = (a * a).Sqrt() * e - a;
            var H = I - (TNumber)2 * (a_d.Column() * a_d.Row()) / (a_d * a_d);
            leftSide = TMatrix.Cast(H * leftSide);
            rightSide = TVector.Cast(H * TVector.Cast(rightSide));
        }

        return SolveQR<TNumber, TVector, TMatrix>(leftSide, rightSide);
    }
    public static TVector SolveQR<TNumber, TVector, TMatrix>(TMatrix leftSideUTM, TVector rightSide) where TNumber : unmanaged, INumber<TNumber> where TVector : IVector<TNumber, TVector> where TMatrix : IMatrix<TNumber, TMatrix>
    {
        var r = rightSide.Copy();
        for (int i = rightSide.Dimension - 1; i >= 0; i--)
        {
            var x = r[i] /= leftSideUTM[i, i];
            for (int j = i - 1; j >= 0; j--)
            {
                r[j] -= x * leftSideUTM[j, i]; 
            }
        }
        return r;
    }
}
