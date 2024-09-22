With a reliable means of reproducing the bug, my hope was to eradicate it with as little change to the architecture as possible. However, in order to make it pass constrained randomized testing, I had to make some non-trivial changes which are offered as suggestions.

These changes were tested in a loop that repeatedly selects a random `ItemViewModel` from `Items` and uses it to invoke `SelectItem`, and this was run for >100 iterations.
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
            await Shell.Current.GoToAsync(nameof(SelectPage));
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

##### Minor Changes

- Removed Singleton Registrations for MainPage, MainPageViewModel, and SelectPage.
- Made all C'Tors parameterless.


