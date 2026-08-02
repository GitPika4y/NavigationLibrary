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

    public static void ApplyParameter(this INavigationTarget target, object? parameter)
    {
        var iface = target.GetType()
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(INavigationTarget<>));

        if (iface is null) return;

        if (parameter is null)
            throw new Exception($"'{target}' expected '{iface.GetGenericArguments()[0]}' parameter");

        if (!iface.GetGenericArguments()[0].IsInstanceOfType(parameter))
            throw new Exception($"'{target}' does not accept parameter of type '{parameter.GetType()}'.");

        iface.GetMethod(nameof(INavigationTarget<object>.OnNavigatedTo))
            !.Invoke(target, [parameter]);
    }
}