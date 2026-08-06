using NavigationLibrary.Abstractions;
using NavigationLibrary.Cache;

namespace NavigationLibrary.Extensions;

public static class NavigationTargetExtensions
{
    public static bool IsLayout(this INavigationTarget target, out ILayout layout)
    {
        if (target is not ILayout layoutInstance)
        {
            layout = null!;
            return false;
        }

        layout = layoutInstance;
        return true;
    }

    public static void ApplyParameter(this INavigationTarget target, object? parameter)
    {
        NavigationCache.ApplyParameter(target, parameter);
    }
}