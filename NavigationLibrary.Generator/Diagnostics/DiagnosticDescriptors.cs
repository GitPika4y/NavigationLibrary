using Microsoft.CodeAnalysis;

namespace NavigationLibrary.Generator.Diagnostics;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor MissingViewAttribute = new(
        id: "NAV001",
        title: "Missing [View<T>] attribute",
        messageFormat: "Class '{0}' implements INavigationTarget but is missing a [View<TView>] attribute",
        category: "NavigationLibrary",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Every non-abstract class implementign INavigationTarget must be decorated with [View<TView>] to specify which view renders it."
    );
}