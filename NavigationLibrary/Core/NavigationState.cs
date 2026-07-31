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
    public void Synchronize(Type layoutType) => Synchronize(layoutType, []);

    private void Synchronize(Type layoutType, HashSet<Type> visited)
    {
        if (!visited.Add(layoutType))
            throw new InvalidOperationException(
                $"Cyclic default content detected involving '{layoutType}'");

        var defaultContentType = layoutType.GetDefaultContentType();
        SynchronizeWith(layoutType, defaultContentType, visited);
    }

    /// <summary>
    /// Sets the layout to the passed content
    /// </summary>
    /// <param name="layoutType"></param>
    /// <param name="contentType"></param>
    public void SynchronizeWith(Type layoutType, Type contentType) => SynchronizeWith(layoutType, contentType, []);


    private void SynchronizeWith(Type layoutType, Type contentType, HashSet<Type> visited)
    {
        var layoutNode = GetOrCreate(layoutType);
        layoutNode.TrimAfter();
        var content = CreateAndSetContent(layoutNode.Instance, contentType);

        if (content.IsILayout(out var contentLayout))
        {
            var contentLayoutType = contentLayout.GetType();
            RegisterLayout(contentLayout, contentLayoutType);
            Synchronize(contentLayoutType, visited);
        }
    }

    private ViewModelBase CreateAndSetContent(ILayout layoutInstance, Type contentType)
    {
        var content = factory.CreateFrom(contentType);
        layoutInstance.Content = content;

        return content;
    }

    private LayoutNode GetOrCreate(Type layoutType)
    {
        return _layoutsChain?.Find(layoutType)
               ?? CreateAndRegister(layoutType);
    }

    public bool IsRegistered(Type layoutType) => _layoutsChain?.Any(layoutType) ?? false;

    public LayoutNode CreateAndRegister(Type layoutType)
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