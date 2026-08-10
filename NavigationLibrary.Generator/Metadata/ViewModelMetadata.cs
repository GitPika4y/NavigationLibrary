namespace NavigationLibrary.Generator.Metadata;

public class ViewModelMetadata(
    string classType,
    string viewType,
    string? parentLayoutType,
    string? defaultContentType,
    string? parameterType,
    string? onNavigatedToMethodName)
{
    public string ClassType { get; } = classType;
    public string ViewType { get; } = viewType;
    public string? ParentLayoutType { get; } = parentLayoutType;
    public string? DefaultContentType { get; } = defaultContentType;
    public string? ParameterType { get; } = parameterType;
    public string? OnNavigatedToMethodName { get; } = onNavigatedToMethodName;
}