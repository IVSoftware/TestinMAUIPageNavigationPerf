The SELF_TEST provides a reliable means of reproducing the bug, and testing changes against it. I was hoping to improve things with as little change to the architecture as possible. However, in order to make it pass constrained randomized testing, I had to make some non-trivial changes. If you like, you can experiment with these and see whether it moves things forward on your end. It's working pretty well here; I've run it for  >100 iterations on Windows Machine where the loop repeatedly selects a random `ItemViewModel` from `Items` and uses it to invoke `SelectItem`.
___

#### Changes

The biggest change is moving the `SelectItem` command to the `MainPageViewModel`:

```
public partial class MainPageViewModel : ObservableObject
{
    public static ItemViewModel? SelectedItemViewModel { get; set; }

    [RelayCommand]
    private async Task SelectItem(ItemViewModel item)
    {
        SelectedItemViewModel = item;
        try
        {
            if (App.Current?.MainPage?.Handler != null)
            {
                await Shell.Current.GoToAsync(nameof(SelectPage));
            }
        }
        catch (Exception e)
        {
            Debug.Fail(e.Message);
        }
    }
    public ItemViewModel[] Items { get; } = new[]
    {
        new ItemViewModel{ Title = "One" },
        new ItemViewModel{ Title = "Two" },
        new ItemViewModel{ Title = "Three" },
        new ItemViewModel{ Title = "Four" },
        new ItemViewModel{ Title = "Five" },
    };
}
```

In doing so, all the `SelectPage` needs to do is pull the current BC in the `OnAppearing` method.

```
public partial class SelectPage : ContentPage
{
    public SelectPage()
    {
        InitializeComponent();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(MainPageViewModel.SelectedItemViewModel is ItemViewModel valid)
        {
            BindingContext = valid;
        }
    }
#if SELF_TEST
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await Task.Delay(AppShell.TestInterval);
        if (Handler != null)
        {
            // Discard task to keep self-test stack from creeping from recursion
            _ = Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }
#endif
}
```

This is reflected these changes to **MainPage.xaml**:

```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:views="clr-namespace:TestinMAUIPageNavigationPerf.Sources.Views"
             xmlns:viewmodels="clr-namespace:TestinMAUIPageNavigationPerf.Sources.ViewModels"
             x:Class="TestinMAUIPageNavigationPerf.Sources.Views.MainPage"
             x:DataType="viewmodels:MainPageViewModel">
    <!--Set BC here with x:Ref="Page"-->
    <ContentPage.BindingContext>
        <viewmodels:MainPageViewModel x:Name="Page"/>
    </ContentPage.BindingContext>
    <ScrollView>
        .
        .
        .
        <CollectionView>
            <CollectionView.ItemTemplate>
              <DataTemplate x:DataType="viewmodels:ItemViewModel">
                <Border>
                  .
                  .
                  .
                  <Border.GestureRecognizers>
                        <TapGestureRecognizer
                            NumberOfTapsRequired="2"
                            Command="{Binding SelectItemCommand, Source={x:Reference Page }}"
                            CommandParameter="{Binding .}"/>
                  </Border.GestureRecognizers>
              </DataTemplate>
            </CollectionView.ItemTemplate>
          </CollectionView>
          .
          .
          .
    </ScrollView>
</ContentPage>

```

The only added functionality in the code behind for MainPage is the SELF_TEST block.

```
public partial class MainPage : ContentPage
{
    public MainPage() => InitializeComponent();
    new MainPageViewModel BindingContext =>(MainPageViewModel)base.BindingContext;
#if SELF_TEST
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
            await Task.Delay(AppShell.TestInterval);
            // After for optional 'long' setup interval for the first
            // iteration, followed by test at smaller increments.
            AppShell.TestInterval = TimeSpan.FromSeconds(1);
            Debug.WriteLine($"Count = {_debugCount++}");
            var randoBC = BindingContext.Items[_rando.Next(BindingContext.Items.Length)];
            BindingContext.SelectItemCommand.Execute(randoBC);
    }
    int _debugCount = 1;
    Random _rando = new Random(Seed: 1);
#endif
}
```

##### Minor Changes

- Removed Singleton Registrations for MainPage, MainPageViewModel, and SelectPage.
- Made all C'Tors parameterless.


