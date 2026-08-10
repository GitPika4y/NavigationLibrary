using NavigationLibrary.Abstractions;

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
}