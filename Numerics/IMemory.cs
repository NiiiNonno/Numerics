using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
public interface IMemory : IDisposable
{
    nint Alloc(int length);
    void Free(nint p, int length);
    void Clear();

    [ThreadStatic]
    private static IMemory? _default;
    public static IMemory Default
    {
        get => _default ??= new StackMemory();
        set => _default = value;
    }
}

public interface IUnmanagedReference
{
    void Delete(IMemory from);
}

public unsafe class StackMemory : IMemory
{
    readonly (Stack<nint>, List<(nint p, int l)>)[] _stacks = Array.Empty<(Stack<nint>, List<(nint p, int l)>)>();

    readonly int _memorySize_min;

    public StackMemory(int initialMemorySize = 16)
    {
        _memorySize_min = initialMemorySize;
    }

    ~StackMemory()
    {
        Dispose(false);
    }

    [MI(MIO.AggressiveInlining)]
    (Stack<nint>, List<(nint p, int l)>) GetStack(int i)
    {
        if (_stacks.Length <= i) Extend();
        return _stacks[i];

        [MI(MIO.NoInlining)]
        void Extend()
        {
            var neo = new Stack<nint>[i];
            Array.Copy(_stacks, neo, _stacks.Length);
            for (int j = _stacks.Length; j < neo.Length; j++) neo[i] = new();
        }
    }

    public nint Alloc(int length)
    {
        if (length <= 0) ThrowHelper.ArgumentOutOfRange(length);
        int i = 0;
        length--;
        while (length != 0)
        {
            i++;
            length >>= 1;
        }

        var (stack, list) = GetStack(i);
        if (!stack.TryPop(out var r)) r = Extend(1 << i, stack, list);
        return r;

        [MI(MIO.NoInlining)]
        nint Extend(int unit, Stack<nint> stack, List<(nint p, int l)> list)
        {
            var l = (_memorySize_min << list.Count) * unit;
            var r = Marshal.AllocHGlobal(l);
            list.Add((r, l));
            for (nint r_ = r + unit; r_ < l; r_ += unit)
            {
                stack.Push(r_);
            }
            return r;
        }
    }

    public void Free(nint p, int length)
    {
        if (length <= 0) ThrowHelper.ArgumentOutOfRange(length);
        int i = 0;
        length--;
        while (length != 0)
        {
            i++;
            length >>= 1;
        }

        var (stack, _) = GetStack(i);
        stack.Push(p);
    }

    public void Clear()
    {
        foreach (var (stack, list) in _stacks)
        {
            stack.Clear();
            foreach (var (p, l) in list)
            {
                for (nint i = p; i < l; i++)
                {
                    stack.Push(i);
                }
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected void Dispose(bool disposing)
    {
        foreach (var (_, list) in _stacks)
        {
            foreach (var (p, _) in list)
            {
                Marshal.FreeHGlobal(p);
            }
        }
    }
}

public unsafe class HeapMemory : IMemory
{
    readonly List<nint> _ps;
    readonly int _len_min;
    nint _next;
    int _rest;

    public HeapMemory(int minimumSectionLength = 256)
    {
        _ps = new();
        _len_min = minimumSectionLength;
    }
    ~HeapMemory() => Dispose(false);

    public nint Alloc(int length)
    {
        if (_rest < length) Extend(length);

        var r = _next;
        _rest -= length;
        _next += length;
        return r;     

        void Extend(int length)
        {
            var cb = length < _len_min ? _len_min : length;
            _next = Marshal.AllocCoTaskMem(cb);
            _ps.Add(_next);
            _rest = cb;
        }
    }
    public void Free(nint p, int length) { }

    public void Clear()
    {
        foreach (var p in _ps)
        {
            Marshal.FreeCoTaskMem(p);
        }
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        foreach (var p in _ps)
        {
            Marshal.FreeCoTaskMem(p);
        }
    }
}

public partial class Utils
{
    public static unsafe T* Alloc<T>(this IMemory @this) where T : unmanaged
    {
        return (T*)@this.Alloc(sizeof(T));
    }

    public static unsafe void Free<T>(this IMemory @this, T* p) where T : unmanaged
    {
        @this.Free((nint)p, sizeof(T));
    }

    public static void Delete(this IUnmanagedReference @this) => @this.Delete(IMemory.Default);
}
