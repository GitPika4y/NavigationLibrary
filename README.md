# NavigationLibrary

Compile-time навигация для WPF и Avalonia. Никакой рефлексии в рантайме — вся связь между ViewModel, View и родительскими layout'ами вычисляется на этапе компиляции source generator'ом и кладётся в обычные `Dictionary`.

## Структура:
- [Требования](#требования)
- [Как это устроено](#как-это-устроено)
- [Установка](#установка)
- [Инициализация](#инициализация)
  - [Avalonia](#avalonia)
  - [WPF](#wpf)
- Использование
  - [Базовые классы](#базовые-классы-viewmodel)
    - [CommunityToolkit.Mvvm](#с-communitytoolkitmvvm)
    - [INotifyPropertyChanged](#с-inotifypropertychanged-напрямую-wpf)
  - [Навигация](#навигация)
  - [Пример использования](#пример-использования-с-communitytoolkitmvvm)
- [Зачем `ILayout` и его `Content`](#зачем-нужен-ilayout-и-что-делать-с-его-content-в-xaml)
- [Диагностика на этапе компиляции](#диагностика-на-этапе-компиляции)


## Требования

- .NET 8
- WPF (`net8.0-windows`) или Avalonia (`net8.0`, пакет `Avalonia` подключается вашим проектом самостоятельно)

## Как это устроено

Библиотека описывает связи между экранами тремя способами прямо на ViewModel:

- `[View<TView>]` — какой View отображает эту ViewModel. **Обязателен** для любого не абстрактного класса, реализующего `INavigationTarget` — если забыть его поставить, проект просто не скомпилируется (ошибка `NAV001`).
- `[ParentLayout<TLayout>]` — в каком layout'е показывать эту ViewModel при навигации. Не нужен только для самого корневого layout'а (например, `WindowViewModel`).
- `INavigationTarget` / `INavigationTarget<TParameter>` — маркер «это экран, которым управляет навигация», второй вариант даёт `OnNavigatedTo(TParameter parameter)` для приёма параметра.
- `ILayout<TDefaultContent>` — для классов-контейнеров (окно, вкладка, что угодно с `Content`), которые сами показывают вложенный `INavigationTarget`.

Source generator анализирует эти атрибуты и интерфейсы в **проекте, который подключил библиотеку**, и генерирует:

- `NavigationRegistry` — реализацию `INavigationRegistry` с готовыми словарями `View → ViewModel`, `ViewModel → ParentLayout`, `ViewModel → (ParameterType, OnNavigatedTo)`.
- `DataTemplatesOutput` — регистрацию `DataTemplate`/`FuncDataTemplate` для каждой пары ViewModel/View, под WPF или Avalonia (определяется автоматически по тому, какие сборки подключены к проекту).
- Один метод `AddNavigation(...)`, который регистрирует всё сразу в DI.

## Установка

Пакет пока не опубликован на nuget.org — `.nupkg` доступен в разделе [релизов](../../releases). Установка займёт на одну строку больше, чем обычно: нужно сначала скачать файл и указать локальную папку как источник пакетов.

**1. Скачайте `.nupkg`**

Возьмите файл `NavigationLibrary.1.0.1.nupkg` из [релизов](../../releases) этого репозитория (или из корня, если он закоммичен туда) и положите в любую папку, например `C:\NuGetLocal` или `~/NuGetLocal`.

**2. Укажите эту папку как источник пакетов**

Проще всего — добавить `NuGet.config` рядом с вашим `.sln`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="Local" value="C:\NuGetLocal" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

Либо тем же способом через UI (Rider: `NuGet(Alt+Shift+7) → Sources`; Visual Studio: `Tools → NuGet Package Manager → Package Sources`) — добавить папку как ещё один источник, ничего не удаляя.

**3. Подключите пакет как обычно**

```xml
<ItemGroup>
    <PackageReference Include="NavigationLibrary" Version="1.0.1" />
</ItemGroup>
```

После этого NuGet подхватит `.nupkg` из локальной папки — дальше всё работает точно так же, как с обычным пакетом из nuget.org, включая генератор (он уже упакован внутри `analyzers/dotnet/cs/`, отдельно подключать ничего не нужно).

> Если после обновления версии в репозитории пакет не подхватывается — почистите кэш NuGet: `dotnet nuget locals all --clear`, затем `dotnet restore`. NuGet кэширует пакеты по номеру версии и может не заметить, что файл в локальной папке обновился.

## Инициализация

### Avalonia

```csharp
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        services.AddNavigation(DataTemplates);

        var provider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new WindowView
            {
                DataContext = provider.InitializeNavigationRoot<WindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
```

### WPF

```csharp
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        services.AddNavigation(Resources);

        var provider = services.BuildServiceProvider();

        var window = new WindowView
        {
            DataContext = provider.InitializeNavigationRoot<WindowLayoutViewModel>()
        };
        window.Show();
    }
}
```

`AddNavigation` принимает `ResourceDictionary` (WPF) или `DataTemplates` (Avalonia) — именно туда и регистрируются шаблоны. `InitializeNavigationRoot<TRoot>()` создаёт корневой `ILayout` и связывает его с навигационным состоянием — вызывается один раз, для самого верхнего layout'а приложения (обычно окно).

## Базовые классы ViewModel

Библиотека не диктует, как именно у вас реализовано уведомление об изменении свойств — она работает через любой `INavigationTarget`/`ILayout`. Ниже — два готовых варианта на выбор.

### С CommunityToolkit.Mvvm

```csharp
public abstract class NavigationTarget : ObservableObject, INavigationTarget;

public abstract class NavigationTarget<TParameter> : ObservableObject, INavigationTarget<TParameter>
{
    public abstract void OnNavigatedTo(TParameter parameter);
}

public abstract partial class Layout<TDefaultContent> : ObservableObject, ILayout<TDefaultContent>
    where TDefaultContent : INavigationTarget
{
    [ObservableProperty] private INavigationTarget _content = null!;
}
```

### С INotifyPropertyChanged напрямую (WPF)

```csharp
public abstract class ObservableViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public abstract class NavigationTarget : ObservableViewModel, INavigationTarget;

public abstract class Layout<TDefaultContent> : ObservableViewModel, ILayout<TDefaultContent>
    where TDefaultContent : INavigationTarget
{
    private INavigationTarget _content = null!;

    public INavigationTarget Content
    {
        get => _content;
        set => SetField(ref _content, value);
    }
}
```

## Навигация

Для навигации существует `INavigationService`, который предоставляет все необходимые методы:

```csharp
public interface INavigationService
{
    void NavigateTo<TDestination>() where TDestination : INavigationTarget;
    void NavigateTo(Type destinationType);

    void NavigateTo<TDestination, TParameter>(TParameter parameter) where TDestination : INavigationTarget<TParameter>;
    void NavigateTo(Type destinationType, object? parameter);
}
```

Он регистрируется автоматически внутри `AddNavigation(...)' - для использования, получите его через конструктор ViewModel (см. [Примеры](#пример-использования-с-communitytoolkitmvvm))

## Пример использования (с CommunityToolkit.Mvvm)

```csharp
[View<WindowView>]
public partial class WindowViewModel : Layout<MainViewModel>
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";
}

[ParentLayout<WindowViewModel>]
[View<MainView>]
public partial class MainViewModel(INavigationService navigationService) : NavigationTarget
{
    [ObservableProperty] private string _greeting = "";

    [RelayCommand]
    private void Some() => navigationService.NavigateTo<SecondViewModel, string>("Some param");
}

[ParentLayout<WindowViewModel>]
[View<SecondView>]
public class SecondViewModel(INavigationService navigationService) : NavigationTarget<string>
{
    public override void OnNavigatedTo(string parameter)
    {
        Console.WriteLine($"Param from navigation: {parameter}");
    }
}
```

`WindowViewModel` — корневой layout, поэтому у него нет `[ParentLayout<T>]`. `MainViewModel` и `SecondViewModel` указывают `[ParentLayout<WindowViewModel>]` — при навигации на них библиотека сама найдёт `WindowViewModel` в текущем дереве и подставит их в его `Content`.

`navigationService.NavigateTo<SecondViewModel, string>("Some param")` вызовет `SecondViewModel.OnNavigatedTo("Some param")` сразу после создания экземпляра — без единого `Activator`/`MethodInfo.Invoke`, вызов компилируется напрямую в сгенерированном коде.

## Зачем нужен `ILayout` и что делать с его `Content` в XAML

`ILayout<TDefaultContent>` — это не про визуальную структуру, а про то, что внутри есть **одна изменяемая область**, куда навигация подставляет текущий экран. Сам layout (окно, панель, что угодно) рисует всё остальное — заголовки, меню, статичные части — а `Content` отдаёт под конкретный `INavigationTarget`.

На практике это выглядит как один `ContentControl`, забинденный на `Content`, или с статичными частями:

**Avalonia:**

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:AvaloniaApplication1.ViewModels"
        x:Class="AvaloniaApplication1.Views.WindowView"
        x:DataType="vm:WindowViewModel">
    <Design.DataContext>
        <vm:WindowViewModel />
    </Design.DataContext>

    <Grid RowDefinitions="Auto, *, Auto">
        <TextBlock Grid.Row="0" Text="Header"/>
        
        <ContentControl Grid.Row="1" Content="{Binding Content}"/>
        
        <TextBlock Grid.Row="2" Text="Footer"/>
    </Grid>
</Window>
```

**WPF:**

```xml
<Window x:Class="WpfApp1.Views.Window.WindowView"
        xmlns:viewModel="clr-namespace:WpfApp1.ViewModels.Window"
        d:DataContext="{d:DesignInstance Type=viewModel:WindowLayoutViewModel}">
    <Grid> 
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        
        <TextBlock Grid.Row="0" Text="Header"/>
        
        <ContentControl Grid.Row="1" Content="{Binding Content}"/>
        
        <TextBlock Grid.Row="2" Text="Footer"/>
    </Grid>
</Window>
```

`ContentControl` сам разрешает, какой `DataTemplate` использовать для текущего значения `Content` — а эти шаблоны как раз и регистрирует `AddNavigation(...)` при старте приложения (по `[View<T>]` каждой ViewModel). Всё, что вы пишете в XAML layout'а — это статичная «рамка» вокруг одной точки подмены.

## Диагностика на этапе компиляции

Если класс реализует `INavigationTarget` (напрямую или через `ILayout`), но не помечен `[View<TView>]` — сборка падает с ошибкой `NAV001`, подсвеченной прямо на объявлении класса:

```
NAV001: Class 'MainViewModel' implements INavigationTarget but is missing a [View<TView>] attribute
```

Это сделано намеренно: связь ViewModel ↔ View считается обязательной, и её отсутствие — ошибка конфигурации, а не то, что должно тихо проявиться в рантайме.
