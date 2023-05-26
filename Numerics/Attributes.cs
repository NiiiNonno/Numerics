using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonno.Numerics;
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
public sealed class AuthorAttribute : Attribute
{
    public string AuthorName { get; }
    public string? Address { get; init; }

    public AuthorAttribute(string authorName)
    {
        AuthorName = authorName;
    }
}