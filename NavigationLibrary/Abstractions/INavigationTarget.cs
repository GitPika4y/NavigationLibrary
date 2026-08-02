namespace NavigationLibrary.Abstractions;

public interface INavigationTarget;

public interface INavigationTarget<in TParameter> : INavigationTarget
{
    void OnNavigatedTo(TParameter parameter);
}