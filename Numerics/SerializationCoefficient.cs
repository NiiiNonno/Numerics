global using SCoef = Nonno.Numerics.SerializationCoefficient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
// int乗算のほうが早いかもしれない。
// 参見: https://gist.github.com/NiiiNonno/1842cb519b8c115dfe9807da5a34390d
/// <summary>
/// 
/// </summary>
public readonly struct SerializationCoefficient
{
    readonly int v;
    private SerializationCoefficient(int v) => this.v = v;
    public static int operator *(SCoef a, int b) => a.v < 0 ? b * ~a.v : b << a.v;
    public static SCoef From(Shift shift) => new(shift.Exponent);
    public static SCoef From(int coefficient) => coefficient > 0 ? new(~coefficient) : throw new ArgumentOutOfRangeException(nameof(coefficient));
}
