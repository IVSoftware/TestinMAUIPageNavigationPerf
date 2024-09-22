using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TestinMAUIPageNavigationPerf.Sources.Views;

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