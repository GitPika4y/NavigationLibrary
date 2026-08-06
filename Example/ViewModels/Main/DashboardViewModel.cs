using CommunityToolkit.Mvvm.Input;
using Example.ViewModels.Main.Options;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;

namespace Example.ViewModels.Main;

[ParentLayout<MainViewModel>]
public partial class DashboardViewModel(INavigationService navigationService): NavigationTargetViewModel
{
    [RelayCommand]
    private void NavigateToIssues()
    {
        navigationService.NavigateTo<AnotherOptionsViewModel>();
    }
}