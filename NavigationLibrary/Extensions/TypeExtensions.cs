using NavigationLibrary.Abstractions;

namespace NavigationLibrary.Extensions;

internal static class TypeExtensions
{
    internal static void EnsureIsNavigationTarget(this Type type)
    {
        if (!typeof(INavigationTarget).IsAssignableFrom(type))
            throw new Exception($"'{type}' is not inherit from INavigationTarget interface");
    }

    internal static bool IsLayout(this Type type)
    {
        return typeof(ILayout).IsAssignableFrom(type);
    }
}