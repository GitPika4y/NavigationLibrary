using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NavigationLibrary.Generator.Extensions;
using NavigationLibrary.Generator.Metadata;

namespace NavigationLibrary.Generator.Providers;

public static class InvocationProvider
{
    private const string NavigationServiceMetadataName = "NavigationLibrary.Abstractions.INavigationService";
    private const string NavigationTargetParameterizedMetadataName = "NavigationLibrary.Abstractions.INavigationTarget`1";

    public static IncrementalValueProvider<ImmutableArray<NavigateToInvocationMetadata>> Create(
        SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(Filter, Map)
                             .Where(static m => m is not null)
                             .Select(static (m, _) => m!)
                             .Collect();
    }

    private static bool Filter(SyntaxNode node, CancellationToken _)
    {
        return node is InvocationExpressionSyntax
        {
            ArgumentList.Arguments.Count: 0,
            Expression: MemberAccessExpressionSyntax
            {
                Name: GenericNameSyntax
                {
                    Identifier.ValueText            : "NavigateTo",
                    TypeArgumentList.Arguments.Count: 1
                }
            }
        };
    }

    private static NavigateToInvocationMetadata? Map(GeneratorSyntaxContext ctx, CancellationToken _)
    {
        var invocation = (InvocationExpressionSyntax)ctx.Node;

        if (ctx.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol methodSymbol)
            return null;

        if (!IsNavigationServiceNavigateTo(methodSymbol))
            return null;

        if (methodSymbol.TypeArguments[0] is not INamedTypeSymbol destinationType)
            return null;

        var compilation = ctx.SemanticModel.Compilation;

        var parameterizedInterface =
            destinationType.FindImplementedInterface(compilation, NavigationTargetParameterizedMetadataName);

        if (parameterizedInterface is null)
            return null;

        return new NavigateToInvocationMetadata(
            destinationType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            parameterizedInterface.GetGenericArgumentType(),
            invocation.GetLocation());
    }

    private static bool IsNavigationServiceNavigateTo(IMethodSymbol method)
    {
        if (method.Name != "NavigateTo" || method.Arity != 1 || method.Parameters.Length != 0)
            return false;

        var containingType = method.ContainingType;
        if (containingType is null)
            return false;

        if (containingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ==
            $"global::{NavigationServiceMetadataName}")
            return true;

        return containingType.AllInterfaces.Any(i =>
            i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ==
            $"global::{NavigationServiceMetadataName}");
    }
}