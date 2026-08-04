using CommunityToolkit.Mvvm.Input;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;

namespace Example.ViewModels.Main.Options;

[ParentLayout<OptionsViewModel>]
public partial class GeneralOptionsViewModel(INavigationService navigationService) : NavigationTargetViewModel
{
    [RelayCommand]
    private void NavigateToMainView()
    {
        navigationService.NavigateTo<DashboardViewModel>();
    }
}