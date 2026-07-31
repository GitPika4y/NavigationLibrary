using NavigationLibrary.Core;

namespace NavigationLibrary.Abstractions;

public interface ILayout
{
    ViewModelBase Content { get; set; }
}

public interface ILayout<TDefaultContent>: ILayout
    where TDefaultContent : ViewModelBase
{
    static Type DefaultContentType { get; } = typeof(TDefaultContent);
}