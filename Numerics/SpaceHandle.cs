using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
public readonly struct SpaceHandle : IEquatable<SpaceHandle>
{
    public nint Value { get; }

    public SpaceHandle(nint value)
    {
        Value = value;
    }

    public override string ToString() => _definitions[Value];

    static readonly Dictionary<nint, string> _definitions = new();

    public static void Define(nint value, string name)
    {
        _definitions.Add(value, name);
    }

    public override bool Equals(object? obj) => obj is SpaceHandle handle && Equals(handle);
    public bool Equals(SpaceHandle other) => Value.Equals(other.Value);
    public override int GetHashCode() => HashCode.Combine(Value);

    public static bool operator ==(SpaceHandle left, SpaceHandle right) => left.Equals(right);
    public static bool operator !=(SpaceHandle left, SpaceHandle right) => !(left == right);
}

public class Space
{
    string? _name;

    public string Name
    {
        [return: MaybeNull]
        get => _name!;
        set
        {
            if (value is null) ThrowHelper.ArgumentNull(value);

            if (_name is not null)
            {
                var f = _spaces.Remove(_name, out var space);
                if (!f || !ReferenceEquals(this, space)) ThrowHelper.Error();
            }

            _name = value;
            _spaces.Add(value, this);
        }
    }

    public Space()
    {
    }

    static readonly Dictionary<string, object> _spaces = new();
}

public class VectorSpace<T> : Space
{
    public VectorSpace(T zero)
    {
        var c = Interlocked.Increment(ref _cter);

        Zero = zero;
        Name = c == 0 ? typeof(T).ToString() : $"{typeof(T)} ({c})";
    }
    public VectorSpace(string name, T zero)
    {
        Zero = zero;
        Name = name;
    }

    public T Zero { get; }

    static int _cter = -1;
}
