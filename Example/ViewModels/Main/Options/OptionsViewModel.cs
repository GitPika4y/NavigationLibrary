using CommunityToolkit.Mvvm.ComponentModel;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;
using NavigationLibrary.Core;

namespace Example.ViewModels.Main.Options;

[ParentLayout<MainViewModel>]
public partial class OptionsViewModel: NavigationLayoutViewModel<GeneralOptionsViewModel>;