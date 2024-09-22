using System.Diagnostics;
using System.Runtime.CompilerServices;
using TestinMAUIPageNavigationPerf.Sources.ViewModels;

namespace TestinMAUIPageNavigationPerf.Sources.Views;

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
    protected override
        async   // Added for test
        void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
#if SELF_TEST
        await Task.Delay(AppShell.TestInterval);
        if (Handler != null)
        {
            await Shell.Current.GoToAsync($"///{nameof(MainPage)}");
        }
#endif
    }
}