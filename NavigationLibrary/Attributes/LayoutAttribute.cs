using NavigationLibrary.Abstractions;
using NavigationLibrary.Core;

namespace NavigationLibrary.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class LayoutAttribute<TLayoutType> : Attribute, ILayoutAttribute
    where TLayoutType : ViewModelBase, ILayout
{
    public Type LayoutType { get; } = typeof(TLayoutType);
}