using NavigationLibrary.Abstractions;

namespace NavigationLibrary.Core;

internal record LayoutNode(
    Type Type,
    ILayout Instance);