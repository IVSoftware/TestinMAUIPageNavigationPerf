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