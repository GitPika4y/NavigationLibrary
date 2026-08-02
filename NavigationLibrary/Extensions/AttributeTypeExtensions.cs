using System.Reflection;
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
                attr.GetType().GetGenericTypeDefinition() == typeof(ParentLayoutAttribute<>));

        if (attribute is null)
            throw new Exception($"'{inheritorType}' has not {typeof(ParentLayoutAttribute<>)} to navigate");

        return attribute.GetType().GetGenericArguments()[0];
    }
}