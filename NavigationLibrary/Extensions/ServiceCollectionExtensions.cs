using Microsoft.Extensions.DependencyInjection;
using NavigationLibrary.Data;
using NavigationLibrary.Factories;
using NavigationLibrary.Services;

namespace NavigationLibrary.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Inject dependencies for navigation
    /// </summary>
    /// <param name="services"></param>
    public static void AddNavigation(this IServiceCollection services)
    {
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<NavigationState>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<Func<Type, ViewModelBase>>(provider => type =>
            (ViewModelBase)ActivatorUtilities.CreateInstance(provider, type));
    }

    /// <summary>
    /// Register root for navigation state
    /// </summary>
    /// <param name="provider"></param>
    /// <typeparam name="TRoot"></typeparam>
    /// <returns>Instance of TRoot for DataContext</returns>
    public static TRoot RegisterNavigationRoot<TRoot>(this IServiceProvider provider)
        where TRoot : ViewModelBase, ILayout
    {
        var navigationState = provider.GetRequiredService<NavigationState>();
        var rootViewModel = navigationState.Register(typeof(TRoot));
        return (TRoot)rootViewModel.Instance;
    }
}