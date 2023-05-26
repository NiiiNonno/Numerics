using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using static System.String;

namespace Nonno.Numerics.SourceGenerator;
internal static class Utils
{
    public const string PLACEHOLDER_1 = "N";
    public const string PLACEHOLDER_2 = "MN";
    public const string PLACEHOLDER_12 = "OPQRSTUVWXYZ";

    public static IEnumerable<string> Expand(int[] dims)
    {
        int[] curs = new int[dims.Length];
        next:;
        yield return Concat(curs);
        for (int i = curs.Length - 1; i >= 0; i--)
        {
            curs[i]++;
            if (curs[i] != dims[i]) goto next;
            curs[i] = 0;
        }
    }

    public static IEnumerable<string> Chars(int count)
    {
        switch (count)
        {
        case <= 18:
            var e = 'i' + count;
            for (char i = 'i'; i < e; i++) yield return i.ToString();
            yield break;
        default:
            for (int i = 0; i < count; i++) yield return $"i{i}";
            yield break;
        }
    }

    public static IEnumerable<string> Binary(string[] elements)
    {
        var r = new StringBuilder();
        var ph = elements.Length switch
        {
            0 => String.Empty,
            1 => PLACEHOLDER_1,
            2 => PLACEHOLDER_2,
            <= 12 => PLACEHOLDER_12.Substring(12 - elements.Length),
            _ => throw new ArgumentException(),
        };
        if (elements.Length <= 64)
        {
            ulong c = ~(~0ul << elements.Length) + 1;
            for (ulong i = 0; i < c; i++)
            {
                for (int j = 0; j < elements.Length; j++)
                    _ = (((i >> j) & 1) != 0) ? r.Append(ph[j]) : r.Append(elements[j]);
                yield return r.ToString();
            }
        }
        else
        {
            var arr = ArrayPool<bool>.Shared.Rent(elements.Length);
            next:
            for (int i = 0; i < elements.Length; i++)
                _ = arr[i] ? r.Append(ph[i]) : r.Append(elements[i]);
            yield return r.ToString();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                arr[i] = !arr[i];
                if (arr[i]) goto next;
            }
            ArrayPool<bool>.Shared.Return(arr);
        }
    }
}
