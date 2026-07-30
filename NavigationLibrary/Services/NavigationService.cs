using System.Reflection;
using NavigationLibrary.Data;
using NavigationLibrary.Factories;

namespace NavigationLibrary.Services;

internal class NavigationService(NavigationState state) : INavigationService
{
    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        NavigateTo(typeof(TViewModel));
    }
    private Type GetParentLayoutType(Type type)
    {
        var navigationParentAttribute = type
            .GetCustomAttributes()
            .FirstOrDefault(attr =>
                attr.GetType().IsGenericType &&
                attr.GetType().GetGenericTypeDefinition() == typeof(ParentLayoutAttribute<>));

        if (navigationParentAttribute is null)
            throw new Exception($"'{type} has not NavigationParentAttribute to navigate");

        return navigationParentAttribute.GetType().GetGenericArguments()[0];
    }

    private void NavigateTo(Type destinationType)
    {
        var layoutType = GetParentLayoutType(destinationType);

        if (!state.IsRegistered(layoutType))
            NavigateTo(layoutType);

        state.UpdateWith(layoutType, destinationType);
    }
}