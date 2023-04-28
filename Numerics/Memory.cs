using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
public unsafe class Memory : IDisposable
{
    readonly (Stack<nint>, List<nint>)[] _stacks = Array.Empty<(Stack<nint>, List<nint>)>();

    readonly int _memorySize_min;

    public Memory(int initialMemorySize = 16)
    {
        _memorySize_min = initialMemorySize;
    }

    ~Memory()
    {
        Dispose(false);
    }

    [MI(MIO.AggressiveInlining)]
    (Stack<nint>, List<nint>) GetStack(int i)
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

    [MI(MIO.AggressiveInlining)]
    nint AllocCore(int length)
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
        nint Extend(int unit, Stack<nint> stack, List<nint> list)
        {
            var l = (_memorySize_min << list.Count) * unit;
            var r = Marshal.AllocHGlobal(l);
            list.Add(r);
            for (nint r_ = r + unit; r_ < l; r_ += unit)
            {
                stack.Push(r_);
            }
            return r;
        }
    }

    [MI(MIO.AggressiveInlining)]
    void FreeCore(nint p, int length)
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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected void Dispose(bool disposing)
    {
        foreach (var (_, list) in _stacks)
        {
            foreach (var p in list)
            {
                Marshal.FreeHGlobal(p);
            }
        }
    }

    [ThreadStatic]
    static Memory? _instance;

    public static Memory Default
    {
        get
        {
            _instance ??= new();
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    [MI(MIO.AggressiveInlining)]
    public static nint Alloc(int length) => Default.AllocCore(length);
    [MI(MIO.AggressiveInlining)]
    public static T* Alloc<T>() where T : unmanaged
    {
        return (T*)Alloc(sizeof(T));
    }

    [MI(MIO.AggressiveInlining)]
    public static void Free(nint p, int length) => Default.FreeCore(p, length);
    [MI(MIO.AggressiveInlining)]
    public static void Free<T>(T* p) where T : unmanaged
    {
        Free((nint)p, sizeof(T));
    }
}
