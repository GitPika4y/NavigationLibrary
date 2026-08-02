using NavigationLibrary.Core;

namespace NavigationLibrary.Abstractions;

internal interface IViewModelFactory
{
    INavigationTarget CreateFrom<TViewModel>() where TViewModel : INavigationTarget;
    INavigationTarget CreateFrom(Type type);
}