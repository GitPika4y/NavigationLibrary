using NavigationLibrary.Abstractions;
using NavigationLibrary.Core;
using NavigationLibrary.Extensions;

namespace NavigationLibrary.Services;

internal class NavigationService(NavigationState state) : INavigationService
{
    public void NavigateTo<TViewModel>() where TViewModel : INavigationTarget
    {
        NavigateTo(typeof(TViewModel));
    }

    public void NavigateTo(Type destinationType)
    {
        NavigateTo(destinationType, null);
    }

    public void NavigateTo<TDestination, TParameter>(TParameter parameter) where TDestination : INavigationTarget<TParameter>
    {
        NavigateTo(typeof(TDestination), parameter);
    }

    public void NavigateTo(Type destinationType, object? parameter)
    {
        destinationType.EnsureIsNavigationTarget();
        var layoutType = destinationType.GetParentLayoutType();
        state.EnsureLayoutRegistered(layoutType);
        var content = state.SetContent(layoutType, destinationType, parameter);

        if (content.IsLayout(out _))
            state.SetDefaultContent(destinationType);
    }
}