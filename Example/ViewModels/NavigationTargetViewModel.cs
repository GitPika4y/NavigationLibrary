using System.ComponentModel.Design.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Core;

namespace Example.ViewModels;

public class NavigationTargetViewModel: ObservableObject, INavigationTarget;

public abstract class NavigationTargetViewModel<TParameter>: ObservableObject, INavigationTarget<TParameter>
{
    public abstract void OnNavigatedTo(TParameter parameter);
}