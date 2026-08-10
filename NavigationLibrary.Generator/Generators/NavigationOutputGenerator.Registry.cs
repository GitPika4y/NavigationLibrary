using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NavigationLibrary.Generator.Metadata;

namespace NavigationLibrary.Generator.Generators;

public partial class NavigationOutputGenerator
{
    private static void GenerateNavigationRegistry(SourceProductionContext spc,
        ImmutableArray<ViewModelMetadata> metadata)
    {
        if (metadata.IsDefaultOrEmpty)
            return;

        spc.AddSource("NavigationRegistry.g.cs", CreateNavigationRegistryCode(metadata));
    }

    private static string CreateNavigationRegistryCode(ImmutableArray<ViewModelMetadata> metadata)
    {
        var defaultContentDictionaryCode = GetDefaultContentDictionaryCode(metadata, out var defaultDictionaryName);
        var parentLayoutDictionaryCode = GetParentLayoutDictionaryCode(metadata, out var parentDictionaryName);
        var parameterizedDictionaryCode =
            GetParameterizedNavigationDictionaryCode(metadata, out var parameterizedDictionaryName);

        return $$"""
                 using System;
                 using System.Collections.Generic;
                 using NavigationLibrary.Abstractions;

                 namespace NavigationLibrary.Generated;

                 public sealed class NavigationRegistry : INavigationRegistry
                 {
                     {{defaultContentDictionaryCode}}

                     {{parentLayoutDictionaryCode}}

                     {{parameterizedDictionaryCode}}

                     public Type GetDefaultContentType(Type layoutType)
                     {
                         if (!{{defaultDictionaryName}}.TryGetValue(layoutType, out var contentType))
                             throw new Exception(
                                 $"'{layoutType}' implements ILayout, but not ILayout<TDefaultContent>. " +
                                 $"Layouts must implement ILayout<TDefaultContent> to define their default content.");
                         return contentType;
                     }

                     public Type GetParentLayoutType(Type inheritorType)
                     {
                         if (!{{parentDictionaryName}}.TryGetValue(inheritorType, out var layoutType))
                             throw new Exception($"'{inheritorType}' has no ParentLayoutAttribute<> to navigate");
                         return layoutType;
                     }

                     public void ApplyParameter(INavigationTarget target, object? parameter)
                     {
                         if (!{{parameterizedDictionaryName}}.TryGetValue(target.GetType(), out var entry))
                             return;

                         if (parameter is null)
                             throw new InvalidOperationException($"'{target}' expected '{entry.ParameterType}' parameter");

                         if (!entry.ParameterType.IsInstanceOfType(parameter))
                             throw new InvalidOperationException($"'{target}' does not accept parameter of type '{parameter.GetType()}'.");

                         entry.Apply(target, parameter);
                     }
                 }
                 """;
    }

    private static string GetDefaultContentDictionaryCode(ImmutableArray<ViewModelMetadata> metadata, out string dictionaryName)
    {
        var pairs = metadata
            .Where(m => m.DefaultContentType is not null)
            .Select(m => $"[typeof({m.ClassType})] = typeof({m.DefaultContentType})");

        dictionaryName = "DefaultContents";

        return
            $$"""
                private readonly Dictionary<Type, Type> {{dictionaryName}} = new()
                {
                    {{ string.Join(",\n", pairs) }}
                };
            """;
    }

    private static string GetParentLayoutDictionaryCode(ImmutableArray<ViewModelMetadata> metadata, out string dictionaryName)
    {
        var pairs = metadata
            .Where(m => m.ParentLayoutType is not null)
            .Select(m => $"[typeof({m.ClassType})] = typeof({m.ParentLayoutType})");

        dictionaryName = "ParentLayouts";

        return
            $$"""
                private readonly Dictionary<Type, Type> {{dictionaryName}} = new()
                {
                    {{ string.Join(",\n", pairs) }}
                };
            """;
    }

    private static string GetParameterizedNavigationDictionaryCode(ImmutableArray<ViewModelMetadata> metadata, out string dictionaryName)
    {
        var pairs = metadata
            .Where(m => m.OnNavigatedToMethodName is not null &&
                        m.ParameterType is not null)
            .Select(m => $"""
                           [typeof({m.ClassType})] = (typeof({m.ParameterType}), (target, parameter) =>
                                (({m.ClassType})target).{m.OnNavigatedToMethodName}(({m.ParameterType})parameter!))
                           """);

        dictionaryName = "ParameterizedTargets";

        return
            $$"""
                private readonly Dictionary<Type, (Type ParameterType, Action<INavigationTarget, object?> Apply)> {{dictionaryName}} = new()
                {
                    {{string.Join(",\n", pairs)}}
                };
            """;
    }
}