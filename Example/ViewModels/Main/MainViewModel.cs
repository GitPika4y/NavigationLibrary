using CommunityToolkit.Mvvm.ComponentModel;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;
using NavigationLibrary.Core;

namespace Example.ViewModels.Main;

[Layout<MainWindowViewModel>]
public partial class MainViewModel: ViewModelBase, ILayout<DashboardViewModel>
{
    [ObservableProperty] private ViewModelBase _content;
}