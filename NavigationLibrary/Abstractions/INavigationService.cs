using NavigationLibrary.Core;

namespace NavigationLibrary.Abstractions;

public interface INavigationService
{
    void NavigateTo<TDestination>() where TDestination : INavigationTarget;
    void NavigateTo(Type destinationType);
}