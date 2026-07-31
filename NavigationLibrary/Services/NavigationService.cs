using NavigationLibrary.Abstractions;
using NavigationLibrary.Core;
using NavigationLibrary.Extensions;

namespace NavigationLibrary.Services;

internal class NavigationService(NavigationState state) : INavigationService
{
    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        NavigateTo(typeof(TViewModel));
    }

    public void NavigateTo(Type destinationType)
    {
        destinationType.EnsureIsViewModelBase();

        var destinationLayoutType = destinationType.GetLayoutType();

        if (!state.IsRegistered(destinationLayoutType))
            NavigateTo(destinationLayoutType);

        state.SynchronizeWith(destinationLayoutType, destinationType);
    }
}