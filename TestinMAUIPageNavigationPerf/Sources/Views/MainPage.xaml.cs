using System.Diagnostics;
using TestinMAUIPageNavigationPerf.Sources.ViewModels;

namespace TestinMAUIPageNavigationPerf.Sources.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage() => InitializeComponent();
        new MainPageViewModel BindingContext =>(MainPageViewModel)base.BindingContext;
        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
#if SELF_TEST
                await Task.Delay(AppShell.TestInterval);
                // After first 'long' setup interval, test at smaller increments.
                AppShell.TestInterval = TimeSpan.FromSeconds(1);
                Debug.WriteLine($"Count = {_debugCount++}");
                var randoBC = BindingContext.Items[_rando.Next(BindingContext.Items.Length)];
            if (Handler != null)
            {
                BindingContext.SelectItemCommand.Execute(randoBC);
            }
#endif
        }
        int _debugCount = 1;
        Random _rando = new Random(Seed: 1);
    }
}
