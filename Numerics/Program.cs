global using MI = System.Runtime.CompilerServices.MethodImplAttribute;
global using MIO = System.Runtime.CompilerServices.MethodImplOptions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;
using Nonno.Numerics;

var fp = new FormatProvider();
CultureInfo.CurrentCulture = fp;
using IMemory mem = new HeapMemory(0);
IMemory.Default = mem;

var A = new Matrix<Float32>(3, 3);
A[0, 0] = (Float32)2;
A[0, 1] = (Float32)3;
A[0, 2] = (Float32)1;
A[1, 0] = (Float32)4;
A[1, 1] = (Float32)4;
A[1, 2] = (Float32)2;
A[2, 0] = (Float32)(-2);
A[2, 1] = (Float32)3;
A[2, 2] = (Float32)(-2);
Console.WriteLine($"A:\n{A:S10-0.000}");

var b = new Vector<Float32>(3);
b[0] = (Float32)1;
b[1] = (Float32)(-2);
b[2] = (Float32)14;

var I = Matrix<Float32>.Zero((3, 3));
I[0, 0] = I[1, 1] = I[2, 2] = (Float32)1;

for (int i = 0; i < 2; i++)
{
    var e = Vector<Float32>.Zero(3);
    e[i] = (Float32)1;
    var a = A * e;
    for (int j = 0; j < i; j++)
        a[j] = Float32.Zero;
    var a_d = a.Abs() * e - a;
    var H = I - (Float32)2 * Vector<Float32>.Direct(a_d, a_d) / (a_d * a_d);
    A = H * A;
    b = H * b;

    Console.WriteLine($"H:\n{H:S10-0.000}");
    Console.WriteLine($"A:\n{A:S10-0.000}");
    Console.WriteLine($"b:\n{b:S10-0.000}");
}

var x = Utils.SolveQR<Float32, Vector<Float32>, Matrix<Float32>>(A, b);
Console.WriteLine($"x:\n{x:S10-0.000}");

class FormatProvider : CultureInfo
{
    readonly MatrixFormatInfo _matrixFormatInfo = new();
    public FormatProvider() : base("", true) { }
    public override object? GetFormat(Type? formatType)
    {
        if (formatType == typeof(MatrixFormatInfo)) return _matrixFormatInfo;
        else return base.GetFormat(formatType);
    }
}