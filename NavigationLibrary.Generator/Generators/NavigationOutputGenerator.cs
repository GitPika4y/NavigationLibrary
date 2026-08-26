using System.Linq;
using Microsoft.CodeAnalysis;
using NavigationLibrary.Generator.Providers;

namespace NavigationLibrary.Generator.Generators;

[Generator]
public partial class NavigationOutputGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        InitializeMetadata(context);
        InitializeInvocations(context);
    }

}