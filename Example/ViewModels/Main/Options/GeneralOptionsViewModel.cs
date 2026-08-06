using System;
using CommunityToolkit.Mvvm.Input;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;

namespace Example.ViewModels.Main.Options;

[ParentLayout<OptionsViewModel>]
public partial class GeneralOptionsViewModel(INavigationService navigationService) : NavigationTargetViewModel<string>
{
    [RelayCommand]
    private void NavigateToMainView()
    {
        navigationService.NavigateTo<DashboardViewModel>();
    }

    public override void OnNavigatedTo(string parameter)
    {
        Console.WriteLine(parameter);
    }
}