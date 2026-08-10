using NavigationLibrary.Abstractions;
using NavigationLibrary.Extensions;

namespace NavigationLibrary.Core;

internal class NavigationState(
    IViewModelFactory factory,
    INavigationRegistry registry)
{
    private LayoutNode? _layoutsChain;

    /// <summary>
    /// Ensures that every layout from the registered root down to <paramref name="layoutType"/>
    /// exists and is wired as its parent's content. Unlike <see cref="Synchronize"/>, this never
    /// touches default content — it only guarantees the structural chain is in place so that a
    /// caller can immediately place its own explicit content into <paramref name="layoutType"/>.
    /// </summary>
    internal LayoutNode EnsureLayoutRegistered(Type layoutType)
    {
        var existing = _layoutsChain?.Find(layoutType);
        if (existing is not null) return existing;

        var parentLayoutType = registry.GetParentLayoutType(layoutType);

        var parentNode = EnsureLayoutRegistered(parentLayoutType);
        parentNode.TrimAfter();

        var layout = (ILayout)factory.CreateFrom(layoutType);
        parentNode.Instance.Content = (INavigationTarget)layout;

        return RegisterLayout(layout, layoutType);
    }

    /// <summary>
    /// Places <paramref name="contentType"/> as the content of <paramref name="layoutType"/>, applying
    /// <paramref name="parameter"/> to it. Does NOT resolve default content, even if the created
    /// instance turns out to be a layout — that decision belongs to the caller (see <see cref="SetDefaultContent"/>).
    /// </summary>
    internal INavigationTarget SetContent(Type layoutType, Type contentType, object? parameter)
    {
        var layoutNode = GetOrCreate(layoutType);
        layoutNode.TrimAfter();

        var content = factory.CreateFrom(contentType);
        registry.ApplyParameter(content, parameter);
        layoutNode.Instance.Content = content;

        if (content.IsLayout(out var contentLayout))
            RegisterLayout(contentLayout, contentLayout.GetType());

        return content;
    }

    /// <summary>
    /// Resolves <paramref name="layoutType"/>'s own <c>ILayout&lt;TDefaultContent&gt;</c> content, and
    /// cascades further if that default content is itself a layout. Always uses a null parameter —
    /// default content was not explicitly requested, so it cannot carry a caller-supplied parameter.
    /// </summary>
    internal void SetDefaultContent(Type layoutType, HashSet<Type>? visited = null)
    {
        visited ??= [];
        if (!visited.Add(layoutType))
            throw new InvalidOperationException(
                $"Cyclic default content detected involving '{layoutType}'");

        var defaultContentType = registry.GetDefaultContentType(layoutType);
        var content = SetContent(layoutType, defaultContentType, null);

        if (content.IsLayout(out var contentLayout))
            SetDefaultContent(contentLayout.GetType(), visited);
    }

    private LayoutNode GetOrCreate(Type layoutType)
    {
        return _layoutsChain?.Find(layoutType)
               ?? CreateAndRegister(layoutType);
    }

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
