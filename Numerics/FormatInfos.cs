using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
public class MatrixFormatInfo
{
    //public virtual string Format<TNumber, TMatrix>(TMatrix matrix, string? format, IFormatProvider? provider) where TNumber : unmanaged, INumber<TNumber> where TMatrix : ITensor<TNumber, TMatrix>
    //{
    //    format ??= "T5x1";
    //    switch (format[0])
    //    {
    //    case 'T' or 'S':
    //        {
    //            var c_s = format[0] switch { 'T' => '\t', 'S' => ' ', _ => '?' };
    //            var f = format.AsSpan();
    //            var i_x = f.IndexOf('x');
    //            var i_h = f.IndexOf('-');
    //            var (l_x, l_y, rest) = (i_x, i_h) switch
    //            {
    //                ( < 0, < 0) => (f.Length > 1 ? int.Parse(f[1..]) : 5, 1, ""),
    //                (1, < 0) => (5, 1, ""),
    //                ( > 1, < 0) => (int.Parse(f[1..i_x]), int.Parse(f[(i_x + 1)..]), ""),
    //                ( < 0, 1) => (5, 1, format[(i_h + 1)..]),
    //                ( < 0, > 1) => (int.Parse(f[1..i_h]), 1, format[(i_h + 1)..]),
    //                (1, > 1) => (5, int.Parse(f[(i_x + 1)..i_h]), format[(i_h + 1)..]),
    //                ( > 1, > 1) => (int.Parse(f[1..i_x]), int.Parse(f[(i_x + 1)..i_h]), format[(i_h)..]),
    //                _ => throw new FormatException()
    //            };
                
    //            StringBuilder r = new();

    //            var (m, n) = matrix.Type;
    //            for (int i = 0; i < m; i++)
    //            {
    //                for (int j = 0; j < n; j++)
    //                {
    //                    var c = matrix[i, j];
    //                    var s = c is IFormattable fable ? fable.ToString(rest, provider) : c.ToString() ?? String.Empty;
    //                    if (l_x < s.Length) s = s[..(l_x - 1)] + "~";
    //                    _ = r.Append(s);
    //                    for (int k = s.Length; k < l_x; k++) _ = r.Append(' ');
    //                    _ = r.Append(c_s);
    //                }
    //                _ = r.Append('\n');
    //            }
    //            _ = r.Remove(r.Length - 1, 1);

    //            return r.ToString();
    //        }
    //    default:
    //        throw new FormatException();
    //    }
    //}

    public static MatrixFormatInfo Default { get; } = new();
}

public class ComplexFormatInfo
{
    const bool O = true;
    const bool I = false;

    public virtual string Format<TDecimal>(Complex<TDecimal> complex, string? format, IFormatProvider? provider) where TDecimal : IDecimal<TDecimal>
    {
        return (complex.r.IsPositiveInfinity, complex.r.IsNegativeInfinity, complex.r.IsNaN, complex.r == TDecimal.Zero, complex.i.IsPositiveInfinity, complex.i.IsNegativeInfinity, complex.i.IsNaN, complex.i == TDecimal.Zero) switch {
            (O, I, I, I, O, I, I, I) => "Infinity (orthant: 1)",
            (I, O, I, I, O, I, I, I) => "Infinity (orthant: 2)",
            (I, O, I, I, I, O, I, I) => "Infinity (orthant: 3)",
            (O, I, I, I, I, O, I, I) => "Infinity (orthant: 4)",
            (O, I, I, I, I, I, I, I) => "RePositiveInfinity",
            (I, I, I, I, O, I, I, I) => "ImPositiveInfinity",
            (I, O, I, I, I, I, I, I) => "ReNegativeInfinity",
            (I, I, I, I, I, O, I, I) => "ImNegativeInfinity",
            (I, I, O, I, _, _, _, _) or (_, _, _, _, I, I, O, I) => "NaN",
            (I, I, I, O, I, I, I, O) => "0",
            (I, I, I, O, I, I, I, I) => complex.r is IFormattable f1 ? f1.ToString(format, provider) : complex.r.ToString() ?? "RealNumber",
            (I, I, I, I, I, I, I, O) => complex.i is IFormattable f2 ? f2.ToString(format, provider) : complex.i.ToString() ?? "ImaginaryNumber",
            _ => $"{(complex.r is IFormattable f3 ? f3.ToString(format, provider) : complex.r.ToString())} + {(complex.i is IFormattable f4 ? f4.ToString(format, provider) : complex.i.ToString())}i",
        };
    }

    public static ComplexFormatInfo Default { get; } = new();
}