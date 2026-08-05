
namespace NavigationLibrary.Metadata;

internal record ParameterizedNavigationTargetMetadata(
    Type ParameterType,
    Action OnNavigated);