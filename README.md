#### Self Test

Running on Windows Machine, added self-test blocks to easily reproduce the bug, generally in < 5 automatic cycles. Based on this, I tested Sir Rufo's suggestion of changing to `AddTransient` with no apparent impact on the severity of the issue.

```
public partial class MainPage : ContentPage
{
    .
    .
    .
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    #if SELF_TEST
        await Task.Delay(AppShell.TestInterval);
        // After first 'long' setup interval, test at smaller increments.
        AppShell.TestInterval = TimeSpan.FromSeconds(1);
        await Shell.Current.GoToAsync(nameof(SelectPage));
        Debug.WriteLine($"Count = {_debugCount++}");
    #endif
    }
    int _debugCount = 1;
}
```

```
public partial class SelectPage : ContentPage
{
    public SelectPage()
    {
        InitializeComponent();
    }
    protected override
        async   // Added for test
        void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Task.Delay(1).Wait(); // < Per original design
        BindingContext = MauiProgram.MainPage?.SelectedItemViewModel;


#if SELF_TEST
        await Task.Delay(AppShell.TestInterval);
        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
#endif
    }
}
```