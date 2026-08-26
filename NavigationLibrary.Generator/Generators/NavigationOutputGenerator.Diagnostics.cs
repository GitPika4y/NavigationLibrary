using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NavigationLibrary.Generator.Diagnostics;
using NavigationLibrary.Generator.Metadata;

namespace NavigationLibrary.Generator.Generators;

public partial class NavigationOutputGenerator
{
    private static void ReportViewModelDiagnostics(SourceProductionContext spc, ImmutableArray<ViewModelMetadata> viewModels)
    {
        if (viewModels.IsDefaultOrEmpty)
            return;

        ReportIfMissingViewAttribute(spc, viewModels);
        ReportRootErrors(spc, viewModels);
        ReportParentLayoutErrors(spc, viewModels);
    }


    private static void ReportIfMissingViewAttribute(SourceProductionContext spc,
        ImmutableArray<ViewModelMetadata> metadata)
    {
        foreach (var item in metadata)
        {
            if (item.ViewType is null)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                                         DiagnosticDescriptors.MissingViewAttribute,
                                         item.Location,
                                         item.ClassType));
            }
        }
    }

private static void ReportRootErrors(SourceProductionContext spc, ImmutableArray<ViewModelMetadata> metadata)
{
    var roots = metadata.Where(m => m.IsRoot).ToImmutableArray();

    if (roots.Length == 0)
    {
        spc.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.NoRootLayoutInProject,
            Location.None));
        return;
    }

    if (roots.Length > 1)
    {
        var names = string.Join(", ", roots.Select(r => r.ClassType));
        spc.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.TooManyRootsInProject,
            roots[0].Location,
            names));
        return;
    }

    var root = roots[0];

    if (root.DefaultContentType is null) // не Layout
    {
        spc.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.IncorrectRootClassType,
            root.Location!,
            root.ClassType)); // только имя класса, не готовая фраза
    }

    if (root.ParentLayoutType is not null)
    {
        spc.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.RootCannotHaveParentLayout,
            root.Location!,
            root.ClassType));
    }
}

    private static void ReportParentLayoutErrors(SourceProductionContext spc,
        ImmutableArray<ViewModelMetadata> metadata)
    {
        foreach (var item in metadata)
        {
            if (item.IsRoot)
                continue;

            if (item.ParentLayoutType is null)
                spc.ReportDiagnostic(Diagnostic.Create(
                                         DiagnosticDescriptors.MissingParentLayoutAttribute,
                                         item.Location,
                                         item.ClassType));
        }
    }

    private static void ReportNavigateToMissingParameter(SourceProductionContext spc,
        ImmutableArray<NavigateToInvocationMetadata> invocations)
    {
        if (invocations.IsDefaultOrEmpty)
            return;

        foreach (var invocation in invocations)
        {
            spc.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.NavigateToMissingParameter,
                invocation.Location,
                invocation.DestinationName,
                invocation.ParameterType));
        }
    }
}