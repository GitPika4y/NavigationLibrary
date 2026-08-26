using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NavigationLibrary.Generator.Extensions;
using NavigationLibrary.Generator.Metadata;

namespace NavigationLibrary.Generator.Providers;

public static class MetadataProvider
{
    private const string ParentLayoutAttributeMetadataName = "NavigationLibrary.Attributes.ParentLayoutAttribute`1";
    private const string ViewAttributeMetadataName = "NavigationLibrary.Attributes.ViewAttribute`1";
    private const string RootAttributeMetadataName = "NavigationLibrary.Attributes.RootAttribute";
    private const string LayoutInterfaceMetadataName = "NavigationLibrary.Abstractions.ILayout`1";
    private const string NavigationTargetInterfaceMetadataName = "NavigationLibrary.Abstractions.INavigationTarget";

    private const string NavigationTargetInterfaceMarkedMetadataName =
        "NavigationLibrary.Abstractions.INavigationTarget`1";

    public static IncrementalValueProvider<ImmutableArray<ViewModelMetadata>> Create(SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(Filter, Map)
                             .Where(static s => s is not null)
                             .Select(static (m, _) => m!)
                             .Collect();
    }

    /// <summary>
    /// Filter classes only, which has attribute and inheritance
    /// </summary>
    /// <param name="node"></param>
    /// <param name="_"></param>
    /// <returns></returns>
    private static bool Filter(SyntaxNode node, CancellationToken _) =>
        node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 };


    /// <summary>
    /// Filter classes only, which has concrete attributes and inheritances
    /// and return it's class symbol
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="_"></param>
    /// <returns></returns>
    private static ViewModelMetadata? Map(GeneratorSyntaxContext ctx, CancellationToken _)
    {
        if (ctx.SemanticModel.GetDeclaredSymbol(ctx.Node) is not INamedTypeSymbol symbol)
            return null;

        if (symbol.IsAbstract)
            return null;

        var compilation = ctx.SemanticModel.Compilation;

        var viewAttribute = symbol.FindAttribute(compilation, ViewAttributeMetadataName);
        var navigationTargetInterface =
            symbol.FindImplementedInterface(compilation, NavigationTargetInterfaceMetadataName);

        if (navigationTargetInterface is null)
            return null;

        var parentLayoutAttribute = symbol.FindAttribute(compilation, ParentLayoutAttributeMetadataName);
        var layoutInterface       = symbol.FindImplementedInterface(compilation, LayoutInterfaceMetadataName);
        var navigationTargetParameterizedInterface =
            symbol.FindImplementedInterface(compilation, NavigationTargetInterfaceMarkedMetadataName);

        var className               = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var viewType                = viewAttribute?.AttributeClass?.GetGenericArgumentType();
        var isRoot                  = symbol.FindAttribute(compilation, RootAttributeMetadataName) is not null;
        var parentLayoutType        = parentLayoutAttribute?.AttributeClass?.GetGenericArgumentType();
        var defaultContentType      = layoutInterface?.GetGenericArgumentType();
        var navigationParameterType = navigationTargetParameterizedInterface?.GetGenericArgumentType();
        var onNavigatedToMethodName = navigationTargetParameterizedInterface?.GetMembers()
                                                                             .OfType<IMethodSymbol>()
                                                                             .FirstOrDefault()
                                                                            ?.Name;

        var location = symbol.Locations.FirstOrDefault();

        return new ViewModelMetadata(
            className,
            isRoot,
            viewType,
            parentLayoutType,
            defaultContentType,
            navigationParameterType,
            onNavigatedToMethodName,
            location);
    }
}