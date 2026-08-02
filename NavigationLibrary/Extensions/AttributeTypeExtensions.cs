using System.Reflection;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;

namespace NavigationLibrary.Extensions;

internal static class AttributeTypeExtensions
{
    public static Type GetLayoutType(this Type inheritorType)
    {
        var attribute = inheritorType
            .GetCustomAttributes()
            .FirstOrDefault(attr =>
                attr.GetType().IsGenericType &&
                attr.GetType().GetGenericTypeDefinition() == typeof(ParentLayout<>));

        if (attribute is null)
            throw new Exception($"'{inheritorType} has not {typeof(ParentLayout<>)} to navigate");

        return attribute.GetType().GetGenericArguments()[0];
    }
}