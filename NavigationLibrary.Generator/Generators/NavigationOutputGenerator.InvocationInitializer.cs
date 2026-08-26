using Microsoft.CodeAnalysis;
using NavigationLibrary.Generator.Providers;

namespace NavigationLibrary.Generator.Generators;

public partial class NavigationOutputGenerator
{
    private static void InitializeInvocations(IncrementalGeneratorInitializationContext context)
    {
        var navigateToInvocations = InvocationProvider.Create(context.SyntaxProvider);

        context.RegisterSourceOutput(navigateToInvocations, ReportNavigateToMissingParameter);
    }
}