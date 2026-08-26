using Microsoft.Extensions.DependencyInjection;
using NavigationLibrary.Abstractions;
using NavigationLibrary.Core;
using NavigationLibrary.Factories;
using NavigationLibrary.Services;

namespace NavigationLibrary.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Inject dependencies for navigation
    /// </summary>
    /// <param name="services"></param>
    public static void AddNavigationCore(this IServiceCollection services)
    {
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<NavigationState>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<Func<Type, INavigationTarget>>(provider => type =>
            (INavigationTarget)ActivatorUtilities.CreateInstance(provider, type));
    }

    /// <summary>
    /// Register root for navigation state
    /// </summary>
    /// <param name="provider"></param>
    /// <typeparam name="TRoot"></typeparam>
    /// <returns>Instance of TRoot for DataContext</returns>
    public static TRoot InitializeNavigationRootCore<TRoot>(this IServiceProvider provider)
        where TRoot : ILayout
    {
        var navigationState = provider.GetRequiredService<NavigationState>();
        var rootViewModel = navigationState.CreateAndRegister(typeof(TRoot));
        navigationState.SetDefaultContent(typeof(TRoot));
        return (TRoot)rootViewModel.Instance;
    }
}