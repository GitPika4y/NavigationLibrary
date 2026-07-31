using CommunityToolkit.Mvvm.Input;
using Example.Views.Main;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;
using NavigationLibrary.Core;

namespace Example.ViewModels.Main;

[Layout<MainViewModel>]
public partial class DashboardViewModel(INavigationService navigationService): ViewModelBase
{
    [RelayCommand]
    private void NavigateToIssues()
    {
        navigationService.NavigateTo<Options.OptionsViewModel>();
    }
}