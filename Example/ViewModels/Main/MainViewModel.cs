using CommunityToolkit.Mvvm.ComponentModel;
using Example.ViewModels.Main.Options;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Attributes;

namespace Example.ViewModels.Main;

[ParentLayout<MainWindowViewModel>]
public partial class MainViewModel: NavigationLayoutViewModel<DashboardViewModel>;