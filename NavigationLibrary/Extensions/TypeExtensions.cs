using NavigationLibrary.Abstractions;
using NavigationLibrary.Core;

namespace NavigationLibrary.Extensions;

internal static class TypeExtensions
{
    public static void EnsureIsNavigationViewModel(this Type type)
    {
        if (!typeof(INavigationTarget).IsAssignableFrom(type))
            throw new Exception($"'{type}' is not inherit from INavigationViewModel interface");
    }

    public static bool IsILayout(this INavigationTarget navigationViewModel, out ILayout layout)
    {
        if (navigationViewModel is not ILayout layoutInstance)
        {
            layout = null!;
            return false;
        }

        layout = layoutInstance;
        return true;
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