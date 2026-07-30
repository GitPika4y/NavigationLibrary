using NavigationLibrary.Data;

namespace NavigationLibrary.Factories;

internal class ViewModelFactory(Func<Type, ViewModelBase> factory) : IViewModelFactory
{
    public ViewModelBase CreateFrom<TViewModel>() where TViewModel : ViewModelBase
    {
        var type = typeof(TViewModel);
        return factory.Invoke(type);
    }

    public ViewModelBase CreateFrom(Type type)
    {
        return factory.Invoke(type);
    }
}