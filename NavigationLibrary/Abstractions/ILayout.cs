namespace NavigationLibrary.Abstractions;

public interface ILayout: INavigationTarget
{
    INavigationTarget Content { get; set; }
}

public interface ILayout<TDefaultContent>: ILayout
    where TDefaultContent : INavigationTarget;