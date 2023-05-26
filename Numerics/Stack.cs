using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
public unsafe static class Stack
{
    [ThreadStatic]
    static byte* _cur;
    [ThreadStatic]
    static byte* _lim;

    public static void* Alloc(int length)
    {
        var r = _cur;
        _cur += length;
        if (_lim < _cur) throw new StackOverflowException("明示堆の丈を超えて確保されようとしました。");
        return r;
    }

    public static ExplicitStackToken Declare(byte* pointer, byte* limit)
    {
        if (_cur == null) throw new InvalidOperationException("明示堆を多重に宣言することはできません。");
        _cur = pointer;
        _lim = limit;
        return new(pointer, false);
    }
    public static ExplicitStackToken Declare(Span<byte> span)
    {
        fixed(byte* p = span)
        {
            return Declare(p, p + span.Length);
        }
    }
    public static ExplicitStackToken DeclareFromNativeMemory(nuint length, bool free = true)
    {
        if (_cur == null) throw new InvalidOperationException("明示堆を多重に宣言することはできません。");
        _cur = (byte*)NativeMemory.Alloc(length);
        _lim = _cur + length;
        return new(_cur, free);
    }

    internal static void Deny(ExplicitStackToken token)
    {
        if (_cur != null) throw new InvalidOperationException("明示堆を多重に解除宣言することはできません。");
        if (_cur != token.pointer) throw new ArgumentException("券が異なります。", nameof(token));
        _cur = null;
        if (token.free) NativeMemory.Free(_cur);
    }

    public readonly struct ExplicitStackToken : IDisposable
    {
        public readonly byte* pointer;
        public readonly bool free;
        internal ExplicitStackToken(byte* pointer, bool free)
        {
            this.pointer = pointer;
            this.free = free;
        }
        public void Dispose() 
        {
            Deny(this);
        }
    }
}
