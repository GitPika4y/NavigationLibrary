using CommunityToolkit.Mvvm.ComponentModel;
using NavigationLibrary.Abstractions;

namespace Example.ViewModels;

public partial class NavigationLayoutViewModel<TDefaultContent>: NavigationTargetViewModel, ILayout<TDefaultContent>
    where TDefaultContent : INavigationTarget
{
    [ObservableProperty] private INavigationTarget _content = null!;
}