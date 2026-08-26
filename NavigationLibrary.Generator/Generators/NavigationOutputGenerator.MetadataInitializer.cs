using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NavigationLibrary.Generator.Metadata;
using NavigationLibrary.Generator.Providers;

namespace NavigationLibrary.Generator.Generators;

public partial class NavigationOutputGenerator
{
    private static void InitializeMetadata(IncrementalGeneratorInitializationContext context)
    {
        var metadataList = MetadataProvider.Create(context.SyntaxProvider);
        var platformInfo = context.CompilationProvider.Select(static (compilation, _) =>
                                                                  (HasWpf: HasWpf(compilation),
                                                                   HasAvalonia: HasAvalonia(compilation)));

        var combined = metadataList.Combine(platformInfo);

        context.RegisterSourceOutput(metadataList, ReportViewModelDiagnostics);
        context.RegisterSourceOutput(metadataList, GenerateNavigationRegistry);
        context.RegisterSourceOutput(combined, (spc, pair) => GenerateDataTemplates(spc, pair.Left, pair.Right));
        context.RegisterSourceOutput(metadataList, GenerateRootInitializer);
    }

    private static bool HasAvalonia(Compilation compilation) =>
        compilation.ReferencedAssemblyNames.Any(a => a.Name == "Avalonia.Controls");

    private static bool HasWpf(Compilation compilation) =>
        compilation.ReferencedAssemblyNames.Any(a => a.Name == "PresentationFramework");

    private static void GenerateRootInitializer(SourceProductionContext spc, ImmutableArray<ViewModelMetadata> viewModels)
    {
        var rootType = viewModels.First(v => v.IsRoot).ClassType;

        spc.AddSource("RootInitializer.g.cs",
            $$"""
            using Microsoft.Extensions.DependencyInjection;
            using NavigationLibrary.Extensions;
            using NavigationLibrary.Abstractions;
            
            namespace NavigationLibrary.Generated;
            
            public static class RootInitializer
            {
                public static ILayout InitializeNavigationRoot(this IServiceProvider provider)
                {
                    return provider.InitializeNavigationRootCore<{{rootType}}>();
                }
            }
            """ );
    }
}