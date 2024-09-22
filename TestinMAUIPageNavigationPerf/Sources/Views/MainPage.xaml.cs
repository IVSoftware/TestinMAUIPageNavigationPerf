using System.Diagnostics;
using TestinMAUIPageNavigationPerf.Sources.ViewModels;

namespace TestinMAUIPageNavigationPerf.Sources.Views
{
    public partial class MainPage : ContentPage
    {
        static int _mainPageInstances = 1;
        public MainPage()
        {
            Debug.Assert(_mainPageInstances == 1, "Expecting only one instance of MainPage");
            Debug.WriteLine($"MainPage Instances: {_mainPageInstances++}");
            InitializeComponent();
        }
        new MainPageViewModel BindingContext =>(MainPageViewModel)base.BindingContext;
        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
#if SELF_TEST
            await Task.Delay(AppShell.TestInterval);
            if (_debugCount == 1)
            {
                // After first 'long' setup interval, test at smaller increments.
                AppShell.TestInterval = TimeSpan.FromSeconds(1);
            }
            else
            {
                Debug.WriteLine($@"Cycle time: {_stopwatch.Elapsed:mm\:ss\:fff}");
            }
            _stopwatch.Restart();
            Debug.WriteLine($"Count = {_debugCount++}");
            var randoBC = BindingContext.Items[_rando.Next(BindingContext.Items.Length)];
            if (Handler != null)
            {
                BindingContext.SelectItemCommand.Execute(randoBC);
            }
#endif
        }
        int _debugCount = 1;
        Stopwatch _stopwatch = new Stopwatch();
        Random _rando = new Random(Seed: 1);
    }
}
