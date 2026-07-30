using NavigationLibrary.Data;

namespace NavigationLibrary.Services;

public interface INavigationService
{
    void NavigateTo<TDestination>() where TDestination : ViewModelBase;
}