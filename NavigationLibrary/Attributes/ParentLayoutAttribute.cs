using NavigationLibrary.Abstractions;

namespace NavigationLibrary.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ParentLayoutAttribute<TLayoutType> : Attribute
    where TLayoutType : INavigationTarget, ILayout;