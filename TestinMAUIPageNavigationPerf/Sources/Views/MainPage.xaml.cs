using System.Diagnostics;
using TestinMAUIPageNavigationPerf.Sources.ViewModels;

namespace TestinMAUIPageNavigationPerf.Sources.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage() => InitializeComponent();
        new MainPageViewModel BindingContext =>(MainPageViewModel)base.BindingContext;
#if SELF_TEST
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
#endif
    }
}
