using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
internal static class ThrowHelper
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ArgumentNull(
        object? argument,
        [CallerArgumentExpression(nameof(argument))] string cAE = "",
        [CallerFilePath] string cFP = "",
        [CallerMemberName] string cMN = "",
        [CallerLineNumber] int cLN = -1) => throw new ArgumentNullException(cAE, $"{cAE} ノ引謄 {argument} 無照。於 {cFP} ノ {cMN} ノ {cLN} 行目。");
    
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ArgumentOutOfRange(
        object? argument,
        [CallerArgumentExpression(nameof(argument))] string cAE = "",
        [CallerFilePath] string cFP = "",
        [CallerMemberName] string cMN = "",
        [CallerLineNumber] int cLN = -1) => throw new ArgumentOutOfRangeException(cAE, $"{cAE} ノ引謄 {argument} 外範囲。於 {cFP} ノ {cMN} ノ {cLN} 行目。");

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void InvalidArgument(
        object? argument,
        [CallerArgumentExpression(nameof(argument))] string cAE = "",
        [CallerFilePath] string cFP = "",
        [CallerMemberName] string cMN = "",
        [CallerLineNumber] int cLN = -1) => throw new ArgumentException($"{cAE} ノ引謄 {argument} 異常。於 {cFP} ノ {cMN} ノ    {cLN} 行目。", cAE);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void InvalidBinaryOperation(
        object? a, object? b,
        [CallerArgumentExpression(nameof(a))] string cAE_a = "",
        [CallerArgumentExpression(nameof(b))] string cAE_b = "",
        [CallerFilePath] string cFP = "",
        [CallerMemberName] string cMN = "",
        [CallerLineNumber] int cLN = -1) => throw new ArgumentException($"{cAE_a} ノ {a} と {cAE_b} ノ {b} との演算無効。於 {cFP} ノ {cMN} ノ {cLN} 行目。", cAE_a + ", " + cAE_b);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Error(
    [CallerFilePath] string cFP = "",
    [CallerMemberName] string cMN = "",
    [CallerLineNumber] int cLN = -1) => throw new Exception("異常。於 {cFP} ノ {cMN} ノ {cLN} 行目。");
}
