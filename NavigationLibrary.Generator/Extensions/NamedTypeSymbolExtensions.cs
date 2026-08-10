using Microsoft.CodeAnalysis;

namespace NavigationLibrary.Generator.Extensions;

public static class NamedTypeSymbolExtensions
{
    public static INamedTypeSymbol? FindImplementedInterface(this INamedTypeSymbol classSymbol, Compilation compilation,
        string interfaceMetadataName)
    {
        var interfaceDefinition = compilation.GetTypeByMetadataName(interfaceMetadataName);
        if (interfaceDefinition is null)
            return null;

        foreach (var iface in classSymbol.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(iface.OriginalDefinition, interfaceDefinition))
                return iface;
        }

        return null;
    }

    public static AttributeData? FindAttribute(this INamedTypeSymbol classSymbol, Compilation compilation,
        string attributeMetadataName)
    {
        var attributeDefinition = compilation.GetTypeByMetadataName(attributeMetadataName);
        if (attributeDefinition is null)
            return null;

        foreach (var attribute in classSymbol.GetAttributes())
        {
            var attrClass = attribute.AttributeClass;
            if (attrClass is not null &&
                SymbolEqualityComparer.Default.Equals(attrClass.OriginalDefinition, attributeDefinition))
            {
                return attribute;
            }
        }

        return null;
    }


    public static string GetGenericArgumentType(this INamedTypeSymbol symbol)
    {
        return symbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }
}