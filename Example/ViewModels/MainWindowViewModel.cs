using CommunityToolkit.Mvvm.ComponentModel;
using Example.ViewModels.Main;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;
using NavigationLibrary.Core;

namespace Example.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, ILayout<MainViewModel>
{
    [ObservableProperty] private ViewModelBase _content;
}