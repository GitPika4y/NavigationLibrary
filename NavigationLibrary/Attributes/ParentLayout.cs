using NavigationLibrary.Abstractions;

namespace NavigationLibrary.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ParentLayout<TLayoutType> : Attribute
    where TLayoutType : INavigationTarget, ILayout;