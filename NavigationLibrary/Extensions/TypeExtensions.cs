using NavigationLibrary.Abstractions;
using NavigationLibrary.Core;

namespace NavigationLibrary.Extensions;

internal static class TypeExtensions
{
    public static void EnsureIsViewModelBase(this Type type)
    {
        if (!typeof(ViewModelBase).IsAssignableFrom(type))
            throw new Exception($"'{type}' is not inherit from ViewModelBase class");
    }

    public static bool IsILayout(this ViewModelBase viewModel, out ILayout layout)
    {
        if (viewModel is not ILayout layoutInstance)
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