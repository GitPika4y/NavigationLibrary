using NavigationLibrary.Abstractions;
using NavigationLibrary.Extensions;

namespace NavigationLibrary.Core;

internal class NavigationState(IViewModelFactory factory)
{
    private LayoutNode? _layoutsChain;

    /// <summary>
    /// Sets the layout to its standard content
    /// </summary>
    /// <param name="layoutType"></param>
    /// <param name="parameter"></param>
    internal void Synchronize(Type layoutType, object? parameter = null) => Synchronize(layoutType, parameter, []);

    private void Synchronize(Type layoutType, object? parameter, HashSet<Type> visited)
    {
        if (!visited.Add(layoutType))
            throw new InvalidOperationException(
                $"Cyclic default content detected involving '{layoutType}'");

        var defaultContentType = layoutType.GetDefaultContentType();
        SynchronizeWith(layoutType, defaultContentType, parameter, visited);
    }

    /// <summary>
    /// Sets the layout to the passed content
    /// </summary>
    /// <param name="layoutType"></param>
    /// <param name="contentType"></param>
    /// <param name="parameter"></param>
    internal void SynchronizeWith(Type layoutType, Type contentType, object? parameter = null) => SynchronizeWith(layoutType, contentType, parameter, []);

    private void SynchronizeWith(Type layoutType, Type contentType, object? parameter, HashSet<Type> visited)
    {
        var layoutNode = GetOrCreate(layoutType);
        layoutNode.TrimAfter();
        var content = CreateAndSetContent(layoutNode.Instance, contentType, parameter);

        if (!content.IsLayout(out var contentLayout)) return;

        var contentLayoutType = contentLayout.GetType();
        RegisterLayout(contentLayout, contentLayoutType);
        Synchronize(contentLayoutType, parameter, visited);
    }

    private INavigationTarget CreateAndSetContent(ILayout layoutInstance, Type contentType, object? parameter)
    {
        var content = factory.CreateFrom(contentType);
        content.ApplyParameter(parameter);
        layoutInstance.Content = content;

        return content;
    }

    private LayoutNode GetOrCreate(Type layoutType)
    {
        return _layoutsChain?.Find(layoutType)
               ?? CreateAndRegister(layoutType);
    }

    internal bool IsRegistered(Type layoutType) => _layoutsChain?.Any(layoutType) ?? false;

    internal LayoutNode CreateAndRegister(Type layoutType)
    {
        var layout = (ILayout)factory.CreateFrom(layoutType);
        return RegisterLayout(layout, layoutType);
    }

    private LayoutNode RegisterLayout(ILayout layoutInstance, Type layoutType)
    {
        var node = new LayoutNode(layoutType, layoutInstance);

        if (_layoutsChain is null)
            _layoutsChain = node;
        else
            _layoutsChain.Add(node);

        return node;
    }
}