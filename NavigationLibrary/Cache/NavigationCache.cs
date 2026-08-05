using System.Collections.Concurrent;
using System.Reflection;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;
using NavigationLibrary.Metadata;

namespace NavigationLibrary.Cache;

internal static class NavigationCache
{
    private static readonly ConcurrentDictionary<Type, Type> DefaultContentTypes = [];
    private static readonly ConcurrentDictionary<Type, Type> LayoutTypes = [];
    private static readonly ConcurrentDictionary<Type, ParameterizedNavigationTargetMetadata?> NavigationTargets = [];

    internal static Type GetDefaultContentType(Type layoutType)
    {
        return DefaultContentTypes.GetOrAdd(layoutType, type =>
        {
            var iface = type
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType &&
                                     i.GetGenericTypeDefinition() == typeof(ILayout<>));

            if (iface is null)
                throw new Exception(
                    $"'{type}' implements ILayout, but not ILayout<TDefaultContent>. " +
                    $"Layouts must implement ILayout<TDefaultContent> to define their default content.");

            return iface.GetGenericArguments()[0];
        });
    }

    internal static Type GetParentLayoutType(Type inheritorType)
    {
        return LayoutTypes.GetOrAdd(inheritorType, type =>
        {
            var attribute = type
                .GetCustomAttributes()
                .FirstOrDefault(attr =>
                    attr.GetType().IsGenericType &&
                    attr.GetType().GetGenericTypeDefinition() == typeof(ParentLayoutAttribute<>));

            if (attribute is null)
                throw new Exception($"'{type}' has not {typeof(ParentLayoutAttribute<>)} to navigate");

            return attribute.GetType().GetGenericArguments()[0];
        });
    }

    internal static void ApplyParameter(INavigationTarget target, object? parameter)
    {
        var metadata = NavigationTargets.GetOrAdd(target.GetType(), type =>
        {
            var iface = type
                .GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(INavigationTarget<>));

            if (iface is null)
                return null;

            var parameterType = iface.GetGenericArguments()[0];
            var method = iface.GetMethod(nameof(INavigationTarget<object>.OnNavigatedTo))!;

            return new ParameterizedNavigationTargetMetadata(parameterType,() => method.Invoke(target, [parameter]));
        });

        if (metadata is null)
            return;

        if (parameter is null)
            throw new InvalidOperationException($"'{target}' expected '{metadata.ParameterType}' parameter");

        if (!metadata.ParameterType.IsInstanceOfType(parameter))
            throw new InvalidOperationException($"'{target}' does not accept parameter of type '{parameter.GetType()}'.");

        metadata.OnNavigated.Invoke();
    }
}