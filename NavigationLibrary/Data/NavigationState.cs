using NavigationLibrary.Factories;

namespace NavigationLibrary.Data;
internal class NavigationState(IViewModelFactory factory)
{
    private readonly List<LayoutNode> _layouts = [];
    public void UpdateWith(Type layoutType, Type destinationType)
    {
        var existing = _layouts.Find(l => l.Type == layoutType);
        if (existing is null)
            existing = Register(layoutType);

        ClearChildren(existing);

        var destinationInstance = factory.CreateFrom(destinationType);
        existing.Instance.CurrentViewModel = destinationInstance;

        if (destinationInstance is ILayout destinationLayout)
            _layouts.Add(new LayoutNode(destinationType, destinationLayout));
    }

    private void ClearChildren(LayoutNode node)
    {
        var index = _layouts.IndexOf(node);
        var length = _layouts.Count;
        _layouts.RemoveRange(index + 1, length - index - 1);
    }

    public LayoutNode Register(Type layoutType)
    {
        var layout = (ILayout)factory.CreateFrom(layoutType);
        var node = new LayoutNode(layoutType, layout);
        _layouts.Add(node);
        return node;
    }

    public bool IsRegistered(Type layoutType) => _layouts.Any(l => l.Type == layoutType);
}