using NavigationLibrary.Core;

namespace NavigationLibrary.Abstractions;

public interface INavigationService
{
    void NavigateTo<TDestination>() where TDestination : ViewModelBase;
    void NavigateTo(Type destinationType);
}