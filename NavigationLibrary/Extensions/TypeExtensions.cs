using NavigationLibrary.Abstractions;

namespace NavigationLibrary.Extensions;

internal static class TypeExtensions
{
    public static void EnsureIsNavigationTarget(this Type type)
    {
        if (!typeof(INavigationTarget).IsAssignableFrom(type))
            throw new Exception($"'{type}' is not inherit from INavigationTarget interface");
    }

    public static Type GetDefaultContentType(this Type layoutType)
    {
        var iface = layoutType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType &&
                                 i.GetGenericTypeDefinition() == typeof(ILayout<>));

        if (iface is null)
            throw new Exception(
                $"'{layoutType}' implements ILayout, but not ILayout<TDefaultContent>. " +
                $"Layouts must implement ILayout<TDefaultContent> to define their default content.");

        return iface.GetGenericArguments()[0];
    }
}