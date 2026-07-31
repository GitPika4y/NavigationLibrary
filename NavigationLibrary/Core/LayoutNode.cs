using NavigationLibrary.Abstractions;

namespace NavigationLibrary.Core;

internal class LayoutNode(
    Type type,
    ILayout instance)
{
    public Type Type { get; } = type;
    public ILayout Instance { get; } = instance;
    public LayoutNode? Next { get; private set; }

    public LayoutNode? Find(Type type)
    {
        for (var node = this; node is not null; node = node.Next)
        {
            if (node.Type == type) return node;
        }

        return null;
    }

    public void Add(LayoutNode node)
    {
        var last = this;
        while (last.Next is not null) last = last.Next;

        last.Next = node;
    }
    public void TrimAfter() => Next = null;
    public bool Any(Type type) => Find(type) is not null;
}