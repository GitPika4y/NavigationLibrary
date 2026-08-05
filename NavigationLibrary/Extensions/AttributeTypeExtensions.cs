using NavigationLibrary.Cache;

namespace NavigationLibrary.Extensions;

internal static class AttributeTypeExtensions
{
    public static Type GetParentLayoutType(this Type inheritorType) =>
        NavigationCache.GetParentLayoutType(inheritorType);
}