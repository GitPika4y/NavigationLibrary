namespace NavigationLibrary.Abstractions;

public interface INavigationService
{
    void NavigateTo<TDestination>() where TDestination : INavigationTarget;
    void NavigateTo(Type destinationType);

    void NavigateTo<TDestination, TParameter>(TParameter parameter) where TDestination : INavigationTarget<TParameter>;
    void NavigateTo(Type destinationType, object? parameter);
}