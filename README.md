___
_This is an UPDATED ANSWER. I have been able to:_
 - _Confirm the specific exception being thrown_
 - _Attribute it to `builder.Services.AddSingleton<SelectPage>()` 
 and exonerate the code._
 - _Identify a possible MAUI platform bug that might explain it._
 - _Intercept the exception and successfully use a retry mechanism to test on a long-running loop._


This debug output shows a successful retry at N=5 ***but it also unexpectedly produced a message from the MAUI platform inviting us to submit a bug.*** [_I'm on it..._](https://github.com/dotnet/maui/issues/23050#issuecomment-2368856252)

[![debug log with maui message][2]][2]
___

##### Test loop to Reproduce the Bug


It's not always easy to identify the cause of spurious or seemingly random exceptions, but since the posted question includes a [link to the code repo](https://github.com/elTRexx/TestinMAUIPageNavigationPerf) it made me want to give it a try. I took the approach of setting two Conditional Compilation Symbols in the project so that I could enable an automated ping-ponging back and forth between `MainPage` and `SelectPage` as implemented in the code below, and just let it run.

[![conditional compile symbols][1]][1]

The bug was easily to reproduce by cloning the code repo and adding the **SELF_TEST** loop shown below, with a mean time to failure of < 10 cycles. This experiment identifies that the exception is specifically a `System.Runtime.InteropServices.COMException` occurring (as one might expect) on the Windows platform only.
 - When the line that adds `SelectPage` as a singleton was commented out and the **SELF_TEST** loop run overnight, a **cycle count of >25,000 was reached without failure**. (This means 0 exceptions thrown.) 
 - If the line is reinstated, exceptions seem inevitable and usually occur in << 100 cycles.
 

``` 
public static MauiApp CreateMauiApp()
{
    .
    .
    .
    builder.Services.AddSingleton<SelectPage>();
}
```

___

##### Singleton is Required, However

The final `SelectPage` class used for test is shown below, and enforces the singleton with a `Debug.Assert` if attempts are made to instantiate more than once (as would be the case if `builder.Services.AddTransient<SelectPage>()` were used instead). It also demonstrates how to intervene and retry for the exception that is thrown. In fact, we're going to hit paydirt with this approach as shown in the image that follows.

```
public partial class SelectPage : ContentPage
{
#if SELF_TEST
    private static uint _instanceCounter = 0;
    public SelectPage()
    {
        _instanceCounter++;
        Debug.Assert(_instanceCounter <= 1, "Expecting only one instance of this class.");
        InitializeComponent();
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await Task.Delay(AppShell.TestInterval);
        if (App.Current?.MainPage?.Handler != null)
        {
#if WINDOWS
            retry:
            int tries = 1;
            try
            {
                await Shell.Current.GoToAsync($"///{ nameof(MainPage)}");
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                if (tries == 1)
                    Debug.WriteLine($"{ex.GetType().Name}{Environment.NewLine}{ex.Message}");
                if (tries++ < 5)
                {
                    goto retry;
                }
                else throw new AggregateException(ex);
            }
#else
            try
            {
                await Shell.Current.GoToAsync(nameof(SelectPage));
            }
            catch(Exception ex) 
            {
                Debug.WriteLine($"{ex.GetType().Name} is unexpected on this platform.");
            }
#endif
        }
    }
#else
	public SelectPage() => InitializeComponent();
#endif
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(MainPageViewModel.SelectedItemViewModel is ItemViewModel valid)
        {
            BindingContext = valid;
        }
    }
}
```

___

#### Test conditions

Here's the remainder of the test code I ended up using: [Clone: windows-machine-successful-test](https://github.com/IVSoftware/TestinMAUIPageNavigationPerf/tree/windows-machine-successful-test)

The biggest change is moving the `SelectItemCommand` to the `MainPageViewModel`:

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
#if WINDOWS
                retry:
                int tries = 1;
                try
                {
                    await Shell.Current.GoToAsync(nameof(SelectPage));
                }
                catch(System.Runtime.InteropServices.COMException ex) 
                {
                    if(tries == 1) 
                        Debug.WriteLine($"{ex.GetType().Name}{Environment.NewLine}{ex.Message}");
                    if (tries++ < 5)
                    {
                        goto retry;
                    }
                    else throw new AggregateException(ex);
                }
#else
                try
                {
                    await Shell.Current.GoToAsync(nameof(SelectPage));
                }
                catch(Exception ex) 
                {
                    Debug.WriteLine($"{ex.GetType().Name} is unexpected on this platform.");
                }
#endif
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

In doing so, all the `SelectPage` needs to do is pull the current BC in the `OnAppearing` method (this code was already listed above). To the extent that there may have been suspicion around the original code changing the binding context synchronously within the `OnNavigatedTo` block, these changes were intended to perform the same bindings in a more conventional way.

___

The move of the `SelectItemCommand` is reflected the modified binding in **MainPage.xaml**:

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
#if SELF_TEST
    private static uint _instanceCounter = 0;
    public MainPage()
    {
        _instanceCounter++;
        Debug.Assert(_instanceCounter <= 1, "Expecting only one instance of this class.");
        InitializeComponent();
    }
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
#else 
    public MainPage() => InitializeComponent();
#endif
       
    new MainPageViewModel BindingContext =>(MainPageViewModel)base.BindingContext;
}
```

##### Minor Changes

- Removed Singleton Registrations for MainPage, MainPageViewModel
- Made all C'Tors parameterless.

___


  [1]: https://i.sstatic.net/OBrggB18.png
  [2]: https://i.sstatic.net/DXYq3o4E.png