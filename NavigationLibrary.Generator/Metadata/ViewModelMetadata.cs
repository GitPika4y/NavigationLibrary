using Microsoft.CodeAnalysis;

namespace NavigationLibrary.Generator.Metadata;

public class ViewModelMetadata(
    string classType,
    bool isRoot,
    string? viewType,
    string? parentLayoutType,
    string? defaultContentType,
    string? parameterType,
    string? onNavigatedToMethodName,
    Location? location)
{
    public string ClassType { get; } = classType;
    public bool IsRoot { get; } = isRoot;
    public string? ViewType { get; } = viewType;
    public string? ParentLayoutType { get; } = parentLayoutType;
    public string? DefaultContentType { get; } = defaultContentType;
    public string? ParameterType { get; } = parameterType;
    public string? OnNavigatedToMethodName { get; } = onNavigatedToMethodName;
    public Location? Location { get; } = location;
}