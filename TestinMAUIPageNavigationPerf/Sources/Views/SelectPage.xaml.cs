using System.Diagnostics;
using System.Runtime.CompilerServices;
using TestinMAUIPageNavigationPerf.Sources.ViewModels;

namespace TestinMAUIPageNavigationPerf.Sources.Views;

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
        if (Handler != null)
        {
            // Discard task to keep self-test stack from creeping from recursion
            _ = Shell.Current.GoToAsync($"//{nameof(MainPage)}");
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
#if SELF_TEST
#endif
}