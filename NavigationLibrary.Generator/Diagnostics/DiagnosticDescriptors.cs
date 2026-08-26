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
        description:
        "Every non-abstract class implementation INavigationTarget must be decorated with [View<TView>] to specify which view renders it."
    );

    public static readonly DiagnosticDescriptor MissingParentLayoutAttribute = new(
        id: "NAV005",
        title: "Missing [ParentLayout<T>] attribute",
        messageFormat:
        "Class '{0}' implements INavigationTarget but is missing a [ParentLayout<T>] attribute. If this is the navigation root, decorate it with [Root] instead",
        category: "NavigationLibrary",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "Every non-root navigation target must declare its parent layout via [ParentLayout<T>]. The single root of the navigation tree should use [Root] instead."
    );

    public static readonly DiagnosticDescriptor RootCannotHaveParentLayout = new(
        id: "NAV006",
        title: "[Root] class cannot also have [ParentLayout<T>]",
        messageFormat: "Class '{0}' is decorated with both [Root] and [ParentLayout<T>] — a root has no parent",
        category: "NavigationLibrary",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "A class marked [Root] represents the top of the navigation tree and must not declare a parent layout."
    );

    public static readonly DiagnosticDescriptor NoRootLayoutInProject = new(
        id: "NAV002",
        title: "Missing [Root] class",
        messageFormat: "No INavigationTarget class in the project is decorated with [Root]",
        category: "NavigationLibrary",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "There must be exactly one ILayout<TDefaultContent> class decorated with [Root]."
    );

    public static readonly DiagnosticDescriptor TooManyRootsInProject = new(
        id: "NAV003",
        title: "Too many [Root] classes in project",
        messageFormat: "Multiple classes are decorated with [Root]: '{0}'. Only one root is allowed",
        category: "NavigationLibrary",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "There must be exactly one ILayout<TDefaultContent> class decorated with [Root]."
    );

    public static readonly DiagnosticDescriptor IncorrectRootClassType = new(
        id: "NAV004",
        title: "[Root] class has incorrect implementation type",
        messageFormat: "Root class '{0}' must implement ILayout<TDefaultContent>",
        category: "NavigationLibrary",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The class decorated with [Root] must implement ILayout<TDefaultContent>."
    );

    public static readonly DiagnosticDescriptor NavigateToMissingParameter = new(
        id: "NAV007",
        title: "NavigateTo<T>() called on a target that requires a parameter",
        messageFormat:"'{0}' implements INavigationTarget<{1}> and requires a parameter. Call NavigateTo<{0}, {1}>(parameter) instead",
        category: "NavigationLibrary",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}