﻿using System.Collections;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.String;
using static Nonno.Numerics.SourceGenerator.Utils;

/*
 * 1 指示用の属性を作る。
 * 2 双対指定用の属性を作る。
 * 3 属性の指定内容を調べる。
 * 4 当該の型を生成する。
 * 5 他のテンソル代数の元を取得する。
 * 6 必要ならば一般型を生成する。
 * 7 必要ならばインターフェースを作成する。
 * 8 必要ならば演算に必要な型も追加で定義する。
 */

namespace Nonno.Numerics.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public partial class TensorGenerator : IIncrementalGenerator
{
    const char NUMBER_SPLITTER = '#';
    const int ARGUMENT_INDEX_NUMBER_TYPE = 0;
    const int ARGUMENT_INDEX_SPACES = 1;
    const string HEADER =
        """
        // <auto-generated/>
        #nullable enable
        #pragma warning disable CS8600
        #pragma warning disable CS8601
        #pragma warning disable CS8602
        #pragma warning disable CS8603
        #pragma warning disable CS8604
        using Text = System.Text;
        using IS = System.Runtime.InteropServices;
        using CS = System.Runtime.CompilerServices;
        using SysGC = System.Collections.Generic;
        using SysC = System.Collections;
        using Sys = System;
        using Nu = Nonno.Numerics;
        """;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource($"TensorAttribute.g.cs",
                $$"""
                {{HEADER}}

                namespace Nonno.Numerics.SourceGenerator;
            
                /// <summary>
                /// 㱁量代行に量の分割定義であることを示します。
                /// </summary>
                [Sys::AttributeUsage(Sys::AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
                internal sealed class TensorAttribute : Sys::Attribute
                {
                    /// <param name="spaces">
                    /// 空間を指定します。
                    /// <para>
                    /// 空間名は以下のようになります。<br/>
                    /// <c>$"{DIMENSION}{NAME}#{IDENTIFIER:X}"</c><br/>
                    /// 例えば、<c>"3*#1003"</c>
                    /// </para>
                    /// </param>
                    public TensorAttribute(string numberTypeFullName, params string[] spaces){}
                }
                """);
        });

        var sources = context.SyntaxProvider.ForAttributeWithMetadataName(
            $"Nonno.Numerics.SourceGenerator.TensorAttribute", // 引っ掛ける属性のフルネーム
            static (node, token) => true, // predicate, 属性で既に絞れてるので特別何かやりたいことがなければ基本true
            (context, token) => context); // GeneratorAttributeSyntaxContextにはNode, SemanticModel(Compilation), Symbolが入ってて便利

        // 出力コード部分はちょっとごちゃつくので別メソッドに隔離
        context.RegisterSourceOutput(sources.Combine(context.CompilationProvider), Emit);
    }

    static void Emit(SourceProductionContext context, (GeneratorAttributeSyntaxContext context, Compilation compilation) source)
    {
        try
        {
            // c : 具格
            // b : 共格
            // i : 形格
            // n : 素値型

            // classで引っ掛けてるのでTypeSymbol/Syntaxとして使えるように。
            // SemaintiModelが欲しい場合は source.context.SemanticModel
            // Compilationが欲しい場合は source.context.SemanticModel.Compilation から

            var tSb_c = (INamedTypeSymbol)source.context.TargetSymbol;
            var tNd_c = (TypeDeclarationSyntax)source.context.TargetNode;
            var dTNm_c = tSb_c.Name;
            var fNm_c = $"{dTNm_c}.g.cs";//.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            var tNm_c = tSb_c.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            var addMs = new List<string>();
            var baseNm = GetBaseName(dTNm_c);
            var attrDs = source.context.Attributes;
            var attrD = attrDs.Single(x => x.AttributeClass?.Name == "TensorAttribute");
            var ns = tSb_c.ContainingNamespace.IsGlobalNamespace ? null : tSb_c.ContainingNamespace.ToString();
            var spaces = GetSpaces(attrD, context);
            var rank = spaces.Length;
            var (dims, dimsP, size) = GetDimensions(spaces, tNd_c, context);
            var spaceSbs = GetSpaceSymbols(spaces, tNd_c, context);
            var phs = Binary(spaceSbs);

            var tNm_n = (string?)attrD.ConstructorArguments[ARGUMENT_INDEX_NUMBER_TYPE].Value ?? throw new NullReferenceException();

            //var dTNm_i = rank switch { 0 => "IScalar", 1 => "IVector", _ => "ITensor" };
            //var fNm_i = $"{dTNm_i}.g.cs";
            //var tNm_i = $"{dTNm_i}<{Join(", ", Binary())}>";
            //var tSb_i = source.compilation.GetTypeByMetadataName(ns is null ? $"{ns}.{tNm_i}" : tNm_i);
            //var existsON_i = tSb_i is not null;

            //var dTNm_b = rank switch { 0 => "Scalar", 1 => "Vector", _ => "Tensor" } + Concat(spaces.Select(x => { var i = x.IndexOf(NUMBER_SPLITTER); return x.Substring(0, i); }));
            //var fNm_b = $"{dTNm_b}.g.cs";
            //var tNm_b = dTNm_b + "<T>";
            //var tSb_b = source.compilation.GetTypeByMetadataName(ns is null ? $"{ns}.{tNm_b}" : fNm_b);
            //var existsON_b = tSb_b is not null;

            context.AddSource(fNm_c, CTensor(
                @namespace: ns,
                name: tNm_c,
                @interface: "[interface]",//tNm_i,
                numberT: tNm_n,
                typeParams: Array.Empty<string>(),
                dimsExpand: Expand(dims),
                indexes: Chars(dims.Length).ToArray(),
                isReadonly: true,
                implementSetterOrNot: true,
                dims: dims,
                dimsPro: dimsP,
                size: size,
                additionalMessage: addMs
                ));

            //if (existsON_b) goto fin_b;
            //context.AddSource(fNm_b, BTensor(
            //    ));
            //fin_b:;

            //if (existsON_i) goto fin_i;
            //context.AddSource(fNm_i, ITensor(
            //    ));
            //fin_i:;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"****{e}****");
            throw;
        }

        static string GetBaseName(string fullName)
        {
            var index = fullName.IndexOfAny("0123456789".ToCharArray());
            if (index < 0) return fullName;
            else return fullName.Substring(0, index);
        }

        static string[] GetSpaces(AttributeData attributeData, SourceProductionContext context)
        {
            var arg = attributeData.ConstructorArguments[ARGUMENT_INDEX_SPACES];
            var r = new string[arg.Values.Length];
            for (var i = 0; i < arg.Values.Length; i++)
            {
                TypedConstant value = arg.Values[i];
                r[i] = (string)value.Value!;
            }
            return r;
        }

        static string[] GetSpaceSymbols(string[] spaces, TypeDeclarationSyntax classDeclarationSyntax, SourceProductionContext context)
        {
            var r = new string[spaces.Length];
            for (int i = 0; i < spaces.Length; i++)
            {
                var index = spaces[i].IndexOf('#');
                if (index < 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.InvalidDimensionFormat, classDeclarationSyntax.Identifier.GetLocation()));
                    r[i] = spaces[i];
                }
                else
                {
                    r[i] = spaces[i].Substring(0, index);
                }
            }
            return r;
        }

        static (int[] dimensions, int[] dimensionProducts, int size) GetDimensions(string[] spaces, TypeDeclarationSyntax classDeclarationSyntax, SourceProductionContext context)
        {
            var dims = new int[spaces.Length];
            var dimsPro = new int[dims.Length];
            int pro = 1;
            for (int i = spaces.Length - 1; i >= 0; i--)
            {
                int to = 0;
                for (; to < spaces[i].Length; to++)
                    if (spaces[i][to] is < '0' or > '9') break;
                if (!int.TryParse(spaces[i].Substring(0, to), out int dim))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.InvalidDimensionFormat, classDeclarationSyntax.Identifier.GetLocation()));
                    dim = 1;
                }
                dims[i] = dim;
                dimsPro[i] = pro;
                pro *= dim;
            }
            return (dims, dimsPro, pro);
        }

    }

    static string CTensor(
        string? @namespace, // Nonno.Numerics
        string name, // Vector2
        string @interface, // IVector
        string numberT, // Float32
        IEnumerable<string> typeParams, // Vector2<T> VectorN<T>
        IEnumerable<string> dimsExpand, // 00 01 02 10 11 12 20 21 22
        string[] indexes, // i j k l m n
        bool isReadonly,
        bool implementSetterOrNot,
        int[] dims, // 2 3 4
        int[] dimsPro, // 1 2 6
        int size, // 24 
        IEnumerable<string> additionalMessage) =>
        $$""""
        {{HEADER}}

        /*{{Join("\n * ", additionalMessage)}}*/
        
        {{(IsNullOrEmpty(@namespace) ? null : $"namespace {@namespace};")}} // namespace

        [IS::StructLayout(IS::LayoutKind.Sequential, Pack = 0)]
        unsafe partial struct {{name}}// : {{@interface}}<{{numberT}}, {{Join(", ", typeParams)}}>
        {
            public const int SIZE_TS = {{size}};
            public static readonly int SIZE_BYTES = SIZE_TS * sizeof({{numberT}});

            {{(isReadonly ? "readonly" : "")}} {{numberT}} {{Join(", ", dimsExpand.Select(x => "v" + x))}};
        
            public {{numberT}} this[{{Join(", ", indexes.Select(x => $"int {x}"))}}]
            {
                get
                {
                    {{Join("\n\t\t\t", indexes.Select((x, i) => $"if (unchecked((uint){x}) >= {dims[i]}) ThrowAOE({x});"))}}
                    return Pointer[{{Join(" + ", indexes.Select((x, i) => $"{dimsPro[i]} * {x}"))}}];
                }
                {{(implementSetterOrNot ? "set" : "init")}}
                {
                    {{Join("\n\t\t\t", indexes.Select((x, i) => $"if (unchecked((uint){x}) >= {dims[i]}) ThrowAOE({x});"))}}
                    Pointer[{{Join(" + ", indexes.Select((x, i) => $"{dimsPro[i]} * {x}"))}}] = value;
                }
            }
            private {{numberT}}* Pointer
            {
                get
                {
                    fixed ({{name}}* p = &this)
                    {
                        return ({{numberT}}*)p;
                    }
                }
            }

            public override string ToString()
            {
                var r = new Text::StringBuilder();
                var p = Pointer;
        {{Concat(indexes.Select((x, i) => $"{new string('\t', 2 + i)}for (int {x} = 0; {x} < {dims[i]}; {x}++)\n{new string('\t', 2 + i)}{{\n"))}}
        {{new string('\t', 2 + indexes.Length)}}_ = r.Append(*p++).Append("\t");
        {{Concat(indexes.Reverse().Select((x, i) => $"\n{new string('\t', indexes.Length + 1 - i)}}}\n{new string('\t', indexes.Length + 1 - i)}_ = r.Append(\"\\n\"); // end of {x}."))}}
                _ = r.Remove(r.Length - {{indexes.Length}}, {{indexes.Length}});
                return r.ToString();
            }

            [CS::MethodImpl(CS::MethodImplOptions.NoInlining)]
            void ThrowAOE(object? argument) => throw new ArgumentOutOfRangeException();
        }
        """";

    static string BTensor(
        string? @namespace,
        string name,
        //string @interface,
        IEnumerable<string> additionalMessage) =>
        $$""""
        {{HEADER}}
        
        /*{{Join("\n * ", additionalMessage)}}*/
        
        {{(IsNullOrEmpty(@namespace) ? "" : $"namespace {@namespace};")}} // namespace
        
        [IS::StructLayout(IS::LayoutKind.Sequential, Pack = 0)]
        public readonly struct {{name}}// : {@interface}<>
        {
            
        }
        """";

    static string ITensor(
        string? @namespace,
        string name,
        IEnumerable<string> additionalMessage) =>
        $$""""
        {{HEADER}}
        
        /*{{Join("\n * ", additionalMessage)}}*/
        
        {{(IsNullOrEmpty(@namespace) ? "" : $"namespace {@namespace};")}} // namespace
        
        public interface {{name}}
        {
            
        }
        """";
}

public static partial class DiagnosticDescriptors
{
    const string CATEGORY = "㱁量代行";

    public static readonly DiagnosticDescriptor InvalidDimensionFormat = new(
        id: "TENSOR001",
        title: "無効次元指定詞",
        messageFormat: "量 {0} の次元指定詞𛂦無効や。",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
