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
        // Task.Delay(1).Wait(); // < Per original design

        await Task.Delay(50);
        // BindingContext = MauiProgram.MainPage?.SelectedItemViewModel;


#if LOOP_TEST
        await Task.Delay(AppShell.TestInterval);
        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
#endif
    }
}