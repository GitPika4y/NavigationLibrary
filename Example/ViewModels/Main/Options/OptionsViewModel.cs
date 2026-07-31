using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;
using NavigationLibrary.Core;

namespace Example.ViewModels.Main.Options;

[Layout<MainViewModel>]
public partial class OptionsViewModel: ViewModelBase, ILayout<GeneralOptionsViewModel>
{
    [ObservableProperty] private ViewModelBase _content;

}