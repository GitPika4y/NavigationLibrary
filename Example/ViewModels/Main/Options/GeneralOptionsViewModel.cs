using CommunityToolkit.Mvvm.Input;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;
using NavigationLibrary.Core;

namespace Example.ViewModels.Main.Options;

[Layout<OptionsViewModel>]
public partial class GeneralOptionsViewModel(INavigationService navigationService) : ViewModelBase
{
    [RelayCommand]
    private void NavigateToMainView()
    {
        navigationService.NavigateTo<MainViewModel>();
    }
}