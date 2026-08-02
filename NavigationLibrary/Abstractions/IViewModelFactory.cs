namespace NavigationLibrary.Abstractions;

internal interface IViewModelFactory
{
    INavigationTarget CreateFrom(Type type);
}