using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TestinMAUIPageNavigationPerf.Sources.Views;

public partial class SelectPage : ContentPage
{
    public SelectPage()
    {
        InitializeComponent();
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Task.Delay(1).Wait();
        BindingContext = MauiProgram.MainPage?.SelectedItemViewModel;
    }
}