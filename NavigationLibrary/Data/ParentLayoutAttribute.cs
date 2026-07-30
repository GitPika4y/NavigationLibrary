namespace NavigationLibrary.Data;

[AttributeUsage(AttributeTargets.Class)]
public class ParentLayoutAttribute<TLayoutType>
    : Attribute
    where TLayoutType : ILayout
{
    public Type LayoutType { get; } = typeof(TLayoutType);
}