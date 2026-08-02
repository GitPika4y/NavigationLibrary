using NavigationLibrary.Abstractions;
using NavigationLibrary.Core;
using NavigationLibrary.Extensions;

namespace NavigationLibrary.Factories;

internal class ViewModelFactory(Func<Type, INavigationTarget> factory) : IViewModelFactory
{
    public INavigationTarget CreateFrom<TViewModel>() where TViewModel : INavigationTarget
    {
        return CreateFrom(typeof(TViewModel));
    }

    public INavigationTarget CreateFrom(Type type)
    {
        type.EnsureIsNavigationTarget();
        return factory.Invoke(type);
    }
}