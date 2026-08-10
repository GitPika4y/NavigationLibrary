using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NavigationLibrary.Generator.Diagnostics;
using NavigationLibrary.Generator.Metadata;

namespace NavigationLibrary.Generator.Generators;

[Generator]
public partial class NavigationOutputGenerator: IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var metadataList = MetadataProvider.Create(context.SyntaxProvider);
        var platformInfo = context.CompilationProvider.Select(static (compilation, _) =>
            (HasWpf: HasWpf(compilation), HasAvalonia: HasAvalonia(compilation)));

        var combined = metadataList.Combine(platformInfo);

        context.RegisterSourceOutput(metadataList, ReportDiagnostics);
        context.RegisterSourceOutput(metadataList, GenerateNavigationRegistry);
        context.RegisterSourceOutput(combined, (spc, pair) => GenerateDataTemplates(spc, pair.Left, pair.Right));
    }

    private static void ReportDiagnostics(SourceProductionContext spc, ImmutableArray<ViewModelMetadata> metadata)
    {
        if (metadata.IsDefaultOrEmpty)
            return;

        foreach (var item in metadata)
        {
            if (item.ViewType is null)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.MissingViewAttribute,
                    item.Location,
                    item.ClassType ));
            }
        }
    }

    private static bool HasAvalonia(Compilation compilation) =>
        compilation.ReferencedAssemblyNames.Any(a => a.Name == "Avalonia.Controls");

    private static bool HasWpf(Compilation compilation) =>
        compilation.ReferencedAssemblyNames.Any(a => a.Name == "PresentationFramework");
}