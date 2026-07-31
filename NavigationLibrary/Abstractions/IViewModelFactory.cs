using NavigationLibrary.Core;

namespace NavigationLibrary.Abstractions;

internal interface IViewModelFactory
{
    ViewModelBase CreateFrom<TViewModel>() where TViewModel : ViewModelBase;
    ViewModelBase CreateFrom(Type type);
}