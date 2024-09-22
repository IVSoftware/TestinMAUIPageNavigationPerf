using System.Diagnostics;
using TestinMAUIPageNavigationPerf.Sources.ViewModels;

namespace TestinMAUIPageNavigationPerf.Sources.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage() => InitializeComponent();
        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
#if LOOP_TEST
            await Task.Delay(AppShell.TestInterval);
            // After first 'long' setup interval, test at smaller increments.
            AppShell.TestInterval = TimeSpan.FromSeconds(1);
            await Shell.Current.GoToAsync(nameof(SelectPage));
            Debug.WriteLine($"Count = {_debugCount++}");
#endif
        }
        int _debugCount = 1;
    }
}
