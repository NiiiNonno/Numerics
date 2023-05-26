using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nonno.Numerics.SourceGenerator;

namespace Nonno.Numerics.Vectors;

[Tensor("T", "2#2")]
public partial struct Vector2<T> where T : unmanaged { }

[Tensor("T", "3#3")]
public partial struct Vector3<T> { }