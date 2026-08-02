using NavigationLibrary.Abstractions;

namespace NavigationLibrary.Core;

internal class LayoutNode(Type type, ILayout instance)
{
    private readonly Type _type = type;
    private LayoutNode? _next;
    public ILayout Instance { get; } = instance;

    public LayoutNode? Find(Type type)
    {
        for (var node = this; node is not null; node = node._next)
        {
            if (node._type == type) return node;
        }

        return null;
    }

    public void Add(LayoutNode node)
    {
        var last = this;
        while (last._next is not null) last = last._next;

        last._next = node;
    }
    public void TrimAfter() => _next = null;
    public bool Any(Type type) => Find(type) is not null;
}