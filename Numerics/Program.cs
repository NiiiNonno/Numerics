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

//var A = new Matrix<Float32>((3, 3))
//{
//    [0, 0] = 2, [0, 1] = 3, [0, 2] = 1,
//    [1, 0] = 4, [1, 1] = 4, [1, 2] = 2,
//    [2, 0] = -2, [2, 1] = 3, [2, 2] = -2,
//};
//Console.WriteLine($"A:\n{A:S10-0.000}");

//var b = new Vector<Float32>(3)
//{
//    [0] = 1,
//    [1] = -2,
//    [2] = 14
//};
//Console.WriteLine($"b:\n{b:S10-0.000}");

//var x = Utils.Solve<Float32, Vector<Float32>, Matrix<Float32>>(A, b);
//Console.WriteLine($"x:\n{x:S10-0.000}");

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