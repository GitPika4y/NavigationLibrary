namespace NavigationLibrary.Abstractions;

public interface INavigationRegistry
{
    Type GetDefaultContentType(Type layoutType);
    Type GetParentLayoutType(Type inheritorType);
    void ApplyParameter(INavigationTarget target, object? parameter);
}