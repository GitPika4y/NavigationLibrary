using Microsoft.CodeAnalysis;

namespace NavigationLibrary.Generator.Metadata;

public class NavigateToInvocationMetadata(
    string destinationName,
    string parameterType,
    Location location)
{
    public Location Location => location;
    public string ParameterType => parameterType;
    public string DestinationName => destinationName;
}