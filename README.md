The Android branch is predicated on the Windows branch.

The navigation doesn't seem very performant on Android; it takes almost a second to show the `SelectPage`.

What's worst is that there seems to be a cumulative effect of performing the navigation repeatedly (either interactively, or running the test loop), adding a sbout a half second to the response every time.

___

#### Android-specific changes

With that in mind, an activity indicator has been superimposed on MainPage to let user know that the request is being worked on.

```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:views="clr-namespace:TestinMAUIPageNavigationPerf.Sources.Views"
             xmlns:viewmodels="clr-namespace:TestinMAUIPageNavigationPerf.Sources.ViewModels"
             x:Class="TestinMAUIPageNavigationPerf.Sources.Views.MainPage"
             x:DataType="viewmodels:MainPageViewModel">

    <ContentPage.BindingContext>
        <viewmodels:MainPageViewModel x:Name="Page"/>
    </ContentPage.BindingContext>
    <Grid>
        <ScrollView>
            <VerticalStackLayout>
            .
            .
            .
            </VerticalStackLayout>
        </ScrollView>
        <ActivityIndicator 
            x:Name="activityIndicator" 
            HeightRequest="50"
            WidthRequest="50"
            IsVisible="{Binding IsRunning, Mode=OneWay}"
            IsRunning="{Binding IsRunning, Mode=OneWay}"/>
    </Grid>
</ContentPage>

```

It's bound to `IsRunning` in the VM

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
            IsRunning = true;
            // REGISTERED AS:
            // Routing.RegisterRoute($"{nameof(MainPage)}/{nameof(SelectPage)}", typeof(SelectPage));
            await Shell.Current.GoToAsync(nameof(SelectPage));
        }
        catch (Exception e)
        {
            Debug.Fail(e.Message);
        }
        finally
        {
            IsRunning = false;
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

    [ObservableProperty]
    private bool _isRunning;
}
```
