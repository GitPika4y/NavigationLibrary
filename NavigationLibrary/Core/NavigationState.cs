using NavigationLibrary.Abstractions;
using NavigationLibrary.Extensions;

namespace NavigationLibrary.Core;

internal class NavigationState(IViewModelFactory factory)
{
    private readonly List<LayoutNode> _layouts = [];

    public LayoutNode CreateAndRegister(Type layoutType)
    {
        var layout = (ILayout)factory.CreateFrom(layoutType);
        return RegisterLayout(layout, layoutType);
    }

    public bool IsRegistered(Type layoutType) => _layouts.Any(l => l.Type == layoutType);

    /// <summary>
    /// Sets the layout to its standard content
    /// </summary>
    /// <param name="layoutType"></param>
    public void Synchronize(Type layoutType) => Synchronize(layoutType, []);

    /// <summary>
    /// Sets the layout to the passed content
    /// </summary>
    /// <param name="layoutType"></param>
    /// <param name="contentType"></param>
    public void SynchronizeWith(Type layoutType, Type contentType) => SynchronizeWith(layoutType, contentType, []);

    private void Synchronize(Type layoutType, HashSet<Type> visited)
    {
        if (!visited.Add(layoutType))
            throw new InvalidOperationException(
                $"Cyclic default content detected involving '{layoutType}'");

        var defaultContentType = layoutType.GetDefaultContentType();
        SynchronizeWith(layoutType, defaultContentType, visited);
    }


    private void SynchronizeWith(Type layoutType, Type contentType, HashSet<Type> visited)
    {
        var layoutNode = GetOrCreate(layoutType);
        TrimAfter(layoutNode);
        var content = CreateAndSetContent(layoutNode.Instance, contentType);

        if (content.IsILayout(out var contentLayout))
        {
            var contentLayoutType = contentLayout.GetType();
            RegisterLayout(contentLayout, contentLayoutType);
            Synchronize(contentLayoutType, visited);
        }
    }

    private LayoutNode GetOrCreate(Type layoutType)
    {
        return _layouts.FirstOrDefault(n => n.Type == layoutType)
               ?? CreateAndRegister(layoutType);
    }

    private void TrimAfter(LayoutNode layout)
    {
        var index = _layouts.IndexOf(layout);
        var length = _layouts.Count;
        _layouts.RemoveRange(index + 1, length - index - 1);
    }

    private ViewModelBase CreateAndSetContent(ILayout layoutInstance, Type contentType)
    {
        var content = factory.CreateFrom(contentType);
        layoutInstance.Content = content;

        return content;
    }

    private LayoutNode RegisterLayout(ILayout layoutInstance, Type layoutType)
    {
        var node = new LayoutNode(layoutType, layoutInstance);
        _layouts.Add(node);
        return node;
    }
}