using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NavigationLibrary.Generator.Metadata;

namespace NavigationLibrary.Generator.Generators;

public partial class NavigationOutputGenerator
{
    private static void GenerateDataTemplates(SourceProductionContext spc,
        ImmutableArray<ViewModelMetadata> metadata,
        (bool HasWpf, bool HasAvalonia) platformInfo)
    {
        if (metadata.IsDefaultOrEmpty)
            return;

        if (platformInfo is { HasWpf: false, HasAvalonia: false })
            return;

        spc.AddSource("DataTemplateOutput.g.cs", CreateDataTemplatesCode(metadata, platformInfo));
    }

    private static string CreateDataTemplatesCode(ImmutableArray<ViewModelMetadata> metadata,
        (bool HasWpf, bool HasAvalonia) platformInfo)
    {
        var dataTemplatesBody = platformInfo.HasWpf
            ? GetWpfCode(metadata)
            : GetAvaloniaCode(metadata);

        return $$"""
                 using Microsoft.Extensions.DependencyInjection;
                 using NavigationLibrary.Extensions;
                 using NavigationLibrary.Abstractions;
                 
                 namespace NavigationLibrary.Generated;
                 
                 internal static class DataTemplatesOutput
                 {
                    {{dataTemplatesBody}}
                 }
                 
                 public static class NavigationRegistration
                 {
                    public static void AddNavigation(this IServiceCollection services, 
                        {{ (platformInfo.HasWpf
                            ? "System.Windows.ResourceDictionary"
                            : "Avalonia.Controls.Templates.DataTemplates")}} resources)
                    {
                        services.AddNavigationCore();
                        services.AddSingleton<INavigationRegistry, NavigationRegistry>();
                        resources.RegisterDataTemplates();
                    }
                 }
                 """;
    }

    private static string GetWpfCode(ImmutableArray<ViewModelMetadata> metadata)
    {
        var pattern = metadata.Select(m => $"Register<{m.ClassType}, {m.ViewType}>(resources);");

        return $$"""
                internal static void RegisterDataTemplates(this System.Windows.ResourceDictionary resources)
                {
                    {{string.Join("\n\t\t", pattern)}}
                }
                
                private static void Register<TViewModel, TView>(System.Windows.ResourceDictionary resources)
                {
                    var viewModelType = typeof(TViewModel);
                
                    resources.Add(
                        new System.Windows.DataTemplateKey(viewModelType),
                        new System.Windows.DataTemplate
                        {
                            DataType = viewModelType,
                            VisualTree = new System.Windows.FrameworkElementFactory(typeof(TView))
                        } 
                    );
                }
                """;
    }

    private static string GetAvaloniaCode(ImmutableArray<ViewModelMetadata> metadata)
    {
        var pattern = metadata.Select(m =>
            $"templates.Add(new Avalonia.Controls.Templates.FuncDataTemplate<{m.ClassType}>((vm, _) => new {m.ViewType} {{ DataContext = vm }}));");

        return $$"""
                 internal static void RegisterDataTemplates(this Avalonia.Controls.Templates.DataTemplates templates)
                 {
                    {{string.Join("\n\t\t", pattern)}}
                 }
                 """;
    }
}