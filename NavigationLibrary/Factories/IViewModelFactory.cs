using NavigationLibrary.Data;

namespace NavigationLibrary.Factories;

public interface IViewModelFactory
{
    ViewModelBase CreateFrom<TViewModel>() where TViewModel : ViewModelBase;
    ViewModelBase CreateFrom(Type type);
}