using System.Reflection;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;

namespace NavigationLibrary.Extensions;

internal static class AttributeTypeExtensions
{
    private static ILayoutAttribute GetLayoutAttribute(Type inheritorType, Type attributeType)
    {
        var attribute = inheritorType
            .GetCustomAttributes()
            .FirstOrDefault(attr =>
                attr.GetType().IsGenericType &&
                attr.GetType().GetGenericTypeDefinition() == attributeType);

        if (attribute is null)
            throw new Exception($"'{inheritorType} has not {attributeType} to navigate");

        return (ILayoutAttribute)attribute;
    }

    public static Type GetLayoutType(this Type inheritorType)
    {
        var attribute = GetLayoutAttribute(inheritorType, typeof(LayoutAttribute<>));
        return attribute.LayoutType;
    }

}