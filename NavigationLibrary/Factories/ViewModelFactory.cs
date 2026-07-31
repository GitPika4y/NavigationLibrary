using NavigationLibrary.Abstractions;
using NavigationLibrary.Core;
using NavigationLibrary.Extensions;

namespace NavigationLibrary.Factories;

internal class ViewModelFactory(Func<Type, ViewModelBase> factory) : IViewModelFactory
{
    public ViewModelBase CreateFrom<TViewModel>() where TViewModel : ViewModelBase
    {
        return CreateFrom(typeof(TViewModel));
    }

    public ViewModelBase CreateFrom(Type type)
    {
        type.EnsureIsViewModelBase();
        return factory.Invoke(type);
    }
}