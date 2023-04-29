global using MI = System.Runtime.CompilerServices.MethodImplAttribute;
global using MIO = System.Runtime.CompilerServices.MethodImplOptions;
using System.Globalization;
using Nonno.Numerics;

var fp = new FormatProvider();
CultureInfo.CurrentCulture = fp;
using IMemory mem = new HeapMemory();
IMemory.Default = mem;

var m = new Matrix<Float32>(3, 3);
m[0, 0] = (Float32)2;
m[0, 1] = (Float32)3;
m[0, 2] = (Float32)1;
m[1, 0] = (Float32)4;
m[1, 1] = (Float32)4;
m[1, 2] = (Float32)2;
m[2, 0] = (Float32)(-2);
m[2, 1] = (Float32)3;
m[2, 2] = (Float32)(-2);
Console.WriteLine($"m:\n{m:S10-0.00}"); 
var v = Vector<Float32>.Zero(3);
v[0] = (Float32)1;
Console.WriteLine($"v:\n{v:S10-0.00}");

var a = m * v;
Console.WriteLine($"a:\n{a:S10-0.00}");
var l = a.Abs();
Console.WriteLine($"r:\n{l}");
var b = a - l * v;
Console.WriteLine($"b:\n{b:S10-0.00}");
var d = Vector<Float32>.Direct(b, b);
Console.WriteLine($"d:\n{d:S10-0.00}");
var f = (Float32)2 * d / (b * b);
Console.WriteLine($"f:\n{f:S10-0.00}");

var u = Matrix<Float32>.Zero((3,3));
Console.WriteLine($"u:\n{u:S10-0.00}");
u[0, 0] = u[1, 1] = u[2, 2] = (Float32)1;
Console.WriteLine($"u:\n{u:S10-0.00}");
var h = u - f;
Console.WriteLine($"h:\n{h:S10-0.00}");

var r = h * m;
Console.WriteLine($"u:\n{r:S10-0.00}");

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