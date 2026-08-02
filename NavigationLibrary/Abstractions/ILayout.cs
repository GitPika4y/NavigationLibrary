using NavigationLibrary.Core;

namespace NavigationLibrary.Abstractions;

public interface ILayout
{
    INavigationTarget Content { get; set; }
}

public interface ILayout<TDefaultContent>: ILayout
    where TDefaultContent : INavigationTarget;